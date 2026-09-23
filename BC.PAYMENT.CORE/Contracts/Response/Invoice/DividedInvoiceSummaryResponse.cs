namespace BC.PAYMENT.CORE.Contracts.Response.Invoice;

public class DividedInvoiceSummaryResponse
{
    public string? DeliveryName { get; set; }
    public string? New { get; set; }
    public string? Change { get; set; }
    public string? Old { get; set; }
    public string? Total { get; set; }
}