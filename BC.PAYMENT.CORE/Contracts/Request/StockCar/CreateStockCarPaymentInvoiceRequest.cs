using System.ComponentModel.DataAnnotations;

namespace BC.PAYMENT.CORE.Contracts.Request.StockCar;

public class CreateStockCarPaymentInvoiceRequest
{
    [Required] public int InvoiceId { get; set; }
    [Required] public double AmountPaid { get; set; }
    public string? CreatedBy { get; set; }
}