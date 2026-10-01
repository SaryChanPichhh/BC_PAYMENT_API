namespace BC.PAYMENT.CORE.Contracts.Request.Expense;

public class UpdateBcPaymentDetailRequest
{
    public int Id { get; set; }
    public double Total { get; set; }
    public double Dollar { get; set; }
    public double Riel { get; set; }
    public double Exchange { get; set; }
    public string? DescExp1 { get; set; }
    public string? DescExp2 { get; set; }
    public string? DescExp3 { get; set; }
    public double ExpAmount1 { get; set; }
    public double ExpAmount2 { get; set; }
    public double ExpAmount3 { get; set; }
    public double MoneyBias { get; set; }
}