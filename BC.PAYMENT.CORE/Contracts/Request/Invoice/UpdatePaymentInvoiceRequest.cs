namespace BC.PAYMENT.CORE.Contracts.Request.Invoice;

public class UpdatePaymentInvoiceRequest
{
    public int DividedId { get; init; }
    public double NewAmount { get; init; }
    public double OldAmount { get; init; }
    public string Description { get; init; } = string.Empty;
    public int InvoiceId { get; init; }
    public int PaymentId { get; init; }
}