namespace BC.PAYMENT.CORE.Contracts.Request.Submit;

public class BcInvoiceSubmittedRequest
{
    public int InvoiceId { get; set; }
    public string CustomerCode { get; set; } = string.Empty;
    public string TransactionCode { get; set; } = string.Empty;
    public double Money { get; set; }
    public double Paid { get; set; }
}