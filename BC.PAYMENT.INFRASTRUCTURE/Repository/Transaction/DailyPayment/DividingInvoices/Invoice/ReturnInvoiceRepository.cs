using BC.PAYMENT.APPLICATION.Interfaces.Transaction.DailyPayment.DividingInvoices.Invoice;
using BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DividingInvoices.Invoice;
using BC.PAYMENT.INFRASTRUCTURE.DBAccess;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Transaction.DailyPayment.DividingInvoices.Invoice
{
    public class ReturnInvoiceRepository : IReturnInvoiceRepository
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public ReturnInvoiceRepository(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<List<ReturnInvoiceModel>> GetReturnInvoiceAsync(string dbCode)
        {
            const string sql =
                @"SELECT N.ID InvoiceId,N.TRANSACTION_REF TransactionCode,N.CUSTOMER_CODE CustomerCode,S.ADD_LINE_1KH CustomerName,HEADER_TRANSACTION_VALUES InvoiceValue,M.MARKET_KHMER_NAME Market,A.AREA_NAME_KHMER Area,N.STATUS Status,S.STORE Store FROM NEW_INVOICE N
                LEFT JOIN SIADD S ON S.ADD_CODE = N.CUSTOMER_CODE
                INNER JOIN TB_BCMARKET M ON M.MARKET_ID = S.MARKET_ID
                INNER JOIN TB_AREAS A ON A.AREA_ID = S.AREA_ID
                WHERE CREATED_DATE = CONVERT(DATE,GETDATE()) AND N.CUSTOM_FIELD_1 = 'Return Invoice'
                AND S.DB_CODE = @DB_CODE AND A.DB_CODE = @DB_CODE AND M.DB_CODE=  @DB_CODE AND N.DB_CODE = @DB_CODE";

            var param = new
            {
                DB_CODE = dbCode,
            };
            var results = await _sqlDataAccess.LoadData<ReturnInvoiceModel, dynamic>(sql, param);
            return results.ToList();
        }

        public async Task<List<ReturnInvoiceModel>> GetReturnInvoiceByDateAsync(string dbCode, DateTime fromDate, DateTime toDate)
        {
            const string sql =
                @"SELECT TRANSACTION_REF TransactionCode,CUSTOMER_CODE CustomerCode,ACC_NAME_KH CustomerName,HEADER_TRANSACTION_VALUES InvoiceValue,
                S.STORE Store,M.MARKET_KHMER_NAME Market,A.AREA_NAME_KHMER Area,N.STATUS Status
                FROM NEW_INVOICE N INNER JOIN PC_DIVIDED_INVOICE P 
                ON P.INVOICE_ID = N.ID INNER JOIN PC_RETURN_INVOICE R ON R.DIVIDED_INIOVICE_ID = P.DIVIDED_INVOICE_ID
                LEFT JOIN SIADD S ON S.ADD_CODE = N.CUSTOMER_CODE
                INNER JOIN TB_BCMARKET M ON M.MARKET_ID = S.MARKET_ID
                INNER JOIN TB_AREAS A ON A.AREA_ID = S.AREA_ID
                WHERE N.DB_CODE = @DB_CODE AND P.DB_CODE = @DB_CODE AND R.DB_CODE = @DB_CODE
                AND P.CREATE_DATE BETWEEN @FROM_DATE AND @TO_DATE";
            var param = new
            {
                DB_CODE = dbCode,
                FROM_DATE = fromDate,
                TO_DATE = toDate
            };
            var results = await _sqlDataAccess.LoadData<ReturnInvoiceModel, dynamic>(sql, param);
            return results.ToList();
        }
    }
}
