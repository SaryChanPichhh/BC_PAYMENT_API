using BC.PAYMENT.CORE.Entities.CashFlow;

namespace BC.PAYMENT.APPLICATION.Interfaces.CashFlow;

public interface ICashFlowAuditRepository
{
    Task<List<CashFlowHeaderModel>> GetSubmittedHeadersAsync(string dbCode);
    Task<List<CashFlowModelSubmittedModel>> GetPendingDetailsAsync(string dbCode, int headerId);
    Task<int> UpdateStatusAsync(string dbCode, int headerId, SubmittedStatus status, string updatedBy);
}
