using ItemModel = BC.PAYMENT.CORE.Entities.CommondityExchange.DailyRefundItems.ItemModel;

namespace BC.PAYMENT.APPLICATION.Interfaces.CommondityExchange.DailyRefundItems;

public interface IDailyRefundItemRepository
{
    Task<List<ItemModel>> GetItemsRefundByAllBranchAsync();
    Task<List<ItemModel>> GetItemsRefundByByBranchAsync(string dbCode);
    Task<int> DeleteRequestItem(int requestMaster, int requestDetail);
    Task<int> InsertReceivedItem(string userName, int detailId);
    Task<byte[]> GetImageByDetailIdAsync(int detailId);
}