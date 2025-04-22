using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO
{
    public class CreateCourseDTO
    {
        public string CourseName { get; set; }
        public string Description { get; set; }
        public decimal MaxPoint { get; set; }
        public bool IsFree { get; set; }
        public bool IsPremium { get; set; }
        public int LevelId { get; set; }
        public List<CreateTopicDTO> Topics { get; set; }
    }
}
