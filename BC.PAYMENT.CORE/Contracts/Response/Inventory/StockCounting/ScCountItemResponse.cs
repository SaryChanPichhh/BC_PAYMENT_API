namespace BC.PAYMENT.CORE.Contracts.Response.Inventory.StockCounting;

public class ScCountItemResponse
{
    public int Id { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string ItemDesc { get; set; } = string.Empty;
    public string ItemDescKh { get; set; } = string.Empty;
    public int Quantity { get; set; }
}