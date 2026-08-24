namespace BC.PAYMENT.CORE.DTO.CommondityExchange.ExchangeItem
{
    public class ItemExchangeDto
    {
        public int ReceivedId { get; set; }
        public int RequestExchangeDetailId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double Total => Quantity * UnitPrice;
        public string Description { get; set; }
        public DateTime RequestDate { get; set; }
        public string ItemLine { get; set; }
        public string Invoice { get; set; }
        public string Status => !string.IsNullOrEmpty(Invoice) ? "ធ្លាប់ទិញ" : "មិនធ្លាប់ទិញ";
    }

    public class ItemForExchaneDto
    {
        public int ReceivedId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double Total => Quantity * UnitPrice;
    }

    public class CustomerExchangeItemDto : CustomerRespondDto
    {
        public int MasterId { get; set; }
    }
    public class ExchangeItemParamsDto
    {
        public CustomerExchangeItemDto Customer { get; set; }
        public List<ItemForExchaneDto> InBoundItems { get; set; }
        public List<ItemForExchaneDto> OutBoundItems { get; set; }
        public NewInvoiceCompletedDto Invoices { get;set; }
    }
}
