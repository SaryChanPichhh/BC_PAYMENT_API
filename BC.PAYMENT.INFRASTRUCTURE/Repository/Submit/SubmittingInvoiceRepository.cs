using BC.PAYMENT.APPLICATION.Interfaces.Submit;
using BC.PAYMENT.CORE.Contracts.Response.Paid;
using BC.PAYMENT.CORE.Contracts.Response.SubmitInvoice;
using BC.PAYMENT.CORE.Entities;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Submit
{
    public class SubmittingInvoiceRepository(ISqlDataAccess sqlDataAccess, IDbConnection dbConnection) : ISubmittingInvoiceRepository
    {
        public async Task<List<PaymentInvoiceResponse>> GetAllNotSubmitPaidInvoice(string dbCode, string fromDate, string toDate)
        {
            var sql = $@"SELECT A.InvoiceId ,A.DeliveryName ,A.CustomerCode,A.CustomerName,A.TransactionCode
      ,A.AMOUNT Amount,A.InvoiceAmount,A.Paid,A.HalfPaid,A.Total,A.Status,A.InvoiceType,A.CreateDate 
      FROM (SELECT DISTINCT N.ID InvoiceId,D.DELIVERIES_KHMER DeliveryName,N.CUSTOMER_CODE CustomerCode,
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
			AND LEFT(N.TRANSACTION_REF,2) = 'FF') A 
            UNION
			(
			SELECT DISTINCT N.ID,D.DELIVERIES_KHMER,N.CUSTOMER_CODE,
            N.ACC_NAME_KH, N.TRANSACTION_REF, 1.00 AS AMOUNT, N.HEADER_TRANSACTION_VALUES,
            ISNULL(CASE WHEN N.HEADER_TRANSACTION_VALUES = PAID.AMOUNT THEN PAID.AMOUNT END, 0) 'Paid',
            ISNULL(CASE WHEN N.HEADER_TRANSACTION_VALUES > PAID.AMOUNT THEN PAID.AMOUNT END, 0) 'HalfPaid',

            N.HEADER_TRANSACTION_VALUES - PAID.AMOUNT TOTAL,
            CONVERT(BIT,CASE WHEN N.HEADER_TRANSACTION_VALUES = PAID.AMOUNT THEN 1 ELSE 0 END) 'Status',
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
            var execute = await  sqlDataAccess.LoadData<PaymentInvoiceResponse, dynamic>(sql, param);
            return execute.ToList();                     
        }

        public async Task<int> AddSubmittedInvoices(List<BcInvoiceSubmitted> submittedInvoices)
        {
            const string sql =
                @"INSERT INTO BCINVOICE_SUMITTED(DB_CODE,CUSTOMER_CODE,TRANSACTION_CODE,INVOICE_ID,MONEY,PAID,SUBMITTED_BY,SUBMITTED_DATE,STATUS,SUBMISSION_STATUS)
                VALUES (@DB_CODE,@CUSTOMER_CODE,@TRANSACTION,@INVOICE_ID,@MONEY,@PAID,@SUBMITTED_BY,@SUBMITTED_DATE,@STATUS,@SUBMISSION_STATUS)";
            if (dbConnection.State == ConnectionState.Closed) dbConnection.Open();
            using var transaction = dbConnection.BeginTransaction();
            var rowAffected = 0;
            foreach (var invoice in submittedInvoices)
            {
                var param = new
                {
                    DB_CODE = invoice.DbCode,
                    CUSTOMER_CODE = invoice.CustomerCode,
                    TRANSACTION = invoice.TransactionCode,
                    INVOICE_ID = invoice.InvoiceId,
                    MONEY = invoice.Money,
                    PAID = invoice.Paid,
                    SUBMITTED_BY = invoice.SubmittedBy,
                    SUBMITTED_DATE = invoice.SubmittedDate,
                    STATUS = "1",
                    SUBMISSION_STATUS = "Pending"
                };
                rowAffected += await dbConnection.ExecuteAsync(sql, param, transaction);
            }

            if (rowAffected == submittedInvoices.Count)
            {
                transaction.Commit();
                return rowAffected;
            }
            transaction.Rollback();
            return 0;
        }

        public async Task<int> CancelingSubmittedInvoiceAsync(string dbCode, string createBy, string submittedId)
        {
            var sql = $@"UPDATE BCINVOICE_SUMITTED SET SUBMISSION_STATUS = 'Cancel',UPDATED_BY = @UPDATED_BY,UPDATED_DATE = @UPDATED_DATE WHERE ID = @ID AND DB_CODE = @DB_CODE";
            var param = new
            {
                ID = submittedId,
                DB_CODE = dbCode,
                UPDATED_BY = createBy,
                UPDATED_DATE = DateTime.Now
            };
            var affectedRow = await sqlDataAccess.ExecuteAsync(sql, param);
            return affectedRow;
        }
        public async Task<List<SubmitInvoiceResponse>> GetSubmittedInvoiceByDateAsync(string dbCode, string fromDate, string toDate)
        {
            var sql = @"SELECT S.ID Id,DE.DELIVERIES_KHMER DeliveryName,N.CUSTOMER_CODE CustomerCode,ACC_NAME_KH CustomerName
                     ,N.TRANSACTION_REF InvoiceCode,1.00 AS 'Amount',S.MONEY Money,PAID Paid,HEADER_TRANSACTION_VALUES - PAID Total
                     ,SUBMITTED_DATE SubmittedDate,SUBMITTED_BY SubmittedBy,N'កំពុងដំណើរការ' StatusDesc,
                     CONVERT(bit,CASE WHEN S.MONEY = PAID THEN 1 ELSE 0 END) IsFullPaid
                     FROM NEW_INVOICE N INNER JOIN PC_DIVIDED_INVOICE D ON D.INVOICE_ID = N.ID INNER JOIN BCINVOICE_SUMITTED S ON S.INVOICE_ID = N.ID
                     INNER JOIN TB_BCDELIVERIES DE ON DE.DELIVERIES_ID = D.DELIVERY_ID
                     WHERE D.CREATE_DATE BETWEEN @FROM_DATE AND @TO_DATE AND N.DB_CODE = @DB_CODE AND D.DB_CODE = @DB_CODE AND S.DB_CODE = @DB_CODE
                     -- AND DE.DB_CODE = @DB_CODE
                     AND S.STATUS = '1' AND S.SUBMISSION_STATUS != 'Cancel'
                     GROUP BY N.ID,S.ID,DE.DELIVERIES_KHMER,N.CUSTOMER_CODE,ACC_NAME_KH,N.TRANSACTION_REF,S.MONEY,PAID,HEADER_TRANSACTION_VALUES - PAID,SUBMITTED_DATE,SUBMITTED_BY";
            var param = new
            {
                DB_CODE = dbCode,
                FROM_DATE = fromDate,
                TO_DATE = toDate
            };
            var execute = await sqlDataAccess.LoadData<SubmitInvoiceResponse, dynamic>(sql, param);
            return execute.ToList();
        }

        public async Task<List<SubmitInvoiceResponse>> GetSubmittedPendingInvoiceByDateAsync(string dbCode, string fromDate, string toDate)
        {
          
            var data = await sqlDataAccess.LoadData<SubmitInvoiceResponse, dynamic>(SubmitInvoiceQueries.GetPendingSubmittedInvoice, new
            {
                DB_CODE = dbCode,
                FROM_DATE = fromDate,
                TO_DATE = toDate
            });
            return data.ToList();
        }

        public async Task<List<ApproveSubmitInvoiceResponse>> GetApprovedSubmittedInvoiceByDateAsync(string dbCode, string fromDate, string toDate)
        { 
            var param = new
            {
                DB_CODE = dbCode,
                FROM_DATE = fromDate,
                TO_DATE = toDate,
            };
            var criteria = $@"AND D.CREATE_DATE BETWEEN @FROM_DATE AND @TO_DATE";
            var execute = await sqlDataAccess.LoadData<ApproveSubmitInvoiceResponse,dynamic>
                (SubmitInvoiceQueries.GetApprovedSubmittedInvoice(criteria),param);
            return execute.ToList();    
        }

        public async Task<List<ApproveSubmitInvoiceResponse>> GetApprovedSubmittedInvoicePeriodAsync(string dbCode, int year, int month)
        {
            var arguments = new
            {
                DB_CODE = dbCode, 
                YEAR = year,
                MONTH = month,
            };
            var criteria = $@"AND YEAR(D.CREATE_DATE) = @YEAR AND MONTH(D.CREATE_DATE) = @MONTH";
            var execute = await sqlDataAccess.LoadData<ApproveSubmitInvoiceResponse,dynamic>
                (SubmitInvoiceQueries.GetApprovedSubmittedInvoice(criteria),arguments);
            return execute.ToList();
        }

        public async Task<List<ApproveSubmitInvoiceResponse>> GetApprovedSubmittedInvoiceByInvoiceCodeAndDateAsync(string dbCode, DateTime date, string invoiceCode)
        {
            var arguments = new
            {
                DB_CODE = dbCode, 
                DATE = date,
                INVOICE_CODE = invoiceCode,
            };
            var criteria = $@"AND YEAR(D.CREATE_DATE) = @YEAR AND MONTH(D.CREATE_DATE) = @MONTH";
            var addReference = $@"INNER JOIN PC_PAYMENT_INVOICE PPI ON PPI.DIVDIE_INVOICE_ID = PDI.DIVIDED_INVOICE_ID and PPI.DB_CODE = @DB_CODE";
            var execute = await sqlDataAccess.LoadData<ApproveSubmitInvoiceResponse,dynamic>
                (SubmitInvoiceQueries.GetApprovedSubmittedInvoice(criteria),arguments);
            return execute.ToList();
        }

        public async Task<bool> UpdateStatusBcInvoiceSubmittedAsync(string submittedInvoiceId, string status)
        {
            var sql = $@"UPDATE BCINVOICE_SUMITTED SET STATUS = 0,SUBMISSION_STATUS = @STATUS WHERE ID = @SUBMITTED_ID";
            var arguments = new
            {   
                SUBMITTED_ID =  submittedInvoiceId,
                STATUS = status
            };
            var execute = await sqlDataAccess.LoadSingleData<bool,dynamic>(sql, arguments);
            return execute;
        }

        public async Task<int> AddNewApprovalInvoice(BcApprovalInvoice request)
        {
            var sql = $@"INSERT INTO BCAPPROVAL_INVOICE (BI.SUBMITTED_ID,DB_CODE, APPROVAL_STATUS, DESCRIPTION, APPROVAL_BY, APPROVAL_DATE)
                        VALUES (@SUBMITTED_ID, @DB_CODE, @APPROVAL_STATUS, @DESCRIPTION, @APPROVAL_BY, GETDATE());";
            var arguments = new
            {
                SUBMITTED_ID = request.SubmittedId,
                DB_CODE = request.DbCode,
                APPROVAL_STATUS = request.ApprovalStatus,
                APPROVAL_BY = request.ApprovalBy,
                DESCRIPTION = request.Description,
            };
            var execute = await sqlDataAccess.ExecuteAsync(sql, arguments);
            return execute;
        }
    }
}
