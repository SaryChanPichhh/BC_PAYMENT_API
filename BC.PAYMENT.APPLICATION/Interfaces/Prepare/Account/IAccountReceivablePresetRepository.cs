using BC.PAYMENT.APPLICATION.Interfaces.BaseInterface;
using BC.PAYMENT.CORE.DTO.Prepare.Account;
using BC.PAYMENT.CORE.Entities.Prepare.Account;

namespace BC.PAYMENT.APPLICATION.Interfaces.Prepare.Account
{
    public interface IAccountReceivablePresetRepository : IBaseRepository<AccountReceivablePresetModel>
    {
        Task<int> DeleteAccountReceivableAsync(AccountReceivablePresetModel model);
    }
}
