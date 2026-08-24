

namespace BC.PAYMENT.CORE.DTO.CommondityExchange.ExchangeItem
{
    public class InvoiceForExchangeDto
    {
        public string MovLine { get; set; }
        public int Quantity { get; set; }
        public string ItemCode { get; set; }
        public string ItemDesc { get; set; }
        public string Transaction { get; set; }
        public string TransactionLine { get; set; }
        public DateTime TransactionDate { get; set; }
        public string ExpiredDate { get; set; }
    }
}
