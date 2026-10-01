namespace BC.PAYMENT.CORE.Entities.Transaction.Audit.VerifyInvoice;

public class VerifyInvoiceModel
{
    public string? Transaction { get; set; }
    public int Period { get; set; }
    public string? CustomerCode { get; set; }
    public string? CustomerName { get; set; }
    public double Total { get; set; }
    public string? Employee { get; set; }
    public bool Status { get; set; }
}