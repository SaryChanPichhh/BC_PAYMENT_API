namespace BC.PAYMENT.CORE.Entities;

public class PcReturningInvoiceAudit
{
    public int ReturnId { get; set; }
    public int ProcessingStatus { get; set; }
    public string ApprovalStatus { get; set; } = string.Empty;
    public DateTime LastUpdatedDate { get; set; }
    public string LastUpdatedBy { get; set; } = string.Empty;
}