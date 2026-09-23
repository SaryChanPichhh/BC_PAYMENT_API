namespace BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DividingInvoices.Invoice
{
    public class DividedInvoiceModel
    {
        public string? Id { get; set; }
        public string? DeliveryName { get; set; }
        public string? CustomerCode { get; set; }
        public string? CustomerName { get; set; }
        public string? Store { get; set; }
        public string? TransactionCode { get; set; }
        public double Amount { get; set; }
        public double InvoiceValue { get; set; }
        public string? Type { get; set; }
    }
}
