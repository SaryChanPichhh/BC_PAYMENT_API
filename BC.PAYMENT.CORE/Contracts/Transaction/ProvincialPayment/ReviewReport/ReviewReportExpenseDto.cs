namespace BC.PAYMENT.CORE.DTO.Transaction.ProvincialPayment.ReviewReport;

public class ReviewReportExpenseDto
{
    public string? Id { get; set; }
    public string? Description { get; set; }
    public string? Province { get; set; }
    public int Quantity { get; set; }
    public double UnitPrice { get; set; }
    public double AmountDollar { get; set; }
    public double AmountRiel { get; set; }
    public double ExchangeRate { get; set; }
    public DateTime ExpenseDate { get; set; }
}