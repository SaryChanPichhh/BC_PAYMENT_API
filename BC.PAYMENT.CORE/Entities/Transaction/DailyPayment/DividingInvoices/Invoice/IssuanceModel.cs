namespace BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DividingInvoices.Invoice;

public class IssuanceModel : Customer
{
    public string? Transaction { get; set; }
    public double Amount { get; set; }
    public string? Type { get; set; }
    public string? AreaId { get; set; }
    public string? Id { get; set; }
}