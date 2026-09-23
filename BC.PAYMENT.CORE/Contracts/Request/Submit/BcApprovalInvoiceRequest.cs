namespace BC.PAYMENT.CORE.Contracts.Request.Submit;

public class BcApprovalInvoiceRequest
{
    public int SubmittedId { get; set; }
    public string ApprovalStatus { get; set; } = string.Empty;
    public string Description { get; set; } =  string.Empty;
}