using BC.PAYMENT.CORE.Contracts.CashFlow;

namespace BC.PAYMENT.APPLICATION.Interfaces.CashFlow;

public interface ICashFlowDetailRepository
{
    Task<int> AddAsync(string dbCode, CashFlowDetailRequest request, string createdBy, string entriesCode);
    Task<string?> GetHeaderStatusAsync(string dbCode, int headerId, DateTime date);
    Task<DateTime?> GetDetailDateAsync(string dbCode, int id);
    Task<int> UpdateAsync(string dbCode, int id, CashFlowDetailRequest request, string updatedBy);
}
