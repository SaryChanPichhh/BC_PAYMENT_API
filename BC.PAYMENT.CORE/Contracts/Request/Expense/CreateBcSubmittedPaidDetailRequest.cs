namespace BC.PAYMENT.CORE.Contracts.Request.Expense;

public class CreateBcSubmittedPaidDetailRequest
{
    public int PaidDetailId { get; set; }
    public double Dollar { get; set; }
    public double Riel { get; set; }
    public double Exchange { get; set; }
    public double Total { get; set; }
    public double ExpenseRiel { get; set; }
    public double ExpenseDollar { get; set; }
    public double MoneyBias { get; set; }
    public bool Status { get; set; }
}