namespace BC.PAYMENT.APPLICATION.Interfaces.CashFlow.CashFlowData;

public interface ICashFlowDataRepository
{
    #region Cash Flow Header

    Task<List<CashFlowDataHeader>> GetCashFlowHeaderAsync(string dbCode);
    Task<int> AddNewCashFlowHeaderAsync(CashFlowDataModel model);
    Task<int> CancelCashFlowHeaderAsync(string dbCode, int id);
    Task<int> DeleteCashFlowHeaderAsync(string dbCode, int id);
    Task<int> UpdateCashFlowHeaderAsync(string dbCode, int id, string cashFlowDate);

    #endregion

    #region Cash Flow Detail

    Task<List<CashFlowDataDetailModel>> GetPaymentCashFlowDetailByIdAsync(string dbCode, int id);
    Task<int> UpdateCashFlowDetailAsync(CashFlowDataDetailModel model);
    Task<int> AddNewCashFlowDetailAsync(CashFlowDataDetailModel model);
    Task<int> AddNewCashFlowSubmittedAsync(List<CashFlowDataDetailModel> model);
    Task<int> DeleteCashFlowDetailAsync(string dbCode, int id);
    Task<List<string>> GetDescriptionCashFlowDetailAsync(string dbCode);

    #endregion
}