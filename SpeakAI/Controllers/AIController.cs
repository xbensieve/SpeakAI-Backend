using BLL.Interface;
using Common.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SpeakAI.Controllers
{
    [Route("api/ai")]
    [ApiController]
    public class AIController : ControllerBase
    {
        private readonly IAIService _aiService;

        public AIController(IAIService aIService)
        {
            _aiService = aIService;
        }
        [HttpPost("start")]
        public async Task<IActionResult> StartTopic([FromBody] StartTopicRequestDTO request)
        {
            try
            {
                var response = await _aiService.StartTopicAsync(request.TopicId);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {

                return StatusCode(500, "Failed to start topic");
            }
        }
        [HttpPost("conversations/{conversationId}/messages")]
        public async Task<IActionResult> ProcessConversation([FromBody] ConversationProcessRequestDTO request)
        {
            try
            {

                if (string.IsNullOrEmpty(request.Text))
                {
                    return BadRequest("Text must be provided");
                }



                _aiService.SetCurrentTopic(request.TopicId); 
                    
            
                var response = await _aiService.ProcessConversationAsync(request.Text);
                
           
                return Ok(new
                {
                    response.IsComplete,
                    response.BotResponse,
                    response.Evaluation,
                    response.TurnsRemaining,
                    UserMessage = request.Text
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", details = ex.Message });
            }
        }
        [HttpPost("conversations/end")]
        public async Task<IActionResult> EndConversation()
        {
            try
            {
                var response = await _aiService.EndConversationAsync();
                return Ok(new
                {
                    response.IsComplete,
                    response.BotResponse,
                    response.Summary,
                    response.Strengths,
                    response.Weaknesses,
                    response.Improvements,
                    response.TurnsRemaining
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to end conversation", details = ex.Message });
            }
        }
    }
}
