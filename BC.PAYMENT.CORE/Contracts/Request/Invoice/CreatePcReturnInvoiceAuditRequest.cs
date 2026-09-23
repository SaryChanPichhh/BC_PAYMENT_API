namespace BC.PAYMENT.CORE.Contracts.Request.Invoice;

public class CreatePcReturnInvoiceAuditRequest
{
    public int ReturnId { get; set; }
    public int ProcessingStatus { get; set; } = 1;
    public string ApprovalStatus { get; set; } = string.Empty;
    public DateTime? LastUpdatedDate { get; set; }
    public string? LastUpdatedBy { get; set; }
}
