using System.Data;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.APPLICATION.Interfaces.Invoice;
using BC.PAYMENT.CORE.DTO.Filter;
using BC.PAYMENT.CORE.DTO.Invoice;
using BC.PAYMENT.CORE.Entities.Invoice;
using BC.PAYMENT.CORE.Enums;
using BC.PAYMENT.INFRASTRUCTURE.DBAccess;
using BC.PAYMENT.INFRASTRUCTURE.Helper;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Invoice
{
    public class InvoiceRepository : IInvoiceRepository
    {
        #region Private Field
        private readonly ISqlDataAccess _sqlDataAccess;
        private readonly ICustomerRepository _customerRepository;
        private readonly IConfiguration _configuration;
        private readonly IDbConnection _dbConnection;

        #endregion

        #region Constructor
        public InvoiceRepository(ISqlDataAccess sqlDataAccess, ICustomerRepository customerRepository, IConfiguration configuration, IDbConnection dbConnection)
        {
            _sqlDataAccess = sqlDataAccess;
            _customerRepository = customerRepository;
            _configuration = configuration;
            _dbConnection = dbConnection;
        }

        #endregion

        #region New Invoices
        public async Task<List<NewInvoiceModel>> GetInvoiceByInvoiceCode(NewInvoicesRequestDTO newInvoiceRequestCodeDto)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DBConnection"));
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            // Execute stored procedure
            var sqlCommand = new SqlCommand($"{newInvoiceRequestCodeDto.DbCode}SELECT_NEW_INVOICE_DATE", connection);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Parameters.AddWithValue("@DB_CODE", newInvoiceRequestCodeDto.DbCode);
            sqlCommand.Parameters.AddWithValue("@CODE_1", newInvoiceRequestCodeDto.InvoiceCode1);
            sqlCommand.Parameters.AddWithValue("@CODE_2", newInvoiceRequestCodeDto.InvoiceCode2);
            sqlCommand.Parameters.AddWithValue("@FROM_DATE", newInvoiceRequestCodeDto.FromDate);
            sqlCommand.Parameters.AddWithValue("@TO_DATE", newInvoiceRequestCodeDto.ToDate);

            var reader = await sqlCommand.ExecuteReaderAsync();

            // Parse results from database query
            var result = new List<NewInvoiceModel>();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    var transactionCode = reader.GetString(0);         // InvoiceCode
                    var customerCode = reader.GetString(41);           // CustomerCode
                    var customerName = reader.GetString(43);           // CustomerName
                    var invoiceValue = Convert.ToDouble(reader[16]);   // InvoiceAmount

                    result.Add(new NewInvoiceModel
                    {
                        InvoiceCode = transactionCode,
                        CustomerName = customerName,
                        CustomerCode = customerCode,
                        InvoiceAmount = invoiceValue,
                        DbCode = newInvoiceRequestCodeDto.DbCode!,
                        EntriesCode = newInvoiceRequestCodeDto.EntriesCode!,
                        CreatedBy = newInvoiceRequestCodeDto.Username!,
                        InvoiceTypes = newInvoiceRequestCodeDto.InvoiceTypes,
                    });
                }
            }
            reader.Close();

            // Fetch Customers and Perform Join Operation in One Step
            var customers = await _customerRepository.GetCustomer();
            var invoice = customers.Join(result,
                invoice => invoice.CustomerCode,
                customer => customer.CustomerCode,
                (customer, invoice) => new NewInvoiceModel
                {
                    DbCode = invoice.DbCode,
                    InvoiceCode = invoice.InvoiceCode,
                    CustomerName = customer.CustomerName,
                    CustomerCode = customer.CustomerCode,
                    InvoiceAmount = invoice.InvoiceAmount,
                    Market = customer.Market,
                    Area = customer.Area,
                    Store = customer.Store,
                    CreatedBy = invoice.CreatedBy,
                    EntriesCode = invoice.EntriesCode,
                }).ToList();
            return invoice;
        }

        public async Task<List<InvoicesModel>> GetInvoices(InvoiceTypes type, string dbCode, DateTime createdDate)
        {
            const string sql = @"SELECT ID InvoiceId,TRANSACTION_REF InvoiceCode,CUS.*,HEADER_TRANSACTION_VALUES InvoiceAmount,
                CASE WHEN N.STATUS = 'C' THEN 'ChangeInvoice'
                WHEN N.STATUS = 'N' THEN 'NewInvoice'
                WHEN N.STATUS = 'O' THEN 'OldInvoice' END [InvoiceType],
                CREATED_DATE CreatedDate,
                CREATED_BY CreatedBy,
                IS_DIVIDED IsDivided,
                ENTRIES_CODE EntriesCode
                FROM NEW_INVOICE N LEFT JOIN GET_CUSTOMERS_BY_DB_CODE() CUS ON CUS.CustomerCode = N.CUSTOMER_CODE
                WHERE N.DB_CODE = @DB_CODE AND STATUS = @STATUS AND N.CREATED_DATE = @CREATED_DATE";
            var param = new
            {
                DB_CODE = dbCode,
                CREATED_DATE = createdDate.Date,
                STATUS = type switch
                {
                    InvoiceTypes.NewInvoice => "N",
                    InvoiceTypes.OldInvoice => "O",
                    InvoiceTypes.ChangeInvoice => "C",
                    _ => "N"
                }
            };
            var invoicesModels = (await _sqlDataAccess.LoadData<InvoicesModel, dynamic>(sql, param)).ToList();
            return invoicesModels;
        }

        public async Task SaveInvoices(List<NewInvoiceModel> invoices, DateTime createdDate)
        {
            const string sql = @"
                IF NOT EXISTS (
                    SELECT 1 FROM NEW_INVOICE 
                    WHERE TRANSACTION_REF = @INVOICE_CODE 
                      AND CREATED_DATE = CONVERT(DATE, GETDATE())
                      AND STATUS = @STATUS 
                      AND DB_CODE = @DB_CODE 
                      AND IS_DIVIDED = 1
                )
                INSERT INTO NEW_INVOICE 
                (
                    DB_CODE, TRANSACTION_REF, CUSTOMER_CODE, ACC_NAME_KH, HEADER_TRANSACTION_VALUES, 
                    STATUS, CREATED_DATE, CREATED_BY, IS_DIVIDED, ENTRIES_CODE
                )
                VALUES 
                (
                    @DB_CODE, @INVOICE_CODE, @CUSTOMER_CODE, @CUSTOMER_NAME, @INVOICE_VALUE, 
                    @STATUS, GETDATE(), @CREATED_BY, @IS_DIVIDED, @ENTRIES_CODE
                )";


            // Map parameters outside the loop
            var parameters = invoices.Select(invoice => new
            {
                DB_CODE = invoice.DbCode,
                INVOICE_CODE = invoice.InvoiceCode,
                CUSTOMER_CODE = invoice.CustomerCode,
                CUSTOMER_NAME = invoice.CustomerName,
                INVOICE_VALUE = invoice.InvoiceAmount,
                STATUS = MapInvoiceType(invoice.InvoiceTypes),
                CREATED_DATE = createdDate,
                CREATED_BY = invoice.CreatedBy,
                IS_DIVIDED = true,
                ENTRIES_CODE = invoice.EntriesCode
            });

            // Perform batch execution
            var results = await _sqlDataAccess.ExecuteAsync(sql, parameters);
            Console.WriteLine($"{results} rows inserted.");

        }

        private string MapInvoiceType(InvoiceTypes type) => type switch
        {
            InvoiceTypes.NewInvoice => "N",
            InvoiceTypes.OldInvoice => "O",
            InvoiceTypes.ChangeInvoice => "C",
            _ => "N"
        };

        #endregion

        #region Old Invoices
        public async Task<List<OldInvoicesModel>> GetAllOldInvoices(OldInvoiceRequestDto oldInvoiceRequestDto)
        {

            var sql =
                $@"{oldInvoiceRequestDto.DbCode}PM_SELECT_AGING";
            var parameter = new
            {
                DB_CODE = oldInvoiceRequestDto.DbCode,
                BY_DATE = oldInvoiceRequestDto.Date,
                FROM_ACC = oldInvoiceRequestDto.FromAccount,
                TO_ACC = oldInvoiceRequestDto.ToAccount,
                ACC_TYPE = "D",
                T = "",
                FROM_ANAL = oldInvoiceRequestDto.FromAnal,
                TO_ANAL = oldInvoiceRequestDto.ToAnal,
                oldInvoiceRequestDto.T0,
                oldInvoiceRequestDto.T1,
                oldInvoiceRequestDto.T2,
                oldInvoiceRequestDto.T3,
                oldInvoiceRequestDto.T4,
                oldInvoiceRequestDto.T5,
                oldInvoiceRequestDto.T6,
                oldInvoiceRequestDto.T7,
                oldInvoiceRequestDto.T8,
                oldInvoiceRequestDto.T9,
                //OFF_SET = offset,
                //PAGESIZE = pageSize

            };
            var oldInvoiceList = await _sqlDataAccess.LoadData<OldInvoicesModel, dynamic>(sql, parameter, CommandType.StoredProcedure);
            return oldInvoiceList.ToList();

        }

        public async Task<List<OldInvoicesModel>> GetAllOldInvoiceByStatus(string dbCode, int offset, int pageSize)
        {
            const string sql =
                @"PM_SELECT_OLDINV";
            var param = new
            {
                DB_CODE = dbCode,
                OFF_SET = offset,
                PAGESIZE = pageSize
            };
            var result = await _sqlDataAccess.LoadData<OldInvoicesModel, dynamic>(sql, param, CommandType.StoredProcedure);
            return result.ToList();
        }

        public async Task<int> SaveOldInvoice(List<OldInvoiceDTO> dto)
        {
            const string sql = "INSERT_OLD_INVOICE";

            var dataTable = AppExtension.ConvertToDataTable(dto);

            // Define the parameter for the stored procedure
            var param = new DynamicParameters();
            param.Add("@OldInvoice", dataTable.AsTableValuedParameter("OLD_INVOICES")); // Specify table type name

            var rowAffected = await _sqlDataAccess.ExecuteAsync(sql, param, CommandType.StoredProcedure);
            return rowAffected;
        }

        public async Task<int> UpdateStatusOldInvoiceByTransactionCode(InvoiceDTO invoiceDto)
        {
            const string
                sql = @"IF EXISTS(SELECT * FROM OLD_INVOICE WHERE CODE = @CODE AND DB_CODE = @DB_CODE AND CREATE_DATE = CONVERT(DATE, GETDATE()))
	                        INSERT INTO NEW_INVOICE(DB_CODE, TRANSACTION_REF, CUSTOMER_CODE, ACC_NAME_KH, HEADER_TRANSACTION_VALUES,
	                        STATUS, CREATED_DATE, CREATED_BY, IS_DIVIDED,ENTRIES_CODE) 
	                        SELECT DB_CODE,  CODE,  CUSTOMER_CODE, ACC_NAME_KH, MONEY, 'O', CONVERT(DATE, GETDATE()), @USER_CREATED, 1, @ENTRIES_CODE 
	                        FROM OLD_INVOICE WHERE CODE = @CODE AND DB_CODE = @DB_CODE AND STATUS = 1 AND CREATE_DATE = CONVERT(DATE, GETDATE()) 
                        IF @@ROWCOUNT > 0 
	                        UPDATE OLD_INVOICE SET STATUS = 0 WHERE CODE = @CODE AND CREATE_DATE = CONVERT(DATE, GETDATE())";
            var parameter = new { DB_CODE = invoiceDto.DbCode, USER_CREATED = invoiceDto.CreatedBy, CODE = invoiceDto.TransactionCode, ENTRIES_CODE = invoiceDto.EntryCode };

            var rowAffected = await _sqlDataAccess.ExecuteAsync(sql, parameter);
            return rowAffected;
        }

        public async Task<OldInvoicesModel> GetOneOldInvoiceByTransactionCode(string dbCode, string transactionCode)
        {
            const string sql =
                @"  SELECT ID InvoiceId, CODE TransactionCode, CUSTOMER_CODE CustomerCode, ACC_NAME_KH CustomerName, MONEY InvoiceValue, EMPLOYEE AnalysisT0, ISNULL(STORE, '-') 
               Store, ISNULL(MARKET_KHMER_NAME, '-') Market, STATUS Status FROM(SELECT ID, CODE, CUSTOMER_CODE, ACC_NAME_KH, MONEY, ISNULL(LAST_NAME + ' ' + FIRST_NAME, EMPLOYEE)[EMPLOYEE],
               O.STATUS FROM OLD_INVOICE O LEFT JOIN BCUSERS S ON CONVERT(varchar, S.USER_ID) = O.EMPLOYEE WHERE DB_CODE = @DB_CODE AND O.CREATE_DATE = CONVERT(DATE, GETDATE()) AND CODE = @CODE)
               TAB1 LEFT JOIN(SELECT ADD_CODE, STORE, MARKET_KHMER_NAME FROM SIADD C INNER JOIN TB_BCMARKET M ON M.MARKET_ID = C.MARKET_ID WHERE STORE
               IS NOT NULL AND M.DB_CODE = @DB_CODE AND C.DB_CODE = @DB_CODE) TAB2
               ON TAB2.ADD_CODE = TAB1.CUSTOMER_CODE";
            var param = new { DB_CODE = dbCode, CODE = transactionCode };
            var result = await _sqlDataAccess.LoadSingleData<OldInvoicesModel, dynamic>(sql, param);
            return result;
        }

        public async Task<int> UpdateStatus(string dbCode, string transaction)
        {
            const string sql =
                @"UPDATE OLD_INVOICE SET IS_VERIFY = 0 WHERE CODE = @CODE AND DB_CODE = @DB_CODE AND CREATE_DATE = CONVERT(DATE,GETDATE())";
            var param = new { CODE = transaction, DB_CODE = dbCode, CREATED_DATE = DateTime.Today };
            var result = await _sqlDataAccess.ExecuteAsync(sql, param);
            return result;
        }

        #endregion

        #region Change and fix invoice
        public async Task<List<ChangeInvoice>> GetLocalChangeAndFixInvoice(FilterDTO dto)
        {
            const string sql =
                @"PM_SELECT_CHANGE_FIX_INVOICE_LOCAL";
            var param = new
            {
                DB_CODE = dto.DbCode,
                FROM_DATE = dto.FromDate,
                TO_DATE = dto.ToDate,
                OFF_SET = dto.Page,
                PAGESIZE = dto.PageSize,
            };
            var result = await _sqlDataAccess.LoadData<ChangeInvoice, dynamic>(sql, param, CommandType.StoredProcedure);
            return result.ToList();
        }

        public async Task<List<ChangeInvoice>> GetChangeAndFixInvoice(FilterDTO dto)
        {
            const string sql =
                @"PM_SELECT_CHANGE_FIX_INVOICE";
            var param = new
            {
                DB_CODE = dto.DbCode,
                FROM_DATE = dto.FromDate,
                TO_DATE = dto.ToDate,
                OFF_SET = dto.Page,
                PAGESIZE = dto.PageSize,
            };
            var result = await _sqlDataAccess.LoadData<ChangeInvoice, dynamic>(sql, param, CommandType.StoredProcedure);
            return result.ToList();
        }

        public async Task<int> SaveFixInvoice(SaveInvoiceDTO dto)
        {
            const string sql =
                @"PM_SAVE_EXCH_FIX_INVOICE";
            var param = new
            {
                DB_CODE = dto.DbCode,
                TRANSACTION = dto.TransactionCode,
                CUSTOMER_CODE = dto.CustomerCode,
                CUSTOMER_NAME = dto.CustomerName,
                VALUE = dto.InvoiceValue,
                CREATED_BY = dto.CreatedBy,
                ENTRIES_CODE = dto.EntryCode
            };
            var result = await _sqlDataAccess.ExecuteAsync(sql, param, CommandType.StoredProcedure);
            return result;
        }

        #endregion

        #region Return Invoices
        public async Task<List<ReturnInvoice>> GetReturnInvoice(FilterDTO dto)
        {
            const string sql =
                @"PM_SELECT_RETURN_INVOICE";
            var param = new
            {
                DB_CODE = dto.DbCode,
                FROM_DATE = dto.FromDate,
                TO_DATE = dto.ToDate,
                OFF_SET = dto.Page,
                PAGESIZE = dto.PageSize,
            };
            var result = await _sqlDataAccess.LoadData<ReturnInvoice, dynamic>(sql, param, CommandType.StoredProcedure);
            return result.ToList();
        }

        public async Task<List<ReturnInvoice>> GetTodayReturnInvoice(string dbCode, int offset, int pagesize)
        {
            const string sql =
                @"PM_SELECT_TODAYS_RETURN_INVOICES";
            var param = new
            {
                DB_CODE = dbCode,
                OFF_SET = offset,
                PAGESIZE = pagesize,
            };
            var result = await _sqlDataAccess.LoadData<ReturnInvoice, dynamic>(sql, param, CommandType.StoredProcedure);
            return result.ToList();
        }

        public async Task<int> SaveReturnInvoice(SaveInvoiceDTO dto)
        {
            const string sql =
                @"PM_INSERT_RETURN_INVOICE";
            var param = new
            {
                DB_CODE = dto.DbCode,
                TRANSACTION = dto.TransactionCode,
                CUSTOMER_CODE = dto.CustomerCode,
                CUSTOMER_NAME = dto.CustomerName,
                VALUE = dto.InvoiceValue,
                CREATED_BY = dto.CreatedBy,
                ENTRIES_CODE = dto.EntryCode
            };
            var result = await _sqlDataAccess.ExecuteAsync(sql, param, CommandType.StoredProcedure);
            return result;
        }

        public async Task<bool> PostPrintInvoiceAsync(string transactionInvoice, RequestType type, string dbCode,string period,string  userName)
        {
            var proName = @$"INSERT INTO {dbCode}SISOHDR
            SELECT 'I', @NEW_TRANS_REF, HEADER_ID, CUST_CODE, DELIV_ADD, TRANS_DATE, '85',
                TRANS_CD, TRANS_CODE, ORDER_NO, ORDER_DATE, PRN_DATE, DEL_DATE, @INV_DATE, @INV_PRD,
                CUST_REF, DEL_REF, COMMENTS, TRANS_VAL, PAY_DATE, ANAL_M0, ANAL_M1, ANAL_M2, ANAL_M3,
                ANAL_M4, ANAL_M5, ANAL_M6, ANAL_M7, ANAL_M8, ANAL_M9, QUOTE_CONVERT, QUOTE_PRINT,
                QUOTE_EXPIRY, QUOTED_PRD, QUOTATION_REF, DATE_QUOTED, VOID_STATUS, USER_CODE
            FROM {dbCode}SISOHDR
            WHERE REC_TYPE='O' AND TRANS_REF=@TRANS_REF AND VOID_STATUS='N'";
            var param = new
            {
                NEW_TRANS_REF = transactionInvoice,
                TRANS_REF = transactionInvoice,
                INV_DATE = DateTime.Today.ToString("MM/dd/yyyy"),
                INV_PRD = period
            };
            if (_dbConnection.State == ConnectionState.Closed) _dbConnection.Open();
            using var transaction = _dbConnection.BeginTransaction();
            try
            {
                await _dbConnection.ExecuteAsync($"{dbCode}_SISOPRINT_INVOICE", new
                {
                    DB_CODE = dbCode,
                    TRANS_REF = transactionInvoice,
                    INV_DATE = DateTime.Today.ToString("MM/dd/yyyy"),
                    NEW_TRANS_REF = transactionInvoice,
                    INV_PRD = period,
                    USER_INVOICED = userName
                }, transaction, commandType: CommandType.StoredProcedure);
                await _dbConnection.ExecuteAsync(proName, param, transaction);
                await _dbConnection.ExecuteAsync(
                    @$"UPDATE {dbCode}SISOHDR SET STATUS='80' WHERE REC_TYPE='O' AND TRANS_REF=@TRANS_REF AND VOID_STATUS='N'",
                    new
                    {
                        TRANS_REF = transactionInvoice
                    },
                    transaction
                );
                await _dbConnection.ExecuteAsync(
                    $@"UPDATE {dbCode}SISODET SET STATUS='80',DEL_DATE=(CASE WHEN DEL_DATE='' THEN @INV_DATE ELSE DEL_DATE END),
                     INV_DATE=@INV_DATE,INV_PRD=@INV_PRD,INV_NO=@NEW_TRANS_REF,USER_INVOICED=@USER_INVOICED WHERE TRANS_REF=@TRANS_REF AND REC_TYPE='D'",
                    new
                    {
                        INV_DATE = DateTime.Today.ToString("MM/dd/yyyy"),
                        INV_PRD = period,
                        NEW_TRANS_REF = transactionInvoice,
                        USER_INVOICED = userName,
                        TRANS_REF = transactionInvoice
                    }, transaction);
                if (type == RequestType.Exchange)
                    await _dbConnection.ExecuteAsync(
                        "UPDATE TB_BC_CHANGEINVOICE_EXCHANGE_INVOICE SET STATUS = 1 WHERE TRANSACTION_CODE = @TRANSACTION_CODE",
                        new { TRANSACTION_CODE = transactionInvoice }, transaction);
                else
                    await _dbConnection.ExecuteAsync(
                        "UPDATE TB_BC_CHANGEINVOICE_REPAIR_INVOICE SET STATUS = 1 WHERE INVOICE_NUMBER = @TRANSACTION_CODE",
                        new { TRANSACTION_CODE = transactionInvoice }, transaction);
                transaction.Commit();
                return true;
            }
            catch (SqlException e)
            {
                transaction.Rollback();
                return false;
            }
        }

        #endregion

    }
}
