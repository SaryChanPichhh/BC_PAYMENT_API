namespace BC.PAYMENT.CORE.DTO.CommondityExchange.ItemRepairReport
{
    public class ReportRepairItemInHandDto : Customer
    {
        public DateTime Date { get; set; }
        public string BranchName { get; set; }
        public string Seller { get; set; }
        public string ItemCode { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; }
        public string StatusText => "មិនទាន់ផ្ញើរទៅជាង";
        public string Note { get; set; }
    }
}
