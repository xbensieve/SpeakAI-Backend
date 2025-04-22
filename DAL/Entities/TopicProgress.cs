using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class TopicProgress : BaseEntity
    {
        
        public decimal ProgressPoints { get; set; }
        public bool IsCompleted { get; set; }
 
        public DateTime UpdatedAt { get; set; }
        public Guid EnrolledCourseId { get; set; }
        public Guid TopicId { get; set; }
        public Guid UserId { get; set; }

        // Navigation properties
        public virtual EnrolledCourse EnrolledCourse { get; set; }
        public virtual Topic Topic { get; set; }
        public virtual User User { get; set; }
    }


}
