using BC.PAYMENT.CORE.Entities.General;

namespace BC.PAYMENT.CORE.Entities.Invoice
{
    public class ChangeInvoice : Customer
    {
        public string TransactionCode { get; set; }
        public DateTime CreatedDate { get; set; }
        public double InvoiceValue { get; set; }
    }
}
