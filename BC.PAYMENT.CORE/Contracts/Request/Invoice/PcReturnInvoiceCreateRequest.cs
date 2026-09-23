namespace BC.PAYMENT.CORE.Contracts.Request.Invoice;

public class PcReturnInvoiceCreateRequest
{
    public int DividedId { get; set; }
    public string Description { get; set; } = string.Empty;
}