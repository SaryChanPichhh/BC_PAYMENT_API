namespace BC.PAYMENT.CORE.Entities.Inventory.Inventory;

public class InventoryTrackingWarehouseModel
{
    public int Id { get; set; }
    public string DbCode { get; set; }
    public string Warehouse { get; set; }
    public InventoryTrackingTypes InventoryTrackingTypes { get; set; }

    public string InventoryTrackingTypesDescription =>
        InventoryTrackingTypes switch
        {
            InventoryTrackingTypes.Plus => "ស្តុកបូកបញ្ចូល",
            InventoryTrackingTypes.Transfer => "ស្តុកផ្ទេរ",
            _ => "ស្តុកដកចេញ"
        };

    public string Description { get; set; }
    public bool SelectedTotal { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public string UpdatedBy { get; set; }
    public DateTime UpdatedDate { get; set; }
}