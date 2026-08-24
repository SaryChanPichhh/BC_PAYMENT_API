namespace BC.PAYMENT.CORE.DTO.CommondityExchange.CreditNote
{
    public class CreditNoteReportDto : Customer
    {
        public string ReasonDesc { get; set; }
        public string UserCode { get; set; }
        public string Id { get; set; }
        public string TransactionCode { get; set; }
        public string OldTransactionCode { get; set; }
        public decimal TransactionValue { get; set; }
        public bool Status { get; set; }
        public string Location { get; set; }
        public DateTime Date { get; set; }
        public string ItemCode { get; set; }
        public int Quantity { get; set; }
        public string TodayDateTime { get; set; } = DateTime.Today.ToShortDateString();
    }
}
