namespace BC.PAYMENT.SQL.Queries;

public static class ReturnInvoiceQueries
{
    public const string GetReturnInvoice =
        @"SELECT N.ID InvoiceId,N.TRANSACTION_REF TransactionCode,N.CUSTOMER_CODE CustomerCode,S.ADD_LINE_1KH CustomerName,HEADER_TRANSACTION_VALUES InvoiceValue,
            M.MARKET_KHMER_NAME Market,A.AREA_NAME_KHMER Area,N.STATUS Status,S.STORE Store FROM NEW_INVOICE N
                LEFT JOIN SIADD S ON S.ADD_CODE = N.CUSTOMER_CODE
                INNER JOIN TB_BCMARKET M ON M.MARKET_ID = S.MARKET_ID
                INNER JOIN TB_AREAS A ON A.AREA_ID = S.AREA_ID
                WHERE CREATED_DATE = CONVERT(DATE,GETDATE()) AND N.CUSTOM_FIELD_1 = 'Return Invoice'
                AND S.DB_CODE = @DB_CODE AND A.DB_CODE = @DB_CODE AND M.DB_CODE=  @DB_CODE AND N.DB_CODE = @DB_CODE";

    public static string GetReturnInvoiceByDate(string addOnField, string criteria)
    {
        return
            @$"SELECT TRANSACTION_REF TransactionCode,C    USTOMER_CODE CustomerCode,ACC_NAME_KH CustomerName,HEADER_TRANSACTION_VALUES InvoiceValue,
                S.STORE Store,M.MARKET_KHMER_NAME Market,A.AREA_NAME_KHMER Area,N.STATUS Status,{addOnField}
                FROM NEW_INVOICE N INNER JOIN PC_DIVIDED_INVOICE P 
                ON P.INVOICE_ID = N.ID INNER JOIN PC_RETURN_INVOICE R ON R.DIVIDED_INIOVICE_ID = P.DIVIDED_INVOICE_ID
                LEFT JOIN SIADD S ON S.ADD_CODE = N.CUSTOMER_CODE
                INNER JOIN TB_BCMARKET M ON M.MARKET_ID = S.MARKET_ID
                INNER JOIN TB_AREAS A ON A.AREA_ID = S.AREA_ID
                WHERE N.DB_CODE = @DB_CODE AND P.DB_CODE = @DB_CODE AND R.DB_CODE = @DB_CODE
                {criteria}";
    }

    public const string InsertReturnInvoice = @"
            IF NOT EXISTS (SELECT * FROM NEW_INVOICE WHERE TRANSACTION_REF = @TRANSACITON AND CREATED_DATE = CONVERT(DATE,GETDATE()) AND DB_CODE = @DB_CODE AND IS_DIVIDED = 1)
            INSERT INTO NEW_INVOICE
            (DB_CODE,TRANSACTION_REF,CUSTOMER_CODE,ACC_NAME_KH,HEADER_TRANSACTION_VALUES,STATUS,CREATED_DATE,CREATED_BY,IS_DIVIDED,CUSTOM_FIELD_1,ENTRIES_CODE)
            VALUES (@DB_CODE,@TRANSACITON,@CUSTOMER_CODE,@CUSTOMER_NAME,@INVOICE_VALUE,@STATUS,@CREATED_DATE,@CREATED_BY,1,'Return Invoice',@ENTRIES_CODE)";

    public static string CreatePcReturnInvoice => @"
            IF NOT EXISTS (SELECT * FROM PC_RETURN_INVOICE WHERE DB_CODE = @DB AND DIVIDED_INIOVICE_ID = @DDID)
            INSERT INTO PC_RETURN_INVOICE(DB_CODE,DIVIDED_INIOVICE_ID,PAYMENT_HEADER_ID,DESCRIPTION,CREATED_DATE,CREATED_BY,STATUS)
            VALUES(@DB, @DDID, @PAYMENT_HEADER_ID, @DESC, CONVERT(DATE, GETDATE()), @CB, 1)
            UPDATE PC_DIVIDED_INVOICE SET STATUS = '0' WHERE DIVIDED_INVOICE_ID = @DDID AND DB_CODE = @DB";

    public static string GetPcReturnInvoices => @"SELECT RETURN_ID ReturnId
      ,DB_CODE DbCode
      ,CAST(DIVIDED_INIOVICE_ID AS INT) DividedId
      ,DESCRIPTION [Description]
      ,CREATED_DATE CreatedDate
      ,CREATED_BY CreatedBy
      ,STATUS Status
      ,PAYMENT_HEADER_ID HeaderId FROM PC_RETURN_INVOICE WHERE DB_CODE = @DB_CODE";

    public static string DeletePcReturnInvoice => @$"
            DELETE FROM PC_RETURN_INVOICE WHERE DIVIDED_INIOVICE_ID = @DIVIDED_ID";

    public static string UpdateDividedInvoiceStatusAfterReturnDelete => @$"
            IF NOT EXISTS(SELECT * FROM PC_PAYMENT_INVOICE WHERE DIVDIE_INVOICE_ID = @DIVIDED_ID)
            UPDATE PC_DIVIDED_INVOICE SET STATUS = 1 WHERE DIVIDED_INVOICE_ID = @DIVIDED_ID";

    public static string GetPcReturnInvoiceAudit(string criteria)
    {
        return $@"
            SELECT ReturnId,DeliveryName,CustomerCode,CustomerName,TransactionCode,Change,New,Cancel,Description,CreatedDate,CASE WHEN RETURN_ID IS NOT NULL THEN '1' END STATUS FROM (
                SELECT R.RETURN_ID ReturnId, DELIVERIES_KHMER DeliveryName, N.CUSTOMER_CODE CustomerCode, ACC_NAME_KH CustomerName, TRANSACTION_REF TransactionCode, 
                CASE 
                    WHEN N.STATUS = 'C' THEN '1' END 'Change',
                CASE
                    WHEN N.STATUS = 'N' THEN '1' END 'New',
                CASE
                    WHEN UPPER(DESCRIPTION) LIKE '%OFFICE%' THEN 'Office'
                    WHEN UPPER(DESCRIPTION) LIKE '%CANCEL%' THEN 'Cancel' END 'Cancel',
                CASE
                    WHEN UPPER(DESCRIPTION) NOT LIKE '%OFFICE%' AND UPPER(DESCRIPTION) NOT LIKE '%CANCEL%'
                    THEN DESCRIPTION END 'Description',
                P.CREATE_DATE CreatedDate,
                PRIA.RETURN_ID
                FROM NEW_INVOICE N
                INNER JOIN PC_DIVIDED_INVOICE P ON P.INVOICE_ID = N.ID
                INNER JOIN PC_RETURN_INVOICE R ON R.DIVIDED_INIOVICE_ID = P.DIVIDED_INVOICE_ID
                INNER JOIN TB_BCDELIVERIES T ON T.DELIVERIES_ID = P.DELIVERY_ID
                LEFT JOIN PC_RETURNING_INVOICE_AUDIT PRIA on R.RETURN_ID = PRIA.RETURN_ID
                WHERE P.DB_CODE = @DB_CODE
                AND R.DB_CODE = @DB_CODE
                AND T.DB_CODE = @DB_CODE
                {criteria}
                -- AND P.CREATE_DATE BETWEEN @FROM_DATE AND @TO_DATE
                ) TAB;
    ";
    }

    public static string InsertPcReturnInvoiceAudit =>
        $@"INSERT INTO PC_RETURNING_INVOICE_AUDIT(RETURN_ID, PROCESSING_STATUS, APPROVAL_STATUS, LAST_UPDATED_DATE, LAST_UPDATED_BY)
                               VALUES(@RETURN_ID, @PROCESSING_STATUS,@APPROVAL_STATUS, @LAST_UPDATED_DATE, @LAST_UPDATED_BY)";
}