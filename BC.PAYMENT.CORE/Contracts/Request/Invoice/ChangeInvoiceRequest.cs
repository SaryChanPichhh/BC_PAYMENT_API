namespace BC.PAYMENT.CORE.Contracts.Request.Invoice;

public class ChangeInvoiceRequest
{
    public string? TransactionCode { get; set; }
    public string? CustomerCode { get; set; }
    public string? CustomerName { get; set; }
    public double InvoiceValue { get; set; }
    public bool IsExists { get; set; }
}