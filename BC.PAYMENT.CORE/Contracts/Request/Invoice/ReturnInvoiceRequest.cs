namespace BC.PAYMENT.CORE.Contracts.Request.Invoice;

public class ReturnInvoiceRequest
{
    public string? TransactionCode { get; set; }
    public string? CustomerCode { get; set; }
    public string? CustomerName { get; set; }
    public double InvoiceValue { get; set; }
}