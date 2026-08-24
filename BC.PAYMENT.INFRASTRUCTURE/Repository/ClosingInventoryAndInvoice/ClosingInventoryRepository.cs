namespace BC.PAYMENT.INFRASTRUCTURE.Repository.ClosingInventoryAndInvoice
{
    public class ClosingInventoryRepository : IClosingInventoryRepository
    {
        private readonly ISqlDataAccess _sqlDataAccess;
        private readonly IConfiguration _setting;

        public ClosingInventoryRepository(ISqlDataAccess sqlDataAccess, IConfiguration setting)
        {
            _sqlDataAccess = sqlDataAccess;
            _setting = setting;
        }

        public async Task<List<ClosingStockEntryModel>> GetClosingEntryDay(string dbCode, string date)
        {
            var sql = $@"SELECT DISTINCT
            T.OpeningItemCode ItemCode,
            T.OpeningLocation Location,
            T.CLOSING_BALANCE OpeningBalance,
            T.[Purchase Order] PurchaseOrder,
            T.[Order],
            T.Sale Sale,
            T.Transfer,
			T.[Credit Note] CreditNote,
            T.[Inventory Adjustment] InventoryAdjustment,
			T.[Print]
            FROM(
             SELECT T.*,CASE WHEN C.CLOSING_BALANCE IS NULL THEN C.OPENING_QTY ELSE C.CLOSING_BALANCE END CLOSING_BALANCE
			,C.ITEM_CODE OpeningItemCode,C.LOCATION OpeningLocation FROM (
            SELECT * FROM(SELECT [LOCATION] [Location],ITEM_CODE ItemCode,QUANTITY Quantity,MOV_DATE [Date],
            CASE WHEN REC_TYPE = 'T' THEN 'Transfer' WHEN REC_TYPE = 'P' THEN 'Purchase Order'
            WHEN REC_TYPE = 'S' THEN 'Sale' WHEN REC_TYPE = 'C' THEN 'Credit Note' WHEN REC_TYPE = 'M' THEN 'Inventory Adjustment'  END [Status]
            FROM {dbCode}SIINVMOV WHERE REC_TYPE IN ('T','P','S','C','M') AND TRY_CONVERT(DATETIME,MOV_DATE,101) = CONVERT(DATETIME,@DATE,101)
            UNION ALL
            SELECT D.LOCATION,D.ITEM_CODE,D.VALUE_1,H.ORDER_DATE, CASE WHEN H.STATUS < '10' THEN 'Order'
				WHEN H.STATUS = '10' THEN 'Print'​ END FROM {dbCode}SISODET D INNER JOIN {dbCode}SISOHDR H ON H.TRANS_REF = D.TRANS_REF
            WHERE TRY_CONVERT(DATETIME,H.ORDER_DATE,101) = CONVERT(DATETIME,@DATE,101) AND H.REC_TYPE = 'O' AND D.TRANS_TYPE <> 'SALE-FIX' AND VOID_STATUS <> 'Y'
            ) TAB PIVOT (SUM(Quantity) FOR [Status] IN ([Order],[Print],[Purchase Order],[Sale],[Transfer],[Credit Note],[Inventory Adjustment])) AS [PIVOT] ) T 
            RIGHT JOIN TB_BC_CLOSING_ENTRY C ON TRIM(C.ITEM_CODE)  = T.ItemCode AND C.LOCATION = T.Location
            INNER JOIN SIWAREH S ON S.WAR_CODE = C.LOCATION 
            WHERE C.STATUS = 0 AND C.DB_CODE = @DB_CODE AND S.WAR_STAT = 'A' AND S.DB_CODE = @DB_CODE AND CLOSING_TYPE <> 'Monthly' AND CLOSING_TYPE <> 'Yearly'
            ) T  ORDER BY ItemCode
            ";
            var param = new
            {
                DATE = date,
                DB_CODE = dbCode
            };
            var results = await _sqlDataAccess.LoadData<ClosingStockEntryModel, dynamic>(sql, param);
            return results.ToList();
        }

        public async Task<bool> CheckStockQuantityAsync(string dbCode, string location, string itemCode)
        {
            var sql =
                @$"SELECT CAST(CASE WHEN COUNT(QUANTITY) > 0 THEN 1 ELSE 0 END AS BIT) FROM 
            (SELECT PHYSICAL AS QUANTITY 
	          -- (SELECT PHYSICAL-TAB5.ON_ORDER AS QUANTITY 
			        FROM (SELECT TAB3.LOCATION,TAB3.ITEM_CODE,TAB3.ITEM_DESC,TAB3.UNIT_STOCK,TAB3.PHYSICAL,ISNULL(TAB4.HOLD_SALE,0) ON_ORDER 
			          FROM (SELECT TAB1.LOCATION,TAB1.ITEM_CODE,TAB2.ITEM_DESC,TAB2.UNIT_STOCK,PHYSICAL 
				        FROM (SELECT LOCATION,ITEM_CODE,ISNULL(SUM(QUANTITY),0) PHYSICAL FROM {dbCode}SIINVMOV WHERE IR_STAT='I' AND STATUS='80'AND ALLOC_REF=''  GROUP BY LOCATION,ITEM_CODE) AS TAB1 
				          LEFT JOIN (SELECT ITEM_CODE,ITEM_DESC,UNIT_STOCK FROM SIITEMS WHERE DB_CODE=@DB_CODE) AS TAB2 ON TAB1.ITEM_CODE=TAB2.ITEM_CODE) AS TAB3 
					        LEFT JOIN (SELECT LOCATION,ITEM_CODE,SUM(CASE WHEN STK_QTY_VALUE=1 THEN VALUE_1 WHEN STK_QTY_VALUE=2 THEN VALUE_2 WHEN STK_QTY_VALUE=3 THEN VALUE_3 WHEN STK_QTY_VALUE=4
							           THEN VALUE_4 WHEN STK_QTY_VALUE=5 THEN VALUE_5 WHEN STK_QTY_VALUE=6 THEN VALUE_6 WHEN STK_QTY_VALUE=7 THEN VALUE_7 WHEN STK_QTY_VALUE=8 THEN VALUE_8 WHEN STK_QTY_VALUE=9 THEN VALUE_9 WHEN
							           STK_QTY_VALUE=10 THEN VALUE_10 WHEN STK_QTY_VALUE=11 THEN VALUE_11 WHEN STK_QTY_VALUE=12 THEN VALUE_12 WHEN STK_QTY_VALUE=13 THEN VALUE_13 WHEN STK_QTY_VALUE=14 THEN VALUE_14 WHEN
							           STK_QTY_VALUE=15 THEN VALUE_15 WHEN STK_QTY_VALUE=16 THEN VALUE_16 WHEN STK_QTY_VALUE=17 THEN VALUE_17 WHEN STK_QTY_VALUE=18 THEN VALUE_18 WHEN STK_QTY_VALUE=19 THEN VALUE_19 WHEN
							           STK_QTY_VALUE=20 THEN VALUE_20 ELSE 0 END) HOLD_SALE 
							           FROM {dbCode}SISODET WHERE REC_TYPE='D' AND STATUS<'80' AND CREDIT_STATUS='' GROUP BY LOCATION,ITEM_CODE) AS TAB4 ON TAB3.LOCATION=TAB4.LOCATION AND TAB3.ITEM_CODE=TAB4.ITEM_CODE) AS TAB5 
					           LEFT JOIN (SELECT LOCATION,ITEM_CODE,SUM(QUANTITY) PICK_QTY 
								        FROM {dbCode}SIINVMOVH WHERE  IR_STAT<>'I' AND STATUS='10' GROUP BY LOCATION,ITEM_CODE) 
						                AS TAB6 ON TAB5.LOCATION=TAB6.LOCATION AND TAB5.ITEM_CODE=TAB6.ITEM_CODE WHERE TAB5.LOCATION=@LOCATION AND TAB5.ITEM_CODE= @ITEM_CODE) AS TAB7 WHERE QUANTITY <> 0";
            var param = new { DB_CODE = dbCode, LOCATION = location, ITEM_CODE = itemCode, };
            var results = await _sqlDataAccess.LoadSingleData<bool, dynamic>(sql, param);
            return results;
        }

        public async Task<int> InsertNewItem(NewItemModel model)
        {
            const string sql =
                @"INSERT INTO TB_BC_CLOSING_ENTRY(DB_CODE, LOCATION, ITEM_CODE, OPENING_QTY, CREATED_DATE, CREATED_BY,
                   STATUS,CLOSING_TYPE ) VALUES (@DB_CODE, @LOCATION, @ITEM_CODE, @OPENING_QTY, @CREATED_DATE, @CREATED_BY,
                   @STATUS,@CLOSING_TYPE)";

            var param = new
            {
                DB_CODE = model.DbCode,
                LOCATION = model.Location,
                ITEM_CODE = model.ItemCode,
                OPENING_QTY = 0,
                CREATED_DATE = DateTime.Today,
                CREATED_BY = model.CreatedBy,
                STATUS = "0",
                CLOSING_TYPE = "Daily" // = "Opening"
            };
            var rowAffected = await _sqlDataAccess.ExecuteAsync(sql, param);
            return rowAffected;
        }

        public async Task<bool> ExistItem(NewItemModel model)
        {
            const string sql =
                @"SELECT CAST(CASE WHEN COUNT(*) > 0 THEN 1 ELSE 0 END AS BIT) FROM TB_BC_CLOSING_ENTRY 
                WHERE DB_CODE = @DB_CODE and LOCATION = @LOCATION AND ITEM_CODE = @ITEM_CODE";
            var param = new { DB_CODE = model.DbCode, LOCATION = model.Location, ITEM_CODE = model.ItemCode, };
            var rowAffected = await _sqlDataAccess.ExecuteScalarAsync<bool, dynamic>(sql, param);

            return rowAffected;
        }

        public async Task<bool> CheckDateIsAlreadyClosingEntry(string dbCode, DateTime date)
        {
            const string sql =
                @"SELECT CAST(COUNT(*) AS BIT) FROM TB_BC_CLOSING_ENTRY WHERE CLOSING_DATE = @DATE AND DB_CODE = @DB_CODE";
            var param = new
            {
                DB_CODE = dbCode,
                DATE = date
            };
            var results = await _sqlDataAccess.LoadSingleData<bool, dynamic>(sql, param);
            return results;
        }

        public async Task<int> InsertDailyClosingEntryAsync(List<ClosingStockEntryModel> closingEntryModel, bool status)
        {
            var connection = new SqlConnection(_setting.GetConnectionString("DBConnection"));
            if (connection.State == System.Data.ConnectionState.Closed)
                await connection.OpenAsync();
            var sql =
                @"IF NOT EXISTS (SELECT * FROM TB_BC_CLOSING_ENTRY 
                    WHERE DB_CODE = @DB_CODE 
                    AND LOCATION = @LOCATION
                    AND CLOSING_DATE =@CLOSING_DATE
                    AND ITEM_CODE = @ITEM_CODE 
                    AND STATUS = 0 AND CLOSING_TYPE = @CLOSING_TYPE)
                     INSERT INTO TB_BC_CLOSING_ENTRY(DB_CODE,CLOSING_DATE,LOCATION,ITEM_CODE,OPENING_QTY,PURCHASE_QTY,ORDER_QTY,SALE_QTY,TRANSFER_QTY,CHANGE_QTY,CREDIT_NOTE_QTY,PRINT_QTY,CLOSING_BALANCE,CREATED_DATE,CREATED_BY,CLOSING_TYPE,STATUS)
                     VALUES (@DB_CODE,@CLOSING_DATE,@LOCATION,@ITEM_CODE,@OPENING_QTY,@PURCHASE_QTY,@ORDER_QTY,@SALE_QTY,@TRANSFER_QTY,@CHANGE_QTY,@CREDIT_NOTE_QTY,@PRINT_QTY,@CLOSING_BALANCE,@CREATED_DATE,@CREATED_BY,@CLOSING_TYPE,@STATUS)
                    ELSE 
                    UPDATE TB_BC_CLOSING_ENTRY SET PURCHASE_QTY = @PURCHASE_QTY,ORDER_QTY = @ORDER_QTY,SALE_QTY = @SALE_QTY,TRANSFER_QTY = @TRANSFER_QTY,CREDIT_NOTE_QTY = @CREDIT_NOTE_QTY,PRINT_QTY = @PRINT_QTY,CLOSING_BALANCE = @CLOSING_BALANCE
                    WHERE  DB_CODE = @DB_CODE 
                    AND LOCATION = @LOCATION
                    AND CLOSING_DATE =@CLOSING_DATE
                    AND ITEM_CODE = @ITEM_CODE 
                    AND STATUS = 0 AND CLOSING_TYPE = @CLOSING_TYPE";
            var sqlUpdate =
                @"UPDATE TB_BC_CLOSING_ENTRY SET STATUS = 1,UPDATED_BY = @UPDATED_BY,UPDATED_DATE = @UPDATED_DATE WHERE STATUS = 0 AND DB_CODE = @DB_CODE AND ITEM_CODE = @ITEM_CODE AND LOCATION = @LOCATION ";
            var affectedRows = 0;
            if (status)
            {
                var transaction = await connection.BeginTransactionAsync();

                foreach (var item in closingEntryModel)
                {
                    var param = new
                    {
                        CLOSING_DATE = item.ClosingDate,
                        DB_CODE = item.DbCode,
                        LOCATION = item.Location,
                        ITEM_CODE = item.ItemCode,
                        OPENING_QTY = item.OpeningBalance,
                        PURCHASE_QTY = item.PurchaseOrder,
                        ORDER_QTY = item.Order,
                        SALE_QTY = item.Sale,
                        TRANSFER_QTY = item.Transfer,
                        CREDIT_NOTE_QTY = item.CreditNote,
                        CHANGE_QTY = item.InventoryAdjustment,
                        PRINT_QTY = item.Print,
                        CLOSING_BALANCE = item.ClosingBalance,
                        CREATED_DATE = item.CreatedDate,
                        CREATED_BY = item.CreatedBy,
                        CLOSING_TYPE = item.ClosingEntryType.ToString(),
                        STATUS = "0"
                    };
                    await connection.ExecuteAsync(sqlUpdate,
                        new
                        {
                            DB_CODE = item.DbCode,
                            ITEM_CODE = item.ItemCode,
                            LOCATION = item.Location,
                            UPDATED_BY = item.CreatedBy,
                            UPDATED_DATE = DateTime.Today
                        }, transaction);
                    affectedRows += await connection.ExecuteAsync(sql, param, transaction);
                }

                if (affectedRows == closingEntryModel.Count)
                {
                    await transaction.CommitAsync();
                    return affectedRows;
                }
                else
                {
                    await transaction.RollbackAsync();
                    return affectedRows;
                }
            }
            else
            {
                var transaction = connection.BeginTransaction();
                foreach (var closingStock in closingEntryModel)
                {
                    var param = new
                    {
                        CLOSING_DATE = closingStock.ClosingDate,
                        DB_CODE = closingStock.DbCode,
                        LOCATION = closingStock.Location,
                        ITEM_CODE = closingStock.ItemCode,
                        OPENING_QTY = closingStock.OpeningBalance,
                        PURCHASE_QTY = closingStock.PurchaseOrder,
                        ORDER_QTY = closingStock.Order,
                        SALE_QTY = closingStock.Sale,
                        TRANSFER_QTY = closingStock.Transfer,
                        CREDIT_NOTE_QTY = closingStock.CreditNote,
                        CHANGE_QTY = closingStock.InventoryAdjustment,
                        PRINT_QTY = closingStock.Print,
                        CLOSING_BALANCE = closingStock.ClosingBalance,
                        CREATED_DATE = closingStock.CreatedDate,
                        CREATED_BY = closingStock.CreatedBy,
                        CLOSING_TYPE = closingStock.ClosingEntryType.ToString(),
                        STATUS = "0"
                    };
                    await connection.ExecuteAsync(sql, param, transaction);
                }

                if (affectedRows == closingEntryModel.Count)
                {
                    await transaction.CommitAsync();
                    return affectedRows;
                }
                else
                {
                    await transaction.RollbackAsync();
                    return affectedRows;
                }
            }
        }

        public async Task<List<ClosingStockEntryModel>> GetAllStockBalanceByBranchCode(string dbCode,
            string closingDate)
        {
            var sql =
                $@"SELECT DB_CODE DbCode,CLOSING_DATE ClosingDate,LOCATION Location,ITEM_CODE ItemCode,OPENING_QTY OpeningBalance,PURCHASE_QTY PurchaseOrder,ORDER_QTY [Order], PRINT_QTY [Print],
                    SALE_QTY Sale,TRANSFER_QTY Transfer, CREDIT_NOTE_QTY CreditNote,CHANGE_QTY InventoryAdjustment , CREATED_BY CreatedBy, CREATED_DATE CreatedDate  
                    FROM
                    TB_BC_CLOSING_ENTRY WHERE CLOSING_TYPE <> 'Opening' AND ​CLOSING_TYPE <> 'Monthly' AND MONTH(CLOSING_DATE) = @MONTH AND YEAR(CLOSING_DATE) = @YEAR
                    AND DB_CODE = @DB_CODE 
                    ORDER BY ITEM_CODE";
            var param = new
            {
                DB_CODE = dbCode,
                MONTH = Convert.ToDateTime(closingDate).Month,
                YEAR = Convert.ToDateTime(closingDate).Year,
                CLOSING_DATE = closingDate
            };
            var execute = await _sqlDataAccess.LoadData<ClosingStockEntryModel, dynamic>(sql, param);
            return execute.ToList();
        }

        public async Task<int> InsertClosingMonthlyAndYearlyAsync(List<ClosingStockEntryModel> closingEntryModel,
            ClosingEntryType closeType)
        {
            var sql =
                $@"INSERT INTO TB_BC_CLOSING_ENTRY(DB_CODE, CLOSING_DATE, LOCATION, ITEM_CODE, OPENING_QTY, PURCHASE_QTY, ORDER_QTY, SALE_QTY, TRANSFER_QTY, CREDIT_NOTE_QTY, CHANGE_QTY, PRINT_QTY, CLOSING_BALANCE, CREATED_DATE, CREATED_BY, CLOSING_TYPE, STATUS)
                    VALUES (@DB_CODE, @CLOSING_DATE, @LOCATION, @ITEM_CODE, @OPENING_QTY, @PURCHASE_QTY, @ORDER_QTY, @SALE_QTY, @TRANSFER_QTY, @CREDIT_NOTE_QTY, @CHANGE_QTY, @PRINT_QTY, @CLOSING_BALANCE, @CREATED_DATE, @CREATED_BY, @CLOSING_TYPE,'0')";
            var connection = new SqlConnection(_setting.GetConnectionString("DBConnection"));
            if (connection.State == System.Data.ConnectionState.Closed)
                await connection.OpenAsync();
            var transaction = await connection.BeginTransactionAsync();
            try
            {
                foreach (var item in closingEntryModel)
                {
                    var param = new
                    {
                        DB_CODE = item.DbCode,
                        CLOSING_DATE = item.ClosingDate,
                        LOCATION = item.Location,
                        ITEM_CODE = item.ItemCode,
                        OPENING_QTY = item.OpeningBalance,
                        PURCHASE_QTY = item.PurchaseOrder,
                        ORDER_QTY = item.Order,
                        SALE_QTY = item.Sale,
                        TRANSFER_QTY = item.Transfer,
                        CREDIT_NOTE_QTY = item.CreditNote,
                        CHANGE_QTY = item.InventoryAdjustment,
                        PRINT_QTY = item.Print,
                        CLOSING_BALANCE = item.ClosingBalance,
                        CREATED_DATE = DateTime.Today,
                        CREATED_BY = item.CreatedBy,
                        CLOSING_TYPE = closeType.ToString()
                    };
                    await _sqlDataAccess.ExecuteAsync(sql, param);
                }
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Debug.WriteLine(ex.Message);
            }
            return closingEntryModel.Count;
        }

        public async Task<List<ClosingStockEntryModel>> GetAllStockBalanceMonthlyByBranchCode(string dbCode,
            string closingDate)
        {
            var sql = $@"SELECT DB_CODE DbCode,CLOSING_DATE ClosingDate,LOCATION Location,ITEM_CODE ItemCode,OPENING_QTY OpeningBalance,PURCHASE_QTY PurchaseOrder,ORDER_QTY [Order], PRINT_QTY [Print],
                    SALE_QTY Sale,TRANSFER_QTY Transfer, CREDIT_NOTE_QTY CreditNote,CHANGE_QTY InventoryAdjustment , CREATED_BY CreatedBy, CREATED_DATE CreatedDate  
                    FROM
                    TB_BC_CLOSING_ENTRY WHERE CLOSING_TYPE <> 'Opening' AND YEAR(CLOSING_DATE) = @YEAR
                    AND DB_CODE = @DB_CODE AND CLOSING_TYPE = 'Monthly'
                    ORDER BY ITEM_CODE";
            var param = new
            {
                DB_CODE = dbCode,
                YEAR = Convert.ToDateTime(closingDate).Year,
                CLOSING_DATE = closingDate
            };
            var execute = await _sqlDataAccess.LoadData<ClosingStockEntryModel, dynamic>(sql, param);
            return execute.ToList();
        }
    }
}
