namespace BC.PAYMENT.CORE.Contracts.Response.Invoice;

public class OldInvoiceResponse
{
    public int InvoiceId { get; set; }
    public string TransactionCode { get; set; } = string.Empty;
    public string TransRef { get; set; } = string.Empty;
    public string CustomerCode { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public double InvoiceValue { get; set; }
    public bool Status { get; set; }
    public string Username { get; set; } = string.Empty;
    public string AnalysisT0 { get; set; } = string.Empty;
    public new string Employee => string.IsNullOrEmpty(Username) ? AnalysisT0 : Username;
    public string DbCode { get; set; } = string.Empty;
    public string CreateBy { get; set; } = string.Empty;
    public string Market { get; set; } = string.Empty;
    public string Store { get; set; } = string.Empty;
}