using System;

namespace BC.ACCOUNTING.REPORT.Models
{
    public class OtpEntry
    {
        public string OtpCode { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class OtpRequestModel
    {
        public required string Username { get; set; }
    }
    public class OtpVerifyModel
    {
        public string Username { get; set; }
        public string Otp { get; set; }
    }
}
