namespace BC.PAYMENT.CORE.Contracts.Response.Expense;

public class BcPaymentDetailResponse
{
    public int Id { get; set; }
    public string? DbCode { get; set; }
    public string? DeliveryName { get; set; }
    public string? DeliveryId { get; set; }
    public double Total { get; set; }
    public double? Dollar { get; set; }
    public double? Riel { get; set; }
    public double Exchange { get; set; }
    public string? DescExp1 { get; set; }
    public double ExpAmount1 { get; set; }
    public string? DescExp2 { get; set; }
    public double ExpAmount2 { get; set; }
    public string? DescExp3 { get; set; }
    public double ExpAmount3 { get; set; }
    public double? MoneyBias { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public string? EntriesCode { get; set; }
    public double SubTotal => Total + (ExpAmount1 + ExpAmount2 + ExpAmount3) / Exchange;
    public double ExpenseRiel => ExpAmount1 + ExpAmount2 + ExpAmount3;
    public double ExpenseDollar => (ExpAmount1 + ExpAmount2 + ExpAmount3) / Exchange;
}