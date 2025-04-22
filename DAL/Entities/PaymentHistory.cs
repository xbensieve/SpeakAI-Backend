using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities
{
    [Table("PaymentHistories")]
    public class PaymentHistory
    {
        [Key]
        public string Id { get; set; }
        public string TransactionId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }


        public string UserId { get; set; }
        public string PaymentDescription { get; set; }
        public string PaymentType { get; set; }
    }
}
