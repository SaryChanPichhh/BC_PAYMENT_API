namespace BC.PAYMENT.CORE.Entities.Transaction.Submitting.SubmittingInvoice;

public class SubmittedInvoiceModel
{
    public int InvoiceId { get; set; }
    public string? DeliveryName { get; set; }
    public string? CustomerCode { get; set; }
    public string? CustomerName { get; set; }
    public string? TransactionCode { get; set; }
    public double Amount { get; set; }
    public double InvoiceAmount { get; set; }
    public double Paid { get; set; }
    public double? HalfPaid { get; set; }
    public string? Total { get; set; }
    public string? Status { get; set; }
    public string? InvoiceType { get; set; }
    public DateTime CreateDate { get; set; }
    public string? DbCode { get; set; }
    public string? SubmittedBy { get; set; }
    public string? SubmittedDate { get; set; }
}