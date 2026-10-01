namespace BC.PAYMENT.INFRASTRUCTURE.Repository.ClosingInventoryAndInvoice;

public class OpeningBalanceRepository : IOpeningBalanceRepository
{
    private readonly ISqlDataAccess _sqlDataAccess;
    private readonly IConfiguration _setting;

    public OpeningBalanceRepository(ISqlDataAccess sqlDataAccess, IConfiguration setting)
    {
        _sqlDataAccess = sqlDataAccess;
        _setting = setting;
    }

    public async Task<List<OpeningBalanceModel>> LoadItemInStockByBranItemInStockByBranchCode(string code,
        string location)
    {
        var sql =
            @$"SELECT TAB5.LOCATION,  
                                       TAB5.ITEM_CODE ItemCode,  
                                       TAB5.ITEM_DESC ItemDesc,  
                                       TAB5.UNIT_STOCK UnitStock,  
                                       TAB5.PHYSICAL,  
                                       TAB5.ON_ORDER OnOrder,  
                                       ISNULL(TAB6.PICK_QTY, 0)  PickQty,
									   TAB5.PHYSICAL - TAB5.ON_ORDER FREE
                                       FROM(SELECT TAB3.LOCATION,  
                                       TAB3.ITEM_CODE,  
                                       TAB3.ITEM_DESC,  
                                       TAB3.UNIT_STOCK,  
                                       TAB3.PHYSICAL,  
                                       ISNULL(TAB4.HOLD_SALE, 0) ON_ORDER  
                                       FROM(SELECT TAB1.LOCATION, TAB1.ITEM_CODE, TAB2.ITEM_DESC, TAB2.UNIT_STOCK, PHYSICAL  
                                       FROM(SELECT LOCATION, ITEM_CODE, ISNULL(SUM(QUANTITY), 0) PHYSICAL  
                                       FROM  {code}SIINVMOV  
                                       WHERE IR_STAT = 'I'  
                                       AND STATUS = '80'  
                                       AND ALLOC_REF = ''  
                                       GROUP BY LOCATION, ITEM_CODE)  
                                       AS TAB1  
                                       LEFT JOIN(SELECT ITEM_CODE, ITEM_DESC, UNIT_STOCK  
                                       FROM SIITEMS  
                                       WHERE DB_CODE = @DB_CODE) AS TAB2 ON TAB1.ITEM_CODE = TAB2.ITEM_CODE) AS TAB3  
                                       LEFT JOIN  
                                       (SELECT LOCATION,  
                                       ITEM_CODE,  
                                       SUM(CASE  
                                       WHEN STK_QTY_VALUE = 1 THEN VALUE_1  
                                       WHEN  
                                       STK_QTY_VALUE = 2 THEN VALUE_2  
                                       WHEN STK_QTY_VALUE = 3 THEN VALUE_3  
                                       WHEN  
                                       STK_QTY_VALUE = 4 THEN VALUE_4  
                                       WHEN STK_QTY_VALUE = 5 THEN VALUE_5  
                                       WHEN  
                                       STK_QTY_VALUE = 6 THEN VALUE_6  
                                       WHEN STK_QTY_VALUE = 7 THEN VALUE_7  
                                       WHEN  
                                       STK_QTY_VALUE = 8 THEN VALUE_8  
                                       WHEN STK_QTY_VALUE = 9 THEN VALUE_9  
                                       WHEN  
                                       STK_QTY_VALUE = 10 THEN VALUE_10  
                                       WHEN STK_QTY_VALUE = 11 THEN VALUE_11  
                                       WHEN  
                                       STK_QTY_VALUE = 12 THEN VALUE_12  
                                       WHEN STK_QTY_VALUE = 13 THEN VALUE_13 
                                       WHEN  
                                       STK_QTY_VALUE = 14 THEN VALUE_14 
                                       WHEN STK_QTY_VALUE = 15 THEN VALUE_15 
                                       WHEN 
                                       STK_QTY_VALUE = 16 THEN VALUE_16 
                                       WHEN STK_QTY_VALUE = 17 THEN VALUE_17 
                                       WHEN 
                                       STK_QTY_VALUE = 18 THEN VALUE_18 
                                       WHEN STK_QTY_VALUE = 19 THEN VALUE_19 
                                       WHEN 
                                       STK_QTY_VALUE = 20 THEN VALUE_20 
                                       ELSE 0 END) HOLD_SALE 
                                      FROM   {code}SISODET  
                                     WHERE REC_TYPE = 'D'  
                                     AND STATUS < '80'  
                                     AND CREDIT_STATUS = '' 
                                     GROUP BY LOCATION, ITEM_CODE)  
                                     AS TAB4 ON TAB3.LOCATION = TAB4.LOCATION AND TAB3.ITEM_CODE = TAB4.ITEM_CODE)  
                                     AS TAB5  
                                     LEFT JOIN(SELECT LOCATION, ITEM_CODE, SUM(QUANTITY) PICK_QTY  
                                     FROM  {code}SIINVMOVH  
                                     WHERE IR_STAT <> 'I' 
                                     AND STATUS = '10'  
                                     GROUP BY LOCATION, ITEM_CODE)  
                                     AS TAB6 ON TAB5.LOCATION = TAB6.LOCATION AND TAB5.ITEM_CODE = TAB6.ITEM_CODE  
                                     WHERE TAB5.LOCATION = @LOCATION";
        var param = new
        {
            DB_CODE = code,
            LOCATION = location
        };
        var results = await _sqlDataAccess.LoadData<OpeningBalanceModel, dynamic>(sql, param);
        return results.ToList();
    }

    public async Task<int> OpenClosingEntryInventoryAsync(List<OpeningBalanceModel> ls)
    {
        var affectedRows = 0;
        var connection = new SqlConnection(_setting.GetConnectionString("DBConnection"));
        if (connection.State == ConnectionState.Closed)
            connection.Open();

        var transaction = await connection.BeginTransactionAsync();
        try
        {
            var sql =
                @"INSERT INTO TB_BC_OPENING_STOCK_BALANCE (DB_CODE,LOCATION,ITEM_CODE,ITEM_NAME,QUANTITY,STATUS,CREATED_DATE,CREATED_BY)
            VALUES (@DB_CODE,@LOCATION,@ITEM_CODE,@ITEM_NAME,@QUANTITY,@STATUS,@CREATED_DATE,@CREATED_BY)";
            foreach (var item in ls)
            {
                var param = new
                {
                    DB_CODE = item.DbCode,
                    LOCATION = item.Location,
                    ITEM_CODE = item.ItemCode,
                    ITEM_NAME = item.ItemDesc,
                    QUANTITY = item.Physical,
                    STATUS = "OPB",
                    CREATED_DATE = DateTime.Today,
                    CREATED_BY = item.CreatedBy
                };
                await connection.ExecuteAsync(
                    @"INSERT INTO TB_BC_CLOSING_ENTRY (DB_CODE,LOCATION,ITEM_CODE,OPENING_QTY,CREATED_DATE,CREATED_BY,STATUS,CLOSING_TYPE)
			        VALUES (@DB_CODE,@LOCATION,@ITEM_CODE,@OPENING_QTY,@CREATED_DATE,@CREATED_BY,@STATUS,'Opening')",
                    new
                    {
                        DB_CODE = item.DbCode,
                        LOCATION = item.Location,
                        ITEM_CODE = item.ItemCode,
                        ITEM_NAME = item.ItemDesc,
                        QUANTITY = item.Physical,
                        CREATED_DATE = DateTime.Today,
                        CREATED_BY = item.CreatedBy,
                        STATUS = 0
                    }, transaction);

                affectedRows += await connection.ExecuteAsync(sql, param, transaction);
            }

            if (ls.Count == affectedRows)
            {
                await transaction.CommitAsync();
                return affectedRows;
            }
            else
            {
                await transaction.RollbackAsync();
                return 0;
            }
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new Exception("Error while opening inventory", ex);
        }
    }
}