using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO
{
    public class ExerciseDetailDTO
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public decimal MaxPoint { get; set; }
        public bool IsActive { get; set; }
    }
}
