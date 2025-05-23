
using BC.PAYMENT.CORE.DTO.Items;

namespace BC.PAYMENT.APPLICATION.Interfaces.Items
{
    public interface IItemRepository
    {
        Task<List<ItemDto>> GetItemListAsync(string dbCode);
    }
}
