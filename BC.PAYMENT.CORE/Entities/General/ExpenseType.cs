namespace BC.PAYMENT.CORE.Entities.General;

public class ExpenseType
{
    public string? ExpenseId { get; set; }
    public string? DbCode { get; set; }
    public string? ExpenseName { get; set; }
    public bool? Status { get; set; }
    public string? UserCreated { get; set; }
    public DateTime? CreatedDate { get; set; }
}