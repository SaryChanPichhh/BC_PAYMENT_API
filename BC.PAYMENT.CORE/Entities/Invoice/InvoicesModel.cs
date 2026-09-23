namespace BC.PAYMENT.CORE.Entities.Invoice
{
    public class InvoicesModel : Customer
    {
        public int InvoiceId { get; set; }
        public int TemplateId { get; set; }
        public string? DbCode { get; set; }
        public string? InvoiceCode { get; set; }
        public double InvoiceAmount { get; set; }
        public InvoiceType InvoiceType { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CustomField1 { get; set; }
        public bool? IsDivided { get; set; }
        public string? EntriesCode { get; set; }
        public string? DeliveryName { get; set; }
        public string? Description { get; set; }
        public int Period { get; set; }
        public DateTime TransactionDate { get; set; }
        public string? CreatedBy { get; set; }
    }

    public class StockCarInvoiceModel : Invoice
    {
        public int Period { get; set; }
        public string? CreatedBy { get; set; }
        public string? EmployeeId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime TransactionDate { get; set; }
        public InvoiceStatus InvoiceType { get; set; }
    }
}
