using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO
{
    public class TransactionDTO
    {
        public Guid TransactionId { get; set; }
        public Guid OrderId { get; set; }
        public string TransactionNumber { get; set; } = null!;
        public string PaymentMethod { get; set; } = null!;
        public string TransactionInfo { get; set; } = null!;
        public DateTime? TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = null!;
        public string Email { get; set; } = null!;
        public Guid UserId { get; set; }
   
    }
}
