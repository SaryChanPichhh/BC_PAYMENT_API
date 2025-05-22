

using BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DailyPayment;

namespace BC.PAYMENT.CORE.Entities.Transaction.Submitting.SubmittingInvoice
{
    public class RejectedInvoiceModel 
    {
        public string? SubmittedInvoiceId { get; set; }
        public string? InvoiceId { get; set; }
        public string? DividedInvoiceId { get; set; }
        public string? DeliveryName { get; set; }
        public string? CustomerCode { get; set; }
        public string? CustomerName { get; set; }
        public string? TransactionCode { get; set; }
        public double Amount { get; set; }
        public double Money { get; set; }
        public double Paid { get; set; }
        public double Total { get; set; }
        public DateTime ApprovalDate { get; set; }
        public string? ApprovalBy { get; set; }
        public string? Description { get; set; }
        public string? ApprovalStatus { get; set; }
    }

    public class RejectedInvoicePerDelivery : ExpenseDetailModel
    {
        public DateTime ApprovalDate { get; set; }
        public string? ApprovalBy { get; set; }
        public string? ApprovalStatus { get; set; }
        public string? Description { get; set; }
    }
}
