using Common.DTO;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IVnPayService
    {
        /// <summary>
        /// Create Payment Request To VnPay (Không áp dụng voucher)
        /// </summary>
        /// <param name="paymentInfo">Thông tin thanh toán</param>
        /// <param name="context">Context HTTP</param>
        /// <returns>URL thanh toán</returns>
    
        /// <summary>
        /// Create Payment Request To VnPay with Voucher Applied
        /// </summary>
        /// <param name="paymentInfo">Thông tin thanh toán</param>
        /// <param name="voucherCode">Mã voucher</param>
        /// <param name="context">Context HTTP</param>
        /// <returns>URL thanh toán sau khi áp dụng voucher</returns>
        Task<string> CreatePaymentUrl(PaymentRequestDTO paymentInfo, string voucherCode, HttpContext context);

        /// <summary>
        /// Tạo thanh toán để nâng cấp Premium
        /// </summary>
        /// <param name="userId">ID người dùng</param>
        /// <param name="premiumPackage">Gói Premium</param>
        /// <param name="context">HttpContext</param>
        /// <returns>URL thanh toán</returns>
        Task<string> CreatePremiumPaymentUrl(string userId, string premiumPackage, HttpContext context);
    }


}
