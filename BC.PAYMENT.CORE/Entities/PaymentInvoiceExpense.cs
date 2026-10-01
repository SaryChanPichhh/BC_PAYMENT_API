namespace BC.PAYMENT.CORE.Entities;

public class PaymentInvoiceExpense
{
    public int Id { get; set; }
    public string DbCode { get; set; } = string.Empty;
    public int PaymentHeaderId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public double UnitPrice { get; set; }
    public double Total { get; set; }
    public string CurrencyType { get; set; } = string.Empty;
    public double ExchangeRate { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}