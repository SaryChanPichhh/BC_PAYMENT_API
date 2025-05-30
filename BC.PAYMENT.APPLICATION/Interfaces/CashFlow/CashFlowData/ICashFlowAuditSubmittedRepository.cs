
using BC.PAYMENT.CORE.Entities.CashFlow.CashFlowData;

namespace BC.PAYMENT.APPLICATION.Interfaces.CashFlow.CashFlowData
{
    public interface ICashFlowAuditSubmittedRepository
    {
        Task<List<CashFlowDataHeader>> GetCashFlowHeaderSubmittedAsync(string dbCode);
        Task<int> AuditCashFlowAsync(string createBy,int headerId,string status);
        Task<List<PaymentCashFlowModel>> GetCashFlowDetailPendingAsync(string dbCode, int headerId);
    }
}
