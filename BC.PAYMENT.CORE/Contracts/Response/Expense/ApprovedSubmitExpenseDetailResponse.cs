namespace BC.PAYMENT.CORE.Contracts.Response.Expense;

public class ApprovedSubmitExpenseDetailResponse : SubmitExpenseDetailResponse
{
    public string ApprovedStatus { get; set; } = string.Empty;
    public string Description { get; set; } =  string.Empty;
    public DateTime ApprovedDate { get; set; }
    public string ApprovedBy { get; set; } =  string.Empty;
}