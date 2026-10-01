namespace BC.PAYMENT.CORE.DTO.Inventory;

public class InventoryTrackingWarehouseDto
{
    public string DbCode { get; set; }
    public string Warehouse { get; set; }
    public string ToWarehouse { get; set; }
    public string ItemCode { get; set; }
    public int Quantity { get; set; }
}

public class SummaryTrackingDto
{
    public string Location { get; set; }
    public string InventoryTrackingType { get; set; }
}