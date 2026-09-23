namespace BC.PAYMENT.CORE.Contracts.Request.Invoice;

public class DeleteDividedInvoiceRequest
{
    public int InvoiceId { get; init; } 
    public string TransactionCode { get; init; } = string.Empty;
    public string DeliveryId { get; init; } = string.Empty;
    public string Note { get; init; } = string.Empty;
}
public class DividedInvoiceDeleteDto : DeleteDividedInvoiceRequest
{
}
