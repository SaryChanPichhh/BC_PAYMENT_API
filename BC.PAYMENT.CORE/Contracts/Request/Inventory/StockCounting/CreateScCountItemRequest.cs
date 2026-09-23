namespace BC.PAYMENT.CORE.Contracts.Request.Inventory.StockCounting;

public class CreateScCountItemRequest
{
    public string ItemCode { get; set; } = string.Empty;
    public string ItemDesc { get; set; } = string.Empty;
    public string ItemDescKh { get; set; } = string.Empty;
    public int Quantity { get; set; }
}