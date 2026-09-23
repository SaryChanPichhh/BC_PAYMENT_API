namespace BC.PAYMENT.CORE.Contracts.Response.SubmitInvoice;

public class ApproveSubmitInvoiceResponse : SubmitInvoiceResponse
{
    public string Market { get; set; } = string.Empty;
    public string Area { get; set; } = string.Empty;
    public string Store { get; set; } = string.Empty;
    public DateTime ApprovedDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime DividedDate { get; set; }
}