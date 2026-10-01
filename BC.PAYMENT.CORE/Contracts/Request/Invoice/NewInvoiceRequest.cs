namespace BC.PAYMENT.CORE.Contracts.Request.Invoice;

public class NewInvoiceRequest
{
    public string InvoiceCode { get; set; } = string.Empty;
    public string CustomerCode { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string AccNameKh { get; set; } = string.Empty;
    public double InvoiceAmount { get; set; }
    public InvoiceStatus InvoiceType { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string CustomerField1 { get; set; } = string.Empty;
}