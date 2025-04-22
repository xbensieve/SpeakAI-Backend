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

    public class UserLevelRepository : GenericRepository<UserLevel>, IUserLevelRepository
    {
        public UserLevelRepository(SpeakAIContext speakAIContext) : base(speakAIContext) { }
    }

}
