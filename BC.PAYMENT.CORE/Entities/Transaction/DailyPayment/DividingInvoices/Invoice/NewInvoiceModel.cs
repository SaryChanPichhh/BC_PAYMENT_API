using BC.PAYMENT.CORE.Entities.General;
using BC.PAYMENT.CORE.Enums;

namespace BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DividingInvoices.Invoice
{
    public class NewInvoiceModel : Customer
    {
        public int InvoiceId { get; set; }
        public string? DbCode { get; set; }
        public string? InvoiceCode { get; set; }
        public double InvoiceAmount { get; set; }
        public InvoiceTypes InvoiceTypes { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CustomField1 { get; set; }
        public bool IsDivided { get; set; }
        public string? CreatedBy { get; set; }
        public string? EntriesCode { get; set; }
        public string? DeliveryName { get; set; }
        public string? Description { get; set; }
    }
}
