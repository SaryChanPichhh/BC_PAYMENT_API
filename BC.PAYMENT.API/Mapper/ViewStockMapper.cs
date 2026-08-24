using BC.PAYMENT.CORE.Entities.ViewStock;
using BC.PAYMENT.CORE.DTO.ViewStock;

namespace BC.PAYMENT.API.MapperHelper.ViewStock;

public static class ViewStockMapper
{
    public static SalesTypeDto ToDto(this SalesTypeModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        return new SalesTypeDto
        {
            DbCode = model.DbCode,
            Code = model.Code
        };
    }

    public static BranchDto ToDto(this BranchModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        return new BranchDto
        {
            DbCode = model.DbCode,
            DbName = model.DbName
        };
    }

    public static WarehouseDto ToDto(this WarehouseModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        return new WarehouseDto
        {
            DbCode = model.DbCode,
            WarCode = model.WarCode,
            WarName = model.WarName
        };
    }

    public static AreaDto ToDto(this AreaModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        return new AreaDto
        {
            DbCode = model.DbCode,
            AreaId = model.AreaId,
            AreaName = model.AreaName,
            AreaNameKhmer = model.AreaNameKhmer
        };
    }

    public static List<SalesTypeDto> ToDtoList(
        this IEnumerable<SalesTypeModel>? models)
    {
        return models?
                   .Select(static model => model.ToDto())
                   .ToList()
               ?? new List<SalesTypeDto>();
    }

    public static List<BranchDto> ToDtoList(
        this IEnumerable<BranchModel>? models)
    {
        return models?
                   .Select(static model => model.ToDto())
                   .ToList()
               ?? new List<BranchDto>();
    }

    public static List<WarehouseDto> ToDtoList(
        this IEnumerable<WarehouseModel>? models)
    {
        return models?
                   .Select(static model => model.ToDto())
                   .ToList()
               ?? new List<WarehouseDto>();
    }

    public static List<AreaDto> ToDtoList(
        this IEnumerable<AreaModel>? models)
    {
        return models?
                   .Select(static model => model.ToDto())
                   .ToList()
               ?? new List<AreaDto>();
    }
}