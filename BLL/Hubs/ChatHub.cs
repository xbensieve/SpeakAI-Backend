using Azure.Core;
using BLL.Interface;
using Common.DTO;
using DAL.Entities;
using DAL.UnitOfWork;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IAIService _aiService;
        private readonly IUnitOfWork _unitOfWork;

        public ChatHub(IAIService aiIService, IUnitOfWork unitOfWork)
        {
            _aiService = aiIService;
            _unitOfWork = unitOfWork;
        }
        public async Task<string> SendMessage(ChatHubDTO chatHubDTO)
        {

            var userId = await _unitOfWork.User.GetByIdAsync(chatHubDTO.UserId);
            _aiService.SetCurrentTopic(chatHubDTO.TopicId);
            var response = await _aiService.ProcessConversationAsync(chatHubDTO.Message);
            return response.BotResponse;
        }
        public async Task<string> EndConversation()
        {
            var response = await _aiService.EndConversationAsync();
            await Clients.Caller.SendAsync("ReceiveEvaluation", response);

           
            var fullResponse = $@"
IsComplete: {response.IsComplete}
BotResponse: {response.BotResponse}
Evaluation: {response.Evaluation}
CurrentTopic: {response.CurrentTopic}
TurnsRemaining: {response.TurnsRemaining}
ScenarioPrompt: {response.ScenarioPrompt}
AudioResponseUrl: {response.AudioResponseUrl}
AudioResponse: {(response.AudioResponse != null ? $"[{response.AudioResponse.Length} bytes]" : "null")}
AudioData: {(response.AudioData != null ? $"[{response.AudioData.Length} bytes]" : "null")}
TranslatedResponse: {response.TranslatedResponse}
ResponseText: {response.ResponseText}
Summary: {response.Summary}
Strengths: {response.Strengths}
Weaknesses: {response.Weaknesses}
Improvements: {response.Improvements}";

            return fullResponse;
        }
    }
}
