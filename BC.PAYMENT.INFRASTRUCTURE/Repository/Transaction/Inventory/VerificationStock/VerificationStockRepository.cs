using System.Data;
using System.Diagnostics;
using System.Globalization;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.APPLICATION.Interfaces.Transaction.Inventory.VerificationStock;
using BC.PAYMENT.CORE.DTO.Transaction.Inventory.VerificationStock;
using BC.PAYMENT.CORE.Entities.General;
using BC.PAYMENT.CORE.Entities.Transaction.Inventory.VerificationStock;
using BC.PAYMENT.INFRASTRUCTURE.DBAccess;
using Dapper;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Transaction.Inventory.VerificationStock
{
    public class VerificationStockRepository : IVerificationStockRepository
    {
        private readonly ISqlDataAccess _sqlDataAccess;
        private readonly IDbConnection _dbConnection;
        public VerificationStockRepository(ISqlDataAccess sqlDataAccess, IDbConnection dbConnection)
        {
            this._sqlDataAccess = sqlDataAccess;
            _dbConnection = dbConnection;
        }

        public async Task<List<VerificationStockModel>> GetAllStockItemByLocationAndCountingDateAsync(string dbCode, string location, DateTime fromDate, DateTime toDate, int page, int pageSize)
        {
            var sql = $@"SELECT CASE WHEN C.STOCK_NAME IS NULL THEN S.LOCATION ELSE C.STOCK_NAME END Location,C.COUNT_DATE, 
                        CASE WHEN C.ITEM_CODE IS NULL THEN S.ITEM_CODE ELSE C.ITEM_CODE END ItemCode,  
                        CASE WHEN C.ITEM_DESC IS NULL THEN S.ITEM_DESC ELSE C.ITEM_DESC END ItemDescription,  
                        ISNULL(S.UNIT_STOCK,0) UnitStock,ISNULL(S.PHYSICAL,0) Physical,ISNULL(S.ON_ORDER,0) OnOrder,  
                        ISNULL(S.PHYSICAL - S.ON_ORDER,0) SubTotal,ISNULL(C.QUANTITY, 0) Quantity,ISNULL((S.PHYSICAL - S.ON_ORDER),0) - ISNULL(C.QUANTITY, 0) Total   
                        FROM(SELECT TAB5.LOCATION,TAB5.ITEM_CODE,TAB5.ITEM_DESC,TAB5.UNIT_STOCK,TAB5.PHYSICAL, TAB5.ON_ORDER,  
                        ISNULL(TAB6.PICK_QTY, 0) PICK_QTY FROM(SELECT TAB3.LOCATION, TAB3.ITEM_CODE, TAB3.ITEM_DESC,TAB3.UNIT_STOCK,  
                        TAB3.PHYSICAL, ISNULL(TAB4.HOLD_SALE, 0) ON_ORDER FROM(SELECT TAB1.LOCATION, TAB1.ITEM_CODE, TAB2.ITEM_DESC, TAB2.UNIT_STOCK, PHYSICAL  
                        FROM(SELECT LOCATION, ITEM_CODE, ISNULL(SUM(QUANTITY), 0) PHYSICAL FROM {dbCode}SIINVMOV WHERE IR_STAT = 'I'  
                        AND STATUS = '80' AND ALLOC_REF = '' GROUP BY LOCATION, ITEM_CODE)  AS TAB1 LEFT JOIN(SELECT ITEM_CODE, ITEM_DESC, UNIT_STOCK  
                        FROM SIITEMS WHERE DB_CODE = @DB_CODE) AS TAB2 ON TAB1.ITEM_CODE = TAB2.ITEM_CODE) AS TAB3 LEFT JOIN  
                         (SELECT LOCATION, ITEM_CODE,SUM(
                        CASE 
                            WHEN STK_QTY_VALUE = 1 THEN VALUE_1 WHEN STK_QTY_VALUE = 2 THEN VALUE_2 WHEN STK_QTY_VALUE = 3 THEN VALUE_3  
                            WHEN STK_QTY_VALUE = 4 THEN VALUE_4 WHEN STK_QTY_VALUE = 5 THEN VALUE_5 WHEN STK_QTY_VALUE = 6 THEN VALUE_6  
                            WHEN STK_QTY_VALUE = 7 THEN VALUE_7 WHEN STK_QTY_VALUE = 8 THEN VALUE_8 WHEN STK_QTY_VALUE = 9 THEN VALUE_9  
                            WHEN STK_QTY_VALUE = 10 THEN VALUE_10 WHEN STK_QTY_VALUE = 11 THEN VALUE_11 WHEN STK_QTY_VALUE = 12 THEN VALUE_12  
                            WHEN STK_QTY_VALUE = 13 THEN VALUE_13 WHEN STK_QTY_VALUE = 14 THEN VALUE_14 WHEN STK_QTY_VALUE = 15 THEN VALUE_15 
                            WHEN STK_QTY_VALUE = 16 THEN VALUE_16 WHEN STK_QTY_VALUE = 17 THEN VALUE_17 WHEN STK_QTY_VALUE = 18 THEN VALUE_18 
                            WHEN STK_QTY_VALUE = 19 THEN VALUE_19 WHEN STK_QTY_VALUE = 20 THEN VALUE_20 ELSE 0 END) HOLD_SALE 
                        FROM {dbCode}SISODET WHERE REC_TYPE = 'D' AND STATUS < '80' AND CREDIT_STATUS = '' GROUP BY LOCATION, ITEM_CODE)  
                        AS TAB4 ON TAB3.LOCATION = TAB4.LOCATION AND TAB3.ITEM_CODE = TAB4.ITEM_CODE)  AS TAB5 LEFT JOIN(SELECT LOCATION, ITEM_CODE, SUM(QUANTITY) PICK_QTY  
                        FROM {dbCode}SIINVMOVH WHERE IR_STAT <> 'I' AND STATUS = '10' GROUP BY LOCATION, ITEM_CODE) AS TAB6 ON TAB5.LOCATION = TAB6.LOCATION AND TAB5.ITEM_CODE = TAB6.ITEM_CODE  
                        WHERE TAB5.LOCATION = @LOCATION) S FULL OUTER JOIN (SELECT M.STOCK_NAME,M.COUNT_DATE,D.ITEM_CODE, SUM(D.QUANTITY) QUANTITY,ITEM_DESC
                        FROM SC_STOCK M INNER JOIN SC_COUNT_ITEM D ON D.STOCK_ID = M.STOCK_ID WHERE M.STOCK_NAME = @LOCATION AND M.COUNT_DATE 
                        BETWEEN @FROM_DATE AND @FROM_DATE AND M.DB_CODE = @DB_CODE AND D.DB_CODE = @DB_CODE 
                        AND D.STATUS = '1' GROUP BY ITEM_CODE,ITEM_DESC,M.STOCK_NAME,M.COUNT_DATE) C ON C.ITEM_CODE = S.ITEM_CODE ORDER BY C.COUNT_DATE OFFSET @OFFSET ROWS FETCH NEXT @PAGE_SIZE ROWS ONLY
                            ";
            var param = new
            {
                DB_CODE =  dbCode,
                FROM_DATE = fromDate,
                TO_DATE = toDate,
                LOCATION = location,
                OFFSET = (page - 1) * pageSize,
                PAGE_SIZE = pageSize
            };
            var execute = await _sqlDataAccess.LoadData<VerificationStockModel, dynamic>(sql, param);
            return execute.ToList();
        }

        public async Task<List<VerificationStockModel>> GetAllStockItemByLocationAndCountingPeriodAsync(string dbCode, string location,int month,int year,int page,int pageSize)
        {
            var sql = $@"SELECT CASE WHEN C.STOCK_NAME IS NULL THEN S.LOCATION ELSE C.STOCK_NAME END Location,C.COUNT_DATE, 
                        CASE WHEN C.ITEM_CODE IS NULL THEN S.ITEM_CODE ELSE C.ITEM_CODE END ItemCode,  
                        CASE WHEN C.ITEM_DESC IS NULL THEN S.ITEM_DESC ELSE C.ITEM_DESC END ItemDescription,  
                        ISNULL(S.UNIT_STOCK,0) UnitStock,ISNULL(S.PHYSICAL,0) Physical,ISNULL(S.ON_ORDER,0) OnOrder,  
                        ISNULL(S.PHYSICAL - S.ON_ORDER,0) SubTotal,ISNULL(C.QUANTITY, 0) Quantity,ISNULL((S.PHYSICAL - S.ON_ORDER),0) - ISNULL(C.QUANTITY, 0) Total   
                        FROM(SELECT TAB5.LOCATION,TAB5.ITEM_CODE,TAB5.ITEM_DESC,TAB5.UNIT_STOCK,TAB5.PHYSICAL, TAB5.ON_ORDER,  
                        ISNULL(TAB6.PICK_QTY, 0) PICK_QTY FROM(SELECT TAB3.LOCATION, TAB3.ITEM_CODE, TAB3.ITEM_DESC,TAB3.UNIT_STOCK,  
                        TAB3.PHYSICAL, ISNULL(TAB4.HOLD_SALE, 0) ON_ORDER FROM(SELECT TAB1.LOCATION, TAB1.ITEM_CODE, TAB2.ITEM_DESC, TAB2.UNIT_STOCK, PHYSICAL  
                        FROM(SELECT LOCATION, ITEM_CODE, ISNULL(SUM(QUANTITY), 0) PHYSICAL FROM {dbCode}SIINVMOV WHERE IR_STAT = 'I'  
                        AND STATUS = '80' AND ALLOC_REF = '' GROUP BY LOCATION, ITEM_CODE)  AS TAB1 LEFT JOIN(SELECT ITEM_CODE, ITEM_DESC, UNIT_STOCK  
                        FROM SIITEMS WHERE DB_CODE = @DB_CODE) AS TAB2 ON TAB1.ITEM_CODE = TAB2.ITEM_CODE) AS TAB3 LEFT JOIN  
                         (SELECT LOCATION, ITEM_CODE,SUM(
                        CASE 
                            WHEN STK_QTY_VALUE = 1 THEN VALUE_1 WHEN STK_QTY_VALUE = 2 THEN VALUE_2 WHEN STK_QTY_VALUE = 3 THEN VALUE_3  
                            WHEN STK_QTY_VALUE = 4 THEN VALUE_4 WHEN STK_QTY_VALUE = 5 THEN VALUE_5 WHEN STK_QTY_VALUE = 6 THEN VALUE_6  
                            WHEN STK_QTY_VALUE = 7 THEN VALUE_7 WHEN STK_QTY_VALUE = 8 THEN VALUE_8 WHEN STK_QTY_VALUE = 9 THEN VALUE_9  
                            WHEN STK_QTY_VALUE = 10 THEN VALUE_10 WHEN STK_QTY_VALUE = 11 THEN VALUE_11 WHEN STK_QTY_VALUE = 12 THEN VALUE_12  
                            WHEN STK_QTY_VALUE = 13 THEN VALUE_13 WHEN STK_QTY_VALUE = 14 THEN VALUE_14 WHEN STK_QTY_VALUE = 15 THEN VALUE_15 
                            WHEN STK_QTY_VALUE = 16 THEN VALUE_16 WHEN STK_QTY_VALUE = 17 THEN VALUE_17 WHEN STK_QTY_VALUE = 18 THEN VALUE_18 
                            WHEN STK_QTY_VALUE = 19 THEN VALUE_19 WHEN STK_QTY_VALUE = 20 THEN VALUE_20 ELSE 0 END) HOLD_SALE 
                        FROM {dbCode}SISODET WHERE REC_TYPE = 'D' AND STATUS < '80' AND CREDIT_STATUS = '' GROUP BY LOCATION, ITEM_CODE)  
                        AS TAB4 ON TAB3.LOCATION = TAB4.LOCATION AND TAB3.ITEM_CODE = TAB4.ITEM_CODE)  AS TAB5 LEFT JOIN(SELECT LOCATION, ITEM_CODE, SUM(QUANTITY) PICK_QTY  
                        FROM {dbCode}SIINVMOVH WHERE IR_STAT <> 'I' AND STATUS = '10' GROUP BY LOCATION, ITEM_CODE) AS TAB6 ON TAB5.LOCATION = TAB6.LOCATION AND TAB5.ITEM_CODE = TAB6.ITEM_CODE  
                        WHERE TAB5.LOCATION = @LOCATION) S FULL OUTER JOIN (SELECT M.STOCK_NAME,M.COUNT_DATE,D.ITEM_CODE, SUM(D.QUANTITY) QUANTITY,ITEM_DESC
                        FROM SC_STOCK M INNER JOIN SC_COUNT_ITEM D ON D.STOCK_ID = M.STOCK_ID WHERE M.STOCK_NAME = @LOCATION AND YEAR(M.COUNT_DATE) = @YEAR AND MONTH(M.COUNT_DATE) = @MONTH  AND M.DB_CODE = @DB_CODE AND D.DB_CODE = @DB_CODE 
                        AND D.STATUS = '1' GROUP BY ITEM_CODE,ITEM_DESC,M.STOCK_NAME,M.COUNT_DATE) C ON C.ITEM_CODE = S.ITEM_CODE ORDER BY C.COUNT_DATE OFFSET @OFFSET ROWS FETCH NEXT @PAGE_SIZE ROWS ONLY";
            var param = new
            {
                DB_CODE = dbCode,
                YEAR = year,
                MONTH = month,
                LOCATION = location,
                OFFSET = (page - 1) * pageSize,
                PAGE_SIZE = pageSize
            };
            var execute = await _sqlDataAccess.LoadData<VerificationStockModel, dynamic>(sql, param);
            return execute.ToList();
        }

        public async Task<int> SaveRecordStockItemAfterVerify(List<VerificationStockModel> model)
        {
            if(_dbConnection.State == ConnectionState.Closed)
                _dbConnection.Open();

            var transaction = _dbConnection.BeginTransaction();
            try
            {
                const string sql =
                    @"IF EXISTS (SELECT * FROM BCSTOCK_INVENTORY_VERIFY WHERE ITEM_CODE = @ItemCode AND LOCATION = @Location AND DB_CODE = @DbCode AND CREATED_DATE = CONVERT(DATE,GETDATE()))
                UPDATE BCSTOCK_INVENTORY_VERIFY SET UNIT = @UnitStock,PHYSICAL = @Physical,ON_ORDER =  @OnOrder,SUBTOTAL  = @SubTotal,ITEM_COUNT = @Quantity,TOTAL = @Total WHERE DB_CODE = @DbCode
                AND ITEM_CODE = @ItemCode AND LOCATION = @Location AND CREATED_DATE = CONVERT(DATE,GETDATE())
                ELSE
                INSERT INTO BCSTOCK_INVENTORY_VERIFY(LOCATION,ITEM_CODE,ITEM_NAME,UNIT,PHYSICAL,ON_ORDER,SUBTOTAL,ITEM_COUNT,TOTAL,CREATED_DATE,DB_CODE,PERIOD,CREATED_BY)
                VALUES (@Location,@ItemCode,@ItemName,@UnitStock,
                @Physical,@OnOrder,@SubTotal,@Quantity,
                @Total,GETDATE(),@DbCode,@Period,@CreatedBy)";

                foreach (var item in model)
                {
                    var param = new
                    {
                        ItemCode = item.ItemCode,
                        Location = item.Location,
                        DbCode = item.DbCode,
                        UnitStock = item.UnitStock,
                        Physical = item.Physical,
                        OnOrder = item.OnOrder,
                        SubTotal = item.SubTotal,
                        Quantity = item.Quantity,
                        Total = item.Total,
                        ItemName = item.ItemDescription,
                        Period = DateTime.Now.ToString("MMyyyy"),
                        CreatedBy = item.CreateBy
                    };
                    await _dbConnection.ExecuteAsync(sql, param,transaction);
                }
                transaction.Commit();
                return 1;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                transaction.Rollback();
            }
            return 0;
        }

        public async Task<int> GetMaxSequence(string dbCode)
        {
            var sql = $@"SELECT MAX(SEQUENCE)+1 FROM {dbCode}SIINVMOV";
            var execute = await _sqlDataAccess.LoadSingleData<int,dynamic>(sql,new {});
            return execute;
        }

        public async Task<BcModels> GetRecTypes(string dbCode,string movType)
        {
            var sql = @"SELECT CODE Code,SUBSTRING(SI_DATA,0,30) Description, SUBSTRING(SI_DATA,31,1) MovType
                    FROM SIDATA where DB_CODE = @DB_CODE AND SI_TYPE= 'ICMOV' AND CODE = @CODE";
            var param = new { DB_CODE = dbCode, CODE = movType };
            var results = await _sqlDataAccess.LoadSingleData<BcModels, dynamic>(sql, param);
            return results;
        }
            
        public async Task<double> GetItemCostAsync(string dbCode, string itemCode)
        {
            const string sql = @"SELECT ITEM_DCOST FROM SIITEMS WHERE ITEM_CODE = @ITEM_CODE AND DB_CODE = @DB_CODE";
            var param = new { ITEM_CODE = itemCode, DB_CODE = dbCode, };
            var itemCost =await _sqlDataAccess.LoadSingleData<double, dynamic>(sql, param);
            return itemCost;
        }

        public async Task<int> AdjustInventory(InventoryAdjustmentModel inventory)
        {
            var sql = $@"{inventory.DbCode}_INSERT_SIINVMOV";
            var param = new
            {
                SEQUENCE_0 = inventory.Sequence,
                REC_TYPE_1 = inventory.RecType,
                MOV_PRD_2 = inventory.Period,
                MOV_REF_3 = inventory.MovRef,
                MOV_LINE_4 = inventory.MovLine,
                LOCATION_5 = inventory.Location,
                ITEM_CODE_6 = inventory.ItemCode,
                MOV_DATE_7 = inventory.MovDate,
                STATUS_8 = inventory.StatusInv,
                IR_STAT_9 = inventory.IRStat,
                BATCH_NO_10 = inventory.BatchNo,
                BATCH_LINE_11 = inventory.BatchLine,
                LINE_REF_12 = inventory.LineRef,
                QUANTITY_13 = inventory.Quantity,
                COST_14 = inventory.Cost,
                TOTAL_15 = inventory.TotalPrice,
                MOV_UNITS_16 = inventory.MovUnits,
                MOV_TYPE_17 = inventory.MovType,
                UPDTE_PHYS_18 = inventory.UpdatePhysical,
                UPDTE_ORDR_19 = inventory.UpdateOrder,
                ALLOC_REF_20 = inventory.AllocRef,
                ACCNT_CODE_21 = inventory.AccountCode,
                ASSET_CODE_22 = inventory.AssetCode,
                ANAL_M0_23 = inventory.AnalM0,
                ANAL_M1_24 = inventory.AnalM1,
                ANAL_M2_25 = inventory.AnalM2,
                ANAL_M3_26 = inventory.AnalM3,
                ANAL_M4_27 = inventory.AnalM4,
                ANAL_M5_28 = inventory.AnalM5,
                ANAL_M6_29 = inventory.AnalM6,
                ANAL_M7_30 = inventory.AnalM7,
                ANAL_M8_31 = inventory.AnalM8,
                ANAL_M9_32 = inventory.AnalM9,
                ORIG_LINE_NO_33 = inventory.OrigLineNo,
                PO_VALUE_34 = inventory.PoValue,
                ID_ENTERED_35 = inventory.IdEntered,
                ID_ALLOC_36 = inventory.IdAlloc,
                ANAL_M0_37 = inventory.AnalM0,
                ANAL_M1_38 = inventory.AnalM1,
                ANAL_M2_39 = inventory.AnalM2,
                ANAL_M3_40 = inventory.AnalM3,
                ANAL_M4_41 = inventory.AnalM4,
                ANAL_M5_42 = inventory.AnalM5,
                ANAL_M6_43 = inventory.AnalM6,
                ANAL_M7_44 = inventory.AnalM7,
                ANAL_M8_45 = inventory.AnalM8,
                ANAL_M9_46 = inventory.AnalM9,
                DB_CODE_47 = inventory.DbCode,
                LOC_TRAN_48 = inventory.Location
            };
            var rowAffected = await _sqlDataAccess.ExecuteAsync(sql, param, commandType: CommandType.StoredProcedure);
            return rowAffected;
        }

        #region Verification Inventory Report

        public async Task<List<VerificationStockReportDto>> GetVerificationStockReport(string dbCode, DateTime fromDate, DateTime toDate)
        {
            var sql = $@"SELECT ID Id,
                           LOCATION Location,
                           ITEM_CODE ItemCode,
                           ITEM_NAME ItemName,
                           UNIT Unit,
                           PHYSICAL Physical,
                           ON_ORDER OnOrder,
                           SUBTOTAL SubTotal,
                           ITEM_COUNT Quantity,
                           TOTAL Total,
                           CREATED_DATE CreateDate,
                           DB_CODE DbCode,
                           PERIOD Period,
                           CREATED_BY CreateBy FROM BCSTOCK_INVENTORY_VERIFY WHERE CREATED_DATE BETWEEN @FROM_DATE AND @TO_DATE AND DB_CODE = @DB_CODE";
            var param = new 
            {
                DB_CODE = dbCode,
                FROM_DATE = fromDate,
                TO_DATE = toDate
            };
            var execute = await _sqlDataAccess.LoadData<VerificationStockReportDto, dynamic>(sql, param);
            return execute.ToList();
        }

        public async Task<List<VerificationStockReportDto>> GetVerificationStockReportByPeriod(string dbCode, int month, int year)
        {
            var sql = $@"SELECT ID Id,
                           LOCATION Location,
                           ITEM_CODE ItemCode,
                           ITEM_NAME ItemName,
                           UNIT Unit,
                           PHYSICAL Physical,
                           ON_ORDER OnOrder,
                           SUBTOTAL SubTotal,
                           ITEM_COUNT Quantity,
                           TOTAL Total,
                           CREATED_DATE CreateDate,
                           DB_CODE DbCode,
                           PERIOD Period,
                           CREATED_BY CreateBy FROM BCSTOCK_INVENTORY_VERIFY WHERE YEAR(CREATED_DATE) = @YEAR AND MONTH(CREATE_DATE) = @YEAR AND DB_CODE = @DB_CODE";
            var param = new
            {
                DB_CODE = dbCode,
                MONTH = month,
                YEAR = year    
            };
            var execute = await _sqlDataAccess.LoadData<VerificationStockReportDto, dynamic>(sql, param);
            return execute.ToList();
        }
        #endregion

    }
}
