namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Transaction.Submitting.InvoiceVerify
{
    public class MonthlyInvoiceRepository : IMonthlyInvoiceRepository
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public MonthlyInvoiceRepository(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<List<MonthlyInvoiceModel>> GetInvoiceVerifyByDateAsync(string dbCode, string fromDate, string toDate)
        {
            var sql =
                $@"SELECT D.CREATE_DATE CreateDate,N.CUSTOMER_CODE CustomerCode,N.ACC_NAME_KH CustomerName,N.TRANSACTION_REF TransactionCode,N.HEADER_TRANSACTION_VALUES InvoiceValue
            ,CASE WHEN D.DIVIDED_INVOICE_ID IN (SELECT DIVIDED_INIOVICE_ID FROM PC_RETURN_INVOICE WHERE  UPPER(DESCRIPTION) = 'CANCEL') THEN 'Cancel'
                WHEN D.DIVIDED_INVOICE_ID IN(SELECT DIVDIE_INVOICE_ID FROM PC_PAYMENT_INVOICE) THEN 'PAID'
                ELSE '' END Status, ISNULL(P.AMOUNT, 0) PAID, N.HEADER_TRANSACTION_VALUES - ISNULL(P.AMOUNT, 0) Total
                FROM NEW_INVOICE N INNER JOIN PC_DIVIDED_INVOICE D
                ON D.INVOICE_ID = N.ID
                LEFT OUTER JOIN PC_PAYMENT_INVOICE P
                ON P.DIVDIE_INVOICE_ID = D.DIVIDED_INVOICE_ID
                WHERE D.CREATE_DATE BETWEEN @FROM_DATE AND @TO_DATE AND N.DB_CODE = @DB_CODE
                AND D.DB_CODE = @DB_CODE  AND P.DB_CODE = @DB_CODE
                ORDER BY Status";

            var param = new
            {
                FROM_DATE = fromDate,
                TO_DATE = toDate,
                DB_CODE = dbCode
            };
            var execute = await _sqlDataAccess.LoadData<MonthlyInvoiceModel, dynamic>(sql, param);
            return execute.ToList();
        }

        public async Task<List<MonthlyInvoiceModel>> GetInvoiceVerifyByPeriodAsync(string dbCode, int month, int year)
        {
            var sql =
                $@"SELECT D.CREATE_DATE CreateDate,N.CUSTOMER_CODE CustomerCode,N.ACC_NAME_KH CustomerName,N.TRANSACTION_REF TransactionCode,N.HEADER_TRANSACTION_VALUES InvoiceValue
                ,CASE WHEN D.DIVIDED_INVOICE_ID IN (SELECT DIVIDED_INIOVICE_ID FROM PC_RETURN_INVOICE WHERE  UPPER(DESCRIPTION) = 'CANCEL') THEN 'Cancel'
                WHEN D.DIVIDED_INVOICE_ID IN(SELECT DIVDIE_INVOICE_ID FROM PC_PAYMENT_INVOICE) THEN 'PAID'
                ELSE '' END Status, ISNULL(P.AMOUNT, 0) PAID, N.HEADER_TRANSACTION_VALUES - ISNULL(P.AMOUNT, 0) Total
                FROM NEW_INVOICE N INNER JOIN PC_DIVIDED_INVOICE D
                ON D.INVOICE_ID = N.ID
                LEFT OUTER JOIN PC_PAYMENT_INVOICE P
                ON P.DIVDIE_INVOICE_ID = D.DIVIDED_INVOICE_ID
                WHERE MONTH(D.CREATE_DATE) = @MONTH AND YEAR(D.CREATE_DATE) = @YEAR AND N.DB_CODE = @DB_CODE
                AND D.DB_CODE = @DB_CODE  AND P.DB_CODE = @DB_CODE
                ORDER BY Status";

            var param = new
            {
                MONTH = month,
                YEAR = year,
                DB_CODE = dbCode
            };
            var execute = await _sqlDataAccess.LoadData<MonthlyInvoiceModel, dynamic>(sql, param);
            return execute.ToList();
        }
    }
}
