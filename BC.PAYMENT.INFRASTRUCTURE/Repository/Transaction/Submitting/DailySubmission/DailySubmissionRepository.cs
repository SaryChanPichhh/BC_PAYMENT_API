

using BC.PAYMENT.APPLICATION.Interfaces.Transaction.Submitting.DailySubmission;
using BC.PAYMENT.CORE.Entities.Transaction.Submitting.DailySubmission;
using BC.PAYMENT.INFRASTRUCTURE.DBAccess;
using Microsoft.Data.SqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Transaction.Submitting.DailySubmission
{
    public class DailySubmissionRepository : IDailySubmissionRepository
    {
        private readonly ISqlDataAccess _sqlDataAccess;
        public DailySubmissionRepository(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<List<DailySubmissionModel>> GetSubmittedInvoiceByDateAsync(string dbCode, string fromDate, string toDate, int page, int pageSize)
        {
            int offSet = (page - 1) * pageSize;
            const string sql = @"   
            SELECT S.ID dividedInvoiceId,
            DE.DELIVERIES_KHMER Delivery,
            CUS.*,
            N.TRANSACTION_REF TransactionCode,
            S.MONEY InvoiceValue,S.PAID PaidAmount,
            S.MONEY - S.PAID Total,
            D.CREATE_DATE CreatedDate,
            S.STATUS Status
            FROM BCINVOICE_SUMITTED S
            INNER JOIN NEW_INVOICE N
            ON N.ID = S.INVOICE_ID
            INNER JOIN PC_DIVIDED_INVOICE D ON D.INVOICE_ID = N.ID
            INNER JOIN TB_BCDELIVERIES DE ON DE.DELIVERIES_ID = D.DELIVERY_ID
            LEFT JOIN GET_CUSTOMERS_BY_DB_CODE() CUS ON CUS.CustomerCode = N.CUSTOMER_CODE
            WHERE S.DB_CODE = @DB_CODE
            AND N.DB_CODE = @DB_CODE
            AND D.DB_CODE = @DB_CODE
            AND DE.DB_CODE = @DB_CODE
            AND S.SUBMISSION_STATUS != 'Cancel'
            AND D.CREATE_DATE BETWEEN @FROM_DATE
            AND @TO_DATE
            AND S.ID NOT IN(SELECT SUBMITTED_ID FROM BCAPPROVAL_INVOICE WHERE DB_CODE = @DB_CODE) ORDER BY D.CREATE_DATE
            OFFSET @OFFSET ROWS FETCH NEXT @PAGE_SIZE ROWS ONLY";
            var parameter = new
            {
                DB_CODE = dbCode,
                FROM_DATE = fromDate,
                TO_DATE = toDate,
                OFFSET = offSet,
                PAGE_SIZE = pageSize,
            };
            var execute = await _sqlDataAccess.LoadData<DailySubmissionModel, dynamic>(sql, parameter);
            return execute.ToList();
        }

        public async Task<List<DailySubmissionModel>> GetSubmittedInvoiceByPeriodAsync(string dbCode, int month, int year, int page, int pageSize)
        {
            int offSet = (page - 1) * pageSize;
            const string sql = @"
            SELECT S.ID dividedInvoiceId,
            DE.DELIVERIES_KHMER Delivery,
            CUS.*,
            N.TRANSACTION_REF TransactionCode,
            S.MONEY InvoiceValue,S.PAID PaidAmount,
            S.MONEY - S.PAID Total,
            D.CREATE_DATE CreatedDate,
            S.STATUS Status
            FROM BCINVOICE_SUMITTED S
            INNER JOIN NEW_INVOICE N
            ON N.ID = S.INVOICE_ID
            INNER JOIN PC_DIVIDED_INVOICE D ON D.INVOICE_ID = N.ID
            INNER JOIN TB_BCDELIVERIES DE ON DE.DELIVERIES_ID = D.DELIVERY_ID
            LEFT JOIN GET_CUSTOMERS_BY_DB_CODE() CUS ON CUS.CustomerCode = N.CUSTOMER_CODE
            WHERE S.DB_CODE = @DB_CODE
            AND N.DB_CODE = @DB_CODE
            AND D.DB_CODE = @DB_CODE
            AND DE.DB_CODE = @DB_CODE
            AND S.SUBMISSION_STATUS != 'Cancel'
            AND YEAR(D.CREATE_DATE) = @YEAR AND MONTH(D.CREATE_DATE) = @MONTH
            AND S.ID NOT IN(SELECT SUBMITTED_ID FROM BCAPPROVAL_INVOICE WHERE DB_CODE = @DB_CODE) ORDER BY D.CREATE_DATE

             OFFSET @OFFSET ROWS FETCH NEXT @PAGE_SIZE ROWS ONLY";
            var parameter = new
            {
                DB_CODE = dbCode,
                YEAR = year,
                MONTH = month,
                OFFSET = offSet,
                PAGE_SIZE = pageSize,
            };
            var execute = await _sqlDataAccess.LoadData<DailySubmissionModel, dynamic>(sql, parameter);
            return execute.ToList();
        }

        public async Task<List<ApprovalInvoiceModel>> GetApprovalListByDateAsync(string dbCode, string fromDate, string toDate)
        {
            const string sql = @"SELECT S.ID Id,DE.DELIVERIES_KHMER Delivery,
                N.CUSTOMER_CODE CustomerCode,
                CUSTOMER.CUSTOMER_NAME CustomerName,
                N.TRANSACTION_REF TransactionCode,
                MARKET Market,AREA Area
                ,S.MONEY InvoiceValue,S.PAID Paid,
                D.CREATE_DATE CreatedDate,
                A.APPROVAL_STATUS Status,
                A.DESCRIPTION Description,
                A.APPROVAL_DATE ApprovalDate
                FROM BCINVOICE_SUMITTED S
                INNER JOIN NEW_INVOICE N ON N.ID = S.INVOICE_ID
                INNER JOIN PC_DIVIDED_INVOICE D ON D.INVOICE_ID = N.ID
                INNER JOIN TB_BCDELIVERIES DE ON DE.DELIVERIES_ID = D.DELIVERY_ID
                INNER JOIN BCAPPROVAL_INVOICE A ON A.SUBMITTED_ID = S.ID
                LEFT JOIN (SELECT ADD_CODE CUSTOMER,ADD_LINE_1KH CUSTOMER_NAME,
                AREA_NAME_KHMER AREA,MARKET_KHMER_NAME MARKET 
                FROM SIADD INNER JOIN TB_BCMARKET M ON M.MARKET_ID = SIADD.MARKET_ID
                INNER JOIN TB_AREAS A ON A.AREA_ID = M.AREA_ID 
                WHERE SIADD.DB_CODE = @DB_CODE
                AND M.DB_CODE  = @DB_CODE AND A.DB_CODE = @DB_CODE) 
                CUSTOMER ON CUSTOMER.CUSTOMER = N.CUSTOMER_CODE
                WHERE
                S.DB_CODE = @DB_CODE AND N.DB_CODE = @DB_CODE AND D.DB_CODE = @DB_CODE
                AND DE.DB_CODE = @DB_CODE
                AND D.CREATE_DATE BETWEEN @FROM_DATE AND @TO_DATE";
            var param = new
            {
                DB_CODE = dbCode,
                FROM_DATE = fromDate,
                TO_DATE = toDate,
            };
            var execute = await _sqlDataAccess.LoadData<ApprovalInvoiceModel,dynamic>(sql,param);
            return execute.ToList();
        }

        public async Task<List<ApprovalInvoiceModel>> GetApprovalListByPeriodAsync(string dbCode, int month, int year)
        {
            const string sql = @"SELECT S.ID Id,DE.DELIVERIES_KHMER Delivery,
                N.CUSTOMER_CODE CustomerCode,
                CUSTOMER.CUSTOMER_NAME CustomerName,
                N.TRANSACTION_REF TransactionCode,
                MARKET Market,AREA Area
                ,S.MONEY InvoiceValue,S.PAID Paid,
                D.CREATE_DATE CreatedDate,
                A.APPROVAL_STATUS Status,
                A.DESCRIPTION Description,
                A.APPROVAL_DATE ApprovalDate
                FROM BCINVOICE_SUMITTED S
                INNER JOIN NEW_INVOICE N ON N.ID = S.INVOICE_ID
                INNER JOIN PC_DIVIDED_INVOICE D ON D.INVOICE_ID = N.ID
                INNER JOIN TB_BCDELIVERIES DE ON DE.DELIVERIES_ID = D.DELIVERY_ID
                INNER JOIN BCAPPROVAL_INVOICE A ON A.SUBMITTED_ID = S.ID
                LEFT JOIN (SELECT ADD_CODE CUSTOMER,ADD_LINE_1KH CUSTOMER_NAME,
                AREA_NAME_KHMER AREA,MARKET_KHMER_NAME MARKET 
                FROM SIADD INNER JOIN TB_BCMARKET M ON M.MARKET_ID = SIADD.MARKET_ID
                INNER JOIN TB_AREAS A ON A.AREA_ID = M.AREA_ID 
                WHERE SIADD.DB_CODE = @DB_CODE
                AND M.DB_CODE  = @DB_CODE AND A.DB_CODE = @DB_CODE) 
                CUSTOMER ON CUSTOMER.CUSTOMER = N.CUSTOMER_CODE
                WHERE
                S.DB_CODE = @DB_CODE AND N.DB_CODE = @DB_CODE AND D.DB_CODE = @DB_CODE
                AND DE.DB_CODE = @DB_CODE
                AND YEAR(D.CREATE_DATE) = @YEAR AND MONTH(D.CREATE_DATE) = @MONTH";
            var param = new
            {
                DB_CODE = dbCode,
                YEAR = year,
                MONTH = month,
            };
            var execute = await _sqlDataAccess.LoadData<ApprovalInvoiceModel, dynamic>(sql, param);
            return execute.ToList();
        }

        public async Task<int> FindSubmittedInvoiceByTransactionCodeAsync(string dbCode, string transactionCoed)
        {
            const string sql =
                @"SELECT S.ID FROM BCINVOICE_SUMITTED S
                INNER JOIN NEW_INVOICE N ON N.ID = S.INVOICE_ID
                INNER JOIN PC_DIVIDED_INVOICE D ON D.INVOICE_ID = N.ID
                INNER JOIN TB_BCDELIVERIES DE ON DE.DELIVERIES_ID = D.DELIVERY_ID
                WHERE
                S.DB_CODE = @DB_CODE
                AND N.DB_CODE = @DB_CODE
                AND D.DB_CODE = @DB_CODE
                AND DE.DB_CODE = @DB_CODE
				AND S.ID NOT IN(SELECT SUBMITTED_ID FROM BCAPPROVAL_INVOICE WHERE DB_CODE = @DB_CODE)
                AND N.TRANSACTION_REF = @TRANSACTION AND S.SUBMISSION_STATUS = 'Pending' ";

            var param = new
            {
                DB_CODE = dbCode,
                TRANSACTION = transactionCoed
            };
            var result =await  _sqlDataAccess.LoadSingleData<int, dynamic>(sql, param);
            return result;
        }
        public async Task<int> UpdateStatusSubmittedInvoiceBySubmittedInvoiceId(int submittedId, string createBy)
        {
            const string sql =
                "UPDATE BCINVOICE_SUMITTED SET STATUS = 0,UPDATED_DATE = @UPDATED_DATE,UPDATED_BY = @UPDATED_BY,SUBMISSION_STATUS = 'Processing' WHERE ID = @submittedId";
            var param = new
            {
                submittedId = submittedId,
                UPDATED_DATE = DateTime.Now,
                UPDATED_BY = createBy
            };
            var affectedRow = await _sqlDataAccess.ExecuteAsync(sql, param);
            return affectedRow;
        }

        public async Task<List<HistoryPaymentModel>> FindHistoryPaymentByTransactionCode(string dbCode, string transactionCode)
        {
            const string sql = @"SELECT * FROM(
            SELECT TRANSACTION_REF, 
                        HEADER_TRANSACTION_VALUES, AMOUNT, HEADER_TRANSACTION_VALUES - AMOUNT TOTAL, PPI.CREATED_DATE,'Approved' [STATUS]
                        FROM NEW_INVOICE NI
                        INNER JOIN PC_DIVIDED_INVOICE PDI on NI.ID = PDI.INVOICE_ID
                        INNER JOIN PC_PAYMENT_INVOICE PPI ON PPI.DIVDIE_INVOICE_ID = PDI.DIVIDED_INVOICE_ID
			            INNER JOIN BCINVOICE_SUMITTED S ON S.INVOICE_ID = NI.ID
			            INNER JOIN BCAPPROVAL_INVOICE A ON A.SUBMITTED_ID = S.ID
                        WHERE  NI.DB_CODE = @DB_CODE and PDI.DB_CODE = @DB_CODE and PPI.DB_CODE = @DB_CODE
            UNION 
            SELECT TRANSACTION_REF,HEADER_TRANSACTION_VALUES,AMOUNT,TOTAL,CREATED_DATE,[STATUS] FROM (SELECT ROW_NUMBER() OVER(PARTITION BY NI.TRANSACTION_REF ORDER BY PDI.CREATE_DATE ASC) ROWNUM, TRANSACTION_REF, HEADER_TRANSACTION_VALUES, AMOUNT, HEADER_TRANSACTION_VALUES - AMOUNT TOTAL, PPI.CREATED_DATE,'Submitted' [STATUS]
                        FROM NEW_INVOICE NI
                        INNER JOIN PC_DIVIDED_INVOICE PDI on NI.ID = PDI.INVOICE_ID
                        INNER JOIN PC_PAYMENT_INVOICE PPI ON PPI.DIVDIE_INVOICE_ID = PDI.DIVIDED_INVOICE_ID
			            INNER JOIN BCINVOICE_SUMITTED S ON S.INVOICE_ID = NI.ID
                        WHERE  NI.DB_CODE = @DB_CODE and PDI.DB_CODE = @DB_CODE and PPI.DB_CODE = @DB_CODE
			            AND S.ID NOT IN(SELECT SUBMITTED_ID FROM BCAPPROVAL_INVOICE WHERE DB_CODE = @DB_CODE)
			            )TAB WHERE TAB.ROWNUM = 1 AND TAB.TRANSACTION_REF = @TRANSACTION) TAB
			            WHERE TAB.TRANSACTION_REF = @TRANSACTION
			            ORDER BY TAB.TRANSACTION_REF";
            var param = new
            {
                TRANSACTION = transactionCode,
                DB_CODE = dbCode
            };
            var results = await _sqlDataAccess.LoadData<HistoryPaymentModel,dynamic>(sql, param);
            return results.ToList();
        }

        public async Task<List<string>> GetApprovedInvoiceAsync(string dbCode)
        {
            const string sql = @"SELECT DISTINCT D.CREATE_DATE CreatedDate
                FROM BCINVOICE_SUMITTED S
                INNER JOIN NEW_INVOICE N ON N.ID = S.INVOICE_ID
                INNER JOIN PC_DIVIDED_INVOICE D ON D.INVOICE_ID = N.ID
                INNER JOIN TB_BCDELIVERIES DE ON DE.DELIVERIES_ID = D.DELIVERY_ID
                INNER JOIN BCAPPROVAL_INVOICE A ON A.SUBMITTED_ID = S.ID
                LEFT JOIN (SELECT ADD_CODE CUSTOMER,ADD_LINE_1KH CUSTOMER_NAME,
                AREA_NAME_KHMER AREA,MARKET_KHMER_NAME MARKET 
                FROM SIADD INNER JOIN TB_BCMARKET M ON M.MARKET_ID = SIADD.MARKET_ID
                INNER JOIN TB_AREAS A ON A.AREA_ID = M.AREA_ID 
                WHERE SIADD.DB_CODE = @DB_CODE
                AND M.DB_CODE  = @DB_CODE AND A.DB_CODE = @DB_CODE) 
                CUSTOMER ON CUSTOMER.CUSTOMER = N.CUSTOMER_CODE
                WHERE
                S.DB_CODE = @DB_CODE AND N.DB_CODE = @DB_CODE AND D.DB_CODE = @DB_CODE
                AND DE.DB_CODE = @DB_CODE AND S.SUBMISSION_STATUS = 'Approved' ORDER BY D.CREATE_DATE";
            var param = new
            {
                DB_CODE = dbCode
            };
            var execute = await _sqlDataAccess.LoadData<string, dynamic>(sql, param);
            return execute.ToList();
        }

        public async Task<List<string>> GetRejectedInvoiceAsync(string dbCode)
        {
            const string sql = @"SELECT DISTINCT D.CREATE_DATE CreatedDate
                FROM BCINVOICE_SUMITTED S
                INNER JOIN NEW_INVOICE N ON N.ID = S.INVOICE_ID
                INNER JOIN PC_DIVIDED_INVOICE D ON D.INVOICE_ID = N.ID
                INNER JOIN TB_BCDELIVERIES DE ON DE.DELIVERIES_ID = D.DELIVERY_ID
                INNER JOIN BCAPPROVAL_INVOICE A ON A.SUBMITTED_ID = S.ID
                LEFT JOIN (SELECT ADD_CODE CUSTOMER,ADD_LINE_1KH CUSTOMER_NAME,
                AREA_NAME_KHMER AREA,MARKET_KHMER_NAME MARKET 
                FROM SIADD INNER JOIN TB_BCMARKET M ON M.MARKET_ID = SIADD.MARKET_ID
                INNER JOIN TB_AREAS A ON A.AREA_ID = M.AREA_ID 
                WHERE SIADD.DB_CODE = @DB_CODE
                AND M.DB_CODE  = @DB_CODE AND A.DB_CODE = @DB_CODE) 
                CUSTOMER ON CUSTOMER.CUSTOMER = N.CUSTOMER_CODE
                WHERE
                S.DB_CODE = @DB_CODE AND N.DB_CODE = @DB_CODE AND D.DB_CODE = @DB_CODE
                AND DE.DB_CODE = @DB_CODE AND S.SUBMISSION_STATUS = 'Rejected' ORDER BY D.CREATE_DATE ";
            var param = new
            {
                DB_CODE = dbCode
            };
            var execute = await _sqlDataAccess.LoadData<string, dynamic>(sql, param);
            return execute.ToList();
        }

        public async Task<List<string>> GetReSubmitInvoiceAsync(string dbCode)
        {
            const string sql = @"SELECT DISTINCT D.CREATE_DATE CreatedDate
                FROM BCINVOICE_SUMITTED S
                INNER JOIN NEW_INVOICE N ON N.ID = S.INVOICE_ID
                INNER JOIN PC_DIVIDED_INVOICE D ON D.INVOICE_ID = N.ID
                INNER JOIN TB_BCDELIVERIES DE ON DE.DELIVERIES_ID = D.DELIVERY_ID
                INNER JOIN BCAPPROVAL_INVOICE A ON A.SUBMITTED_ID = S.ID
                LEFT JOIN (SELECT ADD_CODE CUSTOMER,ADD_LINE_1KH CUSTOMER_NAME,
                AREA_NAME_KHMER AREA,MARKET_KHMER_NAME MARKET 
                FROM SIADD INNER JOIN TB_BCMARKET M ON M.MARKET_ID = SIADD.MARKET_ID
                INNER JOIN TB_AREAS A ON A.AREA_ID = M.AREA_ID 
                WHERE SIADD.DB_CODE = @DB_CODE
                AND M.DB_CODE  = @DB_CODE AND A.DB_CODE = @DB_CODE) 
                CUSTOMER ON CUSTOMER.CUSTOMER = N.CUSTOMER_CODE
                WHERE
                S.DB_CODE = @DB_CODE AND N.DB_CODE = @DB_CODE AND D.DB_CODE = @DB_CODE
                AND DE.DB_CODE = @DB_CODE AND A.APPROVAL_STATUS = 'ReSubmit' ORDER BY D.CREATE_DATE ;";
            var param = new
            {
                DB_CODE = dbCode
            };
            var execute = await _sqlDataAccess.LoadData<string, dynamic>(sql, param);
            return execute.ToList();
        }

    }
}
