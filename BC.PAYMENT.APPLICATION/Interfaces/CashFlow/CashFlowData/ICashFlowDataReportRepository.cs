using BC.PAYMENT.CORE.Entities.CashFlow.CashFlowData;

namespace BC.PAYMENT.APPLICATION.Interfaces.CashFlow.CashFlowData
{
    public interface ICashFlowDataReportRepository
    {
        Task<List<PaymentCashFlowModel>> GetPaymentCashFlowReportAsync(string dbCode);
        Task<List<PaymentCashFlowModel>> GetPaymentCashFlowReportByDateAsync(string dbCode,DateTime fromDate,DateTime toDate);
    }
}
