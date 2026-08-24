namespace BC.PAYMENT.CORE.Entities.CommondityExchange.ItemRepairReport
{
    public class ItemRepairAnalysis
    {
        public string ItemCode { get; set; }
        public string? Description { get; set; }
        public DateTime Date { get; set; }
        public int Quantity { get; set; }
    }
}
