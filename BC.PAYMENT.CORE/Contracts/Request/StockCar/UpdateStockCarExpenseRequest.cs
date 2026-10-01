using System.ComponentModel.DataAnnotations;

namespace BC.PAYMENT.CORE.Contracts.Request.StockCar;

public class UpdateStockCarExpenseRequest
{
    [Required] public int Id { get; set; }
    public int ExpenseTypeId { get; set; }
    public int ProvinceId { get; set; }
    public double Quantity { get; set; }
    public double UnitPrice { get; set; }
    public double AmountDollar { get; set; }
    public double AmountRiel { get; set; }
    public double ExchangeRate { get; set; }
    [Required] public DateTime ExpenseDate { get; set; }
}