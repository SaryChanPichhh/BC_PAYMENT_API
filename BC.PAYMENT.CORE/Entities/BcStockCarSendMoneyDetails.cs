namespace BC.PAYMENT.CORE.Entities;

public class BcStockCarSendMoneyDetails
{
    public int Id { get; set; }
    public int PaidId { get; set; }
    public DateTime TransactionDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal Dollar { get; set; }
    public decimal Riel { get; set; }
    public decimal Exchange { get; set; }
    public decimal DepositDollar { get; set; }
    public decimal DepositRiel { get; set; }
    public decimal DepositExchange { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } =  string.Empty;
    public string DbCode { get; set; } = string.Empty;
    public string Employee { get; set; } =  string.Empty;
    public bool Enable { get; set; }
    public int TemplateId { get; set; }
    
}