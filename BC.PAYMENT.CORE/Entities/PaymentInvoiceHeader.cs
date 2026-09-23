namespace BC.PAYMENT.CORE.Entities;

public class PaymentInvoiceHeader
{
    public int Id { get; set; }
    public string DbCode { get; set; } = string.Empty;
    public string DeliveryId { get; set; } = string.Empty;
    public int Period { get; set; }
    public DateTime InvoiceDividendDate { get; set; }
    public string EntriesCode { get; set; } =  string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } 
}