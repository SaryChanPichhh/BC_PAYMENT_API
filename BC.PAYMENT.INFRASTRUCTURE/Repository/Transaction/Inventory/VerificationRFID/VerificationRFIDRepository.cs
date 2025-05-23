using BC.PAYMENT.APPLICATION.Interfaces.Transaction.Inventory.VerificationRFID;
using BC.PAYMENT.CORE.DTO.Transaction.Inventory.VerificationStock;
using BC.PAYMENT.CORE.Entities.Transaction.Inventory.VerificationStock;
using BC.PAYMENT.INFRASTRUCTURE.DBAccess;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Transaction.Inventory.VerificationRFID
{
    internal class VerificationRFIDRepository : IVerificationRFIDRepository
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public VerificationRFIDRepository(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<int> UpdateSubmittedStatus(string dbCode, string submitCode)
        {
            var sql = "UPDATE RFIDINVHDR SET IS_SUBMITTED = 1 WHERE SUBMIT_CODE = @SUBMIT_CODE AND DB_CODE = @DB_CODE";
            var param = new
            {
                DB_CODE = dbCode,
                SUBMIT_CODE = submitCode
            };
            //using var connection = new SqlConnection(DatabaseConfiguration.Instance.GetConnection());
            var results = await _sqlDataAccess.ExecuteAsync(sql, param);
            return results;
        }

        public async Task<List<VerificationStockModel>> GetRFIDSubmittedItemDetail(string dbCode, string submitCode, string location)
        {
            const string sql = @"SELECT DB.DB_NAME DbCode, H.WAREHOUSE [Location], H.STOCKER Stocker, H.STOCK_CHECKER StockChecker, H.[ADMIN] [Admin],
                D.ITEM_CODE ItemCode, D.QUANTITY Quantity, H.CREATED_DATE CreatedDate, D.EXPIRED_DATE ExpiredDate
                FROM RFIDINVHDR H 
                INNER JOIN dbo.RFIDINVDET D
                ON H.SUBMIT_CODE = D.SUBMIT_CODE
                INNER JOIN SIDBINFO DB
                ON DB.DB_CODE = H.DB_CODE 
                WHERE H.DB_CODE = @DB_CODE AND H.WAREHOUSE = @LOCATION AND H.SUBMIT_CODE = @SUBMIT_CODE AND H.STATUS = 'A' AND D.STATUS = 1
                ";
            var param = new
            {
                DB_CODE = dbCode,
                SUBMIT_CODE = submitCode,
                LOCATION = location
            };
            var results = (await _sqlDataAccess.LoadData<VerificationStockModel, dynamic>(sql, param)).ToList();
            return results;
        }

        public async Task<List<string>> GetAllWarehouse(string dbCode)
        {
            const string sql = @"SELECT WAR_CODE Location FROM SIWAREH WHERE DB_CODE = @DB_CODE AND WAR_STAT = 'A'";
            var param = new
            {
                DB_CODE = dbCode
            };
            var results = (await _sqlDataAccess.LoadData<string,dynamic>(sql, param)).ToList();
            return results;
        }

        public async Task<List<VerificationStockModel>> GetAllSubmitCodeEntries(string dbCode, string location)
        {
            const string sql = @"
                       SELECT SUBMIT_CODE SubmitCode, WAREHOUSE [Location], STOCK_CHECKER StockChecker, [ADMIN] [Admin], CREATED_DATE CreatedDate 
            FROM RFIDINVHDR WHERE WAREHOUSE = @LOCATION AND DB_CODE = @DB_CODE AND STATUS = 'A' ORDER BY CREATED_DATE DESC";
            var param = new
            {
                DB_CODE = dbCode,
                LOCATION = location
            };
            var results = (await _sqlDataAccess.LoadData<VerificationStockModel, dynamic>(sql, param)).ToList();
            return results;
        }

        public async Task<List<VerificationStockModel>> GetAllStockItemByLocationAndSubmitCode(string dbCode, string submitCode, string location)
        {
            var sql = $@"SELECT CASE WHEN  C.WAREHOUSE IS NULL THEN S.LOCATION ELSE C.WAREHOUSE END Location, 
                            CASE WHEN  C.ITEM_CODE IS NULL THEN S.ITEM_CODE ELSE C.ITEM_CODE END ItemCode,  
                             S.ITEM_DESC ItemDescription,  
                               ISNULL(S.UNIT_STOCK,0) UnitStock,  
                               ISNULL(S.PHYSICAL,0) Physical,  
                               ISNULL(S.ON_ORDER,0) OnOrder,  
                               ISNULL(S.PHYSICAL - S.ON_ORDER,0) 
                               SubTotal,  
                               ISNULL(C.QUANTITY, 0) 
                               Quantity,  
                               ISNULL((S.PHYSICAL - S.ON_ORDER),0) - ISNULL(C.QUANTITY, 0) Total   
                               FROM(SELECT TAB5.LOCATION,  
                               TAB5.ITEM_CODE,  
                               TAB5.ITEM_DESC,  
                               TAB5.UNIT_STOCK,  
                               TAB5.PHYSICAL,  
                               TAB5.ON_ORDER,  
                               ISNULL(TAB6.PICK_QTY, 0) PICK_QTY  
                               FROM(SELECT TAB3.LOCATION,  
                               TAB3.ITEM_CODE,  
                               TAB3.ITEM_DESC,  
                               TAB3.UNIT_STOCK,  
                               TAB3.PHYSICAL,  
                               ISNULL(TAB4.HOLD_SALE, 0) ON_ORDER  
                               FROM(SELECT TAB1.LOCATION, TAB1.ITEM_CODE, TAB2.ITEM_DESC, TAB2.UNIT_STOCK, PHYSICAL  
                               FROM(SELECT LOCATION, ITEM_CODE, ISNULL(SUM(QUANTITY), 0) PHYSICAL  
                               FROM   {dbCode}SIINVMOV  
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
                              FROM    {dbCode}SISODET  
                             WHERE REC_TYPE = 'D'  
                             AND STATUS < '80'  
                             AND CREDIT_STATUS = '' 
                             GROUP BY LOCATION, ITEM_CODE)  
                             AS TAB4 ON TAB3.LOCATION = TAB4.LOCATION AND TAB3.ITEM_CODE = TAB4.ITEM_CODE)  
                             AS TAB5  
                             LEFT JOIN(SELECT LOCATION, ITEM_CODE, SUM(QUANTITY) PICK_QTY  
                             FROM   {dbCode}SIINVMOVH  
                             WHERE IR_STAT <> 'I' 
                             AND STATUS = '10'  
                             GROUP BY LOCATION, ITEM_CODE)  
                             AS TAB6 ON TAB5.LOCATION = TAB6.LOCATION AND TAB5.ITEM_CODE = TAB6.ITEM_CODE  
                             WHERE TAB5.LOCATION = @LOCATION) S 
                             FULL OUTER JOIN 
                             (SELECT H.WAREHOUSE, D.ITEM_CODE, D.QUANTITY FROM dbo.RFIDINVHDR H 
						        INNER JOIN dbo.RFIDINVDET D ON H.SUBMIT_CODE = D.SUBMIT_CODE
						        WHERE H.DB_CODE = @DB_CODE AND H.SUBMIT_CODE = @SUBMIT_CODE
						        ) C ON C.ITEM_CODE = S.ITEM_CODE";
            var param = new
            {
                DB_CODE = dbCode,
                SUBMIT_CODE = submitCode,
                LOCATION = location
            };
            var execute = await _sqlDataAccess.LoadData<VerificationStockModel, dynamic>(sql, param);
            return execute.ToList();
        }
    }
}
