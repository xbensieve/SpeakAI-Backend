using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class User : BaseEntity
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public DateTime Birthday { get; set; }
        public string Gender { get; set; }
        public bool Status { get; set; }
        public DateTime LastLogin { get; set; }
        public byte[] PasswordHash { get; set; } = new byte[32];
        public byte[] PasswordSalt { get; set; } = new byte[32];
        public bool IsPremium { get; set; }
        public string? Otp { get; set; }
        public DateTime? OtpExpiredTime { get; set; }
        public DateTime? PremiumExpiredTime { get; set; }
        public bool IsVerified { get; set; }
        public Guid? UserLevelId { get; set; }
        public bool IsAdmin { get; set; } = false;
        public bool IsLocked { get; set; } = false;

        // Navigation properties
        public virtual ICollection<UserLevel> UserLevel { get; set; }
        public virtual ICollection<EnrolledCourse> EnrolledCourses { get; set; }
        public virtual ICollection<TopicProgress> TopicProgresses { get; set; }
        public virtual ICollection<ExerciseProgress> ExerciseProgresses { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
        public virtual ICollection<Order> Order { get; set; }

        public virtual ICollection<Voucher> Voucher { get; set; }
    }
}
