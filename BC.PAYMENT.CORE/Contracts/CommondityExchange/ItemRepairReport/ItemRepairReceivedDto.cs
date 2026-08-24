namespace BC.PAYMENT.CORE.DTO.CommondityExchange.ItemRepairReport
{
    public class ItemRepairReceivedDto : Customer
    {
        public DateTime Date { get; set; }
        public DateTime CreatedDate { get; set; }
        public string BranchName { get; set; }
        public string ItemCode { get; set; }
        public int Quantity { get; set; }
        public int? Received { get; set; }
        public int Id { get; set; }
        public string Status { get; set; }
        public string Seller { get; set; }
        public string StatusText => Status.Trim() switch
        {
            "Completed" or "Yes" => "បានទទួល",
            "Pending" or "No" => "មិនទាន់បានទទួល",
            _ => "មិនទាន់បានទទួល"
        };
        public string Description { get; set; }
        public string Transaction { get; set; }

        public string TransactionCode { get; set; }

    }
}
