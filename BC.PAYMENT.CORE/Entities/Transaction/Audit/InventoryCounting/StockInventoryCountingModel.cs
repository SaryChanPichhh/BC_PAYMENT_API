namespace BC.PAYMENT.CORE.Entities.Transaction.Audit.InventoryCounting;

public class StockInventoryCountingModel
{
    public string? StockId { get; set; }
    public string? Stock { get; set; }
    public string? Warehouse { get; set; }
    public string? StockController { get; set; }
    public string? Participation { get; set; }
    public string? Period { get; set; }
    public string? CountingDate { get; set; }
    public string? DbCode { get; set; }
    public string? CreateBy { get; set; }
}