
using BC.PAYMENT.CORE.Entities.Invoice;

namespace BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DividingInvoices.DailyReport
{
    public class PaymentInvoiceModel : InvoicesModel
    {
        public int DividedInvoiceId { get; set; }
        public bool ReturnInvoice { get; set; }
        public string? Other { get; set; }
        public string? NewInvoice { get; set; }
        public string? ChangeInvoice { get; set; }
        public double PaidInvoice { get; set; }
        public double Total => InvoiceAmount - PaidInvoice;
        public bool PaymentStatus { get; set; }
    }
}
