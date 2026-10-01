using BC.PAYMENT.CORE.Contracts.Response.Area;
using BC.PAYMENT.CORE.Entities.General;

namespace BC.PAYMENT.APPLICATION.Interfaces.General;

public interface IAreaRepository
{
    Task<List<AreaResponse>> GetArea(string dbCode);
    Task<bool> CreateAreaAsync(Area area);
    Task<bool> UpdateAreaAsync(Area area);
    Task<bool> DeleteAreaAsync(string areaId, string dbCode);
    Task<string> GenerateAreaIdAsync();
}