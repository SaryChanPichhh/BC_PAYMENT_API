namespace BC.PAYMENT.CORE.Entities.Invoice;

public class OldInvoiceModel : Invoice
{
    public string? Username { get; set; }
    public string? AnalysisT0 { get; set; }
    public new string? Employee => string.IsNullOrEmpty(Username) ? AnalysisT0 : Username;
    public string? TransactionCode { get; set; }
    public string? DbCode { get; set; }
    public string? CreateBy { get; set; }
}