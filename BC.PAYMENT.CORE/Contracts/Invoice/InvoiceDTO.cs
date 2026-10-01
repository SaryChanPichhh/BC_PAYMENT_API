namespace BC.PAYMENT.CORE.Contracts.Invoice;

public class InvoiceDTO
{
    [JsonIgnore] public string? DbCode { get; set; }
    [Required] public required string TransactionCode { get; set; }
    [JsonIgnore] public string? EntryCode { get; set; }
    [JsonIgnore] public string? CreatedBy { get; set; }
}

public class InvoiceDetailDto : Customer
{
    public string InvoiceNumber { get; set; }
    public string TransactionCode { get; set; }
    public DateTime InvoiceDate { get; set; }
    public DateTime InvoiceDue { get; set; }
    public string Phone { get; set; }
    public string SalesRepresentative { get; set; }
    public string User { get; set; }
}