using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO
{
    public class SubmitExerciseResponseDTO
    {
        public Guid ExerciseId { get; set; }
        public decimal ExerciseEarnedPoints { get; set; }
        public bool IsExerciseCompleted { get; set; }

        public Guid TopicId { get; set; }
        public decimal TopicTotalPoints { get; set; }
        public bool IsTopicCompleted { get; set; }

        public Guid CourseId { get; set; }
        public decimal CourseTotalPoints { get; set; }
        public bool IsCourseCompleted { get; set; }
    }
}
