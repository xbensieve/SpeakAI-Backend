namespace Common.Constant.Payment
{
    public static class PaymentConst
    {
        // Payment method
        public const string VnPay = "VnPay";

        // Payment status
        public const string PendingStatus = "Đang chờ";
        public const string UnPaidStatus = "Chưa thanh toán";
        public const string PaidStatus = "Đã thanh toán";
        public const string CancelStatus = "Huỷ";

        // voucher
        public const string VoucherApplied = "Voucher đã áp dụng";
        public const string VoucherInvalid = "Voucher không hợp lệ";
        public const string VoucherExpired = "Voucher đã hết hạn";
        public const string VoucherNotEligible = "Không đủ điều kiện sử dụng voucher";

        // Unset
        public const string UnSet = "None";

        // message
        public const string PENDING = "Đang chờ thanh toán";
        public const string SUCCESS = "Thanh toán thành công";
        public const string FAIL = "Thanh toán thất bại";
        public const string INVALID_TRANS = "Giao dịch không hợp lệ";

        // description
        public const string PAYMENT_DESCRIPTION = "Mua ";
    }
}
