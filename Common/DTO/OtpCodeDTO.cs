namespace Common.DTO
{
    public class OtpCodeDTO
    {
        public string OTPCode { get; set; } = null!;
        public DateTime ExpiredTime { get; set; }
    }
}
