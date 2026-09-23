namespace BC.PAYMENT.CORE.Entities;

public class PcPaymentInvoice
{
    public int PaymentId { get; set; }
    public string DbCode { get; set; } = string.Empty;
    public int DividedInvoiceId { get; set; }
    public double Amount { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public bool Status { get; set; }
    public int PaymentHeaderId { get; set; }
}