namespace BC.PAYMENT.CORE.DTO.CommondityExchange.ItemExchangeReport
{
    public class ExchangeItemReportDto : Customer
    {
        public string ItemCode { get; set; }
        public string Description { get; set; }
        public DateTime RequestDate { get; set; }
        public int Quantity { get; set; }
        public ExchangeStatus Status { get; set; }
        public string MasterStatus { get; set; }
        public double? Total { get; set; }
        public DateTime? CompletedDate { get; set; }
        public DateTime SubmittedDate { get; set; }
        public string Seller { get; set; }
        public string DbName { get; set; }
        public string Transaction { get; set; }
        public string StatusText => Status switch
        {
            ExchangeStatus.Pending => "កំពុងរង់ចាំ",
            ExchangeStatus.Yes or ExchangeStatus.CreditNote or ExchangeStatus.ExChanged => "បានទទួល",
            ExchangeStatus.Completed => "បានបញ្ចប់",
            ExchangeStatus.Rejected => "បានបដិសេធ",
            _ => "កំពុងរង់ចាំ"
        };
    }
}
