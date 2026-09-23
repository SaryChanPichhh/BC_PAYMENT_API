namespace BC.PAYMENT.CORE.Contracts.Request.StockCar;

public class CreateBcStockCarRequest
{
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerCode { get; set; } = string.Empty;
    public string TransactionCode { get; set; } = string.Empty;
    public double InvoiceValue { get; set; }
    public DateTime TransactionDate { get; set; }
    public int TemplateId { get; set; }
    public InvoiceStatus InvoiceType { get; set; } 
}