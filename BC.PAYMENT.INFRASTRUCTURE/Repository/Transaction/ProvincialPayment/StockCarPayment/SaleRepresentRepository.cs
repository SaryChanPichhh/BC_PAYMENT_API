

using System.Data;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.APPLICATION.Interfaces.Transaction.ProvincialPayment.StockCarPayment;
using BC.PAYMENT.CORE.DTO.Invoice;
using BC.PAYMENT.CORE.DTO.Transaction.ProvincialPayment.StockCarPayment;
using BC.PAYMENT.CORE.Entities.Invoice;
using BC.PAYMENT.CORE.Entities.Transaction.ProvincialPayment.StockCarPayment;
using BC.PAYMENT.CORE.Enums;
using BC.PAYMENT.INFRASTRUCTURE.DBAccess;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Transaction.ProvincialPayment.StockCarPayment
{
    public  class SaleRepresentRepository : ISaleRepresentRepository
    {
        private readonly ISqlDataAccess _sqlDataAccess;
        private readonly IDbConnection _dbConnection;
        public SaleRepresentRepository(ISqlDataAccess sqlDataAccess, IDbConnection dbConnection)
        {
            _sqlDataAccess = sqlDataAccess;
            _dbConnection = dbConnection;
        }

        #region SaleRepresent

        public async Task<int> AddNewAsync(SaleRepresentModel model)
        {
            const string sql =
                "INSERT INTO TEMPLATE(Employee,CreatedDate,CreatedBy,Description,DbCode,IsEnable) OUTPUT inserted.Id VALUES(@Employee,@CreatedDate,@CreatedBy,@Description,@DbCode,1)";
            var parameter = new
            {
                Employee = model.EmployeeId,
                CreatedDate = DateTime.Now,
                CreatedBy = model.CreatedBy,
                Description = model.Description,
                DbCode = model.DbCode
            };
            var affectedRow = await _sqlDataAccess.ExecuteAsync(sql, parameter);
            return affectedRow;
        }
        public async Task<int> UpdateAsync(SaleRepresentModel model)
        {
            const string sql = "UPDATE TEMPLATE SET Employee = @EMPLOYEE,Description = @DESCRIPTION WHERE Id = @Id";
            var parameter = new
            {
                Id = model.Id,
                EMPLOYEE = model.EmployeeId,
                DESCRIPTION = model.Description
            };
            var affectedRow = await _sqlDataAccess.ExecuteAsync(sql, parameter);
            return affectedRow;
        }

        public async Task<List<SaleRepresentModel>> GetAsync(string dbCode)
        {
            const string sql = @"SELECT Id,CreatedDate,CreatedBy,UPPER(EmployeeName) Employee,EmployeeId,Description FROM TEMPLATE INNER JOIN (SELECT USER_ID EmployeeId,USER_NAME EmployeeName FROM BCUSERS)
                                    Employee on Employee.EmployeeId = TEMPLATE.Employee WHERE DbCode = @DbCode AND IsEnable = '1'";
            var parameter = new
            {
                DbCode = dbCode
            };
            var execute = await _sqlDataAccess.LoadData<SaleRepresentModel, dynamic>(sql, parameter);
            return execute.ToList();
        }

        public async Task<int> DeleteAsync(string code)
        {
            const string sql = "DELETE FROM TEMPLATE WHERE Id = @ID";
            var parameter = new
            {
                ID = code
            };
            var affectedRow = await _sqlDataAccess.ExecuteAsync(sql, parameter);
            return affectedRow;
        }
        public async Task<int> DisableTemplateById(string createBy, int templateId)
        {
            const string sql = "UPDATE TEMPLATE SET IsEnable = 0,ClosingDate = GETDATE(),ClosingBy = @ClosingBy WHERE Id = @ID";
            var parameter = new
            {
                ID = templateId,
                ClosingBy = createBy
            };
            var affectedRow = await _sqlDataAccess.ExecuteAsync(sql, parameter);
            return affectedRow;
        }
        public async Task<SaleRepresentModel> GetAllTemplateByEmployeId(int employeeId)
        {
            const string sql = "SELECT Id,Description,ClosingDate,CreateBy,Employee,DbCode,Description FROM TEMPLATE WHERE Employee = @Employee";
            var param = new { Employee = employeeId };
            var execute = await _sqlDataAccess.LoadData<SaleRepresentModel, dynamic>(sql, param);
            return execute.FirstOrDefault() ?? new SaleRepresentModel();
        }
        #endregion

        #region Invoice

        public async Task<List<OldInvoiceResponeDto>> GetAllInvoiceAsync(string dbCode, string fromSaleCode, string toSaleCode, DateTime fromDate, DateTime toDate)
        {
            var newResponds = new List<OldInvoiceResponeDto>();
            var sql = dbCode + "SELECT_NEW_INVOICE_DATE";
            var parameter = new
            {
                DB_CODE = dbCode,
                CODE_1 = fromSaleCode,
                CODE_2 = toSaleCode,
                FROM_DATE = fromDate,
                TO_DATE = toDate
            };
            var result = await _dbConnection.ExecuteReaderAsync(sql, parameter, commandType: CommandType.StoredProcedure);
            var table = new DataTable();
            table.Load(result);

            foreach (DataRow row in table.Rows)
            {
                var code = row["CODE"].ToString() ?? string.Empty;
                var customerCode = row["Customer Code"].ToString() ?? string.Empty;
                var customerName = row["Customer Name"].ToString() ?? string.Empty;
                var invoiceValue = Convert.ToDouble(row["Header Transaction Value"]);
                var transactionDate = Convert.ToDateTime(row["Header Transaction Date"]);
                var employee = row["Header Analysis M0"].ToString() ?? string.Empty;
                var oldRespond = new OldInvoiceResponeDto(transactionDate, customerCode, customerName, invoiceValue, employee, code);
                newResponds.Add(oldRespond);
            }
            return newResponds;
        }

        public async Task<List<StockCarInvoicesModel>> GetAllInvoiceByInvoiceTypeAsync(string dbCode, InvoiceTypes invoiceTypes, int templateId)
        {
            const string sql = @"
                       SELECT N.ID InvoiceId,CUSTOMER_CODE CustomerCode,CUSTOMER_NAME CustomerName,CODE TransRef,VALUE InvoiceValue,
	                        TRANSACTION_DATE TransactionDate,
	                        UPPER(Employee.USER_NAME) Employee,Market,Area
                         FROM BCSTOCK_CAR N INNER JOIN TEMPLATE T ON T.Id = N.TEMPLATE_ID 
	                        INNER JOIN (SELECT USER_NAME,USER_ID FROM BCUSERS) Employee
                         ON Convert(varchar,Employee.USER_ID) = T.Employee
	                        LEFT JOIN (SELECT S.ADD_CODE CustomerCode,M.MARKET_KHMER_NAME Market,A.AREA_NAME_KHMER Area
	                        FROM SIADD S INNER JOIN TB_BCMARKET M ON M.MARKET_ID = S.MARKET_ID
	                        INNER JOIN TB_AREAS A ON A.AREA_ID = S.AREA_ID
	                        WHERE S.DB_CODE = @DB_CODE AND A.DB_CODE = @DB_CODE AND M.DB_CODE = @DB_CODE) Cust
	                        on Cust.CustomerCode = CUSTOMER_CODE
                         WHERE N.DB_CODE = @DB_CODE and T.DbCode = @DB_CODE and N.TYPE = @InvoiceType and T.Id = @TemplateId
                        ";
            var invoiceType = Enum.GetName(typeof(InvoiceTypes), invoiceTypes)?.Substring(0, 1);
            var param = new
            {
                DB_CODE = dbCode,
                InvoiceType = invoiceType,
                TemplateId = templateId
            };
            var execute = await _sqlDataAccess.LoadData<StockCarInvoicesModel, dynamic>(sql, param);
            return execute.ToList();
        }

        public async Task<int> DeleteInvoiceByIdAsync(int invoiceId)
        {
            const string sql = "DELETE FROM BCSTOCK_CAR WHERE ID = @ID";
            var parameter = new
            {
                ID = invoiceId
            };
            var affectedRow = await _sqlDataAccess.ExecuteAsync(sql, parameter);
            return affectedRow;
        }

        public async Task<int> UpdateInvoiceByIdAsync(StockCarInvoicesModel model)
        {
            const string sql =
                "UPDATE BCSTOCK_CAR SET CUSTOMER_CODE = @CustomerCode,CUSTOMER_NAME = @CustomerName,VALUE = @Value Where ID = @Id";
            var parameter = new
            {
                CustomerCode = model.CustomerCode,
                CustomerName = model.CustomerName,
                Value = model.InvoiceValue,
                Id = model.InvoiceId
            };
            var affectedRow = await _sqlDataAccess.ExecuteAsync(sql, parameter);
            return affectedRow;
        }

        public async Task<List<OldInvoiceResponeDto>> GetOldInvoiceByRangeAsync(OldInvoiceRequestDto model)
        {
            var newResponds = new List<OldInvoiceResponeDto>();
            var storeProcedure = model.DbCode + "SI_SELECT_AGING";
            var param = new
            {
                DB_CODE = model.DbCode,
                BY_DATE = model.Date,
                FROM_ACC = model.FromAccount,
                TO_ACC = model.ToAccount,
                FROM_ANAL = model.FromAnal,
                TO_ANAL = model.ToAnal,
                T = "",
                T0 = "%",
                T1 = "%",
                T2 = "%",
                T3 = "%",
                T4 = "%",
                T5 = "%",
                T6 = "%",
                T7 = "%",
                T8 = "%",
                T9 = "%",
            };
            var result = await _dbConnection.ExecuteReaderAsync(storeProcedure, param, commandType: CommandType.StoredProcedure);
            var table = new DataTable();
            table.Load(result);
            foreach (DataRow row in table.Rows)
            {
                var code = row["TransactionCode"].ToString() ?? string.Empty;
                var customerCode = row["CustomerCode"].ToString() ?? string.Empty;
                var customerName = row["CustomerName"].ToString() ?? string.Empty;
                var invoiceValue = Convert.ToDouble(row["InvoiceValue"]);
                var transactionDate = Convert.ToDateTime(row["TransactionDate"]);
                var employee = row["Employee"].ToString() ?? string.Empty;
                var oldRespondByRange = new OldInvoiceResponeDto(transactionDate, customerCode, customerName, invoiceValue, employee, code);
                newResponds.Add(oldRespondByRange);
            }
            return newResponds;
        }

        public async Task<List<OldInvoiceResponeDto>> GetOldInvoiceByAllAsync(OldInvoiceRequestDto model)
        {
            var newResponds = new List<OldInvoiceResponeDto>();
            var storeProcedure = model.DbCode + "SI_SELECT_AGING";
            var param = new
            {
                DB_CODE = model.DbCode,
                BY_DATE = model.Date,
                FROM_ACC = model.FromAccount,
                TO_ACC = model.ToAccount,
                FROM_ANAL = model.FromAnal,
                TO_ANAL = model.ToAnal,
                T = "",
                T0 = model.T0,
                T1 = model.T1,
                T2 = model.T2,
                T3 = model.T3,
                T4 = model.T4,
                T5 = model.T5,
                T6 = model.T6,
                T7 = model.T7,
                T8 = model.T8,
                T9 = model.T9,
            };
            var result = await _dbConnection.ExecuteReaderAsync(storeProcedure, param, commandType: CommandType.StoredProcedure);
            var table = new DataTable();
            table.Load(result);
            foreach (DataRow row in table.Rows)
            {
                var code = row["TransactionCode"].ToString() ?? string.Empty;
                var customerCode = row["CustomerCode"].ToString() ?? string.Empty;
                var customerName = row["CustomerName"].ToString() ?? string.Empty;
                var invoiceValue = Convert.ToDouble(row["InvoiceValue"]);
                var transactionDate = Convert.ToDateTime(row["TransactionDate"]);
                var employee = row["Employee"].ToString() ?? string.Empty;
                var oldRespondByAll = new OldInvoiceResponeDto(transactionDate, customerCode, customerName, invoiceValue, employee, code);
                newResponds.Add(oldRespondByAll);
            }
            return newResponds;
        }

        public async Task<int> InsertInvoiceAsync(InvoicesModel model)
        {

            const string sql =
                "IF NOT EXISTS (SELECT * FROM BCSTOCK_CAR WHERE TEMPLATE_ID = @TemplateId and CODE = @Code)" +
                " INSERT INTO BCSTOCK_CAR Output inserted.ID Values(@DbCode, @CustomerCode, @CustomerName, @Code, @Value, @Type, @Period, @TransactionDate, @Status, @CreatedDate,@CreatedBy, @TemplateId)";
            var parameter = new
            {
                DbCode = model.DbCode,
                CustomerCode = model.CustomerCode,
                CustomerName = model.CustomerName,
                Code = model.InvoiceCode,
                Value = model.InvoiceAmount,
                Type = Enum.GetName(typeof(InvoiceTypes), model.InvoiceTypes)?.Substring(0, 1),
                Period = model.Period,
                TransactionDate = model.TransactionDate,
                Status = 1,
                CreatedDate = DateTime.Now,
                CreatedBy = model.CreatedBy,
                TemplateId = model.TemplateId,
            };

            var affectedRow = await _sqlDataAccess.ExecuteAsync(sql, parameter);
            return affectedRow;
        }

        #endregion

        #region Transfer Money

        public async Task<List<TransferMoneyModel>> GetTransferMoneyByTemplateIdAsync(string dbCode, int templateId)
        {
            const string sql =
                @"SELECT ID Id,TRANSACTION_DATE TransactionDate,AMOUNT Amount,DESCRIPTION Description,DEPOSIT_DOLLAR DepositDollar,DEPOSIT_RIEL DepositRiel,DEPOSIT_EXCHANGE DepositExchange,DOLLAR DollarFromEmployee,RIEL RielFromEmployee,EXCHANGE ExchangeRateEmployee,UPPER(username)
                Employee,Emp.userid EmployeeId FROM BCSTOCK_CAR_SENT_MONEY_DETAILS S INNER JOIN TEMPLATE T ON T.Id = S.TEMPLATE_ID INNER JOIN(SELECT userid, username FROM USERS WHERE dbcode = @DbCode) Emp on CONVERT(VARCHAR, Emp.userid) = T.Employee
                WHERE DB_CODE = @DbCode AND TEMPLATE_ID = @TemplateId";
            var param = new
            {
                DbCode = dbCode,
                TemplateId = templateId
            };
            var execute = await _sqlDataAccess.LoadData<TransferMoneyModel, dynamic>(sql, param);
            return execute.ToList();
        }

        public async Task<int> AddNewTransferMoneyAsync(TransferMoneyModel model)
        {
            const string sql =
                @"INSERT INTO BCSTOCK_CAR_SENT_MONEY_DETAILS(TRANSACTION_DATE,DESCRIPTION,AMOUNT,DOLLAR,RIEL,EXCHANGE,DEPOSIT_DOLLAR,DEPOSIT_RIEL,DEPOSIT_EXCHANGE,CREATED_DATE,CREATED_BY,DB_CODE,EMPLOYEE,TEMPLATE_ID)VALUES(@TransactionDate,@Description,@Amount,@Dollar,@Riel,@Exchange,@DepositDollar,@DopositRiel,@DepositExchange,@CreatedDate,@CreatedBy,@DbCode,@Employee,@TemplateId)";
            var parameter = new
            {
                TransactionDate = model.TransactionDate,
                Description = model.Description,
                Amount = model.Amount,
                Dollar = model.DollarFromEmployee,
                Riel = model.RielFromEmployee,
                Exchange = model.ExchangeRateEmployee,
                DepositDollar = model.DepositDollar,
                DopositRiel = model.DepositRiel,
                DepositExchange = model.DepositExchange,
                CreatedDate = DateTime.Now,
                CreatedBy = model.CreatedBy,
                DbCode = model.DbCode,
                Employee = model.EmployeeId,
                TemplateId = model.TemplateId
            };
            var affectedRow = await _sqlDataAccess.ExecuteAsync(sql, parameter);
            return affectedRow;
        }

        public async Task<int> UpdateTransferMoneyAsync(TransferMoneyModel model)
        {
            const string sql =
                @"UPDATE BCSTOCK_CAR_SENT_MONEY_DETAILS SET TRANSACTION_DATE = @TransactionDate,DESCRIPTION = @Description,AMOUNT = @Amount,DOLLAR =@Dollar,RIEL = @Riel,EXCHANGE = @ExchangeFromSale,
                DEPOSIT_DOLLAR = @DepositDollar,DEPOSIT_RIEL = @DepositRiel,DEPOSIT_EXCHANGE = @DepositExchange WHERE ID = @Id";
            var parameter = new
            {
                TransactionDate = model.TransactionDate,
                Description = model.Description,
                Amount = model.Amount,
                Dollar = model.DollarFromEmployee,
                Riel = model.RielFromEmployee,
                ExchangeFromSale = model.ExchangeRateEmployee,
                DepositDollar = model.DepositDollar,
                DepositRiel = model.DepositRiel,
                DepositExchange = model.DepositExchange,
                Id = model.Id
            };
            var affectedRow = await _sqlDataAccess.ExecuteAsync(sql, parameter);
            return affectedRow;
        }

        public async Task<int> DeleteTransferMoneyAsync(int transferId)
        {
            const string sql = @"DELETE FROM BCSTOCK_CAR_SENT_MONEY_DETAILS WHERE ID = @ID";
            var parameter = new
            {
                ID = transferId
            };
            var affectedRow = await _sqlDataAccess.ExecuteAsync(sql, parameter);
            return affectedRow;
        }

        #endregion

        #region Expense

        public async Task<List<StockCarExpenseModel>> GetExpenseByTemplateIdAsync(string dbCode, int templateId)
        {
            const string sql =
                @"
                        SELECT ID,UPPER(Employee.USER_NAME) [Employee],T.EXPENSE_NAME ExpenseType,E.EXPENS_TYPE_ID ExpenseTypeId,QTY Quantity,
                        UNIT_PRICE UnitPrice, AMOUNT_RIEL AmountRiel,AMOUNT_DOLLAR AmountDollar, P.PRO_NAME ProvinceName, P.PROID ProvinceId, EXCHANGE_RATE
                        ExchangeRate,DATE ExpenseDate
                        FROM BCSTOCK_CAR_EXPENSE E INNER JOIN BCSTOCK_CAR_EXPENSE_TYPE T ON T.EXPENSE_ID = E.EXPENS_TYPE_ID INNER JOIN PROVINCE P ON P.PROID =
                        E.PROVINCE_ID
                        INNER JOIN TEMPLATE TEMP ON TEMP.Id = E.TEMPLATE_ID INNER JOIN(SELECT USER_ID, USER_NAME FROM BCUSERS ) Employee on CONVERT(varchar, Employee.USER_ID) = TEMP.Employee
                        Where
                        E.DB_CODE = @DB_CODE AND
                        T.DB_CODE = @DB_CODE AND
                        E.TEMPLATE_ID = @TEMPLATE_ID
                        ";
            var param = new
            {
                DB_CODE = dbCode,
                TEMPLATE_ID = templateId
            };
            var execute = await _sqlDataAccess.LoadData<StockCarExpenseModel, dynamic>(sql, param);
            return execute.ToList();
        }

        public async Task<int> AddNewExpenseAsync(StockCarExpenseModel model)
        {
            const string sql =
                "INSERT INTO BCSTOCK_CAR_EXPENSE (DB_CODE,EXCHANGE_RATE,EXPENS_TYPE_ID,PROVINCE_ID,QTY,UNIT_PRICE,AMOUNT_DOLLAR,AMOUNT_RIEL,DATE,CREATED_BY,CREAETD_DATE,TEMPLATE_ID)\r\nOUTPUT inserted.ID\r\nVALUES (@DB_CODE,@EXCHANGE_RATE,@EXPENSE_TYPE_ID,@PROVINCE_ID,@QTY,@UNIT_PRICE,@AMOUNT_DOLLAR,@AMOUNT_RIEL,@DATE,@CREATED_BY,@CREATED_DATE," +
                "@TEMPLATE_ID)";
            var param = new
            {
                DB_CODE = model.DbCode,
                EXCHANGE_RATE = model.ExchangeRate,
                EXPENSE_TYPE_ID = model.ExpenseTypeId,
                PROVINCE_ID = model.ProvinceId,
                QTY = model.Quantity,
                UNIT_PRICE = model.UnitPrice,
                AMOUNT_DOLLAR = model.AmountDollar,
                AMOUNT_RIEL = model.AmountRiel,
                DATE = model.ExpenseDate,
                CREATED_BY = model.CreateBy,
                CREATED_DATE = DateTime.Now,
                TEMPLATE_ID = model.TemplateId
            };
            var affectedRow = await _sqlDataAccess.ExecuteAsync(sql, param);
            return affectedRow;
        }

        public async Task<int> UpdateExpenseAsync(StockCarExpenseModel model)
        {
            const string sql =
                @"UPDATE BCSTOCK_CAR_EXPENSE SET EXCHANGE_RATE = @EXCHANGE_RATE,EXPENS_TYPE_ID = 
                 @EXPENSE_TYPE_ID,PROVINCE_ID = @PROVINCE_ID,QTY = @QTY,UNIT_PRICE = @UNIT_PRICE,AMOUNT_DOLLAR = @AMOUNT_DOLLAR,AMOUNT_RIEL = @AMOUNT_RIEL,DATE = @DATE WHERE ID = @ID";
            var parameter = new
            {
                EXCHANGE_RATE = model.ExchangeRate,
                EXPENSE_TYPE_ID = model.ExpenseTypeId,
                PROVINCE_ID = model.ProvinceId,
                QTY = model.Quantity,
                UNIT_PRICE = model.UnitPrice,
                AMOUNT_DOLLAR = model.AmountDollar,
                AMOUNT_RIEL = model.AmountRiel,
                DATE = model.ExpenseDate,
                ID = model.Id
            };
            var affectedRow = await _sqlDataAccess.ExecuteAsync(sql, parameter);
            return affectedRow;
        }

        public async Task<int> DeleteExpenseAsync(int expenseId)
        {
            const string sql = "DELETE FROM BCSTOCK_CAR_EXPENSE WHERE ID = @ID";
            var parameter = new
            {
                ID = expenseId
            };
            var affectedRow = await _sqlDataAccess.ExecuteAsync(sql, parameter);
            return affectedRow;
        }

        #endregion

        // Expense

        #region Payment Invoice
        public async Task<List<PaymentInvoiceDto>> GetPaymentInvoicesByTemplateIdAsync(string dbCode, int templateId)
        {
            const string sql = @"
                        SELECT C.ID InvoiceId,CUSTOMER_CODE CustomerCode,CUSTOMER_NAME CustomerName,CODE TransactionCode,CASE WHEN TYPE = 'N' THEN N'ថ្មី'
                        WHEN TYPE = 'C' THEN N'ដូរ'
                        WHEN TYPE = 'O' THEN N'ចាស់' ELSE N'មិនស្គាល់
                        ' END 'InvoiceType',VALUE InvoiceValue,SUM(P.AMOUNT) AmountPaid,C.STATUS Status,Cust.Market,Area
                        FROM BCSTOCK_CAR C INNER JOIN TEMPLATE T ON T.Id = C.TEMPLATE_ID LEFT JOIN BCSTOCK_CAR_INVOICE_PAYMENT P 
						ON P.INVOICE_ID = C.ID
                        LEFT JOIN (SELECT S.ADD_CODE CustomerCode,M.MARKET_KHMER_NAME Market,A.AREA_NAME_KHMER Area
						FROM SIADD S INNER JOIN TB_BCMARKET M ON M.MARKET_ID = S.MARKET_ID
						INNER JOIN TB_AREAS A ON A.AREA_ID = S.AREA_ID
						WHERE S.DB_CODE = @DB_CODE AND A.DB_CODE = @DB_CODE AND M.DB_CODE = @DB_CODE) Cust
						on Cust.CustomerCode = CUSTOMER_CODE
						WHERE C.DB_CODE = @DB_CODE AND T.DbCode = @DB_CODE AND T.Id = @TEMPLATE_ID
						AND C.ID NOT IN (SELECT INVOICE_ID FROM BCSTOCK_CAR_INVOICE_RETURN)
                        GROUP BY C.ID,CUSTOMER_CODE ,CUSTOMER_NAME ,CODE,Cust.Market,Area,
						VALUE,C.STATUS,TYPE ORDER BY STATUS
                        ";
            var parameter = new
            {
                DB_CODE = dbCode,
                TEMPLATE_ID = templateId
            };
            var execute = await _sqlDataAccess.LoadData<PaymentInvoiceDto, dynamic>(sql, parameter);
            return execute.ToList();
        }

        public Task<PaymentInvoiceDto> GetPaymentInvoicesByTransactionCodeAsync(string dbCode, string transactionCode)
        {
            throw new NotImplementedException();
        }

        public async Task<int> InsertPaymentInvoiceAsync(PaymentInvoiceDto model)
        {
            if (_dbConnection.State==ConnectionState.Closed)
                _dbConnection.Open();

            var transaction = _dbConnection.BeginTransaction();
            try
            {
                const string sql =
                    "INSERT INTO BCSTOCK_CAR_INVOICE_PAYMENT VALUES (@InvoiceId,@Amount,@CreatedDate,@CreatedBy)";
                const string sqlUpdateStaysInvoice = "Update BCSTOCK_CAR SET STATUS = 0 WHERE ID = @InvoiceId";
                var parameterUpdateStatus = new
                {
                    InvoiceId = model.InvoiceId
                };
                var parameter = new
                {
                    InvoiceId = model.InvoiceId,
                    Amount = model.AmountPaid,
                    CreatedDate = DateTime.Now,
                    CreatedBy = model.CreateBy
                };
                var affectedRow = await _dbConnection.ExecuteAsync(sql, parameter, transaction);
                if (affectedRow > 0)
                {
                    await _dbConnection.ExecuteAsync(sqlUpdateStaysInvoice, parameterUpdateStatus, transaction);
                    transaction.Commit();
                    return 1;
                }
                else
                {
                    transaction.Rollback();
                    return 0;
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Debug.WriteLine("Error Exception : " +ex.Message);
            }
            return 0;
        }
        #endregion


        #region Returnning Invoice

        public async Task<List<ReturningInvoiceDto>> GetReturningInvoicesByTemplateIdAsync(string dbCode, int templateId)
        {
            const string sql = @"
                         SELECT C.ID InvoiceId,CUSTOMER_CODE CustomerCode,CUSTOMER_NAME CustomerName,CODE TransactionCode,VALUE InvoiceValue,
						 R.DESCRIPTION Description,STATUS Status,R.CREATED_BY CreatedBy,CASE WHEN TYPE = 'N' THEN N'ថ្មី' ELSE N'ដូរ' END InvoiceType,
						 Market,Area
                        FROM BCSTOCK_CAR C LEFT JOIN BCSTOCK_CAR_INVOICE_RETURN R ON R.INVOICE_ID = C.ID
						LEFT JOIN (SELECT S.ADD_CODE CustomerCode,M.MARKET_KHMER_NAME Market,A.AREA_NAME_KHMER Area
						FROM SIADD S INNER JOIN TB_BCMARKET M ON M.MARKET_ID = S.MARKET_ID
						INNER JOIN TB_AREAS A ON A.AREA_ID = S.AREA_ID
						WHERE S.DB_CODE = @DB_CODE AND A.DB_CODE = @DB_CODE AND M.DB_CODE = @DB_CODE) Cust
						on Cust.CustomerCode = CUSTOMER_CODE
                        WHERE C.ID NOT IN (SELECT INVOICE_ID FROM BCSTOCK_CAR_INVOICE_PAYMENT)
                        AND C.TEMPLATE_ID = @TemplateId AND C.TYPE IN ('N','C')
                        ";
            var param = new
            {
                TemplateId = templateId,
                DB_CODE = dbCode
            };
            var execute = await _sqlDataAccess.LoadData<ReturningInvoiceDto, dynamic>(sql, param);
            return execute.ToList();
        }

        public async Task<int> AddNewReturningInvoiceAsync(ReturningInvoiceDto model)
        {
            const string sql =
                "insert into BCSTOCK_CAR_INVOICE_RETURN values (@Invoiceid,@Description,convert(date,getdate()),@CreatedBy)";
            var param = new
            {
                InvoiceId = model.InvoiceId,
                Description = model.Description,
                CreatedBy = model.CreateBy,
            };
            var affectedRow =await _sqlDataAccess.ExecuteAsync(sql, param);
            return affectedRow;
        }

        #endregion

        #region Payment
        public async Task<List<PaymentModel>> GetPaymentByTemplateIdAsync(int templateId)
        {
            const string sql = @"
                     SELECT DESCRIPTION Description,CREDITAMOUNT CreditAmount,PAYMENTAMOUNT PaymentAmount FROM (
                     SELECT CASE WHEN DESCRIPTION = 'N' THEN N'ថ្មី' WHEN DESCRIPTION = 'C' THEN N'ដូរ' WHEN DESCRIPTION = 'O' THEN N'ចាស់'
                     ELSE 'UNKNOW' END 'DESCRIPTION' ,SUM(CREDITAMOUNT)CREDITAMOUNT,SUM(PAYMENTAMOUNT)PAYMENTAMOUNT FROM (
                     SELECT CASE WHEN BC.TYPE = 'N' THEN 'N' WHEN BC.TYPE = 'C' THEN 'C' WHEN BC.TYPE = 'O' THEN 'O'
                     ELSE 'UNKNOW' END 'DESCRIPTION',ISNULL(CASE WHEN BC.STATUS = 1 THEN SUM(BC.VALUE) END,0)  CREDITAMOUNT,
                     ISNULL(SUM(BCIP.AMOUNT), 0) PAYMENTAMOUNT FROM BCSTOCK_CAR BC
                     LEFT JOIN BCSTOCK_CAR_INVOICE_PAYMENT BCIP ON BCIP.INVOICE_ID = BC.ID
				     WHERE TEMPLATE_ID =@TemplateId GROUP BY TYPE,STATUS) TAB GROUP BY DESCRIPTION )TAB ";
            var param = new
            {
                TemplateId = templateId
            };
            var execute = await _sqlDataAccess.LoadData<PaymentModel, dynamic>(sql, param);
            return execute.ToList();
        }
        public async Task<List<CollectionPaymentModel>> GetAllTotalCollectionByTemplateIdAsync(int templateId)
        {
            const string sql =
                $@"SELECT AmountExpense,AmountTransfer AmountTransfer,AmountBiased,AmountTransfer +
                 AmountExpense + ABS(AmountBiased) Total FROM (
                 select AmountCollection, AmountTransfer +AmountTransferRiel AmountTransfer,AmountExpense,AmountCollection -
                 (AmountTransfer + AmountTransferRiel) AmountBiased from
                 (select SUM(MoneyFromSaleDollar +(MoneyFromSaleRiel / MoneyFromSaleExchange)) AmountCollection,
                 SUM(TransferDollar) AmountTransfer,SUM(TransferDollarFromRiel) AmountTransferRiel,(SELECT SUM(AMOUNT_DOLLAR) FROM
                 BCSTOCK_CAR_EXPENSE WHERE TEMPLATE_ID = tab.TEMPLATE_ID) AmountExpense
                 from( SELECT TEMPLATE_ID, DOLLAR MoneyFromSaleDollar, RIEL MoneyFromSaleRiel, EXCHANGE MoneyFromSaleExchange, DEPOSIT_DOLLAR
                 TransferDollar,DEPOSIT_RIEL / DEPOSIT_EXCHANGE TransferDollarFromRiel FROM BCSTOCK_CAR_SENT_MONEY_DETAILS
                 WHERE TEMPLATE_ID = @TemplateId) tab Group by TEMPLATE_ID) tab) tab";

            var param = new
            {
                TemplateId = templateId
            };
            var execute = await _sqlDataAccess.LoadData<CollectionPaymentModel, dynamic>(sql, param);
            return execute.ToList();
        }

        #endregion

        #region Credit Invoice
        public async Task<List<StockCarCreditInvoiceDto>> GetAllCreditInvoiceByTemplateIdAsync(string dbCode,int templateId)
        {
            const string sql = @"SELECT CUSTOMER_CODE CustomerCode,CUSTOMER_NAME CustomerName,
						CODE TransactionCode, VALUE InvoiceValue,TYPE Type,
						CASE WHEN STATUS = 0 THEN '1' END 'Return',
						CASE WHEN STATUS = 1 THEN '1' END 'Credit',
						BCIR.DESCRIPTION[Description],
						Market,Area
						FROM BCSTOCK_CAR BC
						LEFT JOIN BCSTOCK_CAR_INVOICE_RETURN BCIR
						ON BCIR.INVOICE_ID = BC.ID
						LEFT JOIN (SELECT S.ADD_CODE CustomerCode,M.MARKET_KHMER_NAME Market,A.AREA_NAME_KHMER Area
						FROM SIADD S INNER JOIN TB_BCMARKET M ON M.MARKET_ID = S.MARKET_ID
						INNER JOIN TB_AREAS A ON A.AREA_ID = S.AREA_ID
						WHERE S.DB_CODE = @DB_CODE AND A.DB_CODE = @DB_CODE AND M.DB_CODE = @DB_CODE) Cust
						on Cust.CustomerCode = CUSTOMER_CODE
						WHERE TEMPLATE_ID = @TemplateId AND BC.ID NOT IN
						(SELECT INVOICE_ID FROM BCSTOCK_CAR_INVOICE_PAYMENT)";
            var param = new
            {
                TemplateId = templateId,
                DB_CODE = dbCode
            };
            var execute = await _sqlDataAccess.LoadData<StockCarCreditInvoiceDto, dynamic>(sql, param);
            return execute.ToList();
        }

        #endregion
    }
}
