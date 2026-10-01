namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Inventory;

public class WarehousePresetRepository : IWarehousePresetRepository
{
    private readonly ISqlDataAccess _sqlDataAccess;

    public WarehousePresetRepository(ISqlDataAccess sqlDataAccess)
    {
        _sqlDataAccess = sqlDataAccess;
    }

    public async Task<List<WarehousePresetModel>> GetWarehousePresetsAsync()
    {
        const string sql =
            @"SELECT ID Id,NAME Name,NOTE Note,STATUS Status,CREATED_DATE CreatedDate,CREATED_BY CreatedBy,UPDATED_BY UpdatedBy,UPDATED_DATE UpdatedDate FROM TB_BC_WAREHOUSE_PRESETS WHERE STATUS = 1";
        var results = await _sqlDataAccess.LoadData<WarehousePresetModel, dynamic>(sql, new { });
        return results.ToList();
    }

    public async Task<int> UpdateWarehouseAsync(WarehousePresetModel warehousePresetModel)
    {
        const string sql =
            @"UPDATE TB_BC_WAREHOUSE_PRESETS SET NAME = @NAME,NOTE = @NOTE,STATUS = @STATUS,UPDATED_DATE = @UPDATED_DATE,UPDATED_BY = @UPDATED_BY WHERE ID = @ID";
        var param = new
        {
            ID = warehousePresetModel.Id,
            NAME = warehousePresetModel.Name,
            NOTE = warehousePresetModel.Note,
            STATUS = warehousePresetModel.Status,
            UPDATED_DATE = warehousePresetModel.CreatedDate,
            UPDATED_BY = warehousePresetModel.CreatedBy
        };
        var results = await _sqlDataAccess.ExecuteAsync(sql, param);
        return results;
    }

    public async Task<int> DeleteWarehouseAsync(int id)
    {
        const string sql = @"DELETE TB_BC_WAREHOUSE_PRESETS WHERE ID = @ID";
        var param = new
        {
            ID = id
        };
        var results = await _sqlDataAccess.ExecuteAsync(sql, param);
        return results;
    }

    public async Task<int> CreateWarehouseAsync(WarehousePresetModel warehousePresetModel)
    {
        const string sql =
            @"INSERT INTO TB_BC_WAREHOUSE_PRESETS(NAME,NOTE,STATUS,CREATED_DATE,CREATED_BY) VALUES(@NAME,@NOTE,@STATUS,@CREATED_DATE,@CREATED_BY)";
        var param = new
        {
            NAME = warehousePresetModel.Name,
            NOTE = warehousePresetModel.Note,
            STATUS = warehousePresetModel.Status,
            CREATED_DATE = warehousePresetModel.CreatedDate,
            CREATED_BY = warehousePresetModel.CreatedBy
        };
        var results = await _sqlDataAccess.ExecuteAsync(sql, param);
        return results;
    }
}