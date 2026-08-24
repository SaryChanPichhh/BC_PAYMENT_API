namespace BC.PAYMENT.CORE.DTO.Inventory
{
    public class InventoryTrackingDto
    {
        public string Warehouse { get; set; }
        public string Description { get; set; }
        public InventoryTrackingTypes InventoryTrackingTypes { get; set; }
        public Boolean SelectedTotal { get; set; }
    }

    public class InventoryTrackingPutDto : InventoryTrackingDto
    {
        public string Id { get; set; }
    }
}
