using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class Exercise : BaseEntity
    {

        public string Content { get; set; }
        public decimal MaxPoint { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public DateTime UpdatedAt { get; set; }
     
        public Guid TopicId { get; set; }

        // Navigation properties
        public virtual Topic Topic { get; set; }
        public virtual ICollection<ExerciseProgress> ExerciseProgresses { get; set; }
    }
}
