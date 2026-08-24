using ItemDto = BC.PAYMENT.CORE.DTO.Items.ItemDto;

namespace BC.PAYMENT.APPLICATION.Interfaces.Items
{
    public interface IItemRepository
    {
        Task<List<ItemDto>> GetItemListAsync(string dbCode);
    }
}
