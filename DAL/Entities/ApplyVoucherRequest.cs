using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class ApplyVoucherRequest
    {
        public string VoucherCode { get; set; }    // Mã voucher người dùng nhập
        public decimal TotalAmount { get; set; }   // Tổng số tiền người dùng muốn thanh toán
    }
}
