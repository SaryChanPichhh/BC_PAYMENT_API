using BC.PAYMENT.APPLICATION.Interfaces.ViewStock;
using BC.PAYMENT.CORE.Entities.ViewStock;
using BC.PAYMENT.INFRASTRUCTURE.DBAccess;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.ViewStock;

public sealed class ViewStocRepository : IViewStockupRepository
{
    private readonly ISqlDataAccess _sqlDataAccess;

    public ViewStocRepository(ISqlDataAccess sqlDataAccess)
    {
        _sqlDataAccess = sqlDataAccess
                         ?? throw new ArgumentNullException(nameof(sqlDataAccess));
    }

    public async Task<List<SalesTypeModel>> GetSalesTypesAsync(
        List<string> dbCodes,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        const string sql =
            "SELECT S.DB_CODE AS DbCode,S.CODE AS Code FROM dbo.SIDATA S WHERE S.SI_TYPE = 'SALES' AND S.SI_LOOKUP = 'A' AND S.DB_CODE IN @DB_CODES ORDER BY S.DB_CODE, S.CODE;";

        var rows = await _sqlDataAccess.LoadData<SalesTypeModel, object>(
            sql,
            new { DB_CODES = dbCodes });

        return rows.ToList();
    }

    public async Task<List<BranchModel>> GetDatabasesAsync(
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        const string sql =
            "SELECT DB_CODE AS DbCode, DB_NAME AS DbName FROM dbo.SIDBINFO WHERE DB_STAT = 'A' ORDER BY DB_CODE;";

        var rows = await _sqlDataAccess.LoadData<BranchModel, object>(
            sql,
            new { });

        return rows.ToList();
    }

    public async Task<List<WarehouseModel>> GetWarehousesAsync(
        List<string> dbCodes,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        const string sql =
            "SELECT SI.DB_CODE AS DbCode, SI.WAR_CODE AS WarCode, SI.WAR_NAME AS WarName FROM dbo.SIWAREH SI WHERE SI.DB_CODE IN @DB_CODES ORDER BY SI.DB_CODE, SI.WAR_CODE;";

        var rows = await _sqlDataAccess.LoadData<WarehouseModel, object>(
            sql,
            new { DB_CODES = dbCodes });

        return rows.ToList();
    }

    public async Task<List<AreaModel>> GetAreasAsync(
        List<string> dbCodes,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        const string sql =
            "SELECT TB.DB_CODE AS DbCode, TB.AREA_ID AS AreaId, TB.AREA_NAME AS AreaName, TB.AREA_NAME_KHMER AS AreaNameKhmer FROM dbo.TB_AREAS TB WHERE TB.DB_CODE IN @DB_CODES ORDER BY TB.DB_CODE, TB.AREA_NAME;";

        var rows = await _sqlDataAccess.LoadData<AreaModel, object>(
            sql,
            new { DB_CODES = dbCodes });

        return rows.ToList();
    }
}