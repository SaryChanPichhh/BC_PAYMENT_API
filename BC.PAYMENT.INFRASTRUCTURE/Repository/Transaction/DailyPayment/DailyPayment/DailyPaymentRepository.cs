using ReturnInvoiceModel = BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DailyPayment.ReturnInvoiceModel;

using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Transaction.DailyPayment.DailyPayment
{
    public class DailyPaymentRepository : IDailyPaymentRepository
    {
        private readonly ISqlDataAccess _sqlDataAccess;
        private readonly IDbConnection _dbConnection;
        public DailyPaymentRepository(ISqlDataAccess sqlDataAccess, IDbConnection dbConnection)
        {
            _sqlDataAccess = sqlDataAccess;
            _dbConnection = dbConnection;
        }
        public async Task<List<PaidInvoiceModel>> GetPaidInvoiceByPeriodAsync(string dbCode, int month, int year)
        {
            var sql = $@"SELECT P.DB_CODE DbCode, P.INVOICE_ID InvoiceId,D.DELIVERIES_KHMER DeliveryName, PAID.PAYMENT_ID PaymentId, P.DIVIDED_INVOICE_ID DividedInvoiceId,
                                  N.CUSTOMER_CODE CustomerCode, N.ACC_NAME_KH CustomerName, N.TRANSACTION_REF TransactionCode, 1.00 AS AMOUNT,
                                  N.HEADER_TRANSACTION_VALUES InvoiceValue,
                                  CASE WHEN PAID.AMOUNT < N.HEADER_TRANSACTION_VALUES THEN PAID.AMOUNT END 'HalfPaid',
                                  CASE WHEN PAID.AMOUNT = N.HEADER_TRANSACTION_VALUES THEN PAID.AMOUNT END 'FullPaid',
                                  N.HEADER_TRANSACTION_VALUES - PAID.AMOUNT Total FROM PC_DIVIDED_INVOICE P
                                  INNER JOIN NEW_INVOICE N ON N.ID = P.INVOICE_ID
                                  INNER JOIN PC_PAYMENT_INVOICE PAID ON PAID.DIVDIE_INVOICE_ID = P.DIVIDED_INVOICE_ID
                                  INNER JOIN TB_BCDELIVERIES D ON D.DELIVERIES_ID = P.DELIVERY_ID WHERE 
                                  MONTH(P.CREATE_DATE) = @MONTH AND YEAR(P.CREATE_DATE) = @YEAR AND P.DB_CODE = @DB_CODE
                                  GROUP BY P.DB_CODE,  P.INVOICE_ID,D.DELIVERIES_KHMER, N.CUSTOMER_CODE, N.ACC_NAME_KH,
                                  N.TRANSACTION_REF, N.HEADER_TRANSACTION_VALUES, P.DIVIDED_INVOICE_ID,
                                  PAID.PAYMENT_ID, PAID.AMOUNT, PAID.PAYMENT_ID, 
                                  P.DIVIDED_INVOICE_ID ORDER BY N.CUSTOMER_CODE";
            var param = new
            {
                DB_CODE = dbCode,
                MONTH = month,
                YEAR = year,
            };
            var execute = await _sqlDataAccess.LoadData<PaidInvoiceModel, dynamic>(sql, param);
            return execute.ToList();
        }

        public async Task<List<PaidInvoiceModel>> GetPaidInvoiceByDateAsync(string dbCode, string fromDate, string toDate)
        {
            var sql = $@"SELECT P.DB_CODE, P.INVOICE_ID,D.DELIVERIES_KHMER, PAID.PAYMENT_ID, P.DIVIDED_INVOICE_ID,
                                  N.CUSTOMER_CODE, N.ACC_NAME_KH, N.TRANSACTION_REF, 1.00 AS AMOUNT,
                                  N.HEADER_TRANSACTION_VALUES,
                                  CASE WHEN PAID.AMOUNT < N.HEADER_TRANSACTION_VALUES THEN PAID.AMOUNT END 'HALF_PAID',
                                  CASE WHEN PAID.AMOUNT = N.HEADER_TRANSACTION_VALUES THEN PAID.AMOUNT END 'FULL_PAID',
                                  N.HEADER_TRANSACTION_VALUES - PAID.AMOUNT TOTAL FROM PC_DIVIDED_INVOICE P
                                  INNER JOIN NEW_INVOICE N ON N.ID = P.INVOICE_ID
                                  INNER JOIN PC_PAYMENT_INVOICE PAID ON PAID.DIVDIE_INVOICE_ID = P.DIVIDED_INVOICE_ID
                                  INNER JOIN TB_BCDELIVERIES D ON D.DELIVERIES_ID = P.DELIVERY_ID WHERE 
                                  P.CREATE_DATE BETWEEN @FROMDATE AND @TODATE AND P.DB_CODE = @DB_CODE
                                  GROUP BY P.DB_CODE,  P.INVOICE_ID,D.DELIVERIES_KHMER, N.CUSTOMER_CODE, N.ACC_NAME_KH,
                                  N.TRANSACTION_REF, N.HEADER_TRANSACTION_VALUES, P.DIVIDED_INVOICE_ID,
                                  PAID.PAYMENT_ID, PAID.AMOUNT, PAID.PAYMENT_ID, 
                                  P.DIVIDED_INVOICE_ID ORDER BY N.CUSTOMER_CODE";
            var param = new
            {
                DB_CODE = dbCode,
                FROMDATE = fromDate,
                TODATE = toDate,
            };
            var execute = await _sqlDataAccess.LoadData<PaidInvoiceModel, dynamic>(sql, param);
            return execute.ToList();
        }

        public async Task<int> DeletePaidInvoiceAsync(int paymentId, int dividedId)
        {
            var transaction  = _dbConnection.BeginTransaction();
            try
            {
                var sql = $@"DELETE FROM PC_PAYMENT_INVOICE WHERE PAYMENT_ID = @PAYMENT_ID";
                var param = new
                {
                    PAYMENT_ID = paymentId,
                };
                var affectedRows = await _dbConnection.ExecuteAsync(sql, param, transaction);
                if (affectedRows > 0)
                {
                    var updateQuery =
                        $@"IF NOT EXISTS(SELECT * FROM PC_RETURN_INVOICE WHERE DIVIDED_INIOVICE_ID = @DII)
                           UPDATE PC_DIVIDED_INVOICE SET STATUS = 1 WHERE DIVIDED_INVOICE_ID = @DIVIDED_INVOICE_ID";
                    var updateParam = new
                    {
                        DIVIDED_INVOICE_ID = dividedId,
                    };
                    affectedRows = await _dbConnection.ExecuteAsync(updateQuery, updateParam, transaction);
                    if (affectedRows > 0)
                    {
                        transaction.Commit();
                        return affectedRows;
                    }
                    else
                    {
                        transaction.Rollback();
                        return 0;
                    }
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Debug.WriteLine(ex.StackTrace);
            }

            return 0;
        }

        public async Task<int> UpdatePaidInvoiceAsync(DeliveryGeneralInvoicePaidUpdateModel model)
        {
            if (_dbConnection.State == ConnectionState.Closed)
                _dbConnection.Open();
            var transaction = _dbConnection.BeginTransaction();
            try
            {
                if (!model.OldAmount.Equals(model.NewAmount))
                {
                    var sql =
                        $@"INSERT INTO PC_EDIT_VALUE_DIVIDED_INVOICE VALUES (@DB_CODE,@DIVIDED_ID,@OLD_VALUE,@NEW_VALUE,@DESCRIPTION,@CREATE_DATE,@CREATE_BY)";
                    var param = new
                    {
                        DB_CODE = model.DbCode,
                        DIVIDED_ID = model.DividedInvoiceId,
                        OLD_VALUE = model.OldAmount,
                        NEW_VALUE = model.NewAmount,
                        DESCRIPTION = model.Description,
                        CREATE_DATE = DateTime.Now,
                        CREATE_BY = model.CreateBy,
                    };
                    var affectedRows = await _dbConnection.ExecuteAsync(sql, param, transaction);
                    if (affectedRows > 0)
                    {
                        var updateQuery =
                            $@"UPDATE NEW_INVOICE SET HEADER_TRANSACTION_VALUES = @NEW_VALUE WHERE ID IN (SELECT INVOICE_ID FROM PC_DIVIDED_INVOICE WHERE DIVIDED_INVOICE_ID = @DIVIDED_INVOICE_ID)";
                        var updateParam = new
                        {
                            DIVIDED_INVOICE_ID = model.DividedInvoiceId,
                            NEW_VALUE = model.NewAmount,
                        };
                        affectedRows = await _dbConnection.ExecuteAsync(updateQuery, updateParam, transaction);
                        if (affectedRows > 0)
                        {
                           
                             transaction.Commit();
                             return affectedRows;
                        };
                        if (!model.OldPaidAmount.Equals(model.NewPaidAmount))
                        {
                            var updatePaymentQuery =
                                $@"UPDATE PC_PAYMENT_INVOICE SET AMOUNT = @AMOUNT WHERE PAYMENT_ID = @PAYMENT_ID";
                            var updatePaymentParam = new
                            {
                                AMOUNT = model.PaidAmount,
                                PAYMENT_ID = model.PaymentId,
                            };
                            await _dbConnection.ExecuteAsync(updatePaymentQuery, updatePaymentParam, transaction);
                        }
                    }
                    else
                    {
                        transaction.Rollback();
                        return 0;
                    }
                }
                
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Debug.WriteLine(ex.StackTrace);
            }
            return 0;
        }

        /// <summary>
        /// Money Control
        /// </summary>
        /// <param name="dbCode"></param>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<List<ExpenseDetailModel>> GetExpenseDetailByPeriodAsync(string dbCode, int month, int year)
        {
            var sql =
                $@"SELECT P.ID Id,EXCHANGE ExchangeRate,D.DELIVERIES_KHMER DeliveryName,TOTAL + ((EXP_AMOUNT_1+EXP_AMOUNT_2+EXP_AMOUNT_3)/EXCHANGE) SubTotal,
                DOLLAR Dollar,RIEL Riel,EXP_AMOUNT_1+EXP_AMOUNT_2+EXP_AMOUNT_3 ExpenseRiel,(EXP_AMOUNT_1+EXP_AMOUNT_2+EXP_AMOUNT_3)/EXCHANGE ExpenseDollar,TOTAL Total,MONEY_BIAS Misaligned
                FROM BCPAYMENTDETAILA P INNER JOIN TB_BCDELIVERIES D ON D.DELIVERIES_ID = P.DELIVERYID WHERE YEAR(CREATED_DATE) = @YEAR AND MONTH(CREATED_DATE) = @MONTH AND P.DB_CODE = @DB_CODE;";
            var param = new
            {
                DB_CODE = dbCode,
                MONTH = month,
                YEAR = year
            };
            var execute = await _sqlDataAccess.LoadData<ExpenseDetailModel, dynamic>(sql, param);
            return execute.ToList(); ;
        }

        public async Task<List<ExpenseDetailModel>> GetExpenseDetailByDateAsync(string dbCode, string fromDate, string toDate)
        {
            var sql =
                $@"SELECT P.ID Id,EXCHANGE ExchangeRate,D.DELIVERIES_KHMER DeliveryName,TOTAL + ((EXP_AMOUNT_1+EXP_AMOUNT_2+EXP_AMOUNT_3)/EXCHANGE) SubTotal,
                DOLLAR Dollar,RIEL Riel,EXP_AMOUNT_1+EXP_AMOUNT_2+EXP_AMOUNT_3 ExpenseRiel,(EXP_AMOUNT_1+EXP_AMOUNT_2+EXP_AMOUNT_3)/EXCHANGE ExpenseDollar,TOTAL Total,MONEY_BIAS Misaligned
                FROM BCPAYMENTDETAILA P INNER JOIN TB_BCDELIVERIES D ON D.DELIVERIES_ID = P.DELIVERYID WHERE CREATED_DATE BETWEEN @FROM_DATE AND @TO_DATE AND P.DB_CODE = @DB_CODE;";
            var param = new
            {
                DB_CODE = dbCode,
                FROM_DATE = fromDate,
                TO_DATE = toDate
            };
            var execute = await _sqlDataAccess.LoadData<ExpenseDetailModel, dynamic>(sql, param);
            return execute.ToList();
        }

        public async Task<int> UpdateExpenseDetailAsync(ExpenseDetailDto model)
        {
            var sql = $@"UPDATE BCPAYMENTDETAILA SET TOTAL = @TOTAL,DOLLAR = @DOLLAR,RIEL= @RIEL,EXCHANGE = @EXCHANGE,DESC_EXP_1 = @DESC_EXP_1,DESC_EXP_2 = @DESC_EXP_2,DESC_EXP_3 = @DESC_EXP_3,
                          EXP_AMOUNT_1 = @EXP_AMOUNT_1, EXP_AMOUNT_2 = @EXP_AMOUNT_2, EXP_AMOUNT_3 = @EXP_AMOUNT_3, MONEY_BIAS = @MONEY_BIAS WHERE ID = @ID";
            var param = new
            {
                TOTAL = model.Total,
                DOLLAR = model.Dollar,
                RIEL = model.Riel,
                EXCHANGE = model.Exchange,
                DESC_EXP_1 = model.Description1,
                DESC_EXP_2 = model.Description2,
                DESC_EXP_3 = model.Description3,
                EXP_AMOUNT_1 = model.Expense1,
                EXP_AMOUNT_2 = model.Expense2,
                EXP_AMOUNT_3 = model.Expense3,
                MONEY_BIAS = model.Misaligned,
                ID = model.Id
            };
            var affectedRow = await _sqlDataAccess.ExecuteAsync(sql, param);
            return affectedRow;
        }

        public async Task<List<ReturnInvoiceModel>> GetReturnInvoiceByDateAsync(string dbCode, DateTime fromDate, DateTime toDate)
        {
            var sql = $@"SELECT D.DIVIDED_INVOICE_ID,CUSTOMER_CODE,ACC_NAME_KH,TRANSACTION_REF,HEADER_TRANSACTION_VALUES,1.00 [RETURN],R.DESCRIPTION,R.RETURN_ID 
                 FROM NEW_INVOICE N INNER JOIN PC_DIVIDED_INVOICE D ON D.INVOICE_ID = N.ID INNER JOIN PC_RETURN_INVOICE R ON R.DIVIDED_INIOVICE_ID = D.DIVIDED_INVOICE_ID
                 WHERE R.DB_CODE = @DB_CODE AND D.DB_CODE = @DB_CODE AND N.DB_CODE = @DB_CODE AND D.CREATE_DATE BETWEEN @FROM_DATE AND @TO_DATE)";
            var param = new
            {
                DB_CODE = dbCode,
                FROM_DATE = fromDate,
                TO_DATE = toDate
            };
            var execute = await _sqlDataAccess.LoadData<ReturnInvoiceModel, dynamic>(sql, param);
            return execute.ToList();
        }

        public async Task<List<ReturnInvoiceModel>> GetReturnInvoiceByPeriodAsync(string dbCode, string month, string year)
        {
            var sql = $@"SELECT D.DIVIDED_INVOICE_ID,CUSTOMER_CODE,ACC_NAME_KH,TRANSACTION_REF,HEADER_TRANSACTION_VALUES,1.00 [RETURN],R.DESCRIPTION,R.RETURN_ID 
                 FROM NEW_INVOICE N INNER JOIN PC_DIVIDED_INVOICE D ON D.INVOICE_ID = N.ID INNER JOIN PC_RETURN_INVOICE R ON R.DIVIDED_INIOVICE_ID = D.DIVIDED_INVOICE_ID
                 WHERE R.DB_CODE = @DB_CODE AND D.DB_CODE = @DB_CODE AND N.DB_CODE = @DB_CODE  AND YEAR(D.CREATE_DATE) = @YEAR AND MONTH(D.CREATE_DATE) = @MONTH)";
            var param = new
            {
                DB_CODE = dbCode,
                YEAR = year,
                MONTH = month
            };
            var execute = await _sqlDataAccess.LoadData<ReturnInvoiceModel, dynamic>(sql, param);
            return execute.ToList();
        }

        public async Task<int> UpdateReturnInvoiceAsync(string dividedId)
        {
            if(_dbConnection.State == ConnectionState.Closed)
                _dbConnection.Open();

            var connection = _dbConnection.BeginTransaction();
            try
            {
                var deleteQuery = $@"DELETE FROM PC_RETURN_INVOICE WHERE DIVIDED_INIOVICE_ID = @DIVIDED_ID";
                var deleteParam = new
                {
                    DIVIDED_ID = dividedId,
                };
                var affectedRow = await _dbConnection.ExecuteAsync(deleteQuery, deleteParam, connection);
                if (affectedRow > 0)
                {
                    var updateQuery = $@"IF NOT EXISTS(SELECT * FROM PC_PAYMENT_INVOICE WHERE DIVDIE_INVOICE_ID = @DIVIDED_ID)
                                 UPDATE PC_DIVIDED_INVOICE SET STATUS = 1 WHERE DIVIDED_INVOICE_ID = @DIVIDED_ID";
                    var updateParam = new
                    {
                        DIVIDED_ID = dividedId,
                    };
                    var updateAffectedRow = await _dbConnection.ExecuteAsync(updateQuery, updateParam, connection);
                    connection.Commit();
                    return updateAffectedRow;
                }
                connection.Rollback();
            }
            catch (Exception ex)
            {
                connection.Rollback();
                Debug.WriteLine(ex.StackTrace);
            }
            return 0;

        }

        public async Task<List<ReturnChangeInvoiceModel>> GetReturnChangeInvoiceByDateAsync(string dbCode, DateTime fromDate, DateTime toDate)
        {
            var sql =
                $@"SELECT ReturnId,DeliveryName,CustomerCode,CustomerName,TransactionCode,Change,New,Cancel,Description,CreatedDate,CASE WHEN RETURN_ID IS NOT NULL THEN '1' END STATUS FROM (
                SELECT R.RETURN_ID ReturnId, DELIVERIES_KHMER DeliveryName, N.CUSTOMER_CODE CustomerCode, ACC_NAME_KH CustomerName, TRANSACTION_REF TransactionCode, 
                CASE 
                    WHEN N.STATUS = 'C' THEN '1' END 'Change',
                CASE
                    WHEN N.STATUS = 'N' THEN '1' END 'New',
                CASE
                    WHEN UPPER(DESCRIPTION) LIKE '%OFFICE%' THEN 'Office'
                    WHEN UPPER(DESCRIPTION) LIKE '%CANCEL%' THEN 'Cancel' END 'Cancel',
                CASE
                    WHEN UPPER(DESCRIPTION) NOT LIKE '%OFFICE%' AND UPPER(DESCRIPTION) NOT LIKE '%CANCEL%'
                    THEN DESCRIPTION END 'Description',
                P.CREATE_DATE CreatedDate,
                PRIA.RETURN_ID
                FROM NEW_INVOICE N
                INNER JOIN PC_DIVIDED_INVOICE P ON P.INVOICE_ID = N.ID
                INNER JOIN PC_RETURN_INVOICE R ON R.DIVIDED_INIOVICE_ID = P.DIVIDED_INVOICE_ID
                INNER JOIN TB_BCDELIVERIES T ON T.DELIVERIES_ID = P.DELIVERY_ID
                LEFT JOIN PC_RETURNING_INVOICE_AUDIT PRIA on R.RETURN_ID = PRIA.RETURN_ID
                WHERE P.DB_CODE = @DB_CODE
                AND R.DB_CODE = @DB_CODE
                AND T.DB_CODE = @DB_CODE
                AND P.CREATE_DATE BETWEEN @FROM_DATE AND @TO_DATE
                ) TAB";

            var param = new
            {
                DB_CODE = dbCode,
                FROM_DATE = fromDate,
                TO_DATE = toDate
            };
            var execute = await _sqlDataAccess.LoadData<ReturnChangeInvoiceModel, dynamic>(sql, param);
            return execute.ToList();
        }

        public async Task<List<ReturnChangeInvoiceModel>> GetReturnChangeInvoiceByPeriodAsync(string dbCode, int month, int year)
        {
            var sql =
                $@"SELECT ReturnId,DeliveryName,CustomerCode,CustomerName,TransactionCode,Change,New,Cancel,Description,CreatedDate,CASE WHEN RETURN_ID IS NOT NULL THEN '1' END STATUS FROM (
                SELECT R.RETURN_ID ReturnId, DELIVERIES_KHMER DeliveryName, N.CUSTOMER_CODE CustomerCode, ACC_NAME_KH CustomerName, TRANSACTION_REF TransactionCode, 
                CASE 
                    WHEN N.STATUS = 'C' THEN '1' END 'Change',
                CASE
                    WHEN N.STATUS = 'N' THEN '1' END 'New',
                CASE
                    WHEN UPPER(DESCRIPTION) LIKE '%OFFICE%' THEN 'Office'
                    WHEN UPPER(DESCRIPTION) LIKE '%CANCEL%' THEN 'Cancel' END 'Cancel',
                CASE
                    WHEN UPPER(DESCRIPTION) NOT LIKE '%OFFICE%' AND UPPER(DESCRIPTION) NOT LIKE '%CANCEL%'
                    THEN DESCRIPTION END 'Description',
                P.CREATE_DATE CreatedDate,
                PRIA.RETURN_ID
                FROM NEW_INVOICE N
                INNER JOIN PC_DIVIDED_INVOICE P ON P.INVOICE_ID = N.ID
                INNER JOIN PC_RETURN_INVOICE R ON R.DIVIDED_INIOVICE_ID = P.DIVIDED_INVOICE_ID
                INNER JOIN TB_BCDELIVERIES T ON T.DELIVERIES_ID = P.DELIVERY_ID
                LEFT JOIN PC_RETURNING_INVOICE_AUDIT PRIA on R.RETURN_ID = PRIA.RETURN_ID
                WHERE P.DB_CODE = @DB_CODE
                AND R.DB_CODE = @DB_CODE
                AND T.DB_CODE = @DB_CODE
                AND YEAR(P.CREATE_DATE) = @YEAR AND MONTH(P.CREATE_DATE) = @MONTH
                ) TAB";

            var param = new
            {
                DB_CODE = dbCode,
                YEAR = year,
                MONTH = month,
            };
            var execute = await _sqlDataAccess.LoadData<ReturnChangeInvoiceModel, dynamic>(sql, param);
            return execute.ToList();
        }

        public async Task<int> InsertGetReturnInvoice(ReturnChangeInvoiceModel model)
        {
            const string sql = $@"INSERT INTO PC_RETURNING_INVOICE_AUDIT(RETURN_ID, PROCESSING_STATUS, LAST_UPDATED_DATE, LAST_UPDATED_BY)
                               VALUES(@RETURN_ID, @PROCESSING_STATUS, @LAST_UPDATED_DATE, @LAST_UPDATED_BY)";
            var param = new
            {
                RETURN_ID = model.ReturnId,
                PROCESSING_STATUS = 1,
                LAST_UPDATED_DATE = DateTime.Now,
                LAST_UPDATED_BY = model.CreateBy,
            };
            var affectedRow = await _sqlDataAccess.ExecuteAsync(sql, param);
            return affectedRow;
        }
    }
}
