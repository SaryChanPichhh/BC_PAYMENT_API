namespace BC.PAYMENT.CORE.Contracts.Response.Invoice;

public class DividedInvoiceTransactionResponse
{
    public int InvoiceId { get; set; }
    public int DividedId { get; set; }
    public string InvoiceCode { get; set; } = string.Empty;
    public string CustomerCode { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public double InvoiceValue { get; set; }
    public double IsReturn { get; set; }
    public string Other { get; set; } = string.Empty;
    public double PaidValue { get; set; }
    public double Amount { get; set; }
    public bool IsPaid { get; set; }
}