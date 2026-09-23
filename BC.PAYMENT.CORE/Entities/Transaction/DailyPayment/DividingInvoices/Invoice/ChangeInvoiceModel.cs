namespace BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DividingInvoices.Invoice
{
    public class ChangeInvoiceModel : Customer
    {
        public string? DbCode { get; set; }
        public string? UserName { get; set; }
        public string? EntriesCode { get; set; }
        public int Id { get; set; }
        public double InvoiceValue { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? Status { get; set; }
        public string? Transaction { get; set; }
        public string InvoiceType =>
            Status switch
            {
                "O" => "វិក្កយប័ត្រចាស់",
                "C" => "វិក្កយប័ត្រដូរ",
                _ => "វិក្កយប័ត្រថ្មី"
            };

        public bool IsExists { get; set; }
    }
}
