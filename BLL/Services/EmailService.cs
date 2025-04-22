using BLL.Interface;
using Common.DTO;
using Common.Template;
using DAL.UnitOfWork;
using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Net.Mail;

namespace BLL.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly Random random = new Random();
        private readonly IUnitOfWork _unitOfWork;
        public EmailService(IConfiguration configuration,
            IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Generate OTP code which has 6 characters
        /// </summary>
        /// <returns></returns>
        public OtpCodeDTO GenerateOTP ()
        {
            int otpCode = random.Next(100000, 1000000);
            var otpDto = new OtpCodeDTO
            {
                 OTPCode = otpCode.ToString(),
                 ExpiredTime = DateTime.Now.AddMinutes(15),
            };
            return otpDto;
        }

        /// <summary>
        /// Send otp code
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="userName"></param>
        /// <param name="otpCode"></param>
        public void SendOTPEmail(string userEmail, string userName, string otpCode, string subject)
        {
            var sendEmail = _configuration.GetSection("SendEmailAccount")["Email"];
            var toEmail = userEmail;
            var htmlBody = EmailTemplate.OTPEmailTemplate(userName, otpCode, subject);
            MailMessage mailMessage = new MailMessage(sendEmail, toEmail, subject, htmlBody);
            mailMessage.IsBodyHtml = true;
            
            var smtpServer = _configuration.GetSection("SendEmailAccount")["SmtpServer"];
            int.TryParse(_configuration.GetSection("SendEmailAccount")["Port"], out int port);
            var userNameEmail = _configuration.GetSection("SendEmailAccount")["UserName"];
            var password = _configuration.GetSection("SendEmailAccount")["Password"];

            SmtpClient client = new SmtpClient(smtpServer, port);
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(userNameEmail, password);
            client.EnableSsl = true; // Enable SSL/TLS encryption

            client.Send(mailMessage);
        }

        /// <summary>
        /// Send welcome email
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="userName"></param>
        /// <param name="otpCode"></param>
        /// <param name="subject"></param>
        public void SendWelcomeEmail(string userEmail, string userName, string subject)
        {
            var sendEmail = _configuration.GetSection("SendEmailAccount")["Email"];
            var toEmail = userEmail;
            var htmlBody = EmailTemplate.WelcomeEmailTemplate(userName, subject);
            MailMessage mailMessage = new MailMessage(sendEmail, toEmail, subject, htmlBody);
            mailMessage.IsBodyHtml = true;

            var smtpServer = _configuration.GetSection("SendEmailAccount")["SmtpServer"];
            int.TryParse(_configuration.GetSection("SendEmailAccount")["Port"], out int port);
            var userNameEmail = _configuration.GetSection("SendEmailAccount")["UserName"];
            var password = _configuration.GetSection("SendEmailAccount")["Password"];

            SmtpClient client = new SmtpClient(smtpServer, port);
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(userNameEmail, password);
            client.EnableSsl = true; // Enable SSL/TLS encryption

            client.Send(mailMessage);
        }
        /// <summary>
        /// Send confirmation email after successful Premium upgrade
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="userName"></param>
        public void SendPremiumConfirmationEmail(string userEmail, string userName)
        {
            var sendEmail = _configuration.GetSection("SendEmailAccount")["Email"];
            var subject = "Welcome to Premium Membership!";
            var htmlBody = EmailTemplate.PremiumUpgradeTemplate(userName, subject);

            MailMessage mailMessage = new MailMessage(sendEmail, userEmail, subject, htmlBody);
            mailMessage.IsBodyHtml = true;

            var smtpServer = _configuration.GetSection("SendEmailAccount")["SmtpServer"];
            int.TryParse(_configuration.GetSection("SendEmailAccount")["Port"], out int port);
            var userNameEmail = _configuration.GetSection("SendEmailAccount")["UserName"];
            var password = _configuration.GetSection("SendEmailAccount")["Password"];

            SmtpClient client = new SmtpClient(smtpServer, port);
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(userNameEmail, password);
            client.EnableSsl = true;
            client.Send(mailMessage);
        }

        /// <summary>
        /// Send receipt email after Premium purchase
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="userName"></param>
        /// <param name="amount"></param>
        /// <param name="orderId"></param>
        public void SendPremiumPurchaseReceiptEmail(string userEmail, string userName, decimal amount, string orderId)
        {
            var sendEmail = _configuration.GetSection("SendEmailAccount")["Email"];
            var subject = "Premium Subscription Purchase Receipt";
            var htmlBody = EmailTemplate.PremiumReceiptTemplate(userName, amount, orderId, subject);

            MailMessage mailMessage = new MailMessage(sendEmail, userEmail, subject, htmlBody);
            mailMessage.IsBodyHtml = true;

            var smtpServer = _configuration.GetSection("SendEmailAccount")["SmtpServer"];
            int.TryParse(_configuration.GetSection("SendEmailAccount")["Port"], out int port);
            var userNameEmail = _configuration.GetSection("SendEmailAccount")["UserName"];
            var password = _configuration.GetSection("SendEmailAccount")["Password"];

            SmtpClient client = new SmtpClient(smtpServer, port);
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(userNameEmail, password);
            client.EnableSsl = true;
            client.Send(mailMessage);
        }
    
}
}
