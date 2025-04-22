using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interface
{
    public interface IAIService
    {
        Task<ConversationResponseDTO> ProcessConversationAsync(string userMessage);
        Task<ConversationResponseDTO> StartTopicAsync(int topicId);
        void SetCurrentTopic(int topicId);
        Task<ConversationResponseDTO> EndConversationAsync(); 
    }
}
