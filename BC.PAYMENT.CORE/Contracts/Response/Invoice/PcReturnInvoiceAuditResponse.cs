namespace BC.PAYMENT.CORE.Contracts.Response.Invoice;

public class PcReturnInvoiceAuditResponse
{
    public int ReturnId { get; set; }
    public string DeliveryName { get; set; } = string.Empty;
    public string TransactionCode { get; set; } = string.Empty;
    public string CustomerCode { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string Cancel { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string New { get; set; } = string.Empty;
    public string Change { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public string Status { get; set; } = string.Empty;
}