using BC.PAYMENT.APPLICATION.Interfaces.Transaction.DailyPayment.DividingInvoices.Invoice;
using BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DividingInvoices.Invoice;
using BC.PAYMENT.INFRASTRUCTURE.DBAccess;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Transaction.DailyPayment.DividingInvoices.Invoice
{
    public class ChangeInvoiceRepository : IChangeInvoiceRepository
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public ChangeInvoiceRepository(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<List<ChangeInvoiceModel>> GetLocalInvoiceAsync(string dbCode, DateTime fromDate, DateTime toDate)
        {
            var sql =
                @"SELECT S.ADD_CODE CustomerCode,S.ADD_LINE_1KH CustomerName,INVOICE_NUMBER [Transaction],CREATED_DATE CreatedDate,
            TOTAL_AMOUNT InvoiceValue,M.MARKET_KHMER_NAME [Market],A.AREA_NAME_KHMER [Area],STORE [Store] FROM TB_BC_CHANGEINVOICE_REPAIR_INVOICE R INNER JOIN SIADD S ON S.ADD_CODE = R.CUSTOMER_CODE
            INNER JOIN TB_BCMARKET M ON M.MARKET_ID = S.MARKET_ID INNER JOIN TB_AREAS A ON A.AREA_ID = S.AREA_ID
            WHERE R.DB_CODE_1 = R.DB_CODE AND CONVERT(DATE,R.CREATED_DATE) BETWEEN @FROM_DATE AND @TO_DATE AND R.DB_CODE = @DB_CODE
            UNION 
            SELECT S.ADD_CODE CustomerCode,S.ADD_LINE_1KH CustomerName,TRANSACTION_CODE [Transaction],R.INVOICE_DATE CreatedDate,
            SUB_TOTAL Total,M.MARKET_KHMER_NAME [Market],A.AREA_NAME_KHMER [Area],STORE [Store] FROM TB_BC_CHANGEINVOICE_EXCHANGE_INVOICE R INNER JOIN SIADD S ON S.ADD_CODE = R.CUSTOMER_CODE
            INNER JOIN TB_BCMARKET M ON M.MARKET_ID = S.MARKET_ID INNER JOIN TB_AREAS A ON A.AREA_ID = S.AREA_ID
            WHERE R.DB_CODE = @DB_CODE AND CONVERT(DATE,R.INVOICE_DATE) BETWEEN @FROM_DATE AND @TO_DATE";
            var param = new
            {
                DB_CODE = dbCode,
                FROM_DATE = fromDate,
                TO_DATE = toDate
            };
            var results = await _sqlDataAccess.LoadData<ChangeInvoiceModel, dynamic>(sql, param);
            return results.ToList();
        }

        public async Task<List<ChangeInvoiceModel>> GetOtherBranchInvoiceAsync(string dbCode, DateTime fromDate, DateTime toDate)
        {
            var sql =
                 @"SELECT S.ADD_CODE CustomerCode,S.ADD_LINE_1KH CustomerName,INVOICE_NUMBER [Transaction],CREATED_DATE CreatedDate,
            TOTAL_AMOUNT InvoiceValue,M.MARKET_KHMER_NAME [Market],A.AREA_NAME_KHMER [Area],STORE [Store] FROM TB_BC_CHANGEINVOICE_REPAIR_INVOICE R INNER JOIN SIADD S ON S.ADD_CODE = R.CUSTOMER_CODE
            INNER JOIN TB_BCMARKET M ON M.MARKET_ID = S.MARKET_ID INNER JOIN TB_AREAS A ON A.AREA_ID = S.AREA_ID
			INNER JOIN TB_BC_CHANGEINVOICE_REPAIR_SEND [SEND] ON [SEND].TRANSACTION_REF = R.INVOICE_NUMBER
            WHERE R.STATUS = 2 AND DB_CODE_1 = @DB_CODE AND [SEND].DB_CODE = @DB_CODE AND CONVERT(DATE,[SEND].SEND_DATE) BETWEEN @FROM_DATE  AND @TO_DATE  AND [SEND].STATUS = 'Received'
            UNION 
            SELECT S.ADD_CODE CustomerCode,S.ADD_LINE_1KH CustomerName,TRANSACTION_CODE [Transaction],R.INVOICE_DATE CreatedDate,
            SUB_TOTAL Total,M.MARKET_KHMER_NAME [Market],A.AREA_NAME_KHMER [Area],STORE [Store] FROM TB_BC_CHANGEINVOICE_EXCHANGE_INVOICE R INNER JOIN SIADD S ON S.ADD_CODE = R.CUSTOMER_CODE
            INNER JOIN TB_BCMARKET M ON M.MARKET_ID = S.MARKET_ID INNER JOIN TB_AREAS A ON A.AREA_ID = S.AREA_ID
            WHERE R.DB_CODE = @DB_CODE AND CONVERT(DATE,R.INVOICE_DATE) BETWEEN @FROM_DATE AND @TO_DATE";
            var param = new
            {
                DB_CODE = dbCode,
                FROM_DATE = fromDate,
                TO_DATE = toDate
            };

            var results = await _sqlDataAccess.LoadData<ChangeInvoiceModel, dynamic>(sql, param);
            return results.ToList();
        }

        public async Task<int> InsertIfNotExistsInvoiceAsync(ChangeInvoiceModel model)
        {
            // Insert new invoice
            var rowAffected = await _sqlDataAccess.ExecuteAsync(
                @"INSERT INTO NEW_INVOICE(DB_CODE,TRANSACTION_REF,CUSTOMER_CODE,ACC_NAME_KH,HEADER_TRANSACTION_VALUES,STATUS,CREATED_DATE,CREATED_BY,IS_DIVIDED,ENTRIES_CODE)
                VALUES (@DB_CODE,@TRANSACTION,@CUSTOMER_CODE,@CUSTOMER_NAME,@VALUE,@STATUS,GETDATE(),@CREATED_BY,1,@ENTRIES_CODE)",
                new
                {
                    DB_CODE = model.DbCode,
                    TRANSACTION = model.Transaction,
                    CUSTOMER_CODE = model.CustomerCode,
                    CUSTOMER_NAME = model.CustomerName,
                    VALUE = model.InvoiceValue,
                    STATUS = "C",
                    ENTRIES_CODE = model.EntriesCode,
                    CREATED_BY = model.UserName
                });
            if (rowAffected <= 0) return 0;
            return rowAffected;
        }

        public async Task<int> InsertIfExistsInvoiceAsync(ChangeInvoiceModel model)
        {
            // Check Exist Invoice By Transaction and date
            var affectedRow = 0;
            var sql = $@"SELECT COUNT(*) FROM NEW_INVOICE WHERE TRANSACTION_REF = @TRANSACTION AND DB_CODE = @DB_CODE AND CREATED_DATE = CONVERT(DATE,GETDATE())";
            var existInvoice = await _sqlDataAccess.ExecuteAsync(
                sql,
                new { DB_CODE = model.DbCode, TRANSACTION = model.Transaction });
            // If Exist confirm user override or not
            if (existInvoice > 0)
            {
                affectedRow = await _sqlDataAccess.ExecuteAsync(
                    @"UPDATE NEW_INVOICE SET CUSTOMER_CODE = @CUSTOMER_CODE,ACC_NAME_KH = 
                    @CUSTOMER_NAME,HEADER_TRANSACTION_VALUES = @VALUE,STATUS = @STATUS 
                    WHERE TRANSACTION_REF = @TRANSACTION AND DB_CODE = @DB_CODE AND CONVERT(DATE,CREATED_DATE) = CONVERT(DATE,GETDATE())",
                    new
                    {
                        DB_CODE = model.DbCode,
                        TRANSACTION = model.Transaction,
                        CUSTOMER_CODE = model.CustomerCode,
                        CUSTOMER_NAME = model.CustomerName,
                        VALUE = model.InvoiceValue,
                        STATUS = "C",
                        CREATED_BY = model.UserName
                    });
                if (affectedRow <= 0) return 0;
            }
            return affectedRow;
        }
    }
}
