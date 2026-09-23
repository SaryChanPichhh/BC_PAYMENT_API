namespace BC.PAYMENT.CORE.Contracts.Response.Inventory.StockCounting;

public class ScStockResponse
{
    public int StockId { get; set; }
    public string StockName { get; set; } = string.Empty;
    public string Warehouse { get; set; } = string.Empty;
    public string StockController { get; set; } = string.Empty;
    public string Participation { get; set; } = string.Empty;
    public string Period { get; set; } = string.Empty;
    public DateTime CountDate { get; set; }
}