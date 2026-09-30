namespace BC.PAYMENT.CORE.Contracts.Request.StockCar;

public class UpdateStockCarInvoiceRequest
{
    public string CustomerCode { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public double Value { get; set; }
    public int Id { get; set; }
}