using BC.PAYMENT.APPLICATION.Interfaces.Inventory;
using BC.PAYMENT.CORE.Contracts.Response.Inventory;
using BC.PAYMENT.CORE.Entities.Inventory.Inventory;
using BC.PAYMENT.INFRASTRUCTURE.DBAccess;
using System.Diagnostics;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Inventory
{
    public class InventoryRepository(ISqlDataAccess sqlDataAccess) : IInventoryRepository
    {
        public async Task<List<dynamic>> GetInventoryByMultiWarehouseAsync(Dictionary<string, List<string>> dbCodeAndWarehouses)
        {
            var sql = "";
            
            foreach (var item in dbCodeAndWarehouses)
            {
                var dbCode = item.Key;
                foreach (var warehouse in item.Value)
                {
                    sql +=
                        $@"SELECT TAB5.LOCATION [Location],TAB5.ITEM_CODE ItemCode, TAB5.ITEM_DESC Description, CONVERT(INT,TAB5.PHYSICAL - TAB5.ON_ORDER) Total,(SELECT TOP 1 'Broken' FROM TB_BC_WAREHOUSE_PRESETS WHERE STATUS = 1 AND NAME = TAB5.LOCATION) Broken
                FROM    
                    (SELECT TAB3.LOCATION, TAB3.ITEM_CODE, TAB3.ITEM_DESC, TAB3.UNIT_STOCK, TAB3.PHYSICAL, ISNULL(TAB4.HOLD_SALE,0) ON_ORDER
                    FROM    
                        (SELECT TAB1.LOCATION, TAB1.ITEM_CODE, TAB2.ITEM_DESC, TAB2.UNIT_STOCK, PHYSICAL
                        FROM (SELECT LOCATION, ITEM_CODE,
                                ISNULL(SUM(QUANTITY),0) PHYSICAL
                            FROM {dbCode}SIINVMOV
                            WHERE IR_STAT='I' AND STATUS='80' AND ALLOC_REF=''
                            GROUP BY LOCATION,ITEM_CODE) AS TAB1 LEFT JOIN
                            (SELECT ITEM_CODE, ITEM_DESC, UNIT_STOCK
                            FROM SIITEMS
                            WHERE DB_CODE='{dbCode}') AS TAB2 ON TAB1.ITEM_CODE=TAB2.ITEM_CODE) AS TAB3 LEFT JOIN
                        (SELECT LOCATION, ITEM_CODE, SUM(CASE WHEN STK_QTY_VALUE=1 THEN VALUE_1 WHEN STK_QTY_VALUE=2 THEN VALUE_2 WHEN STK_QTY_VALUE=3 THEN VALUE_3 WHEN STK_QTY_VALUE=4 
                        THEN VALUE_4 WHEN STK_QTY_VALUE=5 THEN VALUE_5 WHEN STK_QTY_VALUE=6 THEN VALUE_6 WHEN STK_QTY_VALUE=7 THEN VALUE_7 WHEN STK_QTY_VALUE=8 THEN VALUE_8 WHEN STK_QTY_VALUE=9 
                        THEN VALUE_9 WHEN STK_QTY_VALUE=10 THEN VALUE_10 WHEN STK_QTY_VALUE=11 THEN VALUE_11 WHEN STK_QTY_VALUE=12 THEN VALUE_12 WHEN STK_QTY_VALUE=13 THEN VALUE_13 
                        WHEN STK_QTY_VALUE=14 THEN VALUE_14 WHEN STK_QTY_VALUE=15 THEN VALUE_15 WHEN STK_QTY_VALUE=16 THEN VALUE_16 WHEN STK_QTY_VALUE=17 THEN VALUE_17 WHEN STK_QTY_VALUE=18 
                        THEN VALUE_18 WHEN STK_QTY_VALUE=19 THEN VALUE_19 WHEN STK_QTY_VALUE=20 THEN VALUE_20 ELSE 0 END) HOLD_SALE
                        FROM {dbCode}SISODET
                        WHERE REC_TYPE='D' AND STATUS<'80' AND CREDIT_STATUS=''
                        GROUP BY LOCATION,ITEM_CODE)
                        AS TAB4 ON TAB3.LOCATION=TAB4.LOCATION AND TAB3.ITEM_CODE=TAB4.ITEM_CODE) AS TAB5 LEFT JOIN (SELECT LOCATION, ITEM_CODE, SUM(QUANTITY) PICK_QTY
                    FROM {dbCode}SIINVMOVH
                    WHERE  IR_STAT<>'I' AND STATUS='10'
                    GROUP BY LOCATION,ITEM_CODE) AS TAB6 ON TAB5.LOCATION=TAB6.LOCATION AND TAB5.ITEM_CODE=TAB6.ITEM_CODE WHERE TAB5.LOCATION = '{warehouse}' UNION ";
                }
            }
            sql = sql.Substring(0, sql.Length - 6);
            var pivotColumns = string.Join(",", dbCodeAndWarehouses
                .SelectMany(kv => kv.Value)
                .Select(w => $"ISNULL(PivotTable.[{w}],0) [{w}]"));

            var finalSql = $@"WITH InventoryData AS ({sql}) SELECT PivotTable.ItemCode,
                           PivotTable.Description,
                           {pivotColumns}
                    FROM InventoryData
                    PIVOT (
                        SUM(Total)
                        FOR Location IN ({string.Join(",", dbCodeAndWarehouses
                            .SelectMany(kv => kv.Value)  
                            .Select(w => $"[{w}]"))})
                    ) AS PivotTable";
            Debug.WriteLine(finalSql);
            var results =
                await sqlDataAccess.LoadData<dynamic, dynamic>(finalSql,
                    new { });

            return results.ToList();
        }


        private string GenerateSql(string dbCode)
        {
            try
            {
                var sql =
                $@"SELECT TAB5.LOCATION Location, TAB5.ITEM_CODE ItemCode,DB.DB_NAME DbCode, TAB5.PHYSICAL Physical, TAB5.ON_ORDER [Order]
            FROM
                (SELECT TAB3.LOCATION, TAB3.ITEM_CODE,DB_CODE, TAB3.PHYSICAL, ISNULL(TAB4.HOLD_SALE,0) ON_ORDER
                FROM
                    (SELECT TAB1.LOCATION, TAB1.ITEM_CODE, DB_CODE, PHYSICAL
                    FROM (SELECT LOCATION, ITEM_CODE,
                            ISNULL(SUM(QUANTITY),0) PHYSICAL
                        FROM {dbCode}SIINVMOV
                        WHERE IR_STAT='I' AND STATUS='80' AND ALLOC_REF=''
                        GROUP BY LOCATION,ITEM_CODE) AS TAB1 LEFT JOIN
                        (SELECT ITEM_CODE,DB_CODE
                        FROM SIITEMS
                        WHERE DB_CODE='{dbCode}') AS TAB2 ON TAB1.ITEM_CODE=TAB2.ITEM_CODE) AS TAB3 LEFT JOIN
                    (SELECT LOCATION, ITEM_CODE, SUM(CASE WHEN STK_QTY_VALUE=1 THEN VALUE_1 WHEN STK_QTY_VALUE=2 THEN VALUE_2 WHEN STK_QTY_VALUE=3 THEN VALUE_3 WHEN STK_QTY_VALUE=4 
            THEN VALUE_4 WHEN STK_QTY_VALUE=5 THEN VALUE_5 WHEN STK_QTY_VALUE=6 THEN VALUE_6 WHEN STK_QTY_VALUE=7 THEN VALUE_7 WHEN STK_QTY_VALUE=8 THEN VALUE_8 WHEN STK_QTY_VALUE=9 
            THEN VALUE_9 WHEN STK_QTY_VALUE=10 THEN VALUE_10 WHEN STK_QTY_VALUE=11 THEN VALUE_11 WHEN STK_QTY_VALUE=12 THEN VALUE_12 WHEN STK_QTY_VALUE=13 THEN VALUE_13 
            WHEN STK_QTY_VALUE=14 THEN VALUE_14 WHEN STK_QTY_VALUE=15 THEN VALUE_15 WHEN STK_QTY_VALUE=16 THEN VALUE_16 WHEN STK_QTY_VALUE=17 THEN VALUE_17 WHEN STK_QTY_VALUE=18 
            THEN VALUE_18 WHEN STK_QTY_VALUE=19 THEN VALUE_19 WHEN STK_QTY_VALUE=20 THEN VALUE_20 ELSE 0 END) HOLD_SALE
                    FROM {dbCode}SISODET
                    WHERE REC_TYPE='D' AND STATUS<'80' AND CREDIT_STATUS=''
                    GROUP BY LOCATION,ITEM_CODE)
            AS TAB4 ON TAB3.LOCATION=TAB4.LOCATION AND TAB3.ITEM_CODE=TAB4.ITEM_CODE) AS TAB5 LEFT JOIN (SELECT LOCATION, ITEM_CODE
                FROM {dbCode}SIINVMOVH
                WHERE  IR_STAT<>'I' AND STATUS='10'
                GROUP BY LOCATION,ITEM_CODE) AS TAB6 ON TAB5.LOCATION=TAB6.LOCATION AND TAB5.ITEM_CODE=TAB6.ITEM_CODE
                INNER JOIN SIDBINFO DB ON DB.DB_CODE = TAB5.DB_CODE
                INNER JOIN SIWAREH W ON W.WAR_CODE = TAB5.[LOCATION]
            WHERE DB.DB_STAT = 'A' AND W.WAR_STAT = 'A'";
                return sql;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return string.Empty;
            }
        }

        public async Task<List<InventoryResponse>> GetInventoryByLocationAsync(string dbCode, string location)
        {
            var param = new
            {
                LOCATION = location,
                DB_CODE = dbCode
            };
            var criteria = $@"AND LOCATION = @LOCATION";
            var results = await sqlDataAccess.LoadData<InventoryResponse, dynamic>
                (InventoryQueries.GetInventory(dbCode,criteria:criteria), param);
            return results.ToList();
        }
    }
}

