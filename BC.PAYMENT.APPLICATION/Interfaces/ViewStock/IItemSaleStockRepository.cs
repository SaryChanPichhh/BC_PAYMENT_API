using BC.PAYMENT.CORE.DTO.ViewStock;
using BC.PAYMENT.CORE.Entities.ViewStock;

namespace BC.PAYMENT.APPLICATION.Interfaces.General;

public interface IItemSaleStockRepository
{
    Task<List<ItemSaleStockModel>> GetItemSaleStockAsync(ItemSaleStockRequestDto request, string? imageUrl,
        CancellationToken cancellationToken = default);
}