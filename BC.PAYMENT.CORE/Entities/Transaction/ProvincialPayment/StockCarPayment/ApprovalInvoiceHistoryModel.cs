namespace BC.PAYMENT.CORE.Entities.Transaction.ProvincialPayment.StockCarPayment;

public class ApprovalInvoiceHistoryModel : PaidInvoiceRequestDto
{
    public int RequestId;
    public string? Status { get; set; }
    public string ApprovalStatus => Status == "Approved" ? "អនុម័ត" : "បដិសេធ";
}