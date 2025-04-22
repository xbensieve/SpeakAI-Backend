using DAL.Entities;

public class Voucher
{
    public Guid VoucherId { get; set; }
    public string VoucherCode { get; set; }
    public string Description { get; set; }
    public decimal DiscountAmount { get; set; }
    public double DiscountPercentage { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
    public decimal MinPurchaseAmount { get; set; }
    public string VoucherType { get; set; }

    public User User { get; set; }
    public Guid? UserId { get; set; }
    //public string Status { get; set; } = "Active";
    public bool Status { get; set; }


    public int RemainingQuantity { get; set; }





    public bool IsVoucherValid(decimal purchaseAmount, DateTime currentDate)
    {
        bool isValid = IsActive &&
                       currentDate >= StartDate &&
                       currentDate <= EndDate &&
                       purchaseAmount >= MinPurchaseAmount &&
                       RemainingQuantity > 0;

        Console.WriteLine($"[DEBUG] Voucher Valid Check: IsActive={IsActive}, StartDate={StartDate}, EndDate={EndDate}, Now={currentDate}, MinPurchaseAmount={MinPurchaseAmount}, PurchaseAmount={purchaseAmount}, RemainingQuantity={RemainingQuantity}, Result={isValid}");

        return isValid;
    }


    public decimal CalculateDiscount(decimal purchaseAmount)
    {
        if (DiscountPercentage > 0)
        {
            return purchaseAmount * (decimal)(DiscountPercentage / 100);
        }
        return DiscountAmount > 0 ? DiscountAmount : 0;
    }
}
