using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO
{
    public class SubmitExerciseDTO
    {
        public Guid UserId { get; set; }
        public decimal EarnedPoints { get; set; }
    }
}
