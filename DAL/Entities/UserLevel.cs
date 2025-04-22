using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class UserLevel : BaseEntity
    {

        public decimal Point { get; set; } 
        public string LevelName { get; set; }
        public DateTime UpdatedAt { get; set; }
        [Key] 
        public int LevelId { get; set; }
        public Guid UserId { get; set; }

        // Navigation properties
        public virtual Level Level { get; set; }
        public virtual User Users { get; set; }
    }
}
