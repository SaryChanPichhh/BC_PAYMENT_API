namespace BC.PAYMENT.APPLICATION.Interfaces.Preset.ItemTransaction;

public interface IItemTransactionAnalysisRepository
{
    Task<List<ItemTransactionAnalysisModel>> GetItemTransactionAsync(int fromMovPrd, int toMovPrd, string itemCode);

    Task<List<string>> GetItemByPeriodAndItemsAsync(string DbCode, int fromMovPrd, int toMovPrd,
        List<string> lsItem);

    Task<List<string>> GetAllItemByPeriodAsync(string DbCode, int fromMovPrd, int toMovPrd);
}