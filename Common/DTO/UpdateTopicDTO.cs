using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO
{
   public class UpdateTopicDTO
    {
        public string TopicName { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
    }
}
