namespace BC.PAYMENT.APPLICATION.Interfaces.Transaction.Audit.StockInventoryCounting
{
    public interface IStockInventoryCountingRepository
    {
        Task<List<StockInventoryCountingModel>> GetInventoryCountingAsync(string dbCode);
        Task<int> InsertInventoryCountingAsync(StockInventoryCountingModel model);
        Task<int> UpdateInventoryCountingAsync(StockInventoryCountingModel model);
        Task<int> DeleteInventoryCountingAsync(string code);
    }
}
