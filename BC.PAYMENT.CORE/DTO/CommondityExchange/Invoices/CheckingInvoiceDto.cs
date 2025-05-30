

namespace BC.PAYMENT.CORE.DTO.CommondityExchange.Invoices
{
    public class CheckingInvoiceDto
    {
        public string Transaction { get; set; }
        public DateTime Date { get; set; }
        public string CustomerCode { get; set; }
        public string? Status { get; set; }
    }
}
