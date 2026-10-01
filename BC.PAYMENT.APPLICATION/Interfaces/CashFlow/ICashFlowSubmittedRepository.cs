using BC.PAYMENT.CORE.Entities.CashFlow;

namespace BC.PAYMENT.APPLICATION.Interfaces.CashFlow;

public interface ICashFlowSubmittedRepository
{
    Task<IEnumerable<CashFlowModelSubmittedModel>> GetSubmittedPaymentCashFlowsAsync(string dbCode, DateTime fromDate, DateTime toDate, SubmittedStatus? status);
    Task<IEnumerable<CashFlowModelSubmittedModel>> GetSubmittedPaymentCashFlowReportAsync(string dbCode, DateTime fromDate, DateTime toDate);
    Task<int> UpdateSubmittedStatusAsync(string dbCode, int id, SubmittedStatus submittedStatus, string updatedBy);
    Task<int> DeleteSubmittedAsync(string dbCode, int id);
}
