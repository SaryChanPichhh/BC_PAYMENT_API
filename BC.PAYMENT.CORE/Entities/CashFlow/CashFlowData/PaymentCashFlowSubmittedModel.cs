namespace BC.PAYMENT.CORE.Entities.CashFlow.CashFlowData;

public class PaymentCashFlowSubmittedModel : PaymentCashFlowDto
{
    public string SubmittedId { get; set; }
    public DateTime UpdatedDate { get; set; }
    public string UpdatedBy { get; set; }
}