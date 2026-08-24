namespace BC.PAYMENT.CORE.DTO.CommondityExchange.ItemRepairReport
{
    public class ItemReceivedDto : Customer
    {
        public int MasterId { get; set; }
        public int DetailId { get; set; }
        public DateTime ReceivedDate { get; set; }
        public string TransactionCode { get; set; }
        public string ItemCode { get; set; }
        public int Quantity { get; set; }
        public int? Received { get; set; }
        public ExchangeStatus Status { get; set; }
        public string ItemStatus =>
            Status switch
            {
                ExchangeStatus.CreditNote => "កំណត់សំគាល់ឥណទាន",
                ExchangeStatus.Yes => "បានទទួល",
                ExchangeStatus.ExChanged => "បានប្តូរទំនិញហើយ",
                ExchangeStatus.Invoice => "វិក្កយប័ត្រ",
                ExchangeStatus.PostCredit => "បានទទួលស្គាល",
                ExchangeStatus.Completed => "រួចរាល់",
                ExchangeStatus.Rejected => "បដិសេធ",
                _ => "រង់ចាំ"
            };
    }
}
