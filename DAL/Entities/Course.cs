using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class Course : BaseEntity
    {
     
        public string CourseName { get; set; }
        public string Description { get; set; }
        public decimal MaxPoint { get; set; }
        public bool IsLock { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsPremium { get; set; } = false;

        public int LevelId { get; set; }
        public int DisplayOrder { get; set; }
        // Navigation properties
        public virtual Level Level { get; set; }
        public virtual ICollection<EnrolledCourse> EnrolledCourses { get; set; }
        public virtual ICollection<Topic> Topics { get; set; }
    }
}
    