using System.ComponentModel.DataAnnotations;

namespace BC.PAYMENT.CORE.Contracts.Request.StockCar;

public class UpdateTransferMoneyRequest
{
    [Required] public int Id { get; set; }
    [Required] public DateTime TransactionDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public int Amount { get; set; }
    public double DollarFromEmployee { get; set; }
    public double RielFromEmployee { get; set; }
    public double ExchangeRateEmployee { get; set; }
    public double? DepositDollar { get; set; }
    public double? DepositRiel { get; set; }
    public double? DepositExchange { get; set; }
}