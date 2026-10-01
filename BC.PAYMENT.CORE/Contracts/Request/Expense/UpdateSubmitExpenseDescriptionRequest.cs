namespace BC.PAYMENT.CORE.Contracts.Request.Expense;

public class UpdateSubmitExpenseDescriptionRequest
{
    public int SubmittedId { get; set; }
    public string? DescExp1 { get; set; }
    public string? DescExp2 { get; set; }
    public string? DescExp3 { get; set; }
}