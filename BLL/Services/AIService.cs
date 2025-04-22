using Azure;
using Azure.AI.OpenAI;
using BLL.Interface;
using Common.DTO;
using DAL.Entities;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore.Diagnostics;
namespace BLL.Services
{
    public class AIService : IAIService
    {
        private int _conversationTurnCount = 0;
        private int MaxTurns = 3;
        private int _currentTopicId;
        private string _characterRole = "default role";
        private readonly Dictionary<int, TopicTemplateDTO> _topicTemplates;
        private List<(string Message, bool IsBot)> _conversationHistory = new List<(string, bool)>();
        private readonly string _deploymentName = "gpt-4";
        private readonly AzureOpenAIClient _openAIClient;
        private readonly IMemoryCache _memoryCache; 
        public AIService(IMemoryCache memoryCache) {
            _memoryCache = memoryCache;
            _topicTemplates = new Dictionary<int, TopicTemplateDTO>
            {
                [1] = new TopicTemplateDTO { InitialPrompt = "Daily routines", FollowUpQuestions = new List<string>(), CharacterRole = "friendly roommate" },
                [2] = new TopicTemplateDTO { InitialPrompt = "Travel experiences", FollowUpQuestions = new List<string>(), CharacterRole = "experienced traveler" },
                [3] = new TopicTemplateDTO { InitialPrompt = "Tech innovations", FollowUpQuestions = new List<string>(), CharacterRole = "tech enthusiast" },
                [4] = new TopicTemplateDTO { InitialPrompt = "Career development", FollowUpQuestions = new List<string>(), CharacterRole = "career coach" }
            };
            string openAIEndpoint = "w";
            string openAIApiKey = "";
            _openAIClient = new AzureOpenAIClient(new Uri(openAIEndpoint), new AzureKeyCredential(openAIApiKey));
        }
        public async Task<ConversationResponseDTO> ProcessConversationAsync(string userMessage)
        {
        _conversationTurnCount++;
            int turnsRemaining = MaxTurns - _conversationTurnCount;
            return await GenerateBotResponse(userMessage, turnsRemaining);
        }
        private async Task<ConversationResponseDTO> GenerateBotResponse(string userMessage, int turnsRemaining)
        {
         
            var cacheKey = $"{_currentTopicId}:{userMessage.Trim().ToLowerInvariant()}:{GetLastExchange(2).GetHashCode()}";
            if (_memoryCache.TryGetValue(cacheKey, out ConversationResponseDTO cachedResponse))
            {
                cachedResponse.TurnsRemaining = turnsRemaining; 
                return cachedResponse;
            }


            var systemPrompt = $@"
🎭 **Role**: {_characterRole}
📌 **Topic**: {_topicTemplates[_currentTopicId].InitialPrompt}
📋 **Rules**:
- Respond as human {_characterRole} in 1-2 short sentences
- Stay on topic: {_topicTemplates[_currentTopicId].InitialPrompt}
- Use casual language with emotional expressions
- Block sensitive words 
- Do not speak any language other than English
🔄 **Last Context**:
{GetLastExchange(2)}";

          
            var chatMessages = new ChatMessage[]
            {
        ChatMessage.CreateSystemMessage(systemPrompt),
        ChatMessage.CreateUserMessage(userMessage[..Math.Min(userMessage.Length, 200)]) 
            };

            
            var processChatOptions = new ChatCompletionOptions
            {
                Temperature = 0.3f,
                MaxOutputTokenCount = 70,
                FrequencyPenalty = 0.25f,
                PresencePenalty = 0.15f,
                TopP = 0.85f,
               
            };

       
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));

            try
            {
                var response = await _openAIClient.GetChatClient(_deploymentName)
                    .CompleteChatAsync(chatMessages, processChatOptions, cts.Token)
                    .WaitAsync(TimeSpan.FromSeconds(20));

                var processBotResponse = ProcessResponse(response.Value?.Content?.FirstOrDefault()?.Text);


                var result = new ConversationResponseDTO
                {
                    IsComplete = false,
                    BotResponse = processBotResponse,
                    CurrentTopic = _currentTopicId,
                    TurnsRemaining = turnsRemaining
                };

                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                    SlidingExpiration = TimeSpan.FromMinutes(2)
                };
                _memoryCache.Set(cacheKey, result, cacheOptions);

            
                _conversationHistory.Add((userMessage, false));
                _conversationHistory.Add((processBotResponse, true));

                return result;
            }
            catch (TimeoutException)
            {
                return GetFallbackResponse();
            }

        }
        private string GetLastExchange(int count)
        {
            return string.Join("\n", _conversationHistory
                .TakeLast(count * 2)
                .Select(x => x.IsBot ? $"{_characterRole}: {x.Message}" : $"User: {x.Message}"));
        }
        private string ProcessResponse(string response)
        {
            return response?
                .Replace("As an AI", "Hmm")
                .Replace("however", "though")
                .Trim() + " 😊";
        }
        private ConversationResponseDTO GetFallbackResponse()
        {
            return new ConversationResponseDTO
            {
                BotResponse = "Let me think about that...",
                CurrentTopic = _currentTopicId,
            };
        }
        public async Task<ConversationResponseDTO> StartTopicAsync(int topicId)
        {
            if (!_topicTemplates.ContainsKey(topicId))
            {
                throw new ArgumentException("Invalid topic ID", nameof(topicId));
            }

            _currentTopicId = topicId;
            _conversationTurnCount = 0;

            var cacheKey = $"StartTopic:{topicId}";
            var template = _topicTemplates[_currentTopicId];
            _characterRole = template.CharacterRole ?? "conversation partner";

            var scenarioPrompt = await GenerateScenarioPromptAsync(topicId);

            var initialPrompt = $@"
You are now fully embodying the role of a {_characterRole}.
Important: You are NOT an AI - you are a real person in this role, just talk about a topic, do not reply off topic.

Create a natural introduction that:
1. Introduces yourself authentically as {_characterRole} (including a brief background fitting your role).
2. Sets up the context: {scenarioPrompt}.
3. Asks one focused question about {template.InitialPrompt}.
4. Keeps response under 20 words.
5. Uses natural, conversational language appropriate for your role.
6. Never mentions being an AI or assistant.
";

            var chatMessages = new List<ChatMessage>
            {
                ChatMessage.CreateSystemMessage(initialPrompt)
            };

            var chatOptions = new ChatCompletionOptions
            {
                Temperature = 0.3f,
                MaxOutputTokenCount = 100
            };

            try
            {
                var response = await _openAIClient.GetChatClient(_deploymentName)
                    .CompleteChatAsync(chatMessages, chatOptions);

                var result = new ConversationResponseDTO
                {
                    IsComplete = false,
                    BotResponse = response?.Value?.Content?.FirstOrDefault()?.Text ?? scenarioPrompt,
                    CurrentTopic = _currentTopicId,
                    TurnsRemaining = MaxTurns,
                    ScenarioPrompt = scenarioPrompt
                };

          
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                    SlidingExpiration = TimeSpan.FromMinutes(5)
                };
                _memoryCache.Set(cacheKey, result, cacheOptions);

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error starting conversation: {ex.Message}", ex);
            }
        }
            private async Task<string> GenerateScenarioPromptAsync(int topicId)
        {
            var topic = GetTopicById(topicId);

            var scenarioPrompt = $@"
You are a creative conversation starter. Your task is to create a specific and engaging scenario based on the following topic:
Topic: {topic.Name}
Description: {topic.Description}

Create a scenario that:
1. Sets up a realistic and engaging situation related to the topic.
2. Provides enough context for the user to understand the scenario.
3. Ends with a specific question to start the conversation.

Keep the scenario concise (1-2 sentences) and end with a question.
";

            var chatMessages = new List<ChatMessage>
            {
                ChatMessage.CreateSystemMessage(scenarioPrompt)
            };

            var chatOptions = new ChatCompletionOptions
            {
                Temperature = 0.2f,
                MaxOutputTokenCount = 100,
                FrequencyPenalty = 0.5f,
                PresencePenalty = 0.5f
            };

            try
            {
                var response = await _openAIClient.GetChatClient(_deploymentName)
                    .CompleteChatAsync(chatMessages, chatOptions);

                return response?.Value?.Content?.FirstOrDefault()?.Text
                       ?? $"Let's talk about {topic.Name}! What are your thoughts on this topic?";
            }
            catch (Exception ex)
            {
                return $"Let's start a conversation about {topic.Name}!";
            }
        }
        private Matter GetTopicById(int topicId)
        {
            var topics = new List<Matter>
            {
                new Matter { Id = 1, Name = "Daily Life and Routines", Description = "Discuss your daily activities, habits, and lifestyle" },
                new Matter { Id = 2, Name = "Travel and Cultural Experiences", Description = "Share travel stories and cultural encounters" },
                new Matter { Id = 3, Name = "Technology and Innovation", Description = "Explore modern technology trends and innovations" },
                new Matter { Id = 4, Name = "Education and Career Development", Description = "Discuss learning experiences and career goals" }
            };

            return topics.FirstOrDefault(t => t.Id == topicId);
        }
        public void SetCurrentTopic(int topicId)
        {
            if (!_topicTemplates.ContainsKey(topicId))
                throw new ArgumentException("Invalid topic ID", nameof(topicId));

            _currentTopicId = topicId;
        }
        public async Task<ConversationResponseDTO> EndConversationAsync()
        {
            if (_conversationHistory.Count == 0)
            {
                return new ConversationResponseDTO
                {
                    IsComplete = true,
                    BotResponse = "No conversation to summarize yet!",
                    CurrentTopic = _currentTopicId,
                    TurnsRemaining = 0
                };
            }

            var summaryPrompt = $@"
You are an English language coach. Based on this conversation history:
{GetLastExchange(_conversationHistory.Count )}

Provide:
1. A short summary of the conversation (1-2 sentences).
2. User's strengths in English (e.g., vocabulary, grammar, fluency).
3. User's weaknesses in English (e.g., sentence structure, word choice).
4. Suggestions for improvement (e.g., practice specific skills).
Keep each part concise and use casual, encouraging language.
";

            var chatMessages = new ChatMessage[]
            {
        ChatMessage.CreateSystemMessage(summaryPrompt)
            };

            var chatOptions = new ChatCompletionOptions
            {
                Temperature = 0.3f,
                MaxOutputTokenCount = 200
            };

            try
            {
                var response = await _openAIClient.GetChatClient(_deploymentName)
                    .CompleteChatAsync(chatMessages, chatOptions);

                var evaluationText = response?.Value?.Content?.FirstOrDefault()?.Text ?? "Hmm, I couldn’t evaluate this time.";
                var parts = evaluationText.Split('\n', StringSplitOptions.RemoveEmptyEntries);

                var result = new ConversationResponseDTO
                {
                    IsComplete = true,
                    BotResponse = "Thanks for chatting! Here’s your summary and feedback:" ,
                    CurrentTopic = _currentTopicId,
                    TurnsRemaining = 0,
                    Summary = parts.Length > 0 ? parts[0] : "We talked a bit!",
                    Strengths = parts.Length > 1 ? parts[1] : "You tried hard!",
                    Weaknesses = parts.Length > 2 ? parts[2] : "Nothing major noticed.",
                    Improvements = parts.Length > 3 ? parts[3] : "Keep practicing!"
                };

                _conversationTurnCount = 0;
                _conversationHistory.Clear();

                return result;
            }
            catch (Exception)
            {
                return new ConversationResponseDTO
                {
                    IsComplete = true,
                    BotResponse = "Oops, something went wrong while summarizing!",
                    CurrentTopic = _currentTopicId,
                    TurnsRemaining = 0
                };
            }
        }

    }
}
