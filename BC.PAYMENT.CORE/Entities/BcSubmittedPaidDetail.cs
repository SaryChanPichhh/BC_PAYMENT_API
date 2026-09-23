namespace BC.PAYMENT.CORE.Entities;

public class BcSubmittedPaidDetail
{
    public int SubmittedId { get; set; }
    public int PaidDetailId { get; set; }
    public double Dollar { get; set; }
    public double Riel { get; set; }
    public double Exchange { get; set; }
    public double Total { get; set; }
    public double ExpenseRiel { get; set; }
    public double ExpenseDollar { get; set; }
    public double MoneyBais { get; set; }
    public double MoneyBias { get => MoneyBais; set => MoneyBais = value; }
    public bool Status { get; set; }
    public string DbCode { get; set; } = string.Empty;
    public DateTime SubmittedDate { get; set; }
    public string SubmittedBy { get; set; } = string.Empty;
}