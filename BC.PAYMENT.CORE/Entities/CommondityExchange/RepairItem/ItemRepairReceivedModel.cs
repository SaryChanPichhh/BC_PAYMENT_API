namespace BC.PAYMENT.CORE.Entities.CommondityExchange.RepairItem
{
    public class ItemRepairReceivedModel : RefundItemDto
    {
        public DateTime CreatedDate { get; set; }
        public string DbCode { get; set; }
        public string CreateBy { get; set; }
        public string Status { get; set; }
        public string StatusText => Status.Trim() switch
        {
            "Completed" or "Yes" => "បានទទួល",
            "Pending" or "No" => "មិនទាន់បានទទួល",
            _ => "មិនទាន់បានទទួល"
        };
        public string Description { get; set; }
        public string Reason { get; set; }
        public string RepairStatus { get; set; }
        public string ItemStatus { get; set; }
        public string RepairToolCode { get; set; }
        public string TransactionCode { get; set; }
        public string ItemDescription { get; set; }
    }
    
}
