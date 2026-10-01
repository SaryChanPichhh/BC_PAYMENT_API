namespace BC.PAYMENT.CORE.Contracts.Response.StockCar;

public class PaymentInvoiceResponse
{
    public int InvoiceId { get; set; }
    public string CustomerCode { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string TransactionCode { get; set; } = string.Empty;
    public string InvoiceType { get; set; } = string.Empty;
    public double InvoiceValue { get; set; }
    public double AmountPaid { get; set; }
    public double Total => InvoiceValue - AmountPaid;
    public bool Status { get; set; }
    public string PaymentStatus => Status ? "" : @"ទូទាត់";
    public string Market { get; set; } = string.Empty;
    public string Area { get; set; } = string.Empty;
}