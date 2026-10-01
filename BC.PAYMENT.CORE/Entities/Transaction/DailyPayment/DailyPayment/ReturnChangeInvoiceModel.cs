namespace BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DailyPayment;

public class ReturnChangeInvoiceModel
{
    public int ReturnId { get; set; }
    public string? DeliveryName { get; set; }
    public string? TransactionCode { get; set; }
    public string? CustomerCode { get; set; }
    public string? CustomerName { get; set; }
    public string? Cancel { get; set; }
    public string? Description { get; set; }
    public string? New { get; set; }
    public string? Change { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? Status { get; set; }
    public string? CreateBy { get; set; }
}