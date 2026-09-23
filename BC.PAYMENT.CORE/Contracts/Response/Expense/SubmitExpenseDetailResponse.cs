namespace BC.PAYMENT.CORE.Contracts.Response.Expense;

public class SubmitExpenseDetailResponse
{
    public int SubmittedId { get; set; }
    public string DeliveryName { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public string SubmittedBy { get; set; } =  string.Empty;
    public double Dollar { get; set; }
    public double Riel { get; set; }
    public double SubTotal { get; set; }
    public double MoneyBias { get; set; }
    public double ExpenseDollar { get; set; }
    public double ExpenseRiel { get; set; }
    public double Total { get; set; }
    public double Exchange { get; set; }
    public string Other { get; set; } = string.Empty;
    public DateTime SubmittedDate { get; set; }
    public string ExpenseDesc { get; set; } =  string.Empty;
    public string ExpenseDesc1 { get; set; } = string.Empty;
    public string ExpenseDesc2 { get; set; } = string.Empty;
    public string ExpenseDesc3 { get; set; } = string.Empty;
}