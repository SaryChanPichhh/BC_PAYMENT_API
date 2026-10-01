namespace BC.PAYMENT.SQL.Queries;

public static class DividedInvoiceQueries
{
    public const string CheckExistsDividedInvoice =
        @"SELECT CAST(CASE WHEN COUNT(*) > 0 THEN 1 ELSE 0 END AS BIT) FROM PC_DIVIDED_INVOICE WHERE INVOICE_ID = @InvoiceId";

    public const string InsertDividedInvoice =
        @"INSERT INTO PC_DIVIDED_INVOICE (DB_CODE, INVOICE_ID, DELIVERY_ID, CREATE_DATE, CREATE_BY, STATUS)
          VALUES (@DbCode, @InvoiceId, @DeliveryId, @CreatedDate, @CreatedBy, 1)";

    public const string UpdateNewInvoiceDividedStatus =
        @"UPDATE NEW_INVOICE SET IS_DIVIDED = 0 WHERE ID = @InvoiceId";

    public static string GetDividedInvoicesByDeliveryIdAndDate =>
        @"SELECT 
                 N.ID AS Id,
                 DE.DELIVERIES_KHMER AS DeliveryName,
                 N.CUSTOMER_CODE AS CustomerCode,
                 N.ACC_NAME_KH AS CustomerName,
                 Customer.STORE AS Store,
                 N.TRANSACTION_REF AS TransactionCode, 
                 1.00 AS Amount,
                 N.HEADER_TRANSACTION_VALUES AS InvoiceValue,
                 CASE WHEN N.STATUS = 'N' THEN N'ប៉ុងថ្មី' WHEN N.STATUS = 'C' THEN N'ប៉ុងដូរ' else N'ប៉ុងចាស់' End 'Type'
                 FROM PC_DIVIDED_INVOICE D
                 INNER JOIN NEW_INVOICE N ON N.ID = D.INVOICE_ID
                 INNER JOIN TB_BCDELIVERIES DE ON DE.DELIVERIES_ID = D.DELIVERY_ID
                 LEFT JOIN(SELECT ADD_CODE, STORE FROM SIADD where DB_CODE =
                 @DB_CODE) Customer on Customer.ADD_CODE = N.CUSTOMER_CODE
                 WHERE N.IS_DIVIDED = '0'
                 AND DE.DELIVERIES_ID = @DELIVERY_ID
                 AND D.CREATE_DATE = @DATE
                 AND D.DB_CODE = @DB_CODE";

    public const string RevokeDividedInvoice = "PC_REVOKE_DIVIDED_INVOICE";

    public static string GetDividedInvoice =>
        $@"SELECT DELIVERIES_NAME DeliveryName,COUNT(CASE WHEN N.STATUS = 'N' THEN 1 END) AS 'NEW',COUNT(CASE WHEN N.STATUS = 'C' THEN 1 END) AS 'CHANGE',COUNT(CASE WHEN N.STATUS = 'O' THEN 1 END) AS 'OLD',
                (COUNT(CASE WHEN N.STATUS = 'N' THEN 1 END) + COUNT(CASE WHEN N.STATUS = 'C' THEN 1 END) + COUNT(CASE WHEN N.STATUS = 'O' THEN 1 END)) AS 'TOTAL'
                 FROM NEW_INVOICE N INNER JOIN PC_DIVIDED_INVOICE P ON P.INVOICE_ID = N.ID INNER JOIN TB_BCDELIVERIES D ON D.DELIVERIES_ID = P.DELIVERY_ID
                 WHERE P.CREATE_DATE = @DATE AND P.DB_CODE = @DB_CODE
                 GROUP BY DELIVERIES_NAME";

    public static string GetDividedInvoiceDetail =>
        $@"SELECT TRANSACTION_REF TransactionCode, CUSTOMER_CODE CustomerCode, ACC_NAME_KH CustomerName,ISNULL(Market,'-')Market,
        ISNULL(CASE WHEN N.STATUS = 'N' THEN '1' END, '-') NewInvoice,
        ISNULL(CASE WHEN N.STATUS = 'C' THEN '1' END, '-') ChangeInvoice,
        N.HEADER_TRANSACTION_VALUES InvoiceValue,
        DIVIDED_INVOICE_ID DividedInvoiceId
        FROM NEW_INVOICE N
        INNER JOIN PC_DIVIDED_INVOICE D ON D.INVOICE_ID = N.ID
        LEFT JOIN (SELECT ADD_CODE,STORE +' '+ MARKET_KHMER_NAME Market FROM SIADD SI INNER JOIN TB_BCMARKET M ON M.MARKET_ID = SI.MARKET_ID
        WHERE SI.DB_CODE = @DB_CODE AND M.DB_CODE = @DB_CODE) TAB ON TAB.ADD_CODE = N.CUSTOMER_CODE
        WHERE D.STATUS = '1' AND N.DB_CODE = @DB_CODE AND D.DB_CODE = @DB_CODE
        AND D.CREATE_DATE BETWEEN @FROM_DATE AND @TO_DATE AND N.STATUS IN ('C','N');";

    public static string GetDividedInvoiceStatus =>
        $@"SELECT D.DIVIDED_INVOICE_ID DividedId,D.CREATE_DATE Date,Store,DL.DELIVERIES_KHMER DeliveryName, N.CUSTOMER_CODE CustomerCode,CUST.CUSTOMER_NAME CustomerName,
                MARKET Market,AREA Area,N.TRANSACTION_REF TransactionCode,N.HEADER_TRANSACTION_VALUES InvoiceValue,CONVERT(BIT,CASE WHEN R.RETURN_ID IS NOT NULL THEN 1 ELSE 0 END) IsReturn,
                CONVERT(BIT,CASE WHEN P.PAYMENT_ID IS NOT NULL THEN 1 ELSE 0 END) IsPaid,R.[DESCRIPTION] Description,P.AMOUNT PaidAmount,N.[STATUS] Status
                FROM NEW_INVOICE N INNER JOIN PC_DIVIDED_INVOICE D ON D.INVOICE_ID = N.ID
                    LEFT JOIN PC_PAYMENT_INVOICE P ON P.DIVDIE_INVOICE_ID = D.DIVIDED_INVOICE_ID
                    LEFT JOIN PC_RETURN_INVOICE R ON R.DIVIDED_INIOVICE_ID = D.DIVIDED_INVOICE_ID
                    LEFT JOIN (SELECT ADD_CODE CUSTOMER_CODE,STORE Store, ADD_LINE_1KH CUSTOMER_NAME, MARKET_KHMER_NAME MARKET, AREA_NAME_KHMER AREA
                    FROM SIADD S INNER JOIN TB_AREAS A ON A.AREA_ID = S.AREA_ID
                        INNER JOIN TB_BCMARKET M ON M.MARKET_ID = S.MARKET_ID
                    WHERE S.DB_CODE = @DB_CODE AND A.DB_CODE = @DB_CODE AND M.DB_CODE = @DB_CODE) CUST ON CUST.CUSTOMER_CODE = N.CUSTOMER_CODE
                    INNER JOIN TB_BCDELIVERIES DL ON DL.DELIVERIES_ID = D.DELIVERY_ID
                    WHERE N.DB_CODE = @DB_CODE AND D.DB_CODE = @DB_CODE --AND DL.DB_CODE = @DB_CODE
                    AND D.CREATE_DATE = @DIVIDED_DATE AND DL.DELIVERIES_ID = @DELIVERY_ID";

    public static string GetDividedTransactionStatus => $@"SELECT 
                                     P.INVOICE_ID InvoiceId,P.DIVIDED_INVOICE_ID DividedId,N.TRANSACTION_REF InvoiceCode,
                                     N.CUSTOMER_CODE CustomerCode, N.ACC_NAME_KH CustomerName, CASE WHEN N.STATUS = 'O' THEN '1.00' END 'Old', CASE WHEN N.STATUS = 'N' THEN '1.00' END 'New', CASE WHEN N.STATUS = 'C' THEN '1.00' END 'Change',
                                     N.HEADER_TRANSACTION_VALUES InvoiceValue, CONVERT(BIT, 0) IsReturn, '' Other, PAID.AMOUNT PaidValue, N.HEADER_TRANSACTION_VALUES - PAID.AMOUNT Amount, CONVERT(BIT, 1) IsPaid
                                     FROM NEW_INVOICE N
                                     INNER JOIN PC_DIVIDED_INVOICE P
                                     ON P.INVOICE_ID = N.ID
                                     INNER JOIN PC_PAYMENT_INVOICE PAID ON PAID.DIVDIE_INVOICE_ID = P.DIVIDED_INVOICE_ID
                                     WHERE N.DB_CODE = @DB_CODE AND P.DB_CODE = @DB_CODE AND P.DELIVERY_ID = @DELIVERY_ID AND PAID.DB_CODE = @DB_CODE AND P.CREATE_DATE = @DIVIDED_DATE
                                     UNION ALL
                                     SELECT
                                     P.INVOICE_ID InvoiceId,P.DIVIDED_INVOICE_ID DividedId,N.TRANSACTION_REF InvoiceCode, N.CUSTOMER_CODE CustomerCode, N.ACC_NAME_KH CustomerCode, CASE WHEN N.STATUS = 'O' THEN '1.00' END 'Old', CASE WHEN N.STATUS = 'N' THEN '1.00' END 'New', CASE WHEN N.STATUS = 'C' THEN '1.00' END 'Change',
                                     N.HEADER_TRANSACTION_VALUES InvoiceValue, CONVERT(BIT, 1)[IsReturn], R.DESCRIPTION Other, 0, 0, CONVERT(BIT, 0) IsPaid
                                     FROM NEW_INVOICE N
                                     INNER JOIN PC_DIVIDED_INVOICE P
                                     ON P.INVOICE_ID = N.ID
                                     INNER JOIN PC_RETURN_INVOICE R ON R.DIVIDED_INIOVICE_ID = P.DIVIDED_INVOICE_ID
                                     WHERE N.DB_CODE = @DB_CODE AND P.DB_CODE = @DB_CODE AND R.DB_CODE = @DB_CODE AND P.DELIVERY_ID = @DELIVERY_ID AND P.CREATE_DATE = @DIVIDED_DATE
                                     UNION ALL
                                     SELECT
                                     P.INVOICE_ID InvoiceId, P.DIVIDED_INVOICE_ID DividedId,N.TRANSACTION_REF InvoiceCode, N.CUSTOMER_CODE CustomerCode, N.ACC_NAME_KH CustomerName, CASE WHEN N.STATUS = 'O' THEN '1.00' END 'Old', CASE WHEN N.STATUS = 'N' THEN '1.00' END 'New', CASE WHEN N.STATUS = 'C' THEN '1.00' END 'Change',
                                     N.HEADER_TRANSACTION_VALUES InvoiceValue, CONVERT(BIT, 0)[IsReturn], '' Other, 0, 0, CONVERT(BIT, 0) IsPaid
                                     FROM NEW_INVOICE N
                                     INNER JOIN PC_DIVIDED_INVOICE P
                                     ON P.INVOICE_ID = N.ID
                                     WHERE P.STATUS = '1' AND N.DB_CODE = @DB_CODE AND P.DB_CODE = @DB_CODE AND P.DELIVERY_ID = @DELIVERY_ID AND P.CREATE_DATE = @DIVIDED_DATE;";
}