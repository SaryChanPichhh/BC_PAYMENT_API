using BC.PAYMENT.CORE.DTO.Preset.ExchangeItemAnalysis;
using BC.PAYMENT.CORE.Entities.ViewStock;

namespace BC.PAYMENT.API.Mapper;

public static class SaleTypeMapper
{
    public static SaleTypeDto ToDto(this SalesTypeModel model)
    {
        return new SaleTypeDto
        {
            FromPeriod = model.Code
        };
    }
}