namespace BC.PAYMENT.CORE.Contracts.Response.SubmitInvoice;

public class SubmitInvoiceResponse
{
    public int Id { get; set; }
    public string DeliveryName { get; set; } =  string.Empty;
    public string CustomerCode { get; set; } =  string.Empty;
    public string CustomerName { get; set; } =   string.Empty;
    public string InvoiceCode { get; set; } = string.Empty;
    public double Amount { get; set; }
    public double Money { get; set; }
    public double Paid { get; set; }
    public double Total { get; set; }
    public bool Status { get; set; } 
    public string StatusDesc { get; set; } = string.Empty;
    public string SubmittedBy { get; set; } = string.Empty;
    public DateTime SubmittedDate { get; set; }
    public bool IsFullPaid { get; set; }    
}