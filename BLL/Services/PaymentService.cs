using BLL.Interface;
using BLL.IService;
using Common.Constant.Payment;
using Common.DTO;
using Common.DTO.Payment;
using DAL.Entities;
using DAL.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Service.IService;

namespace Service.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly ITransactionService _transactionService;
        private readonly IVnPayService _vnpPayService;
        private readonly IUserService _userService;
        private readonly IVoucherService _voucherService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPremiumSubscriptionService _premiumSubscriptionService;
        public PaymentService(ITransactionService transactionService,
                              IVnPayService vnpPayService,
                              IUserService userService,
                              IVoucherService voucherService,
                                IUnitOfWork unitOfWork,
                                IPremiumSubscriptionService premiumSubscriptionService)
        {
            _transactionService = transactionService;
            _vnpPayService = vnpPayService;
            _userService = userService;
            _voucherService = voucherService;
            _unitOfWork = unitOfWork;
            _premiumSubscriptionService=premiumSubscriptionService;
        }



        public async Task<string> CreatePaymentRequest(PaymentRequestDTO paymentInfo, HttpContext context)
        {
            // Lấy thông tin Order từ OrderId
            var order = await _unitOfWork.Order
                .FindAll(o => o.Id == paymentInfo.OrderId)
                .FirstOrDefaultAsync();

            if (order == null)
            {
                return "Order không tồn tại hoặc không ở trạng thái Pending.";
            }


            var userId = order.UserId;
            var totalPrice = order.TotalAmount;
            var paymentMethod = "VnPay";

            string? voucherName = null;
            if (!string.IsNullOrEmpty(paymentInfo.VoucherCode))
            {
                var voucher = await _voucherService.GetVoucherByCode(paymentInfo.VoucherCode);
                if (voucher != null && voucher.IsVoucherValid(totalPrice, DateTime.UtcNow))
                {
                    var discount = voucher.CalculateDiscount(totalPrice);
                    totalPrice -= discount;
                    voucherName = voucher.VoucherCode;
                    voucher.RemainingQuantity--;
                    await _unitOfWork.SaveChangeAsync();
                }

            }
            var unpaidTrans = _transactionService.GetLastTransOfUser(userId);
            if (unpaidTrans != null && unpaidTrans.Status == PaymentConstant.PendingStatus)
            {
                unpaidTrans.Status = PaymentConstant.CancelStatus;
                await _transactionService.UpdateTransaction(unpaidTrans.TransactionId, unpaidTrans);
            }


            var transaction = new Transaction
            {
                PaymentMethod = paymentMethod,
                TransactionDate = DateTime.UtcNow,
                Amount = (decimal)totalPrice,
                TransactionInfo = PaymentConstant.UnSet,
                TransactionNumber = PaymentConstant.UnSet,
                TransactionType = "",
                TransactionReference = "",
                TransactionId = Guid.NewGuid(),
                Status = PaymentConstant.PendingStatus,
                UserId = userId,
                OrderId = paymentInfo.OrderId,
                VoucherName = voucherName
            };

            await _transactionService.AddNewTransaction(transaction);
            await _unitOfWork.SaveChangeAsync();

            var paymentRequest = new PaymentRequestDTO
            {
                OrderId = paymentInfo.OrderId,
                VoucherCode = paymentInfo.VoucherCode
            };

            return await _vnpPayService.CreatePaymentUrl(paymentRequest, paymentInfo.VoucherCode ?? "", context);
        }


        public async Task<bool> HandlePaymentResponse(PaymentResponseDTO response)
        {
            var unpaidTrans = _transactionService.GetLastTransOfUser(response.UserId);

            if (unpaidTrans != null && unpaidTrans.Status == PaymentConstant.PendingStatus)
            {
                string notifyDes;

                if (response.IsSuccess)
                {
                    // Cập nhật trạng thái giao dịch thành Paid
                    unpaidTrans.Status = PaymentConstant.PaidStatus;
                    unpaidTrans.TransactionInfo = response.TransactionInfo;
                    unpaidTrans.TransactionNumber = response.TransactionNumber;
                    notifyDes = PaymentConst.SUCCESS;

                    // Gọi ConfirmPremiumUpgrade tự động
                    var confirmResult = await _premiumSubscriptionService.ConfirmPremiumUpgrade(unpaidTrans.OrderId);
                    if (!confirmResult.IsSuccess)
                    {
                        // Nếu confirm thất bại, có thể log lỗi hoặc xử lý thêm
                        // Ví dụ: throw new Exception("Failed to confirm premium: " + confirmResult.Message);
                        return false; // Hoặc xử lý theo logic của bạn
                    }
                }
                else
                {
                    unpaidTrans.TransactionInfo = response.TransactionInfo;
                    unpaidTrans.TransactionNumber = response.TransactionNumber;
                    unpaidTrans.Status = PaymentConstant.CancelStatus;
                    notifyDes = PaymentConst.CancelStatus;
                }

                await _transactionService.UpdateTransaction(unpaidTrans.TransactionId, unpaidTrans);
                await _unitOfWork.SaveChangeAsync();
                return true;
            }

            return false;
        }
    }
}


        //public async Task<(bool isSuccess, string message, decimal discountAmount)> ApplyVoucher(ApplyVoucherRequest request)
        //{
        //    var voucher = await _voucherService.GetVoucherByCode(request.VoucherCode);

        //    if (voucher == null)
        //        return (false, PaymentConst.VoucherInvalid, 0);

        //    if (!voucher.IsActive || voucher.EndDate < DateTime.UtcNow)
        //        return (false, PaymentConst.VoucherExpired, 0);

        //    // Kiểm tra số tiền tối thiểu để sử dụng voucher
        //    if (voucher.MinPurchaseAmount > request.TotalAmount)
        //        return (false, PaymentConst.VoucherNotEligible, 0);

        //    // Tính số tiền giảm giá
        //    decimal discountAmount = 0;

        //    if (voucher.DiscountAmount > 0)
        //    {
        //        // Nếu voucher có số tiền giảm giá cố định
        //        discountAmount = voucher.DiscountAmount;
        //    }
        //    else if (voucher.DiscountPercentage > 0)
        //    {
        //        // Nếu voucher có tỷ lệ giảm giá
        //        // Nếu voucher có tỷ lệ giảm giá
        //        discountAmount = request.TotalAmount * (decimal)(voucher.DiscountPercentage / 100);

        //    }

        //    // Kiểm tra nếu discountAmount lớn hơn số tiền thanh toán
        //    if (discountAmount > request.TotalAmount)
        //    {
        //        // Trường hợp tính toán được số tiền giảm giá lớn hơn số tiền thanh toán, ta chỉ giảm đến mức số tiền thanh toán
        //        discountAmount = request.TotalAmount;
        //    }

        //    return (true, PaymentConst.VoucherApplied, discountAmount);
        //}

        //public async Task<PaymentHistory?> GetPaymentByTransactionId(string transactionId)
        //{
        //    if (!Guid.TryParse(transactionId, out Guid transId))
        //    {
        //        throw new ArgumentException("Transaction ID không hợp lệ");
        //    }

        //    var transaction = await _transactionService.GetTransactionById(transId);
        //    if (transaction == null) return null;

        //    return new PaymentHistory
        //    {
        //        Id = Guid.NewGuid().ToString(), // Tạo ID mới nếu cần
        //        TransactionId = transaction.TransactionId.ToString(), // Chuyển Guid thành string nếu cần
        //        Amount = transaction.Amount,
        //        Status = transaction.Status,
        //        PaymentMethod = transaction.PaymentMethod,
        //        CreatedAt = transaction.TransactionDate, 
        //        UserId = transaction.UserId.ToString(), 
        //        PaymentDescription = transaction.TransactionInfo, 
        //        PaymentType = transaction.PaymentMethod 
        //    };
        //}

        //public async Task SavePaymentHistory(string transactionId, decimal amount, string status)
        //{
        //    if (!Guid.TryParse(transactionId, out Guid transId))
        //    {
        //        throw new ArgumentException("Transaction ID không hợp lệ");
        //    }

        //    var transaction = await _transactionService.GetTransactionById(transId);
        //    if (transaction != null)
        //    {
        //        transaction.Status = status;
        //        transaction.Amount = amount;
        //        await _transactionService.UpdateTransaction(transId, transaction);
        //    }
        //}

        //public async Task UpgradeUserToPremium(string userId, string package)
        //{
        //    var user = await _userService.GetUserById(Guid.Parse(userId));
        //    if (user != null)
        //    {
        //        user.IsPremium = true;
        //        user.PremiumExpiredTime = DateTime.Now.AddMonths(1); // Hoặc dựa vào package
        //        await _userService.UpdateUser(user);
        //    }
        //}






