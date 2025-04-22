using Common.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class ConversationResponseDTO
    {
        public bool IsComplete { get; set; }
        public string BotResponse { get; set; }
        public string Evaluation { get; set; }
        public int CurrentTopic { get; set; }
        public int TurnsRemaining { get; set; }
        public string ScenarioPrompt { get; set; }
        public string AudioResponseUrl { get; set; }
        public byte[] AudioResponse {  get; set; }  
        public byte[] AudioData { get; set; }
        public string TranslatedResponse { get; set; }
        public string ResponseText { get; set; }
        public string Summary { get; set; } 
        public string Strengths { get; set; } 
        public string Weaknesses { get; set; } 
        public string Improvements { get; set; } 

    }
}
