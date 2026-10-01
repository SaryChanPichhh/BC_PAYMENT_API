namespace BC.PAYMENT.CORE.DTO.Transaction.ProvincialPayment.StockCarPayment;

public class TransferMoneyDto
{
    public DateTime TransactionDate { get; set; }
    public string? Description { get; set; }
    public int Amount { get; set; }
    public double DollarFromEmployee { get; set; }
    public double RielFromEmployee { get; set; }
    public double ExchangeRateEmployee { get; set; }
    public int EmployeeId { get; set; }
    public int TemplateId { get; set; }
    public double DepositDollar { get; set; }
    public double DepositRiel { get; set; }
    public double DepositExchange { get; set; }
}

public class TransferMoneyUpdateDto : TransferMoneyDto
{
    [Required] public int Id { get; set; }
}