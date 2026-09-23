namespace BC.PAYMENT.CORE.Entities.Invoice
{
    public class NewInvoiceModel : Customer
    {
        public int InvoiceId { get; set; }
        public int Id { get => InvoiceId; set => InvoiceId = value; }
        public string? InvoiceCode { get; set; }
        public double InvoiceAmount { get; set; }
        public InvoiceStatus InvoiceType { get; set; }
        public string? DbCode { get; set; }
        public string? CreatedBy { get; set; }
        public string? EntriesCode { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CustomerField { get; set; } = string.Empty;
        public string AccNameKh { get; set; } = string.Empty;
    }
}
