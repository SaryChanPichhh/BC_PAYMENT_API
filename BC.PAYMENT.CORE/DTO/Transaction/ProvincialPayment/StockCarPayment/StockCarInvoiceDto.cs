
using BC.PAYMENT.CORE.Entities.General;

namespace BC.PAYMENT.CORE.DTO.Transaction.ProvincialPayment.StockCarPayment
{
    public class StockCarInvoiceDto : Customer
    {
        public int InvoiceId { get; set; }
        public string? TransactionCode { get; set; }
        public double InvoiceValue { get; set; }
        public double AmountPaid { get; set; }
        public double Total => InvoiceValue - AmountPaid;
        public bool Status { get; set; }
        public string? InvoiceType { get; set; }
        public string? CreateBy { get; set; }
    }
    public class PaymentInvoiceDto : StockCarInvoiceDto
    {
        public string PaymentStatus => Status ? "" : @"ទូទាត់";
    }
    public class ReturningInvoiceDto : StockCarInvoiceDto
    {
        public string? Description { get; set; }
        public string InvoiceStatus => Status ? "" : "វិក័យប័ត្រត្រឡប់";
    }

    public class ReturningInvoicePostDto
    {
        public int InvoiceId { get; set; }
        public string? Description { get; set; }
    }
    public class PaymentInvoicePostDto
    {
        public int InvoiceId { get; set; }
        public double AmountPaid { get; set; }
    }
}
