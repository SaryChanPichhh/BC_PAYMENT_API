namespace BC.PAYMENT.SQL.Queries;

public static class InvoiceClosingEntryQueries
{
    public static string CheckIsEntriesIsAlreadyOpen =>
        "SELECT COUNT(*) FROM PAYMENT_CLOSING_ENTRY_INVOICE WHERE IS_ACTIVE=1 AND DB_CODE=@DB_CODE";

    public static string DeleteEntries => "DELETE FROM PAYMENT_CLOSING_ENTRY_INVOICE WHERE ID=@ID AND DB_CODE=@DB_CODE";

    public static string AddNew =>
        $@"INSERT INTO PAYMENT_CLOSING_ENTRY_INVOICE(CODE,DB_CODE,DESCRIPTION,CREATED_BY,CREATED_DATE,IS_ACTIVE)
                VALUES(@CODE,@DB_CODE,@DESCRIPTION,@CREATED_BY,@CREATED_DATE,@IS_ACTIVE)";

    public static string GenerateEntriesCode =>
        $@"SELECT ISNULL(MAX(SUBSTRING(CODE,8,10)),0) FROM PAYMENT_CLOSING_ENTRY_INVOICE
         WHERE SUBSTRING(CODE,4,2) = MONTH(GETDATE()) AND DB_CODE = @DB_CODE";
}