namespace BC.PAYMENT.CORE.Contracts.Response.Invoice;

public class ReturnInvoiceResponse
{
    public string? Area { get; set; }
    public string? Store { get; set; }
    public string? Market { get; set; }
    public string CustomerCode { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public int InvoiceId { get; set; }
    public int DividedId { get; set; }
    public string TransactionCode { get; set; } = string.Empty;
    public double InvoiceValue { get; set; }
    public int Return { get; set; }
    public string Description { get; set; } = string.Empty;
    public int ReturnId { get; set; }
    public string? Status { get; set; }

    public string InvoiceType =>
        Status switch
        {
            "N" => "វិក្កយប័ត្រថ្មី",
            "O" => "វិក្កយប័ត្រចាស់",
            "C" => "វិក្កយប័ត្រដូរ",
            _ => ""
        };
}