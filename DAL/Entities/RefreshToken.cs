using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class RefreshToken : BaseEntity
    {
       
        public string JwtId { get; set; } =  string.Empty;
        public string Refresh_Token { get; set; }
        public bool IsValid { get; set; }
        public DateTime ExpiredAt { get; set; }
        [ForeignKey("UserId")]
        public Guid UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
        public DateTime CreateAt { get; set; }
    }
}
