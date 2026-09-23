namespace BC.PAYMENT.CORE.Contracts.Request.Invoice;

public class PaymentInvoiceRequest
{
    public int DividedInvoiceId { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
    public double Amount { get; set; }
    public string Description { get; set; } = string.Empty;
}