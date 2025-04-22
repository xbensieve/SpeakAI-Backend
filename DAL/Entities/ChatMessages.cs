using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class ChatMessages
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public int TopicId { get; set; }    
        public string Message { get; set; }
        public bool IsBot { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
