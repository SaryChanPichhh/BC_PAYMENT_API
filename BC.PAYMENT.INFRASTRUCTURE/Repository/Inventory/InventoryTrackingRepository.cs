namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Inventory;

public class InventoryTrackingRepository : IInventoryTrackingRepository
{
    private readonly ISqlDataAccess _sqlDataAccess;
    private readonly IConfiguration _setting;

    public InventoryTrackingRepository(ISqlDataAccess sqlDataAccess, IConfiguration setting)
    {
        _sqlDataAccess = sqlDataAccess;
        _setting = setting;
    }

    public async Task<List<InventoryTrackingWarehouseModel>> GetInventoryTrackingWarehouseListByDbCodeAsync(
        string dbCode)
    {
        const string sql =
            @"SELECT ID Id,WAREHOUSE Warehouse,DESCRIPTION Description,TRACKING_TYPE InventoryTrackingTypes,DB_CODE DbCode,
            SELECTED_TOTAL SelectedTotal,
            CREATED_DATE CreatedDate,
            CREATED_BY CreatedBy,
            UPDATED_BY UpdatedBy,
            UPDATED_DATE UpdatedDate
            FROM DT_INVENTORY_TRACKING_WAREHOUSE WHERE DB_CODE = @DB_CODE";
        var param = new { DB_CODE = dbCode };
        var results = await _sqlDataAccess.LoadData<InventoryTrackingWarehouseModel, dynamic>(sql, param);
        return results.ToList();
    }

    public async Task<bool> ExistInventoryTrackingWarehouseByDbCodeAndNameAsync(string dbCode, string name)
    {
        const string sql =
            @"SELECT COUNT(*) FROM DT_INVENTORY_TRACKING_WAREHOUSE WHERE WAREHOUSE = @WAREHOUSE AND DB_CODE = @DB_CODE";
        var param = new { DB_CODE = dbCode, WAREHOUSE = name };
        var exists = await _sqlDataAccess.ExecuteScalarAsync<int, dynamic>(sql, param);
        return exists > 0;
    }

    public async Task<int> UpdateInventoryTrackingAsync(InventoryTrackingWarehouseModel model)
    {
        const string sql =
            @"UPDATE DT_INVENTORY_TRACKING_WAREHOUSE SET WAREHOUSE = @WAREHOUSE,DESCRIPTION = @DESCRIPTION,TRACKING_TYPE = @TRACKING_TYPE,SELECTED_TOTAL = @SELECTED_TOTAL,
            UPDATED_DATE = @UPDATED_DATE,UPDATED_BY = @UPDATED_BY
            WHERE ID = @ID AND DB_CODE = @DB_CODE";
        var param = new
        {
            ID = model.Id,
            DB_CODE = model.DbCode,
            WAREHOUSE = model.Warehouse,
            DESCRIPTION = model.Description,
            TRACKING_TYPE = model.InventoryTrackingTypes.ToString(),
            SELECTED_TOTAL = model.SelectedTotal,
            UPDATED_DATE = model.UpdatedDate,
            UPDATED_BY = model.UpdatedBy
        };
        return await _sqlDataAccess.ExecuteAsync(sql, param);
    }

    public async Task<int> DeleteInventoryTrackingAsync(string dbCode, string Id)
    {
        const string sql =
            @"DELETE FROM DT_INVENTORY_TRACKING_WAREHOUSE WHERE ID = @ID AND DB_CODE = @DB_CODE";
        var param = new
        {
            DB_CODE = dbCode,
            ID = Id
        };
        return await _sqlDataAccess.ExecuteAsync(sql, param);
    }

    public async Task<List<InventoryTrackingWarehouseDataModel>>
        GetInventoryTrackingWarehouseDataListByDbCodeAndWarehouseAsync(string dbCode, string warehouse,
            InventoryTrackingTypes inventoryTrackingTypes)
    {
        const string sql =
            @"SELECT ID Id,WAREHOUSE Warehouse,ITEM_CODE ItemCode,ITEM_DESC ItemDescription,QUANTITY Quantity,CREATED_BY CreatedBy,CREATED_DATE CreatedDate,TRANS_DATE TransactionDate FROM DT_INVENTORY_TRACKING_WAREHOUSE_DATA
            WHERE DB_CODE = @DB_CODE AND WAREHOUSE = @WAREHOUSE AND TRACKING_TYPE = @TRACKING_TYPE AND STATUS = 'E'";
        var param = new { DB_CODE = dbCode, WAREHOUSE = warehouse, TRACKING_TYPE = inventoryTrackingTypes.ToString() };
        var results = await _sqlDataAccess.LoadData<InventoryTrackingWarehouseDataModel, dynamic>(sql, param);
        return results.ToList();
    }

    public async Task<int> AddWarehouseData(InventoryTrackingWarehouseDataModel model)
    {
        const string sql =
            @"INSERT INTO DT_INVENTORY_TRACKING_WAREHOUSE_DATA(DB_CODE,TRACKING_TYPE,WAREHOUSE,ITEM_CODE,ITEM_DESC,QUANTITY,CREATED_DATE,CREATED_BY,TRANS_DATE,REFERENCE,STATUS)
            VALUES(@DB_CODE,@TRACKING_TYPE,@WAREHOUSE,@ITEM_CODE,@ITEM_DESC,@QUANTITY,@CREATED_DATE,@CREATED_BY,@TRANSACTION_DATE,@REFERENCE,@STATUS)";
        var rowAffected = 0;
        switch (model.InventoryTrackingTypes)
        {
            case InventoryTrackingTypes.Plus:
            {
                var param = new
                {
                    DB_CODE = model.DbCode,
                    TRACKING_TYPE = model.InventoryTrackingTypes.ToString(),
                    WAREHOUSE = model.Warehouse,
                    ITEM_CODE = model.ItemCode,
                    ITEM_DESC = model.ItemDescription,
                    QUANTITY = model.Quantity,
                    CREATED_DATE = model.CreatedDate,
                    CREATED_BY = model.CreatedBy,
                    TRANSACTION_DATE = model.TransactionDate,
                    REFERENCE = model.Warehouse,
                    STATUS = "E"
                };
                rowAffected += await _sqlDataAccess.ExecuteAsync(sql, param);
                break;
            }
            case InventoryTrackingTypes.Subtract:
            {
                model.Quantity *= -1;
                var param = new
                {
                    DB_CODE = model.DbCode,
                    TRACKING_TYPE = model.InventoryTrackingTypes.ToString(),
                    WAREHOUSE = model.Warehouse,
                    ITEM_CODE = model.ItemCode,
                    ITEM_DESC = model.ItemDescription,
                    QUANTITY = model.Quantity,
                    CREATED_DATE = model.CreatedDate,
                    CREATED_BY = model.CreatedBy,
                    TRANSACTION_DATE = model.TransactionDate,
                    REFERENCE = model.Warehouse,
                    STATUS = "E"
                };
                rowAffected += await _sqlDataAccess.ExecuteAsync(sql, param);
                break;
            }
            case InventoryTrackingTypes.Transfer:
            {
                var quantityTransfer = model.Quantity * -1;
                var paramTransfer = new
                {
                    DB_CODE = model.DbCode,
                    TRACKING_TYPE = model.InventoryTrackingTypes.ToString(),
                    WAREHOUSE = model.Warehouse,
                    ITEM_CODE = model.ItemCode,
                    ITEM_DESC = model.ItemDescription,
                    QUANTITY = quantityTransfer,
                    CREATED_DATE = model.CreatedDate,
                    CREATED_BY = model.CreatedBy,
                    TRANSACTION_DATE = model.TransactionDate,
                    REFERENCE = model.ToWarehouse,
                    STATUS = "E"
                };
                rowAffected += await _sqlDataAccess.ExecuteAsync(sql, paramTransfer);
                var param = new
                {
                    DB_CODE = model.DbCode,
                    TRACKING_TYPE = InventoryTrackingTypes.Plus.ToString(),
                    WAREHOUSE = model.ToWarehouse,
                    ITEM_CODE = model.ItemCode,
                    ITEM_DESC = model.ItemDescription,
                    QUANTITY = model.Quantity,
                    CREATED_DATE = model.CreatedDate,
                    CREATED_BY = model.CreatedBy,
                    TRANSACTION_DATE = model.TransactionDate,
                    REFERENCE = model.Warehouse,
                    STATUS = "E"
                };
                rowAffected += await _sqlDataAccess.ExecuteAsync(sql, param);
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }

        return rowAffected;
    }

    public async Task<int> UpdateWarehouseData(InventoryTrackingWarehouseDataModel model)
    {
        const string sql =
            @"UPDATE DT_INVENTORY_TRACKING_WAREHOUSE_DATA SET ITEM_CODE = @ITEM_CODE,ITEM_DESC = @ITEM_DESC,QUANTITY = @QUANTITY,TRANS_DATE = @TRANSACTION_DATE 
            WHERE ID = @ID AND DB_CODE = @DB_CODE";
        var param = new
        {
            ITEM_CODE = model.ItemCode,
            ITEM_DESC = model.ItemDescription,
            QUANTITY = model.Quantity,
            ID = model.Id,
            DB_CODE = model.DbCode,
            TRANSACTION_DATE = model.TransactionDate
        };
        return await _sqlDataAccess.ExecuteAsync(sql, param);
    }

    public async Task<int> DeleteWarehouseData(string dbCode, string id)
    {
        const string sql = @"DELETE FROM DT_INVENTORY_TRACKING_WAREHOUSE_DATA WHERE ID = @ID AND DB_CODE = @DB_CODE";
        var param = new
        {
            ID = id,
            DB_CODE = dbCode
        };
        return await _sqlDataAccess.ExecuteAsync(sql, param);
    }

    public async Task<List<InventoryTrackingWarehouseDataModel>> GetSummaryInventoryTrackingByDbCodeAsync(string dbCode)
    {
        const string sql =
            @"SELECT SUM(QUANTITY) Quantity,D.WAREHOUSE Warehouse,ITEM_CODE ItemCode,CREATED_DATE CreatedDate FROM DT_INVENTORY_TRACKING_WAREHOUSE_DATA D
            WHERE WAREHOUSE IN (SELECT WAREHOUSE FROM DT_INVENTORY_TRACKING_WAREHOUSE WHERE SELECTED_TOTAL = '1' AND DB_CODE = @DB_CODE)
            AND D.DB_CODE=  @DB_CODE AND D.STATUS = 'E'
            GROUP BY D.WAREHOUSE,ITEM_CODE,CREATED_DATE";
        var param = new { DB_CODE = dbCode };
        var results = await _sqlDataAccess.LoadData<InventoryTrackingWarehouseDataModel, dynamic>(sql, param);
        return results.ToList();
    }

    public async Task<List<InventoryTrackingWarehouseDataModel>> GetSummaryInventoryTrackingByDbCodeAndDateAsync(
        string dbCode, DateTime dateTime)
    {
        const string sql =
            @"SELECT SUM(QUANTITY) Quantity,D.WAREHOUSE Warehouse,ITEM_CODE ItemCode,D.CREATED_DATE CreatedDate FROM DT_INVENTORY_TRACKING_WAREHOUSE_DATA D
            WHERE WAREHOUSE IN (SELECT WAREHOUSE FROM DT_INVENTORY_TRACKING_WAREHOUSE WHERE SELECTED_TOTAL = '1' AND DB_CODE = @DB_CODE)
            AND D.DB_CODE=  @DB_CODE AND CREATED_DATE=@CREATED_DATE AND  D.STATUS = 'E'
            GROUP BY D.WAREHOUSE,ITEM_CODE,D.CREATED_DATE";
        var param = new { DB_CODE = dbCode, CREATED_DATE = dateTime };
        var results = await _sqlDataAccess.LoadData<InventoryTrackingWarehouseDataModel, dynamic>(sql, param);
        return results.ToList();
    }

    public async Task<string> ClosingTrackingInventoryAsync(List<InventoryTrackingWarehouseDataModel> list)
    {
        const string sql =
            @"INSERT INTO DT_INVENTORY_TRACKING_PERMANENT(DB_CODE,WAREHOUSE,ITEM_CODE,QUANTITY,CREATED_DATE,CREATED_BY)
            VALUES(@DB_CODE,@WAREHOUSE,@ITEM_CODE,@QUANTITY,@CREATED_DATE,@CREATED_BY)";
        await using var connection = new SqlConnection(_setting.GetConnectionString("DBConnection"));
        if (connection.State == ConnectionState.Closed) await connection.OpenAsync();
        var transaction = connection.BeginTransaction();
        try
        {
            foreach (var item in list)
            {
                await connection.ExecuteAsync(sql,
                    new
                    {
                        DB_CODE = item.DbCode,
                        WAREHOUSE = item.Warehouse,
                        ITEM_CODE = item.ItemCode,
                        QUANTITY = item.Quantity,
                        CREATED_DATE = item.CreatedDate,
                        CREATED_BY = item.CreatedBy
                    }, transaction);
                await connection.ExecuteAsync(
                    @"UPDATE DT_INVENTORY_TRACKING_WAREHOUSE_DATA SET STATUS = 'D',UPDATED_DATE = @UPDATED_DATE,UPDATED_BY = @UPDATED_BY WHERE DB_CODE = @DB_CODE",
                    new
                    {
                        ITEM_CODE = item.ItemCode,
                        DB_CODE = item.DbCode,
                        WAREHOUSE = item.Warehouse,
                        UPDATED_DATE = DateTime.Now,
                        UPDATED_BY = item.CreatedBy
                    }, transaction);
            }

            transaction.Commit();
            return string.Empty;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            return ex.Message;
        }
    }

    public async Task<int> AddInventoryTrackingAsync(InventoryTrackingWarehouseModel model)
    {
        const string sql =
            @"INSERT INTO DT_INVENTORY_TRACKING_WAREHOUSE(DB_CODE,WAREHOUSE,DESCRIPTION,TRACKING_TYPE,SELECTED_TOTAL,CREATED_DATE,CREATED_BY)
            VALUES (@DB_CODE,@WAREHOUSE,@DESCRIPTION,@TRACKING_TYPE,@SELECTED_TOTAL,@CREATED_DATE,@CREATED_BY)";
        var param = new
        {
            DB_CODE = model.DbCode,
            WAREHOUSE = model.Warehouse,
            DESCRIPTION = model.Description,
            TRACKING_TYPE = model.InventoryTrackingTypes.ToString(),
            SELECTED_TOTAL = model.SelectedTotal,
            CREATED_DATE = model.CreatedDate,
            CREATED_BY = model.CreatedBy
        };
        return await _sqlDataAccess.ExecuteAsync(sql, param);
    }
}