using DAL.Entities;
using System.ComponentModel.DataAnnotations;

public class Transaction : BaseEntity
{
    [Key]
    public Guid TransactionId { get; set; }
    public Guid UserId { get; set; }
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public string TransactionType { get; set; }
    public string TransactionInfo { get; set; } = null!;
    public string TransactionNumber { get; set; }
    public string Status { get; set; }
    public DateTime TransactionDate { get; set; }
    public string PaymentMethod { get; set; }
    public string TransactionReference { get; set; }

    // Navigation properties
    public virtual User User { get; set; }
    public virtual Order Order { get; set; }

   public Guid? VoucherId { get; set; }
    public Voucher Voucher { get; set; }
    public string? VoucherName { get; set; }

    
}
