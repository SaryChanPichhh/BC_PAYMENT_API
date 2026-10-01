namespace BC.PAYMENT.APPLICATION.Interfaces.Prepare.Account;

public interface IAccountReceivablePresetRepository : IBaseRepository<AccountReceivablePresetModel>
{
    Task<int> DeleteAccountReceivableAsync(AccountReceivablePresetModel model);
}