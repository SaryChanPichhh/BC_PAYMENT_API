namespace BC.PAYMENT.CORE.Contracts.Invoice;

public record OldInvoiceDTO
{
    [JsonIgnore] public string? DbCode { get; set; }

    [Required] public string Code { get; set; }

    [JsonIgnore] public string? Period { get; set; }

    [Required] public string CustomerCode { get; set; }
    [Required] public string CustomerName { get; set; }
    [Required] public decimal InvoiceValue { get; set; }
    [Required] public string Employee { get; set; }

    [JsonIgnore] public string? CreatedBy { get; set; }
}