namespace BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DailyPayment
{
    public class PaidInvoiceModel 
    {
        public string? DbCode { get; set; }
        public string? DeliveryName { get; set; }
        public string? InvoiceId { get; set; }
        public string? PaymentId { get; set; }
        public string? DividedInvoiceId { get; set; }
        public string? CustomerCode { get; set; }
        public string? CustomerName { get; set; }
        public string? TransactionCode { get; set; }
        public string? Amount { get; set; }
        public string? InvoiceValue { get; set; }
        public string? HalfPaid { get; set; }
        public string? FullPaid { get; set; }
        public string? Total { get; set; }
    }
}
