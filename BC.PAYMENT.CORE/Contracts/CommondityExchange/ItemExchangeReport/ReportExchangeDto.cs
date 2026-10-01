namespace BC.PAYMENT.CORE.DTO.CommondityExchange.ItemExchangeReport;

public class ReportExchangeDto : Customer
{
    public DateTime InvoiceDate { get; set; }
    public string InvoiceNumber { get; set; }
    public string ItemCode { get; set; }
    public int Quantity { get; set; }
    public double UnitPrice { get; set; }
    public double Total { get; set; }
}