namespace BC.PAYMENT.CORE.Contracts.Request.Expense;

public class PaymentInvoiceExpenseCreateRequest
{
    public string Name { get; set; } =  string.Empty;
    public string Description { get; set; } =  string.Empty;
    public double UnitPrice { get; set; }
    public double Total { get; set; }
    public double ExchangeRate { get; set; }
}