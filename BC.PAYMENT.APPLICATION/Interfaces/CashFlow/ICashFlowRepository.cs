using BC.PAYMENT.CORE.Entities.CashFlow;

namespace BC.PAYMENT.APPLICATION.Interfaces.CashFlow;

public interface ICashFlowRepository
{
    Task<List<CashFlowModel>> GetPaymentCashFlowsAsync(string dbCode, DateTime fromDate, DateTime toDate);
    Task<List<CashFlowModel>> GetPaymentsByDbCodeAndEntryCodeAsync(string dbCode, string entryCode, DateTime fromDate, DateTime toDate);
    Task<List<CashFlowModel>> GetPaymentsByDbCodeAndMultiEntryCodesAsync(string dbCode, List<string> entryCodes);
}
