using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO
{
    public class ChatHubDTO
    {
        public Guid UserId { get; set; }
        public string Message { get; set; }
        public int TopicId { get; set; }
    }
}
