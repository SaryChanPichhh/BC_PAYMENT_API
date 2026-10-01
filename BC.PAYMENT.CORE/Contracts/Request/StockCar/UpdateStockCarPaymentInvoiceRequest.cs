using System.ComponentModel.DataAnnotations;

namespace BC.PAYMENT.CORE.Contracts.Request.StockCar;

public class UpdateStockCarPaymentInvoiceRequest
{
    [Required] public int Id { get; set; }
    [Required] public double Amount { get; set; }
}