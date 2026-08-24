namespace BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DividingInvoices.Invoice
{
    public class ReturnInvoiceModel : Customer
    {
        public int InvoiceId { get; set; }
        public string? TransactionCode { get; set; }
        public double InvoiceValue { get; set; }
        public string? Status { get; set; }

        public string InvoiceType => Status == "N" ? "វិក្កយប័ត្រថ្មី" :
            Status == "O" ? "វិក្កយប័ត្រចាស់" :
            Status == "C" ? "វិក្កយប័ត្រដូរ" : "វិក្កយប័ត្រថ្មី";
    }
}
