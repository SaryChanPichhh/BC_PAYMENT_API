using BC.PAYMENT.CORE.DTO.CashFlow.CashFlowData;
using BC.PAYMENT.CORE.Entities.CashFlow.CashFlowData;

namespace BC.PAYMENT.APPLICATION.Interfaces.CashFlow.CashFlowData
{
    public interface ICashFlowDataRepository
    {
        #region Cash Flow Header

        Task<List<CashFlowDataHeader>> GetCashFlowHeaderAsync(string dbCode);
        Task<int> AddNewCashFlowHeaderAsync(CashFlowDataModel model);
        Task<int> CancelCashFlowHeaderAsync(int id);
        Task<int> DeleteCashFlowHeaderAsync(int id);
        Task<int> UpdateCashFlowHeaderAsync(int id, string cashFlowDate);

        #endregion

        #region Cash Flow Detail
        Task<List<CashFlowDataDetailModel>> GetPaymentCashFlowDetailByIdAsync(string dbCode, int id);
        Task<int> UpdateCashFlowDetailAsync(CashFlowDataDetailModel model);
        Task<int> AddNewCashFlowDetailAsync(CashFlowDataDetailModel model);
        Task<int> AddNewCashFlowSubmittedAsync(List<CashFlowDataDetailModel> model);
        Task<int> DeleteCashFlowDetailAsync(int id);
        Task<List<string>> GetDescriptionCashFlowDetailAsync(string dbCode);
        #endregion

    }
}
