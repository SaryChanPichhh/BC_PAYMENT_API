namespace BC.PAYMENT.CORE.DTO.CommondityExchange.PostInvoice;

public class PostInvoiceReportDto
{
    public DateTime CreatedDate { get; set; }
    public string ItemCode { get; set; }
    public int Quantity { get; set; }
    public double UnitPrice { get; set; }
    public double Total { get; set; }
    public string CreatedBy { get; set; }
    public string Type { get; set; }
}