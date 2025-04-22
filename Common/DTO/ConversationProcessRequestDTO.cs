using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO
{
    public class ConversationProcessRequestDTO
    {
        public string UserId { get; set; }
        public string Text { get; set; }
        public int TopicId { get; set; }
    }
}
