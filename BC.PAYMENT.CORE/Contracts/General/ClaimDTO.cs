namespace BC.PAYMENT.CORE.Contracts.General
{
    public record ClaimDTO
    {
        public int UserId { get; set; }
        public string? Username { get; set; }
        public string CompanyCode { get; set; }
        public string? AppCode { get; set; }
        public string? DbCode { get; set; }
        public DateTime CurrectDate { get; set; }
        public string? InvoiceEntryCode { get; set; }
        public string? Period { get; set; }
        public string? Role { get; set; }
    }
}
