using BC.PAYMENT.CORE.DTO.ViewStock;
using BC.PAYMENT.CORE.Entities.ViewStock;

namespace BC.PAYMENT.CORE.Mappers;

public static class SaleStockMapper
{
    public static ItemSaleStockDto ToListDto(this ItemSaleStockModel model)
    {
        return new ItemSaleStockDto()
        {
            DbCode = model.DbCode,
            ItemCode = model.ItemCode,
            AvailableQty =  model.AvailableQty,
            ImageUrl =  model.ImageUrl,
            ItemDesc =  model.ItemDesc,
            ItemNameKhmer =  model.ItemNameKhmer,
            SaleQty =  model.SaleQty,
            SaleStatus =   model.SaleStatus,
            StockStatus =   model.StockStatus,
            StockStatusDesc =  model.StockStatusDesc,
        };
    }
}