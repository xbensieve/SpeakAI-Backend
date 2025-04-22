using Common.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interface
{
    public interface IEmailService
    {
        OtpCodeDTO GenerateOTP();
        void SendOTPEmail(string userEmail, string userName, string otpCode, string subject);
        void SendWelcomeEmail(string userEmail, string userName, string subject);
        void SendPremiumConfirmationEmail(string userEmail, string userName);
        void SendPremiumPurchaseReceiptEmail(string userEmail, string userName, decimal amount, string orderId);
    }
}
