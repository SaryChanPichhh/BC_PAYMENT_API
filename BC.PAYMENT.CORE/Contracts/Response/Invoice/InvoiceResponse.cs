namespace BC.PAYMENT.CORE.Contracts.Response.Invoice;

public class InvoiceResponse : Entities.General.Customer
{
    public int InvoiceId { get; set; }
    public string DbCode { get; set; }
    public string InvoiceCode { get; set; }
    public double InvoiceAmount { get; set; }
    public InvoiceStatus InvoiceType { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CustomField1 { get; set; }
    public bool IsDivided { get; set; }
    public string CreatedBy { get; set; }
    public string EntriesCode { get; set; }
    public string DeliveryName { get; set; }
    public string Description { get; set; }
}