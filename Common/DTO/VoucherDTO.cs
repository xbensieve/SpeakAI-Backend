namespace Common.DTO
{
    public class VoucherDTO
    {
        public string VoucherCode { get; set; }  
        public string Description { get; set; }  
        public int? DiscountPercentage { get; set; } 
        public bool IsActive { get; set; }  
        public DateTime StartDate { get; set; }  
        public DateTime EndDate { get; set; }  

        public bool Status { get; set; }  = false;

        public int RemainingQuantity { get; set; }

        public string VoucherType { get; set; } 
    }
}
