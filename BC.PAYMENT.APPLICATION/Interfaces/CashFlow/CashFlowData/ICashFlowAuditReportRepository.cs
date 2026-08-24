namespace BC.PAYMENT.APPLICATION.Interfaces.CashFlow.CashFlowData
{
    public interface ICashFlowAuditReportRepository
    {
        Task<List<PaymentCashFlowSubmittedModel>> GetPaymentCashFlowAuditReportByDateAsync(string dbCode,DateTime fromDate ,DateTime toDate,string status);
    }
}
