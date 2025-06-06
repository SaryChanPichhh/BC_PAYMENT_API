namespace BC.PAYMENT.CORE.Entities.Preset.InventoryValue
{
    public class InventoryValueModel
    {
        public string BranchName { get; set; }
        public string BranchCode { get; set; }
        public string Location { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public double Cost { get; set; }
        public int Quantity { get; set; }
        public double Amount { get; set; }
        public double Total => Cost * Quantity;
    }
}
