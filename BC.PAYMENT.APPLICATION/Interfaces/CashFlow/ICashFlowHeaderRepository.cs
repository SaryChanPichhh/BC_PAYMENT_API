using BC.PAYMENT.CORE.Entities.CashFlow;

namespace BC.PAYMENT.APPLICATION.Interfaces.CashFlow;

public interface ICashFlowHeaderRepository
{
    Task<List<CashFlowHeaderModel>> GetAllAsync(string dbCode);
    Task<int> AddAsync(string dbCode, DateTime date, string createdBy, string entriesCode);
    Task<int> SubmitAsync(string dbCode, int id, string submittedBy);
    Task<int> CancelSubmitAsync(string dbCode, int id, string updatedBy);
    Task<int> DeleteAsync(string dbCode, int id);
}
