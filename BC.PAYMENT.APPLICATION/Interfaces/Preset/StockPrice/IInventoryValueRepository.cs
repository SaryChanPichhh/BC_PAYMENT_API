namespace BC.PAYMENT.APPLICATION.Interfaces.Preset.StockPrice
{
    public interface IInventoryValueRepository
    {
        Task<List<InventoryValueModel>> GetInventoryValueAsync(Dictionary<string,string> branch,int page,int pageSize);
    }
}
