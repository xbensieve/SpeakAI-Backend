using System.Data;

namespace Common.Constant.Payment
{
    public static class PaymentConstant
    {
        // Payment method
        public const string VnPay = "VnPay";

        // Payment status
        public const string PendingStatus = "Pending";
        public const string UnPaidStatus = "UnPaid";
        public const string PaidStatus = "Paid";
        public const string CancelStatus = "Cancel";

        // Unset
        public const string UnSet = "None";
    }
}
