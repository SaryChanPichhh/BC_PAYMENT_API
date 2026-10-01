namespace BC.PAYMENT.CORE.Entities.CashFlow;

public class CashFlowHeaderModel
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string EntriesCode { get; set; }
    public SubmittedStatus? Status { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public int PendingCount { get; set; }
}
