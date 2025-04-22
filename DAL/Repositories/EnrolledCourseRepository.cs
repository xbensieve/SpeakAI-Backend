using DAL.Data;
using DAL.Entities;
using DAL.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{

    public class EnrolledCourseRepository : GenericRepository<EnrolledCourse>, IEnrolledCourseRepository
    {
        public EnrolledCourseRepository(SpeakAIContext speakAIContext) : base(speakAIContext) { }
    }

}
