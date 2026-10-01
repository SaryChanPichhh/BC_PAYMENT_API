namespace BC.PAYMENT.CORE.Entities.Inventory.InventoryReport;

public class InventoryTrackingWarehouseDataModel
{
    public string Id { get; set; }
    public string DbCode { get; set; }
    public DateTime TransactionDate { get; set; }
    public InventoryTrackingTypes InventoryTrackingTypes { get; set; }
    public string Warehouse { get; set; }
    public string ToWarehouse { get; set; }
    public string ItemCode { get; set; }
    public string ItemDescription { get; set; }
    public int Quantity { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
}