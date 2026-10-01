namespace BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DailyPayment;

public class ExpenseDetailModel
{
    public int Id { get; set; }
    public string? DeliveryName { get; set; }
    public double ExchangeRate { get; set; }
    public double SubTotal { get; set; }
    public double? Dollar { get; set; }
    public double Riel { get; set; }
    public double ExpenseRiel { get; set; }
    public double ExpenseDollar { get; set; }
    public double Total { get; set; }
    public double Misaligned { get; set; }
    public double ExpenseDescription { get; set; }
    public string? CreateBy { get; set; }
    public string? DbCode { get; set; }
    public DateTime CreateDate { get; set; }
}