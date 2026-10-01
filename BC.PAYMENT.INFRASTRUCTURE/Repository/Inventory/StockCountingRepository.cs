using BC.PAYMENT.APPLICATION.Interfaces.Inventory;
using BC.PAYMENT.CORE.Contracts.Response.Inventory.StockCounting;
using BC.PAYMENT.CORE.Entities;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Inventory;

public class StockCountingRepository(ISqlDataAccess sqlDataAccess, IDbConnection dbConnection)
    : IStockCountingRepository
{
    public async Task<List<ScStockResponse>> GetCountingStockAsync(string dbCode)
    {
        const string sql =
            @"SELECT ST.STOCK_ID StockId,ST.STOCK_NAME StockName,ST.WAREHOUSE Warehouse,ST.STOCK_CONTROLLER StockController
,ST.PARTICIPATION Participation,ST.PERION Period,ST.COUNT_DATE CountDate
FROM SC_STOCK ST WHERE ST.DB_CODE = @DBCODE AND ST.STATUS = 1";
        var param = new
        {
            DBCODE = dbCode
        };
        var results = await sqlDataAccess.LoadData<ScStockResponse, dynamic>(sql, param);
        return results.ToList();
    }

    public async Task<List<ScCountItemResponse>> GetCountingStockItemDetailAsync(string dbCode, int stockId)
    {
        var sql =
            $@"SELECT ID Id,ITEM_CODE ItemCode,ITEM_DESC ItemDesc,ITEM_DESC_KH ItemDescKh,QUANTITY Quantity 
                FROM SC_COUNT_ITEM WHERE STATUS = 1 AND DB_CODE = @DB_CODE AND STOCK_ID = @STOCK_ID";
        var arguments = new
        {
            DB_CODE = dbCode,
            STOCK_ID = stockId
        };
        var execute = await sqlDataAccess.LoadData<ScCountItemResponse, dynamic>(sql, arguments);
        return execute.ToList();
    }

    public async Task<int> UpdateCountingStockItemDetailAsync(ScCountItem req)
    {
        var sql =
            $@"UPDATE SC_COUNT_ITEM SET ITEM_CODE = @ITEM_CODE, ITEM_DESC = @ITEM_DESC, ITEM_DESC_KH = @ITEM_DESC_KH, QUANTITY = @QTY, 
                         UPDATE_DATE = CONVERT(DATE, GETDATE()), UPDATE_BY = @UPDATE_BY WHERE ID = @ID";
        var param = new
        {
            ITEM_CODE = req.ItemCode,
            ITEM_DESC = req.ItemDesc,
            ITEM_DESC_KH = req.ItemDescKh,
            QTY = req.Quantity,
            UPDATE_BY = req.UpdatedBy,
            ID = req.Id
        }; // ← Add the ID parameter here
        return await sqlDataAccess.ExecuteAsync(sql, param);
    }

    public Task<int> DeleteCountingStockItemDetailAsync(int id)
    {
        var sql = $@"DELETE FROM SC_COUNT_ITEM WHERE ID = @ID";
        return sqlDataAccess.ExecuteAsync(sql, new { ID = id });
    }

    public async Task<int> SaveCountingItemDetailAsync(ScCountItem req)
    {
        var insertSql = @"
                INSERT INTO SC_COUNT_ITEM 
                (DB_CODE, ITEM_CODE, ITEM_DESC, ITEM_DESC_KH, QUANTITY, STATUS, STOCK_ID, CREATE_DATE, CREATE_BY, PERIOD)
                VALUES
                (@DB_CODE, @ITEM_CODE, @ITEM_DSEC, @ITEM_DESC_KH, @QUANTITY, 1, @STOCK_ID, CONVERT(DATE, GETDATE()), @CREATED_BY, @PERIOD)";

        var insertParams = new
        {
            DB_CODE = req.DbCode,
            ITEM_CODE = req.ItemCode,
            ITEM_DSEC = req.ItemDesc,
            ITEM_DESC_KH = req.ItemDescKh,
            QUANTITY = req.Quantity,
            STOCK_ID = req.StockId,
            CREATED_BY = req.CreatedBy,
            PERIOD = req.Period
        };
        var affectedRow = await sqlDataAccess.ExecuteAsync(insertSql, insertParams);
        return affectedRow;
    }

    public async Task<int> SaveCountingItemDetailAsync(ScStock header, List<ScCountItem> req)
    {
        var deleteSql = @"DELETE FROM SC_COUNT_ITEM WHERE DB_CODE = @DB_CODE AND STOCK_ID = @STOCK_ID";
        var deleteParam = new { DB_CODE = header.DbCode, STOCK_ID = header.StockId };
        var insertSql = @"
                INSERT INTO SC_COUNT_ITEM 
                (DB_CODE, ITEM_CODE, ITEM_DESC, ITEM_DESC_KH, QUANTITY, STATUS, STOCK_ID, CREATE_DATE, CREATE_BY, PERIOD)
                VALUES
                (@DB_CODE, @ITEM_CODE, @ITEM_DSEC, @ITEM_DESC_KH, @QUANTITY, 1, @STOCK_ID, CONVERT(DATE, GETDATE()), @CREATED_BY, @PERIOD)";

        var insertParams = req.Select(item => new
        {
            DB_CODE = item.DbCode,
            ITEM_CODE = item.ItemCode,
            ITEM_DSEC = item.ItemDesc,
            ITEM_DESC_KH = item.ItemDescKh,
            QUANTITY = item.Quantity,
            STOCK_ID = item.StockId,
            CREATED_BY = item.CreatedBy,
            PERIOD = item.Period
        }).ToList();
        dbConnection.Open();

        using var tx = dbConnection.BeginTransaction();
        try
        {
            // Step 1: DELETE
            await dbConnection.ExecuteAsync(deleteSql, deleteParam, tx);

            // Step 2: BULK INSERT
            var affectedRows = await dbConnection.ExecuteAsync(insertSql, insertParams, tx);
            tx.Commit();
            return affectedRows; // returns number of rows inserted
        }
        catch (Exception)
        {
            tx.Rollback();
            dbConnection.Close();
            throw;
        }
        finally
        {
            dbConnection.Close();
        }
    }
}