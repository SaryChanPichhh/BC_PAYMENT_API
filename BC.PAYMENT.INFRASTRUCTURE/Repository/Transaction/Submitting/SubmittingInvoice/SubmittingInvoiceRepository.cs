namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Transaction.Submitting.SubmittingInvoice
{
    public class SubmittingInvoiceRepository : ISubmittingInvoiceRepository
    {
        private readonly ISqlDataAccess _sqlDataAccess;
        private readonly IDbConnection _dbConnection;
        public SubmittingInvoiceRepository(ISqlDataAccess sqlDataAccess, IDbConnection dbConnection)
        {
            _sqlDataAccess = sqlDataAccess;
            _dbConnection = dbConnection;
        }

        public async Task<List<SubmittedInvoiceModel>> GetAllNotSubmitPaidInvoice(string dbCode, string fromDate, string toDate)
        {
            var sql = $@"SELECT * FROM (SELECT DISTINCT N.ID InvoiceId,D.DELIVERIES_KHMER DeliveryName,N.CUSTOMER_CODE CustomerCode,
            N.ACC_NAME_KH CustomerName, N.TRANSACTION_REF TransactionCode, 1.00 AS AMOUNT, N.HEADER_TRANSACTION_VALUES InvoiceAmount,
            ISNULL(CASE WHEN N.HEADER_TRANSACTION_VALUES = PAID.AMOUNT THEN PAID.AMOUNT END, 0) 'Paid',
            ISNULL(CASE WHEN N.HEADER_TRANSACTION_VALUES > PAID.AMOUNT THEN PAID.AMOUNT END, 0) 'HalfPaid',
            N.HEADER_TRANSACTION_VALUES - PAID.AMOUNT Total,
            CONVERT(BIT,CASE WHEN N.HEADER_TRANSACTION_VALUES = PAID.AMOUNT THEN 1 ELSE 0 END) 'Status',
            N.STATUS [InvoiceType],P.CREATE_DATE CreateDate
			,H.TRANS_REF 
            FROM
            PC_DIVIDED_INVOICE P
            INNER JOIN NEW_INVOICE N ON N.ID = P.INVOICE_ID
            INNER JOIN PC_PAYMENT_INVOICE PAID ON PAID.DIVDIE_INVOICE_ID = P.DIVIDED_INVOICE_ID
            INNER JOIN TB_BCDELIVERIES D ON D.DELIVERIES_ID = P.DELIVERY_ID
            LEFT JOIN (SELECT TRANS_REF FROM {dbCode}SISOHDR WHERE VOID_STATUS = 'N') H ON H.TRANS_REF = N.TRANSACTION_REF  
            WHERE
            PAID.STATUS = '1'
            AND P.CREATE_DATE BETWEEN @FROM_DATE AND @TO_DATE AND P.DB_CODE = @DB_CODE
            AND N.ID NOT IN (SELECT INVOICE_ID FROM BCINVOICE_SUMITTED S WHERE S.SUBMISSION_STATUS != 'Cancel'  AND S.DB_CODE = @DB_CODE)
			AND LEFT(N.TRANSACTION_REF,2) = 'FF') A UNION
			(
			SELECT DISTINCT N.ID,D.DELIVERIES_KHMER,N.CUSTOMER_CODE,
            N.ACC_NAME_KH, N.TRANSACTION_REF, 1.00 AS AMOUNT, N.HEADER_TRANSACTION_VALUES,
            ISNULL(CASE WHEN N.HEADER_TRANSACTION_VALUES = PAID.AMOUNT THEN PAID.AMOUNT END, 0) 'PAID',
            ISNULL(CASE WHEN N.HEADER_TRANSACTION_VALUES > PAID.AMOUNT THEN PAID.AMOUNT END, 0) 'HALF_PAID',

            N.HEADER_TRANSACTION_VALUES - PAID.AMOUNT TOTAL,
            CONVERT(BIT,CASE WHEN N.HEADER_TRANSACTION_VALUES = PAID.AMOUNT THEN 1 ELSE 0 END) 'STATUS',
            N.STATUS [InvoiceType],P.CREATE_DATE
			,H.TRANS_REF
            FROM
            PC_DIVIDED_INVOICE P
            INNER JOIN NEW_INVOICE N ON N.ID = P.INVOICE_ID
            INNER JOIN PC_PAYMENT_INVOICE PAID ON PAID.DIVDIE_INVOICE_ID = P.DIVIDED_INVOICE_ID
            INNER JOIN TB_BCDELIVERIES D ON D.DELIVERIES_ID = P.DELIVERY_ID
            LEFT JOIN (SELECT TRANS_REF FROM {dbCode}SISOHDR WHERE VOID_STATUS = 'N') H ON H.TRANS_REF = N.TRANSACTION_REF  
            WHERE
            PAID.STATUS = '1'
            AND P.CREATE_DATE BETWEEN @FROM_DATE AND @TO_DATE AND P.DB_CODE = @DB_CODE
            AND N.ID NOT IN (SELECT INVOICE_ID FROM BCINVOICE_SUMITTED S WHERE S.SUBMISSION_STATUS != 'Cancel'  AND S.DB_CODE = @DB_CODE)
			AND H.TRANS_REF IS NOT NULL ) ;";
            var param = new
            {
                DB_CODE = dbCode,
                FROM_DATE = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture),
                TO_DATE = DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture)
            };
            var execute = await  _sqlDataAccess.LoadData<SubmittedInvoiceModel, dynamic>(sql, param);
            return execute.ToList();                     
        }

        public async Task<int> AddSubmittedInvoices(List<SubmittedInvoiceModel> submittedInvoices)
        {
            const string sql =
                @"INSERT INTO BCINVOICE_SUMITTED(DB_CODE,CUSTOMER_CODE,TRANSACTION_CODE,INVOICE_ID,MONEY,PAID,SUBMITTED_BY,SUBMITTED_DATE,STATUS,SUBMISSION_STATUS)
                VALUES (@DB_CODE,@CUSTOMER_CODE,@TRANSACTION,@INVOICE_ID,@MONEY,@PAID,@SUBMITTED_BY,@SUBMITTED_DATE,@STATUS,@SUBMISSION_STATUS)";
            if (_dbConnection.State == ConnectionState.Closed) _dbConnection.Open();
            using var transaction = _dbConnection.BeginTransaction();
            var rowAffected = 0;
            foreach (var invoice in submittedInvoices)
            {
                var param = new
                {
                    DB_CODE = invoice.DbCode,
                    CUSTOMER_CODE = invoice.CustomerCode,
                    TRANSACTION = invoice.TransactionCode,
                    INVOICE_ID = invoice.InvoiceId,
                    MONEY = invoice.InvoiceAmount,
                    PAID = invoice.Paid,
                    SUBMITTED_BY = invoice.SubmittedBy,
                    SUBMITTED_DATE = invoice.SubmittedDate,
                    STATUS = "1",
                    SUBMISSION_STATUS = "Pending"
                };
                rowAffected += await _dbConnection.ExecuteAsync(sql, param, transaction);
            }

            if (rowAffected == submittedInvoices.Count)
            {
                transaction.Commit();
                return rowAffected;
            }
            transaction.Rollback();
            return 0;
        }

        public Task<int> AddSubmittedInvoices(SubmittedInvoiceModel submittedInvoice)
        {
            throw new NotImplementedException();
        }
    }
}
