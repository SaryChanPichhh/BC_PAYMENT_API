namespace BC.PAYMENT.SQL.Queries;

public static class InventoryQueries
{
    public static string GetInventory(string dbCode,string addOnField="",string addReference="",string criteria="",string groupBy="",
        string sortBy="") => $@"
SELECT DISTINCT ITEM_CODE ItemCode, ITEM_DESC ItemDesc,C.ITEM_CUS10_KH AS ItemDescKh
                FROM(SELECT TAB5.LOCATION,
                        TAB5.ITEM_CODE,
                        TAB5.ITEM_DESC,
						TAB5.ITEM_CUS10_KH,
                        TAB5.PHYSICAL,
                        TAB5.ON_ORDER
                    FROM(SELECT TAB3.LOCATION,
                            TAB3.ITEM_CODE,
                            TAB3.ITEM_DESC,
							TAB3.ITEM_CUS10_KH,
                            TAB3.PHYSICAL,
                            ISNULL(TAB4.HOLD_SALE, 0) ON_ORDER
                        FROM(SELECT TAB1.LOCATION, 
                        TAB1.ITEM_CODE, 
                        TAB2.ITEM_DESC, 
						TAB2.ITEM_CUS10_KH,
                        PHYSICAL
                            FROM(SELECT LOCATION, 
                         ITEM_CODE, 
                          ISNULL(SUM(QUANTITY), 0) PHYSICAL 
                                FROM {dbCode}SIINVMOV 
                                WHERE IR_STAT = 'I'
                                    AND STATUS = '80'
                                    AND ALLOC_REF = ''
                  AND LOCATION = @LOCATION
                                GROUP BY LOCATION, ITEM_CODE)  AS TAB1
                                LEFT JOIN(SELECT 
                              ITEM_CODE, 
							  ITEM_DESC,
                              ITEM_CUS10_KH
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
                            FROM {dbCode}SISODET
                            WHERE REC_TYPE = 'D'
                                AND STATUS < '80'
                                AND CREDIT_STATUS = ''
                            {criteria}
                            GROUP BY LOCATION, ITEM_CODE)  
                                                     AS TAB4 ON TAB3.LOCATION = TAB4.LOCATION AND TAB3.ITEM_CODE = TAB4.ITEM_CODE)  
                                                     AS TAB5
                        LEFT JOIN(SELECT LOCATION, ITEM_CODE, SUM(QUANTITY) PICK_QTY
                        FROM {dbCode}SIINVMOVH
                        WHERE IR_STAT <> 'I'
                            AND STATUS = '10'
              AND LOCATION = @LOCATION
                        GROUP BY LOCATION, ITEM_CODE)  
                                                     AS TAB6 ON TAB5.LOCATION = TAB6.LOCATION AND TAB5.ITEM_CODE = TAB6.ITEM_CODE 
                                                     GROUP BY TAB5.LOCATION,
                                 TAB5.ITEM_CODE,
                              TAB5.ITEM_DESC,
                              TAB5.PHYSICAL,
                              TAB5.ON_ORDER,
							  TAB5.ITEM_CUS10_KH) C ORDER BY ITEM_CODE
";
}