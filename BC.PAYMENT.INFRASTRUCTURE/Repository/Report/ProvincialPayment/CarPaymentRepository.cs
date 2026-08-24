using static BC.PAYMENT.CORE.Entities.Report.ProvincialPayment.CarPaymentModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Report.ProvincialPayment
{
    public class CarPaymentRepository : ICarPaymentRepository
    {
        private readonly ISqlDataAccess _sqlDataAccess;
        public CarPaymentRepository(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        #region Credit Invoice
        public async Task<List<CarPaymentModel.CarPaymentCreditInvoiceModel>> ReportAllCreditInvoiceByEmployeeIdAndDate(int employeeId, DateTime dateFrom, DateTime dateTo)
        {
            var sql = $@"SELECT T.ClosingDate,CUSTOMER_CODE CustomerCode,CUSTOMER_NAME CustomerName,CODE TransactionRef,VALUE Value,[dbo].[RETURN_TYPE_INVOICE](TYPE) Invoice,
                CASE WHEN STATUS = '1' THEN N'ឥណទាន' END 'Credit',
                CASE WHEN STATUS = '0' THEN N'វិក័យប័ត្រត្រឡប់' END 'Return',
                BCR.Description
                FROM 
                BCSTOCK_CAR BC INNER JOIN TEMPLATE T 
                ON T.Id = BC.TEMPLATE_ID
                LEFT JOIN BCSTOCK_CAR_INVOICE_RETURN BCR ON BCR.INVOICE_ID = BC.ID
                WHERE BC.ID NOT IN (SELECT INVOICE_ID FROM BCSTOCK_CAR_INVOICE_PAYMENT) AND
                (@EMPLOYEE = '' OR @EMPLOYEE IS NULL OR Employee = @EMPLOYEE ) AND
				CAST(ClosingDate AS DATE) BETWEEN @DATEFROM AND @DATETO";
            var param = new
            {
                EMPLOYEE = employeeId,
                DATEFROM = dateFrom,
                DATETO = dateTo,
            };
            var execute = await _sqlDataAccess.LoadData<CarPaymentModel.CarPaymentCreditInvoiceModel, dynamic>(sql, param);
            return execute.ToList();
        }

        public async Task<List<CarPaymentModel.CarPaymentCreditInvoiceModel>> ReportAllCreditInvoiceByEmployeeIdAndTemplateId(int employeeId, int templateId)
        {
             string sql =
                @"SELECT T.ClosingDate,CUSTOMER_CODE,CUSTOMER_NAME,CODE,VALUE,[dbo].[RETURN_TYPE_INVOICE](TYPE) INVOICE,
                CASE WHEN STATUS = '1' THEN N'ឥណទាន' END 'CREDIT',
                CASE WHEN STATUS = '0' THEN N'វិក័យប័ត្រត្រឡប់' END 'RETURN',
                BCR.DESCRIPTION
                FROM 
                BCSTOCK_CAR BC INNER JOIN TEMPLATE T 
                ON T.Id = BC.TEMPLATE_ID
                LEFT JOIN BCSTOCK_CAR_INVOICE_RETURN BCR ON BCR.INVOICE_ID = BC.ID
                WHERE BC.ID NOT IN (SELECT INVOICE_ID FROM BCSTOCK_CAR_INVOICE_PAYMENT) AND
                (@EMPLOYEE = '' OR @EMPLOYEE IS NULL OR Employee = @EMPLOYEE )
				AND
				(@TEMPLATE_ID = '' OR @TEMPLATE_ID IS NULL OR TEMPLATE_ID = @TEMPLATE_ID)";

            var param = new
            {
                EMPLOYEE = employeeId,
                TEMPLATE_ID = templateId,
            };
            var execute = await _sqlDataAccess.LoadData<CarPaymentModel.CarPaymentCreditInvoiceModel, dynamic>(sql,param);
            return execute.ToList();
        }



        #endregion

        #region Car Payment Invoice
        public async Task<List<CarPaymentInvoiceModel>> ReportAllPaymentInvoiceByEmployeeIdAndTemplateId(string dbCode, int employeeId, int templateId)
        {
            string sql =
                @"SELECT
                    T.CreatedDate,
	                C.AreaNameKhmer Area,
	                C.MarketNameKhmer Market,
                    CUSTOMER_CODE CustomerCode,
                    CUSTOMER_NAME CustomerName,
                    CODE TransactionRef,
                    VALUE Value,
                    CASE WHEN BCIP.AMOUNT < BC.VALUE THEN BCIP.AMOUNT END AS 'HalfPayment',
                    CASE WHEN BCIP.AMOUNT = BC.VALUE THEN BCIP.AMOUNT END AS 'FullPayment',
                    BC.VALUE - BCIP.AMOUNT AS Total,
                    [dbo].[RETURN_TYPE_INVOICE](TYPE) AS Invoice
                FROM
                    BCSTOCK_CAR BC
                INNER JOIN
                    (SELECT SUM(AMOUNT) AMOUNT, INVOICE_ID FROM BCSTOCK_CAR_INVOICE_PAYMENT GROUP BY INVOICE_ID) BCIP
                ON
                    BCIP.INVOICE_ID = BC.ID
                INNER JOIN
                    TEMPLATE T
                ON
                    T.Id = BC.TEMPLATE_ID
                LEFT JOIN 
	                dbo.FN_GETCUSTOMERS(@DB_CODE) C
                ON
	                C.CustomerCode = BC.CUSTOMER_CODE
                WHERE
                    (@EMPLOYEE = '' OR @EMPLOYEE IS NULL OR Employee = @EMPLOYEE)
                AND
				    (@TEMPLATE_ID = '' OR @TEMPLATE_ID IS NULL OR TEMPLATE_ID = @TEMPLATE_ID)";
            var param = new
            {
                DB_CODE = dbCode,
                EMPLOYEE = employeeId,
                TEMPLATE_ID = templateId,
            };
            var execute = await _sqlDataAccess.LoadData<CarPaymentInvoiceModel, dynamic>(sql, param);
            return execute.ToList();
        }
        public async Task<List<CarPaymentInvoiceModel>> ReportAllPaymentInvoiceByEmployeeIdAndDate(int employeeId, DateTime dateFrom, DateTime dateTo)
        {
            string sql =
               @$"SELECT
                    T.CreatedDate,
	                C.AreaNameKhmer Area,
	                C.MarketNameKhmer Market,
                    CUSTOMER_CODE CustomerCode,
                    CUSTOMER_NAME CustomerName,
                    CODE TransactionRef,
                    VALUE Value,
                    CASE WHEN BCIP.AMOUNT < BC.VALUE THEN BCIP.AMOUNT END AS 'HalfPayment',
                    CASE WHEN BCIP.AMOUNT = BC.VALUE THEN BCIP.AMOUNT END AS 'FullPayment',
                    BC.VALUE - BCIP.AMOUNT AS Total,
                    [dbo].[RETURN_TYPE_INVOICE](TYPE) AS Invoice
                FROM
                    BCSTOCK_CAR BC
                INNER JOIN
                    (SELECT SUM(AMOUNT) AMOUNT, INVOICE_ID FROM BCSTOCK_CAR_INVOICE_PAYMENT GROUP BY INVOICE_ID) BCIP
                ON
                    BCIP.INVOICE_ID = BC.ID
                INNER JOIN
                    TEMPLATE T
                ON
                    T.Id = BC.TEMPLATE_ID
                LEFT JOIN 
	                dbo.FN_GETCUSTOMERS(@DB_CODE) C
                ON
	                C.CustomerCode = BC.CUSTOMER_CODE
                WHERE
                    (@EMPLOYEE = '' OR @EMPLOYEE IS NULL OR Employee = @EMPLOYEE)
                    AND CAST(T.ClosingDate AS DATE) BETWEEN @FROM_DATE AND @TO_DATE;";
            var param = new
            {
                EMPLOYEE = employeeId,
                FROM_DATE = dateFrom,
                TO_DATE = dateTo,
            };
            var execute = await _sqlDataAccess.LoadData<CarPaymentInvoiceModel, dynamic>(sql, param);
            return execute.ToList();
        }

        #endregion

        #region Report All Expense

        public async Task<List<CarPaymentExpenseModel>> ReportAllExpenseByEmployeeIdAndTemplateId(int employeeId, int templateId)
        {
            var sql =
                @"SELECT BCE.DATE Date,BCET.EXPENSE_NAME ExpenseName,BCE.QTY Quantity,BCE.UNIT_PRICE UnitPrice,BCE.AMOUNT_RIEL AmountRiel,BCE.AMOUNT_DOLLAR AmountDollar,P.PRO_NAME Province,BCE.EXCHANGE_RATE ExchangeRate,T.ClosingDate
                    FROM BCSTOCK_CAR_EXPENSE BCE INNER JOIN TEMPLATE T ON T.Id = BCE.TEMPLATE_ID INNER JOIN BCSTOCK_CAR_EXPENSE_TYPE BCET ON BCET.EXPENSE_ID = BCE.EXPENS_TYPE_ID INNER JOIN PROVINCE P ON P.PROID = BCE.PROVINCE_ID
                WHERE (@EMPLOYEE = '' OR @EMPLOYEE IS NULL OR Employee = @EMPLOYEE ) AND (@TEMPLATE_ID = '' OR @TEMPLATE_ID IS NULL OR TEMPLATE_ID = @TEMPLATE_ID)";
            var param = new
            {
                EMPLOYEE = employeeId,
                TEMPLATE_ID = templateId,
            };
            var execute = await _sqlDataAccess.LoadData<CarPaymentExpenseModel, dynamic>(sql,param);
            return execute.ToList();
        }

        public async Task<List<CarPaymentExpenseModel>> ReportAllExpenseByEmployeeIdAndDate(int employeeId, DateTime dateFrom, DateTime dateTo)
        {
            var sql = @"SELECT BCE.DATE Date,BCET.EXPENSE_NAME ExpenseName,BCE.QTY Quantity,BCE.UNIT_PRICE UnitPrice,BCE.AMOUNT_RIEL AmountRiel,BCE.AMOUNT_DOLLAR AmountDollar,P.PRO_NAME Province,BCE.EXCHANGE_RATE ExchangeRate,T.ClosingDate FROM BCSTOCK_CAR_EXPENSE BCE
                INNER JOIN TEMPLATE T ON T.Id = BCE.TEMPLATE_ID INNER JOIN BCSTOCK_CAR_EXPENSE_TYPE BCET ON BCET.EXPENSE_ID = BCE.EXPENS_TYPE_ID INNER JOIN PROVINCE P ON P.PROID = BCE.PROVINCE_ID 
                WHERE (@EMPLOYEE = '' OR @EMPLOYEE IS NULL OR Employee = @EMPLOYEE ) AND CAST(T.ClosingDate as date) BETWEEN @DATEFROM AND @DATETO";
            var param = new
            {
                EMPLOYEE = employeeId,
                DATEFROM = dateFrom,
                DATETO = dateTo,
            };
            var execute = await _sqlDataAccess.LoadData<CarPaymentExpenseModel, dynamic>(sql, param);
            return execute.ToList();
        }

        #endregion

        #region Transfer Money

        public async Task<List<TransferMoney>> ReportAllTransferByEmployeeIdAndDate(string dbCode, int employeeId, DateTime dateFrom, DateTime dateTo)
        {
             string sql =
                @"
                SELECT TRANSACTION_DATE TransactionDate,AMOUNT Amount,DESCRIPTION Description,DEPOSIT_DOLLAR DepositDollar,
                    DEPOSIT_RIEL DepositRiel,DEPOSIT_EXCHANGE DepositExchange,DOLLAR DollarFromEmployee,RIEL RielFromEmployee,
                    EXCHANGE ExchangeRateEmployee,UPPER(USER_NAME)                  
                    Employee ,T.ClosingDate ClosingDate      
                    FROM BCSTOCK_CAR_SENT_MONEY_DETAILS S  
                    INNER JOIN TEMPLATE T ON T.Id = S.TEMPLATE_ID    
                    INNER JOIN(SELECT USER_ID, USER_NAME FROM BCUSERS) Emp 
                    on CONVERT(VARCHAR, Emp.USER_ID) = T.Employee  
                    WHERE DB_CODE = @DbCode AND (@EMPLOYEE = '' OR @EMPLOYEE IS NULL OR Employee = @EMPLOYEE )
				AND CAST(T.ClosingDate as date) BETWEEN @DATEFROM AND @DATETO
                ";
            var param = new
            {
                EMPLOYEE = employeeId,
                DbCode = dbCode,
                DATEFROM = dateFrom,
                DATETO = dateTo
            };
            var execute = await _sqlDataAccess.LoadData<TransferMoney,dynamic>(sql,param);
            return execute.ToList();
        }
        public async Task<List<TransferMoney>> ReportAllTransferByEmployeeIdAndTemplateId(string dbCode,int employeeId, int templateId)
        {
            const string sql =
                @"
                SELECT T.ClosingDate ClosingDate,TRANSACTION_DATE TransactionDate,AMOUNT Amount,DESCRIPTION Description,DEPOSIT_DOLLAR DepositDollar,
                DEPOSIT_RIEL DepositRiel,DEPOSIT_EXCHANGE DepositExchange,DOLLAR DollarFromEmployee,RIEL RielFromEmployee,
                EXCHANGE ExchangeRateEmployee,UPPER(USER_NAME)                  
                Employee          
                FROM BCSTOCK_CAR_SENT_MONEY_DETAILS S  
                INNER JOIN TEMPLATE T ON T.Id = S.TEMPLATE_ID    
                INNER JOIN(SELECT USER_ID, USER_NAME FROM BCUSERS ) Emp 
                on CONVERT(VARCHAR, Emp.USER_ID) = T.Employee  
                WHERE DB_CODE = @DbCode AND (@EMPLOYEE = '' OR @EMPLOYEE IS NULL OR Employee = @EMPLOYEE )
                AND
				(@TEMPLATE_ID = '' OR @TEMPLATE_ID IS NULL OR TEMPLATE_ID = @TEMPLATE_ID)
                ";
            var param = new
            {
                TEMPLATE_ID = templateId,
                EMPLOYEE = employeeId,
                DbCode = dbCode
            };
            var execute = await _sqlDataAccess.LoadData<TransferMoney, dynamic>(sql, param);
            return execute.ToList();
        }

        public async Task<List<AmountInvoiceReportModel>> GetAllTotalCollectionByRequestId(int requestId)
        { 
            string sql = @"
                    SELECT Transfer,Total Biased,Expense,Transfer + Expense Total FROM(
                    SELECT Deposit + ABS(Total) Transfer,Total,(SELECT SUM(AMOUNT_DOLLAR) FROM BC_STOCK_CAR_REQUESTS_EXPENSE WHERE REQUEST_ID = @RequestId) Expense FROM
                    (SELECT SUM(DOLLAR_FROM_EMPLOYEE) + SUM(RIEL_FROM_EMPLOYEE / EXCHANGE_FROM_EMPLOYEE) FromEmployee,SUM(DEPOSIT_DOLLAR) + SUM((DEPOSIT_RIEL / DEPOSIT_EXCHANGE)) Deposit,
                    (SUM(DOLLAR_FROM_EMPLOYEE) + SUM(RIEL_FROM_EMPLOYEE / EXCHANGE_FROM_EMPLOYEE)) - (SUM(DEPOSIT_DOLLAR) + SUM((DEPOSIT_RIEL / DEPOSIT_EXCHANGE))) Total
                    FROM BC_STOCK_CAR_REQUESTS_TRANSFER WHERE REQUEST_ID = @RequestId) Transfer) TotalCollection
                    ";
            var param = new
            {
                RequestId = requestId,
            };
            var execute = await _sqlDataAccess.LoadData<AmountInvoiceReportModel, dynamic>(sql, param);
            return execute.ToList();
        }

        public async Task<List<TotalInvoiceReportModel>> GetAllTotalInvoiceByRequestId(int requestId)
        {
            string sql = @"
                    SELECT CASE WHEN INVOICE_TYPE = 'N' THEN N'ថ្មី' WHEN INVOICE_TYPE = 'O' THEN N'ចាស់' WHEN INVOICE_TYPE = 'C' THEN N'ដូរ' END INVOICE_TYPE,
                    SUM(CREDIT_AMOUNT) CREDIT_AMOUNT,SUM(PAYMENT_AMOUNT) PAYMENT_AMOUNT,SUM(PAYMENT_AMOUNT) + SUM(CREDIT_AMOUNT) TOTAL FROM (
                    SELECT BC.TYPE INVOICE_TYPE,0 PAYMENT_AMOUNT,SUM(BCIC.INVOICE_VALUE) CREDIT_AMOUNT FROM BCSTOCK_CAR BC 
                    INNER JOIN BC_STOCK_CAR_REQUESTS_INVOICE_CREDIT BCIC 
                    ON BCIC.INVOICE_ID = BC.ID WHERE REQUEST_ID = @RequestId GROUP BY BC.TYPE
                    UNION
                    SELECT BC.TYPE INVOICE_TYPE,SUM(BCIP.PAID_AMOUNT)PAYMENT_AMOUNT,SUM(BCIP.INVOICE_VALUE - BCIP.PAID_AMOUNT) CREDIT_AMOUNT 
                    FROM BCSTOCK_CAR BC 
                    INNER JOIN BC_STOCK_CAR_REQUESTS_INVOICE_PAID BCIP
                    ON BCIP.INVOICE_ID = BC.ID WHERE REQUEST_ID = @RequestId
                    GROUP BY BC.TYPE ) PAYMENT GROUP BY INVOICE_TYPE
                    ";
            var param = new
            {
                RequestId = requestId
            };
            var execute = await _sqlDataAccess.LoadData<TotalInvoiceReportModel, dynamic>(sql, param);
            return execute.ToList();
        }

        public async Task<List<TotalCollectionModel>> ReportAllTotalCollectionByEmployeeIdAndDate(int employeeId, DateTime dateFrom, DateTime dateTo)
        {
             string sql =
                @"
                    SELECT 
					    AmountExpense,
					    AmountTransfer AmountTransfer,
					    AmountBiased,
					    AmountTransfer + AmountExpense + ABS(AmountBiased) Total,
					    ClosingDate
				    FROM (
					    SELECT 
						    AmountCollection, 
						    AmountTransfer + AmountTransferRiel AmountTransfer,
						    AmountExpense,
						    AmountCollection - (AmountTransfer + AmountTransferRiel) AmountBiased,
						    CLOSINGDATE ClosingDate
					    FROM (
						    SELECT 
							    SUM(MoneyFromSaleDollar + ISNULL((NULLIF(MoneyFromSaleRiel,0) / NULLIF(MoneyFromSaleExchange,0)),0)) AmountCollection,
							    SUM(TransferDollar) AmountTransfer,
							    SUM(TransferDollarFromRiel) AmountTransferRiel,  
							    (SELECT SUM(AMOUNT_DOLLAR)  FROM BCSTOCK_CAR_EXPENSE WHERE TEMPLATE_ID = tab.TEMPLATE_ID) AmountExpense,
							    DATES CLOSINGDATE
						    FROM (
							    SELECT 
								    TEMPLATE_ID, 
								    DOLLAR MoneyFromSaleDollar, 
								    RIEL MoneyFromSaleRiel, 
								    EXCHANGE MoneyFromSaleExchange, 
								    DEPOSIT_DOLLAR TransferDollar,
								    ISNULL(NULLIF(DEPOSIT_RIEL,0) / NULLIF(DEPOSIT_EXCHANGE,0),0) TransferDollarFromRiel
								    ,T.ClosingDate DATES
							    FROM BCSTOCK_CAR_SENT_MONEY_DETAILS BCSMD 
							    INNER JOIN TEMPLATE T ON T.Id = BCSMD.TEMPLATE_ID
							    WHERE (@EMPLOYEE = '' OR @EMPLOYEE IS NULL OR Employee = @EMPLOYEE )
							    AND CAST(T.ClosingDate as date) BETWEEN @DATEFROM AND @DATETO
						    ) tab 
						    GROUP BY TEMPLATE_ID, DATES
					    ) tab
				    ) tab";
             var param = new
             {
                 EMPLOYEE = employeeId,
                 DATEFROM = dateFrom,
                 DATETO = dateTo
             };
             var execute = await _sqlDataAccess.LoadData<TotalCollectionModel, dynamic>(sql, param);
             return execute.ToList();
        }

        public async Task<List<TotalPaymentModel>> ReportAllPaymentByEmployeeIdAndDate(int employeeId, DateTime dateFrom, DateTime dateTo)
        {
            string sql =
                @"
                     SELECT 
                        DESCRIPTION Description,
                        CREDITAMOUNT CreditAmount,
                        PAYMENTAMOUNT PaymentAmount,
                        CREDITAMOUNT + PAYMENTAMOUNT Total,
	                    ClosingDate
                    FROM (
                        SELECT 
                            CASE 
                                WHEN DESCRIPTION = 'N' THEN N'ថ្មី' 
                                WHEN DESCRIPTION = 'C' THEN N'ដូរ' 
                                WHEN DESCRIPTION = 'O' THEN N'ចាស់'
                                ELSE 'UNKNOW' 
                            END 'DESCRIPTION',
                          
                            SUM(CREDITAMOUNT) CREDITAMOUNT,
                            SUM(PAYMENTAMOUNT) PAYMENTAMOUNT,
                        CAST(tm.ClosingDate AS DATE) [ClosingDate]
                        --, ClosingDate ClosingDate
                        FROM (
                            SELECT 
                                CASE 
                                    WHEN BC.TYPE = 'N' THEN 'N' 
                                    WHEN BC.TYPE = 'C' THEN 'C' 
                                    WHEN BC.TYPE = 'O' THEN 'O'
                                    ELSE 'UNKNOW' 
                                END 'DESCRIPTION',
                                ISNULL(CASE WHEN BC.STATUS = 1 THEN SUM(BC.VALUE) END,0)  CREDITAMOUNT,
                                ISNULL(SUM(BCIP.AMOUNT), 0) PAYMENTAMOUNT,BC.TEMPLATE_ID
                            FROM BCSTOCK_CAR BC
                            LEFT JOIN BCSTOCK_CAR_INVOICE_PAYMENT BCIP ON BCIP.INVOICE_ID = BC.ID
                            WHERE TEMPLATE_ID IN (
                                SELECT Id FROM TEMPLATE WHERE (@EMPLOYEE = '' OR @EMPLOYEE IS NULL OR Employee = @EMPLOYEE )
                                AND CAST(TEMPLATE.ClosingDate as date) BETWEEN @DATEFROM  AND  @DATETO )
                            GROUP BY TYPE,STATUS,BC.TEMPLATE_ID
                        ) TAB 
                      INNER JOIN dbo.TEMPLATE tm ON tm.Id=TAB.TEMPLATE_ID
                        GROUP BY DESCRIPTION,CAST(tm.ClosingDate AS DATE)
                    ) TAB
                    ";
            var param = new
            {
                EMPLOYEE = employeeId,
                DATEFROM = dateFrom,
                DATETO = dateTo
            };
            var execute = await _sqlDataAccess.LoadData<TotalPaymentModel, dynamic>(sql, param);
            return execute.ToList();
        }

        public async Task<List<TotalCollectionModel>> ReportAllTotalCollectionByEmployeeIdAndTemplateId(int employeeId, int templateId)
        {
             string sql =
                @"
                SELECT 
					AmountExpense,
					AmountTransfer AmountTransfer,
					AmountBiased,
					AmountTransfer + AmountExpense + ABS(AmountBiased) Total,
					ClosingDate
				FROM (
					SELECT 
						AmountCollection, 
						AmountTransfer + AmountTransferRiel AmountTransfer,
						AmountExpense,
						AmountCollection - (AmountTransfer + AmountTransferRiel) AmountBiased,
						CLOSINGDATE ClosingDate
					FROM (
						SELECT 
							SUM(MoneyFromSaleDollar + ISNULL((NULLIF(MoneyFromSaleRiel,0) / NULLIF(MoneyFromSaleExchange,0)),0)) AmountCollection,
							SUM(TransferDollar) AmountTransfer,
							SUM(TransferDollarFromRiel) AmountTransferRiel,  
							(SELECT SUM(AMOUNT_DOLLAR)  FROM BCSTOCK_CAR_EXPENSE WHERE TEMPLATE_ID = tab.TEMPLATE_ID) AmountExpense,
							DATES CLOSINGDATE
						FROM (
							SELECT 
								TEMPLATE_ID, 
								DOLLAR MoneyFromSaleDollar, 
								RIEL MoneyFromSaleRiel, 
								EXCHANGE MoneyFromSaleExchange, 
								DEPOSIT_DOLLAR TransferDollar,
								ISNULL(NULLIF(DEPOSIT_RIEL,0) / NULLIF(DEPOSIT_EXCHANGE,0),0) TransferDollarFromRiel
								,T.ClosingDate DATES
							FROM BCSTOCK_CAR_SENT_MONEY_DETAILS BCSMD 
							INNER JOIN TEMPLATE T ON T.Id = BCSMD.TEMPLATE_ID
							WHERE (@EMPLOYEE = '' OR @EMPLOYEE IS NULL OR Employee = @EMPLOYEE )
							AND (@TEMPLATE_ID = '' OR @TEMPLATE_ID IS NULL OR TEMPLATE_ID = @TEMPLATE_ID)
						) tab 
						GROUP BY TEMPLATE_ID, DATES
					) tab
				) tab";
            var param = new
            {
                EMPLOYEE = employeeId,
                TEMPLATE_ID = templateId,
            };
            var execute = await _sqlDataAccess.LoadData<TotalCollectionModel,dynamic>(sql,param);
            return execute.ToList();
        }

        public async Task<List<TotalPaymentModel>> ReportAllPaymentByEmployeeIdAndTemplateId(int employeeId, int templateId)
        {
             string sql =
                @"SELECT 
                    DESCRIPTION Description,
                    CREDITAMOUNT CreditAmount,
                    PAYMENTAMOUNT PaymentAmount,
                    CREDITAMOUNT + PAYMENTAMOUNT Total,
	                ClosingDate
                FROM (
                    SELECT 
                        CASE 
                            WHEN DESCRIPTION = 'N' THEN N'ថ្មី' 
                            WHEN DESCRIPTION = 'C' THEN N'ដូរ' 
                            WHEN DESCRIPTION = 'O' THEN N'ចាស់'
                            ELSE 'UNKNOW' 
                        END 'DESCRIPTION',
                      
                        SUM(CREDITAMOUNT) CREDITAMOUNT,
                        SUM(PAYMENTAMOUNT) PAYMENTAMOUNT,
                    CAST(tm.ClosingDate AS DATE) [ClosingDate]
                    --, ClosingDate ClosingDate
                    FROM (
                        SELECT 
                            CASE 
                                WHEN BC.TYPE = 'N' THEN 'N' 
                                WHEN BC.TYPE = 'C' THEN 'C' 
                                WHEN BC.TYPE = 'O' THEN 'O'
                                ELSE 'UNKNOW' 
                            END 'DESCRIPTION',
                            ISNULL(CASE WHEN BC.STATUS = 1 THEN SUM(BC.VALUE) END,0)  CREDITAMOUNT,
                            ISNULL(SUM(BCIP.AMOUNT), 0) PAYMENTAMOUNT,BC.TEMPLATE_ID
                        FROM BCSTOCK_CAR BC
                        LEFT JOIN BCSTOCK_CAR_INVOICE_PAYMENT BCIP ON BCIP.INVOICE_ID = BC.ID
                        WHERE TEMPLATE_ID IN (
                            SELECT Id FROM TEMPLATE WHERE (@EMPLOYEE = '' OR @EMPLOYEE IS NULL OR Employee = @EMPLOYEE )
                            AND (@TEMPLATE_ID = '' OR @TEMPLATE_ID IS NULL OR TEMPLATE_ID = @TEMPLATE_ID))
                        GROUP BY TYPE,STATUS,BC.TEMPLATE_ID
                    ) TAB 
                  INNER JOIN dbo.TEMPLATE tm ON tm.Id=TAB.TEMPLATE_ID
                    GROUP BY DESCRIPTION,CAST(tm.ClosingDate AS DATE)
                ) TAB ";
             var param = new
             {
                 EMPLOYEE = employeeId,
                 TEMPLATE_ID = templateId,
             };
             var execute = await _sqlDataAccess.LoadData<TotalPaymentModel, dynamic>(sql, param);
             return execute.ToList();
        }

        public async Task<List<TotalCollectionModel>> GetAllTotalCollectionByTemplateId(int templateId)
        {
            string sql =
                $@"SELECT AmountExpense,AmountTransfer AmountTransfer,AmountBiased,AmountTransfer +
                 AmountExpense + ABS(AmountBiased) Total FROM (
                 SELECT AmountCollection, AmountTransfer +AmountTransferRiel AmountTransfer,AmountExpense,AmountCollection - (AmountTransfer + AmountTransferRiel) AmountBiased FROM
                 (SELECT SUM(MoneyFromSaleDollar +(MoneyFromSaleRiel / MoneyFromSaleExchange)) AmountCollection,
                 SUM(TransferDollar) AmountTransfer,SUM(TransferDollarFromRiel) AmountTransferRiel,(SELECT SUM(AMOUNT_DOLLAR) FROM
                 BCSTOCK_CAR_EXPENSE WHERE TEMPLATE_ID = tab.TEMPLATE_ID) AmountExpense FROM(
                 SELECT TEMPLATE_ID, DOLLAR MoneyFromSaleDollar, RIEL MoneyFromSaleRiel, EXCHANGE MoneyFromSaleExchange, DEPOSIT_DOLLAR
                 TransferDollar, DEPOSIT_RIEL / DEPOSIT_EXCHANGE TransferDollarFromRiel FROM BCSTOCK_CAR_SENT_MONEY_DETAILS
                 WHERE TEMPLATE_ID = @TemplateId) tab Group by TEMPLATE_ID) tab) tab";
            var param = new
            {
                TemplateId = templateId,
            };
            var execute = await _sqlDataAccess.LoadData<TotalCollectionModel,dynamic>(sql,param);
            return execute.ToList();
        }

        public async Task<List<TotalPaymentModel>> GetAllPaymentByTemplateId(int templateId)
        {
            string sql = @"
                     SELECT DESCRIPTION Description,CREDITAMOUNT CreditAmount,PAYMENTAMOUNT PaymentAmount,CREDITAMOUNT + PAYMENTAMOUNT Total FROM (
                    SELECT CASE WHEN DESCRIPTION = 'N' THEN N'ថ្មី' WHEN DESCRIPTION = 'C' THEN N'ដូរ' WHEN DESCRIPTION = 'O' THEN N'ចាស់'
                               ELSE 'UNKNOW' END                   'DESCRIPTION'
							   ,SUM(CREDITAMOUNT)CREDITAMOUNT,SUM(PAYMENTAMOUNT)PAYMENTAMOUNT FROM (
                                SELECT CASE WHEN BC.TYPE = 'N' THEN 'N' WHEN BC.TYPE = 'C' THEN 'C' WHEN BC.TYPE = 'O' THEN 'O'
                               ELSE 'UNKNOW' END                   'DESCRIPTION',
                            ISNULL(CASE WHEN BC.STATUS = 1 THEN SUM(BC.VALUE) END,0)  CREDITAMOUNT,
                           ISNULL(SUM(BCIP.AMOUNT), 0)                 PAYMENTAMOUNT
                    FROM BCSTOCK_CAR BC
                             LEFT JOIN BCSTOCK_CAR_INVOICE_PAYMENT BCIP ON BCIP.INVOICE_ID = BC.ID
							 WHERE TEMPLATE_ID =@TemplateId
                    GROUP BY TYPE,STATUS) TAB GROUP BY DESCRIPTION )TAB ";
            var param = new
            {
                TemplateId = templateId,
            };
            var execute = await _sqlDataAccess.LoadData<TotalPaymentModel, dynamic>(sql, param);
            return execute.ToList();
        }

        #endregion
    }
}
