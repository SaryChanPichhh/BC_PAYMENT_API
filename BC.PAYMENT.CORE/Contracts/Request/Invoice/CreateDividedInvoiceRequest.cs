namespace BC.PAYMENT.CORE.Contracts.Request.Invoice;

public class CreateDividedInvoiceRequest
{
    public string DeliveryId { get; init; } = string.Empty;
    public int InvoiceId { get; init; }
    public DateTime CreatedDate { get; init; }  = DateTime.UtcNow;
}