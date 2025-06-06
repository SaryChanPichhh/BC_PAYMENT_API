

namespace BC.PAYMENT.CORE.DTO.CommondityExchange.Invoices
{
    public class CheckingInvoiceDto
    {
        public string Transaction { get; set; }
        public DateTime Date { get; set; }
        public string CustomerCode { get; set; }
        public string? Status { get; set; }
    }

    public class ExchangeInvoiceDetailRespondDto
    {
        public List<ItemExchangeInvoiceDto> OutBoundExchangeItems { get; set; }
        public CustomerInvoiceDto Customer { get; set; }
        public List<ItemExchangeInvoiceDto> InBoundExchangeItems { get; set; }
    }

    public class RepairInvoiceDetailRespondDto
    {
        public CustomerInvoiceDto Customer { get; set; }
        public List<ItemRepairInvoiceDto> RepairItems { get; set; }
    }
}
