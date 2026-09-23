namespace BC.PAYMENT.CORE.Contracts.Request.Invoice;

public class PcPaymentInvoiceCreateRequest
{
    public int DividedInvoiceId { get; set; }
    public double Amount { get; set; }
    
}