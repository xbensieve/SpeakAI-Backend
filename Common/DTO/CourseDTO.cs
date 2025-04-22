using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO
{
    public class CourseDTO
    {
        public Guid Id { get; set; }
        public string CourseName { get; set; }
        public string Description { get; set; }
        public decimal MaxPoint { get; set; }
        public bool IsPremium { get; set; }
        public bool IsActive { get; set; }
        public int LevelId { get; set; }
        public List<TopicDetailDTO> Topics { get; set; }
    }
}
