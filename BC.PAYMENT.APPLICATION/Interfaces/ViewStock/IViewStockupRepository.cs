using BC.PAYMENT.CORE.Entities.ViewStock;

namespace BC.PAYMENT.APPLICATION.Interfaces.ViewStock;

public interface IViewStockupRepository
{
    Task<List<SalesTypeModel>> GetSalesTypesAsync(List<string> dbCodes, CancellationToken cancellationToken = default);
    Task<List<BranchModel>> GetDatabasesAsync(CancellationToken cancellationToken = default);
    Task<List<WarehouseModel>> GetWarehousesAsync(List<string> dbCodes, CancellationToken cancellationToken = default);
    Task<List<AreaModel>> GetAreasAsync(List<string> dbCodes, CancellationToken cancellationToken = default);
}
