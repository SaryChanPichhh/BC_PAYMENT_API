namespace BC.PAYMENT.SQL.Queries
{
    public static class NewInvoiceQueries
    {
        public const string GetInvoices = @"SELECT ID InvoiceId,TRANSACTION_REF InvoiceCode,CUS.*,HEADER_TRANSACTION_VALUES InvoiceAmount,
                        CASE WHEN N.STATUS = 'C' THEN 'ChangeInvoice'
                        WHEN N.STATUS = 'N' THEN 'NewInvoice'
                        WHEN N.STATUS = 'O' THEN 'OldInvoice' END [InvoiceType],
                        CREATED_DATE CreatedDate,
                        CREATED_BY CreatedBy,
                        IS_DIVIDED IsDivided,
                        ENTRIES_CODE EntriesCode
                        FROM NEW_INVOICE N LEFT JOIN GET_CUSTOMERS_BY_DB_CODE() CUS ON CUS.CustomerCode = N.CUSTOMER_CODE
                        WHERE N.DB_CODE = @DB_CODE AND STATUS = @STATUS AND N.CREATED_DATE = @CREATED_DATE";

        public static string SelectNewInvoiceDate(string dbCode) => $"{dbCode}SELECT_NEW_INVOICE_DATE";

        public const string SaveInvoices = @"
                IF NOT EXISTS (SELECT 1 FROM NEW_INVOICE WHERE TRANSACTION_REF = @INVOICE_CODE AND CREATED_DATE = CONVERT(DATE,GETDATE()) 
                AND STATUS = @STATUS AND DB_CODE = @DB_CODE AND IS_DIVIDED = 1)
                    INSERT INTO NEW_INVOICE(DB_CODE,TRANSACTION_REF,CUSTOMER_CODE,ACC_NAME_KH,HEADER_TRANSACTION_VALUES,STATUS,CREATED_DATE,CREATED_BY,IS_DIVIDED,ENTRIES_CODE)
                             VALUES(@DB_CODE,@INVOICE_CODE,@CUSTOMER_CODE,@CUSTOMER_NAME,@INVOICE_VALUE,@STATUS,@CREATED_DATE,@CREATED_BY,@IS_DIVIDED,@ENTRIES_CODE)";

        public static string DeleteInvoice => $@"DELETE FROM NEW_INVOICE WHERE INVOICE_ID = @INVOICE_ID";

        public const string InsertEditValueDividedInvoice = @"
            INSERT INTO PC_EDIT_VALUE_DIVIDED_INVOICE(DB_CODE,DIVIDED_INVOICE_ID,OLD_VALUE,NEW_VALUE,DESCRIPTION,CREATE_DATE,CREATE_BY)
            VALUES(@DB_CODE,@DIVIDED_ID,@OLD_VALUE,@NEW_VALUE,@DESCRIPTION,GETDATE(),@CREATED_BY)";

        public const string UpdateNewInvoiceHeaderValue = @"
            UPDATE NEW_INVOICE SET HEADER_TRANSACTION_VALUES = @InvoiceValue WHERE ID IN (SELECT INVOICE_ID FROM PC_DIVIDED_INVOICE WHERE DIVIDED_INVOICE_ID = @ID)";
    }
}
