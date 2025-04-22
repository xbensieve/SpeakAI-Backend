using BLL.Hubs;
using Common.DTO;
using DAL.Entities;
using DAL.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using OpenAI.Chat;

namespace SpeakAI.Controllers
{
    [Route("api/chats")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IUnitOfWork _unitOfWork;

        public ChatController(IHubContext<ChatHub> hubContext, IUnitOfWork unitOfWork)
        {
            _hubContext = hubContext;
            _unitOfWork = unitOfWork;
        }

            [HttpPost("send")]
            public async Task<IActionResult> SendMessage([FromBody] ChatHubDTO chatHubDTO)
            {
                if (chatHubDTO == null || string.IsNullOrEmpty(chatHubDTO.Message))
                {
                    return BadRequest("Invalid message data.");
                }

         
                var userMessage = new ChatMessages
                {
                    Id = Guid.NewGuid(),
                    UserId = chatHubDTO.UserId,
                    Message = chatHubDTO.Message,
                    TopicId = chatHubDTO.TopicId,
                    IsBot = false,
                    Timestamp = DateTime.UtcNow
                };

                await _unitOfWork.ChatMessages.AddAsync(userMessage);

            await _hubContext.Clients.All.SendAsync("ReceiveMessage", chatHubDTO);
            var botMessage = new ChatMessages
                {
                    Id = Guid.NewGuid(),
                    UserId = chatHubDTO.UserId,
                    Message = chatHubDTO.Message,
                    TopicId = chatHubDTO.TopicId,
                    IsBot = true,
                    Timestamp = DateTime.UtcNow
                };

                await _unitOfWork.ChatMessages.AddAsync(botMessage);

                await _unitOfWork.SaveChangeAsync();

                return Ok(new { Message = "Message sent and saved successfully." });
            }

        [HttpGet("history/{userId}")]
        public async Task<IActionResult> GetChatHistory(Guid userId)
        {
            var chatHistory = await _unitOfWork.ChatMessages.GetAllByListAsync(cm => cm.UserId == userId);
            return Ok(chatHistory);
        }
    }
}
