using BC.PAYMENT.CORE.Contracts.Response.Item;
using ItemDto = BC.PAYMENT.CORE.Contracts.Items.ItemDto;

namespace BC.PAYMENT.APPLICATION.Interfaces.Items
{
    public interface IItemRepository
    {
        Task<List<ItemResponse>> GetItemListAsync(string dbCode);
    }
}
