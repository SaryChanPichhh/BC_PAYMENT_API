namespace BC.PAYMENT.CORE.Contracts.Response.StockCar;

public class StockCarInvoiceResponse
{
    public int InvoiceId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerCode { get; set; } = string.Empty;
    public string TransactionCode { get; set; } = string.Empty;
    public string Market { get; set; } = string.Empty;
    public string Area { get; set; } = string.Empty;
    public string Store { get; set; } = string.Empty;
    public double InvoiceValue { get; set; }
    public DateTime TransactionDate { get; set; }
    public string Employee { get; set; } = string.Empty;
}