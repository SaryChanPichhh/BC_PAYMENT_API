namespace BC.PAYMENT.CORE.Entities.CashFlow;

public class CashFlowModelSubmittedModel : CashFlowModel
{
    public string SubmittedId { get; set; }
    public DateTime UpdatedDate { get; set; }
    public string UpdatedBy { get; set; }
}