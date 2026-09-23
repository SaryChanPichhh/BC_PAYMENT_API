using System.ComponentModel.DataAnnotations;

namespace BC.PAYMENT.CORE.Contracts.Request.StockCar;

public class CreateTransferMoneyRequest
{
    [Required]
    public DateTime TransactionDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public int Amount { get; set; }
    public double DollarFromEmployee { get; set; }
    public double RielFromEmployee { get; set; }
    public double ExchangeRateEmployee { get; set; }
    public double? DepositDollar { get; set; }
    public double? DepositRiel { get; set; }
    public double? DepositExchange { get; set; }
    public int EmployeeId { get; set; }
    public int TemplateId { get; set; }
}
