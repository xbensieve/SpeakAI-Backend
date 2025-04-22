using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO
{
    public class EnrolledCourseDetailsDTO
    {
        public CourseDTO Course { get; set; }
        public decimal Progress { get; set; }
        public ICollection<TopicProgressDTO> Topics { get; set; }
    }

}
