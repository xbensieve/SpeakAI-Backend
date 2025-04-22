using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO
{
    public class VoucherResponseDTO
    {
        public Guid VoucherId { get; set; }
        public string VoucherCode { get; set; }
        public string Description { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public bool IsActive { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool Status { get; set; }
        public int RemainingQuantity { get; set; }
    }
}
