namespace BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DailyPayment;

public class ReturnInvoiceModel
{
    public string? DividedInvoiceId { get; set; }
    public string? CustomerCode { get; set; }
    public string? CustomerName { get; set; }
    public string? TransactionCode { get; set; }
    public double? InvoiceValue { get; set; }
    public double? Return { get; set; }
    public string? Description { get; set; }
    public string? ReturnId { get; set; }
}