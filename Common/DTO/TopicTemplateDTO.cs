using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO
{
    public class TopicTemplateDTO
    {
        public string InitialPrompt { get; set; }
        public List<string> FollowUpQuestions { get; set; }
        public string CharacterRole { get; set; }
    }
}
