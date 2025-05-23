using BC.PAYMENT.APPLICATION.Interfaces.Transaction.ProvincialPayment.CheckingStockCarApproval;
using BC.PAYMENT.CORE.Entities.Transaction.ProvincialPayment.CheckingStockCarPayment;
using BC.PAYMENT.INFRASTRUCTURE.DBAccess;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Transaction.ProvincialPayment.CheckingStockCarPayment
{
    public class CheckingStockCarPaymentRepository : ICheckingStockCarPaymentRepository
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public CheckingStockCarPaymentRepository(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<List<CheckingStockSubmittingInvoiceModel>> GetAllStockCarSubmittingInvoiceByPeriodAsync(string dbCode,
            int month, int year)
        {
            var sql =
                $@"SELECT D.SUBMIT_ID,EMPLOYEE,CUSTOMER_CODE,CUSTOMER_NAME,TRANSACTION_DATE,CODE,1.00 QTY,D.VALUES_INVOICE,CASE WHEN D.VALUES_INVOICE = D.PAID THEN D.PAID END 'FULL_PAID',CASE WHEN D.VALUES_INVOICE>D.PAID THEN D.PAID END 'HALT_PAID',D.VALUES_INVOICE - D.PAID TOTAL,CASE WHEN D.INVOICE_TYPE = 'N' THEN 'NEW' ELSE 'CHANGE' END TYPE  FROM BCSTOCK_CAR C INNER JOIN BCSTOCK_CAR_SUBMIT_PAID_INVOICE_DETAIL D ON D.INVOICE_ID = C.ID
                WHERE D.INVOICE_TYPE IN('C', 'N') AND D.DB_CODE = @DB_CODE  AND C.DB_CODE =  @DB_CODE  AND D.SUBMIT_ID NOT IN(SELECT SUBMIT_ID FROM BCSTOCK_CAR_APPROVE_INVOICE_DETAIL) AND MONTH(CLOSE_DATE) = @MONTH  AND YEAR(CLOSE_DATE) =  @YEAR 
                UNION
                SELECT D.SUBMIT_ID, EMPLOYEE, CUSTOMER_CODE, CUSTOMER_NAME, TRANSACTION_DATE, CODE, 1.00 QTY, D.VALUES_INVOICE, CASE WHEN D.VALUES_INVOICE = D.PAID THEN D.PAID END 'FULL_PAID', CASE WHEN D.VALUES_INVOICE > D.PAID THEN D.PAID END 'HALT_PAID', D.VALUES_INVOICE - D.PAID TOTAL, 'OLD' FROM BCSTOCK_CAR_OLD_INVOICES C INNER JOIN BCSTOCK_CAR_SUBMIT_PAID_INVOICE_DETAIL D ON D.INVOICE_ID = C.ID
                 WHERE D.INVOICE_TYPE IN('O') AND D.DB_CODE =  @DB_CODE  AND C.DB_CODE =  @DB_CODE  AND D.SUBMIT_ID NOT IN(SELECT SUBMIT_ID FROM BCSTOCK_CAR_APPROVE_INVOICE_DETAIL) AND MONTH(CLOSE_DATE) = @MONTH  AND YEAR(CLOSE_DATE) = @YEAR ";

            var param = new
            {
                DB_CODE = dbCode,
                MONTH = month,
                YEAR = year
            };
            var execute = await _sqlDataAccess.LoadData<CheckingStockSubmittingInvoiceModel, dynamic>(sql, param);
            return execute.ToList();
        }

        public async Task<List<CheckingStockSubmittingInvoiceModel>> GetAllStockCarSubmittingInvoiceByDateAsync(string dbCode, DateTime fromDate, DateTime toDate,bool status)
        {
            var sql = $@"SELECT 
                    D.SUBMIT_ID, 
	                T.Employee EMPLOYEE, 
                    CUSTOMER_CODE, 
                    CUSTOMER_NAME, 
                    TRANSACTION_DATE, 
                    CODE, 
                    1.00 AS QTY, 
                    D.VALUES_INVOICE, 
                    CASE 
                        WHEN D.VALUES_INVOICE = D.PAID THEN D.PAID 
                    END AS FULL_PAID, 
                    CASE 
                        WHEN D.VALUES_INVOICE > D.PAID THEN D.PAID 
                    END AS PART_PAID, 
                    D.VALUES_INVOICE - D.PAID AS TOTAL, 
                    CASE 
                        WHEN D.INVOICE_TYPE = 'N' THEN 'NEW' 
                        ELSE 'CHANGE' 
                    END AS TYPE
                FROM 
                    BCSTOCK_CAR C 
                INNER JOIN 
                    BCSTOCK_CAR_SUBMIT_PAID_INVOICE_DETAIL D ON D.INVOICE_ID = C.ID
                INNER JOIN	
	                dbo.TEMPLATE T ON T.Id = C.TEMPLATE_ID
                WHERE 
                    D.INVOICE_TYPE IN ('C', 'N') AND 
                    D.DB_CODE = @DB_CODE AND 
                    C.DB_CODE = @DB_CODE AND 
                    D.SUBMIT_ID NOT IN (SELECT SUBMIT_ID FROM BCSTOCK_CAR_APPROVE_INVOICE_DETAIL) AND 
                    T.ClosingDate BETWEEN @FROM_DATE AND @TO_DATE AND 
                    D.STATUS = @STATUS
                UNION
                SELECT 
                    D.SUBMIT_ID, 
                    EMPLOYEE, 
                    CUSTOMER_CODE, 
                    CUSTOMER_NAME, 
                    TRANSACTION_DATE, 
                    CODE, 
                    1.00 AS QTY, 
                    D.VALUES_INVOICE, 
                    CASE 
                        WHEN D.VALUES_INVOICE = D.PAID THEN D.PAID 
                    END AS FULL_PAID, 
                    CASE 
                        WHEN D.VALUES_INVOICE > D.PAID THEN D.PAID 
                    END AS PART_PAID, 
                    D.VALUES_INVOICE - D.PAID AS TOTAL, 
                    'OLD' AS TYPE
                FROM 
                    BCSTOCK_CAR_OLD_INVOICES C 
                INNER JOIN 
                    BCSTOCK_CAR_SUBMIT_PAID_INVOICE_DETAIL D ON D.INVOICE_ID = C.ID

                WHERE 
                    D.INVOICE_TYPE IN ('O') AND 
                    D.DB_CODE = @DB_CODE AND 
                    C.DB_CODE = @DB_CODE AND 
                    D.SUBMIT_ID NOT IN (SELECT SUBMIT_ID FROM BCSTOCK_CAR_APPROVE_INVOICE_DETAIL) AND 
                    C.CLOSE_DATE BETWEEN @FROM_DATE AND @TO_DATE AND 
                    D.STATUS = @STATUS";
            var param = new
            {
                DB_CODE = dbCode,
                FROM_DATE = fromDate,
                TO_DATE = toDate,
                STATUS = status
            };
            var execute =await _sqlDataAccess.LoadData<CheckingStockSubmittingInvoiceModel, dynamic>(sql, param);
            return execute.ToList();
        }
    }
}
