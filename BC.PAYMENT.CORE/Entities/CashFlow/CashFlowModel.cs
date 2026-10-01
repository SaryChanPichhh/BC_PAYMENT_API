namespace BC.PAYMENT.CORE.Entities.CashFlow;

public class CashFlowModel
{
    public int Id { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
    public string Name { get; set; }
    public double Amount { get; set; }
    public string CurrencyFormat { get; set; }
    public double ExchangeRate { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public string CreatedBy { get; set; }
    public SubmittedStatus? Status { get; set; }
}