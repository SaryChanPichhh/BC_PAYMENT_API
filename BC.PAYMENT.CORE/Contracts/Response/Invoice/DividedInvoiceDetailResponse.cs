namespace BC.PAYMENT.CORE.Contracts.Response.Invoice;

public class DividedInvoiceDetailResponse
{
    public string TransactionCode { get; set; } = string.Empty;
    public string CustomerCode { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string Market { get; set; } = string.Empty;
    public string NewInvoice { get; set; } = string.Empty;
    public string ChangeInvoice { get; set; } =  string.Empty;
    public string InvoiceValue { get; set; } =  string.Empty;
    public int DividedInvoiceId { get; set; } = 0;
}