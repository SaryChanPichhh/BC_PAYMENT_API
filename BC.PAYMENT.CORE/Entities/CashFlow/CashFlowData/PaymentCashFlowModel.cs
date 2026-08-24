namespace BC.PAYMENT.CORE.Entities.CashFlow.CashFlowData
{
    public class PaymentCashFlowModel : PaymentCashFlowDto
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; }
        public SubmittedStatus? Status { get; set; }
    }
}
