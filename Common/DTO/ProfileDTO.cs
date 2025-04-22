using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO
{
    public  class ProfileDTO
    {
        public string? UserName { get; set; } 
        public string? UserId { get; set; }
        public string? FullName { get; set; } 
        public string? Gender { get; set; }
        public string? Phone { get; set; } 
        public string? Email { get; set; }

        public string? Birthday { get; set; } 
        public string? Password { get; set; }
        public string? Address { get; set; } 
    }
}
