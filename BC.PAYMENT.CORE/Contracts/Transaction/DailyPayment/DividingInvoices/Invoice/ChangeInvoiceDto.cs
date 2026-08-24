namespace BC.PAYMENT.CORE.DTO.Transaction.DailyPayment.DividingInvoices.Invoice
{
    public class ChangeInvoiceDto
    {
        public string? TransactionCode { get; set; }
        public string? CustomerCode { get; set; }
        public string? CustomerName { get; set; }
        public string? EntriesCode { get; set; }
        public double InvoiceValue { get; set; }
    }
}
