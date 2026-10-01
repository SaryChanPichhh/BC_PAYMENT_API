namespace BC.PAYMENT.CORE.DTO.CommondityExchange.ExchangeItem;

public class CreditInvoiceDetailsDto
{
    public string OldTransaction { get; set; }
    public string OldLine { get; set; }
    public DateTime CreditDate { get; set; } = DateTime.Today;
    public string NewTransaction { get; set; }
    public string CreditPeriod { get; set; }
    public string UserCreated { get; set; }
}