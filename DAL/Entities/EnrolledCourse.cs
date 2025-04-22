using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class EnrolledCourse : BaseEntity
    {

        public bool IsCompleted { get; set; }
        public decimal ProgressPoints { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Guid UserId { get; set; }
        public Guid CourseId { get; set; }

        // Navigation properties
        public  User User { get; set; }
        public  Course Course { get; set; }
        public virtual ICollection<TopicProgress> TopicProgresses { get; set; }
    }

}
