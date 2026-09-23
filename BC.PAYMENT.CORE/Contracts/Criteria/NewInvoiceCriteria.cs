namespace BC.PAYMENT.CORE.Contracts.Criteria
{
    public record NewInvoiceCriteria
    {
        [JsonIgnore]
        public string? DbCode { get; set; }
        [Required]
        public required string InvoiceCode1 { get; set; }
        [Required]
        public required string InvoiceCode2 { get; set; }
        [Required]
        public DateTime FromDate { get; set; }  
        [Required]
        public DateTime ToDate { get; set; }
        [Required]
        public InvoiceStatus InvoiceType { get; set; }
        [JsonIgnore]
        public string? EntriesCode { get; set; }
        [JsonIgnore]
        public string? Username { get; set; }
    }

}
