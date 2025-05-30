namespace BC.PAYMENT.CORE.Entities.CashFlow.CashFlowData
{
    public class CashFlowDataModel
    {
        public string DbCode { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
        public string CreateBy { get; set; }
        public string Period { get; set; }
    }
    public class CashFlowDataHeader
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public DateTime Month { get; set; }
        public string Year { get; set; }
        public string Status { get; set; }
        public string DbCode { get; set; }
    }

    public class CashFlowDataDetailModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public string DbCode { get; set; }
        public double Amount { get; set; }
        public string CurrencyFormat { get; set; }
        public double ExchangeRate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string HeaderId { get; set; }
        public string Status { get; set; }
        public string StatusText => Status switch
        {
            "Pending" => "កំពុងដំណើរការ...",
            "Submitted" => "បានដាក់ស្នើរ",
            "Completed" => "អនុម័ត",
            "Rejected" => "បដិសេធ",
            _ => "កំពុងដំណើរការ..."
        };
    }
}
