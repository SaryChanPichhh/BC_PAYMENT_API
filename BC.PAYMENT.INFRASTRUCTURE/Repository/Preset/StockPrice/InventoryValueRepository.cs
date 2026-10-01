namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Preset.StockPrice;

public class InventoryValueRepository : IInventoryValueRepository
{
    private readonly ISqlDataAccess _sqlDatAccess;

    public InventoryValueRepository(ISqlDataAccess sqlDatAccess)
    {
        _sqlDatAccess = sqlDatAccess;
    }

    public async Task<List<InventoryValueModel>> GetInventoryValueAsync(Dictionary<string, string> branch, int page,
        int pageSize)
    {
        var inventoryValues = new List<InventoryValueModel>();
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 1;
        var offSet = (page - 1) * pageSize;
        foreach (var eachBranch in branch)
        {
            var sql =
                @$"SELECT '{eachBranch.Key}' [BranchCode], N'{eachBranch.Value}' [BranchName], LOCATION [Location], ITEM_CODE [ItemCode], ITEM_DESC [ItemName], PHYSICAL - ON_ORDER [Quantity], TOTAL_AMOUNT [Amount],COST Cost
                FROM(SELECT TAB5.LOCATION,
                        TAB5.ITEM_CODE,
                        TAB5.ITEM_DESC, 
                        TAB5.PHYSICAL,
                        TAB5.ON_ORDER,
                        SUM((TAB5.PHYSICAL - TAB5.ON_ORDER) * COST )TOTAL_AMOUNT,
                        COST
                    FROM(SELECT TAB3.LOCATION,
                            TAB3.ITEM_CODE,
                            TAB3.ITEM_DESC,
                            TAB3.PHYSICAL,
                            ISNULL(TAB4.HOLD_SALE, 0) ON_ORDER ,
			                COST
                        FROM(SELECT TAB1.LOCATION, 
				                TAB1.ITEM_CODE, 
				                TAB2.ITEM_DESC, 
				                PHYSICAL, 
				                COST
                            FROM(SELECT LOCATION, 
					                INV.ITEM_CODE, 
					                ISNULL(SUM(QUANTITY), 0) PHYSICAL , 
					                SI.ITEM_DCOST COST
                                FROM {eachBranch.Key}SIINVMOV INV
                                INNER JOIN SIITEMS SI ON INV.ITEM_CODE = SI.ITEM_CODE
                                WHERE IR_STAT = 'I'
                                    AND STATUS = '80'
                                    AND ALLOC_REF = ''
                                    AND DB_CODE = @DB_CODE
                                GROUP BY LOCATION, INV.ITEM_CODE, SI.ITEM_DCOST)  AS TAB1
                                LEFT JOIN(SELECT 
							                ITEM_CODE, 
							                ITEM_DESC
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
                            FROM {eachBranch.Key}SISODET
                            WHERE REC_TYPE = 'D'
                                AND STATUS < '80'
                                AND CREDIT_STATUS = ''
                            GROUP BY LOCATION, ITEM_CODE)  
                                                     AS TAB4 ON TAB3.LOCATION = TAB4.LOCATION AND TAB3.ITEM_CODE = TAB4.ITEM_CODE)  
                                                     AS TAB5
                        LEFT JOIN(SELECT LOCATION, ITEM_CODE, SUM(QUANTITY) PICK_QTY
                        FROM {eachBranch.Key}SIINVMOVH
                        WHERE IR_STAT <> 'I'
                            AND STATUS = '10'
                        GROUP BY LOCATION, ITEM_CODE,COST)  
                                                     AS TAB6 ON TAB5.LOCATION = TAB6.LOCATION AND TAB5.ITEM_CODE = TAB6.ITEM_CODE 
                                                     GROUP BY TAB5.LOCATION,
												   		  TAB5.ITEM_CODE,
														  TAB5.ITEM_DESC,
														  TAB5.PHYSICAL,
														  TAB5.ON_ORDER, COST) C where PHYSICAL - ON_ORDER <> 0 ORDER BY Location,ItemCode OFFSET @OFF_SET ROWS FETCH NEXT @PAGE_SIZE ROWS ONLY ";
            var param = new
            {
                DB_CODE = eachBranch.Key,
                OFF_SET = offSet,
                PAGE_SIZE = pageSize
            };
            var execute = await _sqlDatAccess.LoadData<InventoryValueModel, dynamic>(sql, param);
            inventoryValues.AddRange(execute.ToList());
        }

        return inventoryValues;
    }
}