namespace BC.PAYMENT.CORE.DTO.Invoice
{
    public record NewInvoicesRequestDTO
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
        public InvoiceTypes InvoiceTypes { get; set; }

        [JsonIgnore]
        public string? EntriesCode { get; set; }

        [JsonIgnore]
        public string? Username { get; set; }
    }

}
