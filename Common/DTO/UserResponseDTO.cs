using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO
{
    public class UserResponseDTO
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        
        public string Email { get; set; }
        public string FullName { get; set; }
        public DateTime Birthday { get; set; }
        public string Gender { get; set; }
        public DateTime? PremiumExpiredTime { get; set; }
        public decimal Point { get; set; }
        public string LevelName { get; set; }
        public bool IsPremium { get; set; }
        public bool IsVerified { get; set; }





    }
}
