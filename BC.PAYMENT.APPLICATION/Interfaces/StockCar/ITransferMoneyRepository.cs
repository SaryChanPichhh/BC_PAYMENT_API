using BC.PAYMENT.CORE.Contracts.Response.StockCar;
using BC.PAYMENT.CORE.Entities.StockCar;

namespace BC.PAYMENT.APPLICATION.Interfaces.StockCar;

public interface ITransferMoneyRepository
{
    Task<List<TransferMoneyResponse>> GetTransferMoneyAsync(string dbCode, int templateId);
    Task<TransferMoneyResponse?> GetTransferMoneyByIdAsync(string dbCode, int templateId, int id);
    Task<int> AddNewTransferMoneyAsync(TransferMoney transferMoney);
    Task<int> UpdateTransferMoneyAsync(TransferMoney transferMoney);
    Task<int> DeleteTransferMoneyAsync(int transferId);
    Task<TotalTransferMoneyResponse?> GetTotalTransferMoneyByTemplateIdAsync(string dbCode, int templateId);
}
