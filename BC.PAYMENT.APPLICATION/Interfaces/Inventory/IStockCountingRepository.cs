using BC.PAYMENT.CORE.Contracts.Response.Inventory.StockCounting;
using BC.PAYMENT.CORE.Entities;

namespace BC.PAYMENT.APPLICATION.Interfaces.Inventory
{
    public interface IStockCountingRepository
    {
        Task<List<ScStockResponse>> GetCountingStockAsync(string dbCode);
        Task<List<ScCountItemResponse>> GetCountingStockItemDetailAsync(string dbCode,int stockId);
        Task<int> UpdateCountingStockItemDetailAsync(ScCountItem req);
        Task<int> DeleteCountingStockItemDetailAsync(int id);
        Task<int> SaveCountingItemDetailAsync(ScCountItem req);
        Task<int> SaveCountingItemDetailAsync(ScStock header,List<ScCountItem> req);
    }
}
