

namespace BC.PAYMENT.CORE.DTO.CommondityExchange.ExchangeItem
{
    public class CreditNoteItemDto
    {
        public string ReceivedId { get; set; }
        public string OldTransaction { get; set; }
        public string ItemCode { get; set; }
        public string OldTranLine { get; set; }
        public int Quantity { get; set; }
        public string CustomerCode { get; set; }
        public decimal Total { get; set; }

    }

    public class CreditNoteItemModel
    {
        public string DbCode { get; set; }
        public string UserName { get; set; }
        public string ReceivedId { get; set; }
        public string Period { get; set; }
        public string NewTransaction { get; set; }
        public string OldTransaction { get; set; }
        public string ItemCode { get; set; }
        public string OldTranLine { get; set; }
        public int Quantity { get; set; }
        public string CustomerCode { get; set; }
        public decimal Total { get; set; }

    }
}
