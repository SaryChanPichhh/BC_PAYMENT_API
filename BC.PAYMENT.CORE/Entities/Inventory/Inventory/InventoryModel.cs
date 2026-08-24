namespace BC.PAYMENT.CORE.Entities.Inventory.Inventory
{
    public class InventoryModel
    {
        public string ItemCode { get; set; }
        public string Location { get; set; }
        public string DbCode { get; set; }
        public int? Physical { get; set; }
        public int? Order { get; set; }
        public int? PickQty { get; set; }
        public int? SubTotal => (Physical ?? 0) - (Order ?? 0);
    }
}
