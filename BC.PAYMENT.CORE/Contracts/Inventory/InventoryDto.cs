namespace BC.PAYMENT.CORE.DTO.Inventory
{
    public class InventoryDto
    {
        public string DbCode { get; set; }
        public List<string> Warehouses { get; set; }
    }
    public class InventoryMultiWarehouseDto 
    {
        public string ItemCode { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public int Total { get; set; }
    }
}
