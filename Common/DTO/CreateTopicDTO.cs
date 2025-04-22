using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO
{
    public class CreateTopicDTO
    {
        public string TopicName { get; set; }
        public List<CreateExerciseDTO> Exercises { get; set; }
    }
}
