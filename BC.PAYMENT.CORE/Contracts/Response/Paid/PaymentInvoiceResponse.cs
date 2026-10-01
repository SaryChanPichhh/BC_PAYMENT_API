namespace BC.PAYMENT.CORE.Contracts.Response.Paid;

public class PaymentInvoiceResponse
{
    public string DbCode { get; set; } = string.Empty;
    public int InvoiceId { get; set; }
    public string DeliveryName { get; set; } = string.Empty;
    public int PaymentId { get; set; }
    public int DividedId { get; set; }
    public string CustomerCode { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string InvoiceCode { get; set; } = string.Empty;
    public double Amount { get; set; }
    public double InvoiceValue { get; set; }
    public double Paid { get; set; }
    public double HalfPaid { get; set; }
    public double FullPaid { get; set; }
    public double Total { get; set; }
    public bool Status { get; set; }
    public string StatusDesc { get; set; } = string.Empty;
    public string InvoiceType { get; set; } = string.Empty;
    public DateTime DividedDate { get; set; }
    public DateTime PaymentDate { get; set; }
}