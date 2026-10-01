using BC.PAYMENT.CORE.Contracts.Request.AccountReceivable;
using static BC.PAYMENT.CORE.Entities.Accounting.AccountReceivableModel;

namespace BC.PAYMENT.APPLICATION.Interfaces.Accounting;

public interface IAccountReceivableRepository
{
    Task<int> InsertAccountReceivable(SiLedgerRequest request, bool isAccountsReceivableCompleted = false);
    Task<List<AccountReceivablePatternModel>> GetAccountReceivablePatterns(string dbCode);
    Task<dynamic> GetJournalTypesByDbCode(string dbCode);
    Task<int> GetJournalIdByDbCode(string dbCode);
    Task<int> GetAllocReferenceByDbCode(string dbCode);

    Task<bool> SplitAccountReceivable(string customerCode, string referenceNo, string dbCode,
        double currentAmount,
        double splitAmount);
}