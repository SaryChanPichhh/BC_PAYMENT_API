namespace BC.PAYMENT.CORE.DTO.Invoice
{
    public record NewInvoiceDTO
    {
        public string DbCode { get; set; }
        public string InvoiceCode { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public double InvoiceAmount { get; set; }
        public InvoiceTypes InvoiceStatus { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public bool IsDivided { get; set; }
        public string EntriesCode { get; set; }
    }
}
