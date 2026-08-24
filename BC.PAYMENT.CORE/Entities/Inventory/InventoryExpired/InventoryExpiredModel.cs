namespace BC.PAYMENT.CORE.Entities.Inventory.InventoryExpired
{
    public class InventoryExpiredModel
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int ExpiredQty { get; set; }
        public DateTime ExpiredDate { get; set; }
        public string ExpiredDuration { get; set; }
        public string? ImagePath { get; set; }
    }
}
