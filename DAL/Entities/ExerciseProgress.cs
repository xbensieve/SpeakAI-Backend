using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class ExerciseProgress : BaseEntity
    {
    
        public decimal ProgressPoints { get; set; }
        public bool IsCompleted { get; set; }
 
        public Guid EnrolledCourseId { get; set; }
        public Guid ExerciseId { get; set; }
        public Guid UserId { get; set; }
        public DateTime? UpdateAt { get; set; }

        // Navigation properties
        public virtual EnrolledCourse EnrolledCourse { get; set; }
        public virtual Exercise Exercise { get; set; }
        public virtual User User { get; set; }
    }
}
