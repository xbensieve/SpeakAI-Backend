using Common.DTO.Payment;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Service.IService;
using System.Net.Sockets;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Common.DTO;
using DAL.UnitOfWork;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Service.Service
{
    public class VnPayService : IVnPayService
    {
        private readonly SortedList<string, string> _requestData = new SortedList<string, string>(new VnPayCompare());
        private readonly IConfiguration _configuration;
        private readonly IVoucherService _voucherService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<VnPayService> _logger;

        public VnPayService(IConfiguration configuration,
                           IVoucherService voucherService,
                           IUnitOfWork unitOfWork,
                           ILogger<VnPayService> logger)
        {
            _configuration = configuration;
            _voucherService = voucherService;
            _unitOfWork = unitOfWork;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<decimal> ApplyVoucherAsync(decimal totalPrice, string voucherCode)
        {
            if (string.IsNullOrWhiteSpace(voucherCode))
            {
                return totalPrice;
            }

            _logger.LogInformation($"Checking voucher: {voucherCode}");

            var voucher = await _voucherService.GetVoucherByCode(voucherCode);
            if (voucher == null || !voucher.IsActive)
            {
                _logger.LogError($"Voucher {voucherCode} is invalid or inactive.");
                throw new Exception("Voucher không hợp lệ hoặc đã hết hạn.");
            }

            if (!voucher.IsVoucherValid(totalPrice, DateTime.UtcNow))
            {
                throw new Exception("Voucher không hợp lệ hoặc đã hết hạn.");
            }

            decimal discount = voucher.CalculateDiscount(totalPrice);
            totalPrice -= discount;
            voucher.RemainingQuantity--;

            _logger.LogInformation($"Applied discount: {discount}, New total: {totalPrice}, Remaining: {voucher.RemainingQuantity}");
            return totalPrice;
        }

        public async Task<string> CreatePaymentUrl(PaymentRequestDTO paymentInfo, string voucherCode, HttpContext context)
        {
            var order = await _unitOfWork.Order
                .FindAll(o => o.Id == paymentInfo.OrderId)
                .FirstOrDefaultAsync();

            if (order == null )
            {
                throw new Exception("Order không tồn tại hoặc không ở trạng thái Pending.");
            }

            decimal totalPrice = order.TotalAmount;

            if (!string.IsNullOrEmpty(voucherCode))
            {
                totalPrice = await ApplyVoucherAsync(totalPrice, voucherCode);
            }

            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]!);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
            var tick = DateTime.UtcNow.Ticks.ToString();
            var urlCallBack = _configuration["PaymentCallBack:ReturnUrl"];

            AddRequestData("vnp_Version", _configuration["Vnpay:Version"]!);
            AddRequestData("vnp_Command", _configuration["Vnpay:Command"]!);
            AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]!);
            AddRequestData("vnp_Amount", ((int)(totalPrice * 100000)).ToString());
            AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]!);
            AddRequestData("vnp_IpAddr", GetIpAddress(context));
            AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]!);
            AddRequestData("vnp_OrderInfo", "Upgrade premium");
            AddRequestData("vnp_OrderType", "Upgrade premium");
            AddRequestData("vnp_ReturnUrl", urlCallBack!);
            AddRequestData("vnp_TxnRef", tick);

            return CreateRequestUrl(_configuration["Vnpay:BaseUrl"]!, _configuration["Vnpay:HashSecret"]!);
        }

        private string GetIpAddress(HttpContext context)
        {
            try
            {
                var remoteIpAddress = context.Connection.RemoteIpAddress;
                if (remoteIpAddress != null)
                {
                    if (remoteIpAddress.AddressFamily == AddressFamily.InterNetworkV6)
                    {
                        remoteIpAddress = Dns.GetHostEntry(remoteIpAddress).AddressList
                            .FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
                    }

                    return remoteIpAddress?.ToString() ?? "127.0.0.1";
                }
            }
            catch
            {
                return "127.0.0.1";
            }

            return "127.0.0.1";
        }

        private void AddRequestData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _requestData[key] = value;
            }
        }

        private string CreateRequestUrl(string baseUrl, string vnpHashSecret)
        {
            var data = new StringBuilder();

            foreach (var (key, value) in _requestData.Where(kv => !string.IsNullOrEmpty(kv.Value)))
            {
                data.Append(WebUtility.UrlEncode(key) + "=" + WebUtility.UrlEncode(value) + "&");
            }

            var querystring = data.ToString().TrimEnd('&');

            var vnpSecureHash = HmacSha512(vnpHashSecret, querystring);
            return $"{baseUrl}?{querystring}&vnp_SecureHash={vnpSecureHash}";
        }

        private string HmacSha512(string key, string inputData)
        {
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key));
            return BitConverter.ToString(hmac.ComputeHash(Encoding.UTF8.GetBytes(inputData))).Replace("-", "").ToLower();
        }

        public async Task<string> CreatePremiumPaymentUrl(string userId, string premiumPackage, HttpContext context)
        {
            try
            {
                _logger.LogInformation($"Bắt đầu tạo URL thanh toán cho UserId: {userId}, Gói: {premiumPackage}");

                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(premiumPackage))
                {
                    _logger.LogWarning("UserId hoặc PremiumPackage không hợp lệ.");
                    throw new ArgumentException("Thông tin nâng cấp không hợp lệ.");
                }

                if (!Guid.TryParse(userId, out Guid parsedUserId))
                {
                    _logger.LogWarning($"UserId không hợp lệ: {userId}");
                    throw new ArgumentException("UserId không hợp lệ.");
                }

                decimal price = 100000;
                if (price <= 0)
                {
                    _logger.LogWarning($"Gói Premium không hợp lệ: {premiumPackage}");
                    throw new ArgumentException("Gói Premium không hợp lệ.");
                }

                _logger.LogInformation($"Giá của gói {premiumPackage} là {price} VND.");

                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    UserId = parsedUserId,
                    TotalAmount = price,
                    OrderDate = DateTime.UtcNow,
                };
                await _unitOfWork.Order.AddAsync(order);
                await _unitOfWork.SaveChangeAsync();

                var paymentRequest = new PaymentRequestDTO
                {
                    OrderId = order.Id
                };

                string paymentUrl = await CreatePaymentUrl(paymentRequest, null, context);

                _logger.LogInformation($"URL thanh toán tạo thành công: {paymentUrl}");
                return paymentUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi tạo URL thanh toán: {ex.Message}");
                throw;
            }
        }

      
    }
}