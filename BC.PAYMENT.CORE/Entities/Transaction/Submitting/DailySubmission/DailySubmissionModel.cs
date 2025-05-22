
using BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DeliveryPaid;

namespace BC.PAYMENT.CORE.Entities.Transaction.Submitting.DailySubmission
{
    public class DailySubmissionModel : GeneralInvoiceModel
    {
        public double? HalfPaid => InvoiceValue > PaidAmount ? PaidAmount : null;
        public double? FullPaid => Math.Abs(InvoiceValue - PaidAmount) == 0 ? PaidAmount : null;
        public double Total { get; set; }
        public bool IsFullPaid => Math.Abs(InvoiceValue - PaidAmount) == 0;
        
    }
    
    public class ApprovalInvoiceModel : DailySubmissionModel
    {
        public string ApprovalStatus => Status == "Approved" ? "អនុម័ត" : "បដិសេធ";
    }
    public class HistoryPaymentModel
    {
        public string? TransactionCode { get; set; }
        public double InvoiceValue { get; set; }
        public double Amount { get; set; }
        public double Total { get; set; }
        public DateTime CreateDate { get; set; }
        public string? Status { get; set; }
    }
}
