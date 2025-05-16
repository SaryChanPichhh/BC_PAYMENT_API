namespace BC.PAYMENT.API.Helper
{
    public class AppSettings
    {
        public required string Secret { get; set; }
        public required string PYS_Key { get; set; }
        public required string HR_Key { get; set; }
        public required string ACC_Key { get; set; }
        public required string Issuer { get; set; }
        public required string Audience { get; set; }
        public required string Subject { get; set; }
        public required string AES_KEY { get; set; }
        public required string AES_IV { get; set; }
    }
}
