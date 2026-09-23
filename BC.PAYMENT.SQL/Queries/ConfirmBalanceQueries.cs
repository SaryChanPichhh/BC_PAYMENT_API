namespace BC.PAYMENT.SQL.Queries;

public static class ConfirmBalanceQueries
{
    public const string AddConfirmAccountReceivable =
        @"INSERT INTO DT_CONFIRM_ACCOUNT_RECEIVABLE(DB_CODE,CONFIRM_BALANCE_OWNER,PARTICIPANTS,DESCRIPTION,STATUS,CREATED_DATE,CREATED_BY)
        VALUES (@DB_CODE,@CONFIRM_BALANCE_OWNER,@PARTICIPANTS,@DESCRIPTION,@STATUS,@CREATED_DATE,@CREATED_BY)";

    public const string DeleteConfirmAccountReceivable =
        @"DELETE FROM DT_CONFIRM_ACCOUNT_RECEIVABLE WHERE ID = @ID";

    public const string UpdateConfirmAccountReceivable =
        @"UPDATE DT_CONFIRM_ACCOUNT_RECEIVABLE SET CONFIRM_BALANCE_OWNER = @CONFIRM_BALANCE_OWNER,PARTICIPANTS = @PARTICIPANTS,DESCRIPTION = @DESCRIPTION WHERE ID = @ID";

    public const string GetConfirmAccountReceivable =
        @"SELECT ID Id,CONFIRM_BALANCE_OWNER ConfirmBalanceOwner,PARTICIPANTS Participants,DESCRIPTION Description,CREATED_DATE CreatedDate,CREATED_BY CreatedBy
        FROM DT_CONFIRM_ACCOUNT_RECEIVABLE
        WHERE STATUS = 'Inprogress' AND DB_CODE = @DB_CODE";
    public static string GetConfirmBalanceDetail =>
        $@"SELECT D.ID ConfirmBalanceDetailsId,D.CUSTOMER_CODE CustomerCode,CUS.CustomerName,CUS.Area,CUS.Market,CUS.Store,D.INVOICE_CODE InvoicedCode,INVOICE_AMOUNT InvoicedAmount,
                D.BALANCE Balance,D.DESCRIPTION Description,D.IS_AGREE IsCustomerAgreed,UPDATED_DATE UpdatedDate,UPDATED_BY UpdatedBy,STATUS Status,D.CUSTOMER_STATUS IsMet
                FROM DT_CONFIRM_ACCOUNT_RECEIVABLEDET D
                LEFT JOIN GET_CUSTOMERS_BY_DB_CODE() CUS ON CUS.CustomerCode = D.CUSTOMER_CODE
                WHERE D.DB_CODE = @DB_CODE AND D.HEADER_ID = @ID";
    public static string UpdateConfirmBalanceDetail =>
        $@"UPDATE DT_CONFIRM_ACCOUNT_RECEIVABLEDET SET 
            BALANCE = @BALANCE,
            DESCRIPTION = @DESCRIPTION,
            IS_AGREE = @IS_AGREE,
            CUSTOMER_STATUS = @IS_MET,
            STATUS = 'Completed',
            UPDATED_DATE = @UPDATED_DATE,
            UPDATED_BY = @UPDATED_BY
            WHERE INVOICE_CODE = @INVOICE_CODE
            AND HEADER_ID = @HEADER_ID;";

    public const string AddConfirmBalanceDetail =
        @"INSERT INTO DT_CONFIRM_ACCOUNT_RECEIVABLEDET
        (HEADER_ID,DB_CODE,INVOICE_CODE,CUSTOMER_NAME,CUSTOMER_CODE,INVOICE_AMOUNT,CREATED_DATE,CREATED_BY,STATUS)        
        VALUES (@HEADER_ID,@DB_CODE,@INVOICE_CODE,@CUSTOMER_NAME,@CUSTOMER_CODE,@INVOICE_AMOUNT,@CREATED_DATE,@CREATED_BY,@STATUS)";

    public const string CheckExistInvoice =
        @"SELECT CAST(COUNT(*) AS BIT) FROM DT_CONFIRM_ACCOUNT_RECEIVABLEDET
        WHERE DB_CODE = @DB_CODE AND HEADER_ID = @HEADER_ID AND INVOICE_CODE = @INVOICE_CODE";
}
