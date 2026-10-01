namespace BC.PAYMENT.CORE.Entities.Transaction.Submitting.InvoiceVerify;

public class MonthlyInvoiceModel
{
    public string? CustomerCode { get; set; }
    public string? CustomerName { get; set; }
    public string? TransactionCode { get; set; }
    public string? InvoiceValue { get; set; }
    public string? Status { get; set; }
    public double Paid { get; set; }
    public double Total { get; set; }
    public DateTime? CreateDate { get; set; }
}