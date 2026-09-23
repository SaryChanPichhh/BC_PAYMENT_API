namespace BC.PAYMENT.CORE.Entities;

public class BcApprovalInvoice
{
    public int SubmittedId { get; set; }
    public string DbCode { get; set; } =  string.Empty;
    public string ApprovalStatus { get; set; } = string.Empty;
    public string Description { get; set; } =  string.Empty;
    public string ApprovalBy { get; set; } =  string.Empty;
    public DateTime ApprovalDate { get; set; }
}