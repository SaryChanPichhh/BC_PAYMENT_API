namespace BC.PAYMENT.CORE.DTO.Inventory
{
    public class InBoundGoodsDto
    {
        public string Warehouse { get; set; }
        public string ItemCode { get; set; }
        public int Quantity { get; set; }
        public string ItemDescription { get; set; }
        public DateTime TransactionDate { get; set; }
        public InventoryTrackingTypes InventoryTrackingTypes { get; set; }
    }
    public class InBoundGoodsPutDto : InBoundGoodsDto
    {
        public string Id { get; set; }
    }

    
}
