using System.Transactions;
using Common.DTO;
using Common.DTO.Payment;
using DAL.Entities;
using Microsoft.AspNetCore.Http;

namespace Service.IService
{
    public interface IPaymentService
    {
        /// <summary>
        /// Create payment request
        /// </summary>
        /// <param name="paymentInfo"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        Task<string> CreatePaymentRequest(PaymentRequestDTO paymentInfo, HttpContext context);

        /// <summary>
        /// Handle payment response from payment method
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        Task<bool> HandlePaymentResponse(PaymentResponseDTO response);

        //Task<(bool isSuccess, string message, decimal discountAmount)> ApplyVoucher(ApplyVoucherRequest request);

        //Task<PaymentHistory?> GetPaymentByTransactionId(string transactionId);
        //Task SavePaymentHistory(string transactionId, decimal amount, string status);
        //Task UpgradeUserToPremium(string userId, string package);




    }
}
