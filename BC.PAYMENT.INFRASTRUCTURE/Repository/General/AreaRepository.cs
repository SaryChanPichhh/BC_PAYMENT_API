using BC.PAYMENT.CORE.Contracts.Response.Area;
using BC.PAYMENT.CORE.Entities.General;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.General;

public class AreaRepository(ISqlDataAccess sqlDataAccess) : IAreaRepository
{
    public async Task<List<AreaResponse>> GetArea(string dbCode)
    {
        var param = new
        {
            DB_CODE = dbCode
        };
        var result = await sqlDataAccess.LoadData<AreaResponse, dynamic>(AreaQueries.GetAllArea, param);
        return result.ToList();
    }

    public async Task<bool> CreateAreaAsync(Area area)
    {
        var param = new
        {
            AreaId = area.AreaId,
            DbCode = area.DbCode,
            AreaName = area.AreaName,
            AreaNameKhmer = area.AreaNameKhmer,
            Other = area.Other,
            Status = area.Status,
            CreatedBy = area.CreatedBy,
            CreatedAt = area.CreatedAt
        };
        var result = await sqlDataAccess.ExecuteAsync(AreaQueries.CreateArea, param);
        return result > 0;
    }

    public async Task<bool> UpdateAreaAsync(Area area)
    {
        var param = new
        {
            AreaId = area.AreaId,
            DbCode = area.DbCode,
            AreaName = area.AreaName,
            AreaNameKhmer = area.AreaNameKhmer,
            Other = area.Other,
            Status = area.Status,
            UpdatedBy = area.UpdatedBy,
            UpdatedAt = area.UpdatedAt
        };
        var result = await sqlDataAccess.ExecuteAsync(AreaQueries.UpdateArea, param);
        return result > 0;
    }

    public async Task<bool> DeleteAreaAsync(string areaId, string dbCode)
    {
        var param = new { AreaId = areaId, DbCode = dbCode };
        var result = await sqlDataAccess.ExecuteAsync(AreaQueries.DeleteArea, param);
        return result > 0;
    }

    public async Task<string> GenerateAreaIdAsync()
    {
        var result = await sqlDataAccess.LoadSingleData<string, dynamic>(AreaQueries.GenerateAreaId, new { });
        return result;
    }
}