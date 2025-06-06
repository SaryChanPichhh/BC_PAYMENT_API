using System.Data;
using System.Diagnostics;
using System.Globalization;
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

        #region Expired Operations
        public async Task<List<SaleDetailsDto>> GetItemExpiredDates(string dbCode, Dictionary<string, List<string>> itemCodeAndExpireDate, string wareHouse)
        {
            List<SaleDetailsDto> result = new();
            string sql =
                @$"SELECT TOP 1 ISNULL(ITEM_CODE,ITEMCODE) ItemCodeCopy,PHYSICAL Physical, ISNULL(HOLD_SALE, 0) OnHold, PHYSICAL-ISNULL(HOLD_SALE, 0) Fees, TAB1.EXPIRE_DATE LineRef 
                  FROM (SELECT ITEM_CODE ITEMCODE, ISNULL(SUM(QUANTITY),0) PHYSICAL, LINE_REF EXPIRE_DATE FROM {dbCode}SIINVMOV TAB1 
                  WHERE IR_STAT='I' AND STATUS='80' AND LOCATION = @WAREHOUSE AND TAB1.ITEM_CODE=@ITEM_CODE AND ALLOC_REF='' 
                  GROUP BY LOCATION,TAB1.ITEM_CODE, LINE_REF) TAB1 LEFT JOIN (SELECT ITEM_CODE,SUM(CASE WHEN STK_QTY_VALUE=1 THEN VALUE_1   
                  WHEN STK_QTY_VALUE=2 THEN VALUE_2 WHEN STK_QTY_VALUE=3 THEN VALUE_3 WHEN STK_QTY_VALUE=4 THEN VALUE_4 
                  WHEN STK_QTY_VALUE=5 THEN VALUE_5 WHEN STK_QTY_VALUE=6 THEN VALUE_6 WHEN STK_QTY_VALUE=7 THEN VALUE_7 
                  WHEN STK_QTY_VALUE=8 THEN VALUE_8 WHEN STK_QTY_VALUE=9 THEN VALUE_9 WHEN STK_QTY_VALUE=10 THEN VALUE_10 
                  WHEN STK_QTY_VALUE=11 THEN VALUE_11 WHEN STK_QTY_VALUE=12 THEN VALUE_12 WHEN STK_QTY_VALUE=13 THEN VALUE_13 
                  WHEN STK_QTY_VALUE=14 THEN VALUE_14 WHEN STK_QTY_VALUE=15 THEN VALUE_15 WHEN STK_QTY_VALUE=16 THEN VALUE_16 
                  WHEN STK_QTY_VALUE=17 THEN VALUE_17 WHEN STK_QTY_VALUE=18 THEN VALUE_18 WHEN STK_QTY_VALUE=19 THEN VALUE_19 
                  WHEN STK_QTY_VALUE=20 THEN VALUE_20 ELSE 0 END) HOLD_SALE, LINE_REF EXPIRE_DATE FROM {dbCode}SISODET 
                  WHERE REC_TYPE='D' AND STATUS<'80' AND CREDIT_STATUS='' AND LOCATION = @WAREHOUSE AND ITEM_CODE=@ITEM_CODE 
                  GROUP BY ITEM_CODE,LINE_REF) TAB2 ON TAB1.ITEMCODE=TAB2.ITEM_CODE AND TAB1.EXPIRE_DATE=TAB2.EXPIRE_DATE 
                  WHERE PHYSICAL-ISNULL(HOLD_SALE, 0)>0 AND TAB1.EXPIRE_DATE NOT IN @EXPIREDDATE ORDER BY CAST(TAB1.EXPIRE_DATE AS DATE);";
            foreach (var key in itemCodeAndExpireDate)
            {
                var expireDateParam = string.Join(",", key.Value.Select(date => $"{date}"));
                var param = new
                {
                    DBCODE = dbCode,
                    ITEM_CODE = key.Key,
                    WAREHOUSE = wareHouse,
                    EXPIREDDATE = key.Value,
                };
                var execute = await _sqlDataAccess.LoadData<SaleDetailsDto,dynamic>(sql, param);
                result.AddRange(execute.ToList());
            }

            return result;
        }

        public async Task<int> CreateInvoiceSaleAsync( string dbCode,SaleHeaderDto saleHeader, List<SaleDetailsDto> detailsDtos)
        {
            var affectedRow = 0;
            if (_dbConnection.State == ConnectionState.Closed)
                _dbConnection.Open();
            var transaction = _dbConnection.BeginTransaction();
            try
            {
                var sql =
                    $@"INSERT INTO {dbCode}SISOHDR(REC_TYPE,TRANS_REF,HEADER_ID,CUST_CODE,DELIV_ADD,TRANS_DATE,STATUS,TRANS_CD,TRANS_CODE,ORDER_NO,ORDER_DATE,PRN_DATE,DEL_DATE,INV_DATE,INV_PRD,CUST_REF,DEL_REF,COMMENTS,TRANS_VAL,PAY_DATE,ANAL_M0,ANAL_M1,ANAL_M2,ANAL_M3,ANAL_M4,ANAL_M5,ANAL_M6,ANAL_M7,ANAL_M8,ANAL_M9,QUOTE_CONVERT,QUOTE_PRINT,QUOTE_EXPIRY,QUOTED_PRD,QUOTATION_REF,DATE_QUOTED,VOID_STATUS,USER_CODE)
            VALUES(@REC_TYPE,@TRANS_REF,@HEADER_ID,@CUST_CODE,@DELIV_ADD,@TRANS_DATE,@STATUS,@TRANS_CD,@TRANS_CODE,@ORDER_NO,@ORDER_DATE,@PRN_DATE,@DEL_DATE,@INV_DATE,
            @INV_PRD,@CUST_REF,@DEL_REF,@COMMENTS,@TRANS_VAL,@PAY_DATE,@ANAL_M0,@ANAL_M1,@ANAL_M2,@ANAL_M3,@ANAL_M4,@ANAL_M5,@ANAL_M6,@ANAL_M7,@ANAL_M8,@ANAL_M9,@QUOTE_CONVERT,@QUOTE_PRINT,@QUOTE_EXPIRY,@QUOTED_PRD,@QUOTATION_REF,@DATE_QUOTED,@VOID_STATUS,@USER_CODE)";
                var param = new
                {
                    REC_TYPE = saleHeader.RecType, //1
                    TRANS_REF = saleHeader.Transaction, //2
                    HEADER_ID = saleHeader.HeaderId, //3
                    CUST_CODE = saleHeader.CustomerCode, //4
                    DELIV_ADD = saleHeader.DeliveryAdd, //5
                    TRANS_DATE = saleHeader.TransactionDate, //6
                    STATUS = saleHeader.Status, //7
                    TRANS_CD = saleHeader.TransactionCd, //8
                    TRANS_CODE = saleHeader.TransactionCode, //9
                    ORDER_NO = saleHeader.OrderNo, //10
                    ORDER_DATE = saleHeader.OrderDate, //11
                    PRN_DATE = saleHeader.PrnDate, //12
                    DEL_DATE = saleHeader.DelDate, //13
                    INV_DATE = saleHeader.InvoiceDate, //14
                    INV_PRD = saleHeader.InvoicePeriod, //15
                    CUST_REF = saleHeader.CustomerRef, //16
                    DEL_REF = saleHeader.DeliveryRef, //17
                    COMMENTS = saleHeader.Comments, //18
                    TRANS_VAL = saleHeader.TransactionValue, //19
                    PAY_DATE = saleHeader.PayDate, //20
                    ANAL_M0 = saleHeader.AnalM0, //21
                    ANAL_M1 = saleHeader.AnalM1, //22
                    ANAL_M2 = saleHeader.AnalM2, //23
                    ANAL_M3 = saleHeader.AnalM3, //24
                    ANAL_M4 = saleHeader.AnalM4, //25
                    ANAL_M5 = saleHeader.AnalM5, //26
                    ANAL_M6 = saleHeader.AnalM6, //27
                    ANAL_M7 = saleHeader.AnalM7, //28
                    ANAL_M8 = saleHeader.AnalM8, //29
                    ANAL_M9 = saleHeader.AnalM9, //30
                    QUOTE_CONVERT = saleHeader.QuoteConvert, //31
                    QUOTE_PRINT = saleHeader.QuotePrint, //32
                    QUOTE_EXPIRY = saleHeader.QuoteExpiry, //33
                    QUOTED_PRD = saleHeader.QuotePeriod, //34
                    QUOTATION_REF = saleHeader.QuotationRef, //35
                    DATE_QUOTED = saleHeader.DateQuoted, //36
                    VOID_STATUS = saleHeader.VoidStatus, //37
                    USER_CODE = saleHeader.UserCode //38
                };
                 affectedRow = await _dbConnection.ExecuteAsync(sql, param, transaction);
                if (affectedRow > 0)
                {
                    sql = $@"INSERT INTO {dbCode}SISODET
                (
                REC_TYPE,
                DETAIL_ID,
                TRANS_TYPE,
                TRANS_REF,
                TRANS_LINE,
                TRANS_CD,
                LOCATION,
                ITEM_CODE,
                DESCRIPTN,
                DUE_DATE,
                STATUS,
                VALUE_1,
                VALUE_2,
                VALUE_3,
                VALUE_4,
                VALUE_5,
                VALUE_6,
                VALUE_7,
                VALUE_8,
                VALUE_9,
                VALUE_10,
                VALUE_11,
                VALUE_12,
                VALUE_13,
                VALUE_14,
                VALUE_15,
                VALUE_16,
                VALUE_17,
                VALUE_18,
                VALUE_19,
                VALUE_20,
                UNIT_SALE,
                ORD_PRD,
                DEL_DATE,
                INV_DATE,
                INV_NO,
                INV_PRD,
                ACCNT_CODE,
                ANAL_M0,
                ANAL_M1,
                ANAL_M2,
                ANAL_M3,
                ANAL_M4,
                ANAL_M5,
                ANAL_M6,
                ANAL_M7,
                ANAL_M8,
                ANAL_M9,
                ASSEMBLY_IND,
                SPLIT_VAL,
                CREDIT_STATUS,
                PRICE_BOOK,
                SALE_QTY_VALUE,
                STK_QTY_VALUE,
                TOT_VALUE,
                DISP_VAL_1,
                DISP_VAL_2,
                FIXED_VAL,
                LINE_REF,
                USER_CODE,
                USER_INVOICED,
                ALLOC_REF,
                UPDATE_STOCK,
                FIXED_VAL_2,
                FIXED_VAL_3
                )
                VALUES
                (
                @REC_TYPE,
                @DETAIL_ID,
                @TRANS_TYPE,
                @TRANS_REF,
                @TRANS_LINE,
                @TRANS_CD,
                @LOCATION,
                @ITEM_CODE,
                @DESCRIPTN,
                @DUE_DATE,
                @STATUS,
                @VALUE_1,
                @VALUE_2,
                @VALUE_3,
                @VALUE_4,
                @VALUE_5,
                @VALUE_6,
                @VALUE_7,
                @VALUE_8,
                @VALUE_9,
                @VALUE_10,
                @VALUE_11,
                @VALUE_12,
                @VALUE_13,
                @VALUE_14,
                @VALUE_15,
                @VALUE_16,
                @VALUE_17,
                @VALUE_18,
                @VALUE_19,
                @VALUE_20,
                @UNIT_SALE,
                @ORD_PRD,
                @DEL_DATE,
                @INV_DATE,
                @INV_NO,
                @INV_PRD,
                @ACCNT_CODE,
                @ANAL_M0,
                @ANAL_M1,
                @ANAL_M2,
                @ANAL_M3,
                @ANAL_M4,
                @ANAL_M5,
                @ANAL_M6,
                @ANAL_M7,
                @ANAL_M8,
                @ANAL_M9,
                @ASSEMBLY_IND,
                @SPLIT_VAL,
                @CREDIT_STATUS,
                @PRICE_BOOK,
                @SALE_QTY_VALUE,
                @STK_QTY_VALUE,
                @TOT_VALUE,
                @DISP_VAL_1,
                @DISP_VAL_2,
                @FIXED_VAL,
                @LINE_REF,
                @USER_CODE,
                @USER_INVOICED,
                @ALLOC_REF,
                @UPDATE_STOCK,
                @FIXED_VAL_2,   
                @FIXED_VAL_3)";

                    foreach (var parameter in detailsDtos.Select(saleDetailsDto => new
                    {
                        REC_TYPE = saleDetailsDto.RefType,
                        DETAIL_ID = saleDetailsDto.DetailId,
                        TRANS_TYPE = saleDetailsDto.TransType,
                        TRANS_REF = saleDetailsDto.TransRef,
                        TRANS_LINE = saleDetailsDto.TransLine,
                        TRANS_CD = saleDetailsDto.TransCd,
                        LOCATION = saleDetailsDto.Location,
                        ITEM_CODE = saleDetailsDto.ItemCode,
                        DESCRIPTN = saleDetailsDto.Description,
                        DUE_DATE = saleDetailsDto.DueDate,
                        STATUS = "05",
                        VALUE_1 = saleDetailsDto.Value1,
                        VALUE_2 = saleDetailsDto.Value2,
                        VALUE_3 = saleDetailsDto.Value3,
                        VALUE_4 = saleDetailsDto.Value4,
                        VALUE_5 = saleDetailsDto.Value5,
                        VALUE_6 = saleDetailsDto.Value6,
                        VALUE_7 = saleDetailsDto.Value7,
                        VALUE_8 = saleDetailsDto.Value8,
                        VALUE_9 = saleDetailsDto.Value9,
                        VALUE_10 = saleDetailsDto.Value10,
                        VALUE_11 = saleDetailsDto.Value11,
                        VALUE_12 = saleDetailsDto.Value12,
                        VALUE_13 = saleDetailsDto.Value13,
                        VALUE_14 = saleDetailsDto.Value14,
                        VALUE_15 = saleDetailsDto.Value15,
                        VALUE_16 = saleDetailsDto.Value16,
                        VALUE_17 = saleDetailsDto.Value17,
                        VALUE_18 = saleDetailsDto.Value18,
                        VALUE_19 = saleDetailsDto.Value19,
                        VALUE_20 = saleDetailsDto.Value20,
                        UNIT_SALE = saleDetailsDto.UnitSale,
                        ORD_PRD = saleDetailsDto.OrdPeriod,
                        DEL_DATE = saleDetailsDto.DelDate,
                        INV_DATE = saleDetailsDto.InvoiceDate,
                        INV_NO = saleDetailsDto.InvoiceNo,
                        INV_PRD = saleDetailsDto.InvoicePeriod,
                        ACCNT_CODE = saleDetailsDto.AccountCode,
                        ANAL_M0 = saleDetailsDto.AnalM0,
                        ANAL_M1 = saleDetailsDto.AnalM1,
                        ANAL_M2 = saleDetailsDto.AnalM2,
                        ANAL_M3 = saleDetailsDto.AnalM3,
                        ANAL_M4 = saleDetailsDto.AnalM4,
                        ANAL_M5 = saleDetailsDto.AnalM5,
                        ANAL_M6 = saleDetailsDto.AnalM6,
                        ANAL_M7 = saleDetailsDto.AnalM7,
                        ANAL_M8 = saleDetailsDto.AnalM8,
                        ANAL_M9 = saleDetailsDto.AnalM9,
                        ASSEMBLY_IND = saleDetailsDto.AssemblyInd,
                        SPLIT_VAL = saleDetailsDto.SplitVal,
                        CREDIT_STATUS = saleDetailsDto.CreditStatus,
                        PRICE_BOOK = saleDetailsDto.PriceBook,
                        SALE_QTY_VALUE = saleDetailsDto.SaleQtyValue,
                        STK_QTY_VALUE = saleDetailsDto.StkQtyValue,
                        TOT_VALUE = saleDetailsDto.TopValue,
                        DISP_VAL_1 = saleDetailsDto.DisplayValue1,
                        DISP_VAL_2 = saleDetailsDto.DisplayValue2,
                        FIXED_VAL = saleDetailsDto.FixedValue,
                        LINE_REF = saleDetailsDto.LineRef,
                        USER_CODE = saleDetailsDto.UserCode,
                        USER_INVOICED = saleDetailsDto.UserInvoice,
                        ALLOC_REF = saleDetailsDto.AllowRef,
                        UPDATE_STOCK = saleDetailsDto.UpdateStock,
                        FIXED_VAL_2 = saleDetailsDto.FixedStockValue2,
                        FIXED_VAL_3 = saleDetailsDto.FixedStockValue3
                    }))

                        affectedRow += await _dbConnection.ExecuteAsync(sql, parameter, transaction);

                    if (affectedRow > 1)
                    {
                        transaction.Commit();
                        return affectedRow;
                    }
                }
                transaction.Rollback();
                return 0;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                Debug.WriteLine($@"Error Exception : ${e.Message}");
            }
            return 0;
        }

        public async Task<int> InsertRecordInvoice(string dbCode,string userName,string transaction, string customerCode, string customerName, double value, DateTime date,
            string entryCode)
        {
            const string sql =
                @"INSERT INTO NEW_INVOICE(DB_CODE,TRANSACTION_REF,CUSTOMER_CODE,ACC_NAME_KH,HEADER_TRANSACTION_VALUES,[STATUS],CREATED_DATE,CREATED_BY,IS_DIVIDED,ENTRIES_CODE) VALUES
                (@DB_CODE,@TRANSACTION_CODE,@CUSTOMER_CODE,@CUSTOMER_NAME,@VALUE,@STATUS,@CREATED_DATE,@CREATED_BY,@IS_DIVIDED,@ENTRIES_CODE)";
            var param = new
            {
                DB_CODE = dbCode,
                TRANSACTION_CODE = transaction,
                CUSTOMER_CODE = customerCode,
                CUSTOMER_NAME = customerName,
                VALUE = value,
                STATUS = "C",
                CREATED_DATE = date,
                CREATED_BY = userName,
                IS_DIVIDED = "1",
                ENTRIES_CODE = entryCode
            };
            var rowAffected = await _sqlDataAccess.ExecuteAsync(sql, param);
            return rowAffected;
        }

        public async Task<int> CreateInvoice(CreateInvoiceDto createInvoiceDto, List<CreateInvoiceDetailDto> createInvoiceDetailDto)
        {
            var connection = new SqlConnection(_configuration.GetConnectionString("DBConnection"));
            var affectedRow = 0;
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                const string sqlMaster =
                 @"INSERT INTO TB_BC_CHANGEINVOICE_REPAIR_INVOICE (DB_CODE,DB_CODE_1,INVOICE_NUMBER,INVOICE_DATE,INVOICE_DUE,CUSTOMER_CODE,DISCOUNT,TAXES,TOTAL_AMOUNT,CREATED_DATE,CREATED_BY,STATUS,DESCRIPTION)
                VALUES (@DB_CODE,@DB_CODE_1,@INVOICE_REFERENCE,@INVOICE_DATE,@INVOICE_DUE,@CUSTOMER_CODE,@DISCOUNT,@TAXES,@TOTAL_AMOUNT,@CREATED_DATE,@CREATED_BY,0,@DESCRIPTION)";
                var paramMaster = new
                {
                    DB_CODE = createInvoiceDto.DbCode,
                    DB_CODE_1 = createInvoiceDto.DbCode,
                    INVOICE_DATE = createInvoiceDto.InvoiceDate,
                    INVOICE_DUE = createInvoiceDto.InvoiceDue,
                    CUSTOMER_CODE = createInvoiceDto.CustomerCode,
                    DISCOUNT = createInvoiceDto.Discount,
                    TAXES = createInvoiceDto.Taxes,
                    TOTAL_AMOUNT = createInvoiceDto.TotalAmount,
                    CREATED_DATE = createInvoiceDto.CreatedDate,
                    CREATED_BY = createInvoiceDto.CreatedBy,
                    INVOICE_REFERENCE = createInvoiceDto.InvoiceReference,
                    DESCRIPTION = createInvoiceDto.Description
                };
                const string sqlDetails =
                    @"INSERT INTO TB_BC_CHANGEINVOICE_REPAIR_INVOICE_DETAILS(INVOICE_ID,TRANS_REF,REPAIR_COMPLETED_ID,QUANTITY,UNIT_PRICE,TOTAL,REQUEST_REPAIR_ID)
                OUTPUT Inserted.REQUEST_REPAIR_ID
                VALUES (@INVOICE_ID,@TRANS_REF,@REPAIR_COMPLETED_ID,@QUANTITY,@UNIT_PRICE,@TOTAL,@REQUEST_REPAIR_ID)";

                affectedRow += await connection.ExecuteAsync(sqlMaster, paramMaster, transaction);
                foreach (var invoiceDetailDto in createInvoiceDetailDto)
                {
                    var paramDetail = new
                    {
                        INVOICE_ID = createInvoiceDto.InvoiceReference,
                        REPAIR_COMPLETED_ID = invoiceDetailDto.RepairCompletedId,
                        QUANTITY = invoiceDetailDto.Quantity,
                        TRANS_REF = invoiceDetailDto.ItemTransaction,
                        UNIT_PRICE = invoiceDetailDto.UnitPrice,
                        TOTAL = invoiceDetailDto.Total,
                        REQUEST_REPAIR_ID = invoiceDetailDto.RequestRepairId,
                    };
                    var requestDeetailId = await connection.ExecuteScalarAsync<int>(sqlDetails, paramDetail, transaction);
                    affectedRow += await connection.ExecuteAsync(
                       "UPDATE TB_BC_CHANGEINVOICE_DETAIL SET IS_RECEIVED = 'Completed' WHERE ID = (SELECT ID FROM TB_BC_CHANGEINVOICE_DETAIL WHERE CHANGE_INVOICE_ID = @RequestDetailId)",
                       new { RequestDetailId = requestDeetailId }, transaction);

                    // update to completed
                    var updateRepairCompleted = $@"UPDATE TB_BC_CHANGEINVOICE_REPAIR_COMPLETED SET STATUS = 'Completed' WHERE ID = @RepairCompletedId";
                    affectedRow += await connection.ExecuteAsync(updateRepairCompleted,
                        new { RepairCompletedId = invoiceDetailDto.RepairCompletedId },transaction);
                    var updateRepairMaster = $@"UPDATE TB_BC_CHANGEINVOICE SET IS_RECEIVED = 'Completed',COMPLETED = @COMPLETED_DATE FROM TB_BC_CHANGEINVOICE HEADER INNER JOIN TB_BC_CHANGEINVOICE_DETAIL DETAIL ON DETAIL.CHANGE_INVOICE_ID = HEADER.ID
                        INNER JOIN TB_BC_CHANGEINVOICE_RECEIVED RECEIVED ON RECEIVED.CHANGE_INVOICE_DETAIL_ID = DETAIL.ID
                        INNER JOIN TB_BC_CHANGEINVOICE_REPAIR REPAIR ON REPAIR.RECEIVED_ID = RECEIVED.ID
                        WHERE REPAIR.TRAN_REF = @TRANSACTION";
                    affectedRow += await connection.ExecuteAsync(updateRepairMaster,
                        new { COMPLETED_DATE = DateTime.Today, TRANSACTION = invoiceDetailDto.ItemTransaction},transaction);
                }

                if (affectedRow > 3)
                {
                    transaction.Commit();   
                    return affectedRow;
                }
                transaction.Rollback();
                return 0;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                Debug.WriteLine($@"Error Exception : {e.Message}");
            }
            return 0;
        }

        public async Task<InvoiceDetailDto> GetInvoiceByCustomerCode(string dbCode,string customerCode)
        {
            const string sql =
                @"SELECT MARKET_KHMER_NAME Market,CusInfo.ADD_LINE_1 CustomerName,CusInfo.ADD_TEL Phone,CUSTOMER_CODE CustomerCode,S.LAST_NAME + ' ' + S.FIRST_NAME SalesRepresentative 
                FROM TB_BC_CHANGEINVOICE TBC INNER JOIN dbo.BCUSERS S 
                ON S.USER_ID = TBC.USER_CODE
				LEFT JOIN (SELECT ADD_CODE,ADD_LINE_1,ADD_TEL FROM SIADD) CusInfo ON CusInfo.ADD_CODE = CUST_CODE
                INNER JOIN (
                SELECT CUS.ADD_CODE CUSTOMER_CODE,ADD_LINE_1 CUSTOMER_NAME,M.MARKET_KHMER_NAME FROM SIADD CUS INNER JOIN TB_BCMARKET M ON M.MARKET_ID = CUS.MARKET_ID
                WHERE CUS.DB_CODE = @DbCode AND M.DB_CODE = @DbCode) CUS ON CUS.CUSTOMER_CODE = TBC.CUST_CODE
                AND CUST_CODE LIKE @CustomerCode";
            var param = new
            {
                CustomerCode = customerCode,
                DbCode = dbCode
            };
            return await _sqlDataAccess.LoadSingleData<InvoiceDetailDto,dynamic>(sql, param);
        }

        public async Task<bool> CheckStockQuantityAsync(string dbCode, string location, string itemCode, int quantityRequest)
        {
            var sql =
            @$"SELECT CAST(CASE WHEN COUNT(QUANTITY) > 0 THEN 1 ELSE 0 END AS BIT) FROM 
	   (SELECT PHYSICAL-TAB5.ON_ORDER AS QUANTITY 
			FROM (SELECT TAB3.LOCATION,TAB3.ITEM_CODE,TAB3.ITEM_DESC,TAB3.UNIT_STOCK,TAB3.PHYSICAL,ISNULL(TAB4.HOLD_SALE,0) ON_ORDER 
			  FROM (SELECT TAB1.LOCATION,TAB1.ITEM_CODE,TAB2.ITEM_DESC,TAB2.UNIT_STOCK,PHYSICAL 
				FROM (SELECT LOCATION,ITEM_CODE,ISNULL(SUM(QUANTITY),0) PHYSICAL FROM {dbCode}SIINVMOV WHERE IR_STAT='I' AND STATUS='80'AND ALLOC_REF=''  GROUP BY LOCATION,ITEM_CODE) AS TAB1 
				  LEFT JOIN (SELECT ITEM_CODE,ITEM_DESC,UNIT_STOCK FROM SIITEMS WHERE DB_CODE=@DB_CODE) AS TAB2 ON TAB1.ITEM_CODE=TAB2.ITEM_CODE) AS TAB3 
					LEFT JOIN (SELECT LOCATION,ITEM_CODE,SUM(CASE WHEN STK_QTY_VALUE=1 THEN VALUE_1 WHEN STK_QTY_VALUE=2 THEN VALUE_2 WHEN STK_QTY_VALUE=3 THEN VALUE_3 WHEN STK_QTY_VALUE=4
							   THEN VALUE_4 WHEN STK_QTY_VALUE=5 THEN VALUE_5 WHEN STK_QTY_VALUE=6 THEN VALUE_6 WHEN STK_QTY_VALUE=7 THEN VALUE_7 WHEN STK_QTY_VALUE=8 THEN VALUE_8 WHEN STK_QTY_VALUE=9 THEN VALUE_9 WHEN
							   STK_QTY_VALUE=10 THEN VALUE_10 WHEN STK_QTY_VALUE=11 THEN VALUE_11 WHEN STK_QTY_VALUE=12 THEN VALUE_12 WHEN STK_QTY_VALUE=13 THEN VALUE_13 WHEN STK_QTY_VALUE=14 THEN VALUE_14 WHEN
							   STK_QTY_VALUE=15 THEN VALUE_15 WHEN STK_QTY_VALUE=16 THEN VALUE_16 WHEN STK_QTY_VALUE=17 THEN VALUE_17 WHEN STK_QTY_VALUE=18 THEN VALUE_18 WHEN STK_QTY_VALUE=19 THEN VALUE_19 WHEN
							   STK_QTY_VALUE=20 THEN VALUE_20 ELSE 0 END) HOLD_SALE 
							   FROM {dbCode}SISODET WHERE REC_TYPE='D' AND STATUS<'80' AND CREDIT_STATUS='' GROUP BY LOCATION,ITEM_CODE) AS TAB4 ON TAB3.LOCATION=TAB4.LOCATION AND TAB3.ITEM_CODE=TAB4.ITEM_CODE) AS TAB5 
					   LEFT JOIN (SELECT LOCATION,ITEM_CODE,SUM(QUANTITY) PICK_QTY 
								FROM {dbCode}SIINVMOVH WHERE  IR_STAT<>'I' AND STATUS='10' GROUP BY LOCATION,ITEM_CODE) 
						        AS TAB6 ON TAB5.LOCATION=TAB6.LOCATION AND TAB5.ITEM_CODE=TAB6.ITEM_CODE WHERE TAB5.LOCATION=@LOCATION AND TAB5.ITEM_CODE= @ITEM_CODE) AS TAB7 WHERE QUANTITY >= @QUANTITYREQUEST";
            var param = new
            {
                DB_CODE = dbCode,
                LOCATION = location,
                ITEM_CODE = itemCode,
                QUANTITYREQUEST = quantityRequest,

            };
            var results = await _sqlDataAccess.LoadSingleData<bool,dynamic>(sql, param);
            return results;
        }

        public async Task UpdateStatusExchangeReceivedToCredit(int id, int status)
        {
            const string sql = "UPDATE TB_BC_CHANGEINVOICE_RECEIVED SET STATUS = @STATUS WHERE ID = @ID";
            var param = new
            {
                ID = id,
                STATUS = status
            };
            await _sqlDataAccess.ExecuteAsync(sql, param);
        }

        public async Task UpdateStatusRequestExchangeDetails(int id, ExchangeStatus exchangeStatus)
        {
            const string sql = @"UPDATE TB_BC_CHANGEINVOICE_DETAIL SET IS_RECEIVED = @STATUS WHERE ID = @ID";
            var param = new
            {
                STATUS = Enum.GetName(typeof(ExchangeStatus), exchangeStatus),
                ID = id
            };
            await _sqlDataAccess.ExecuteAsync(sql, param);
        }

        public async Task SaveRecordItemExchanged(string dbCode,string userName,string transaction, string itemCode, int quantity, double unitPrice)
        {
            const string sql =
                @"INSERT INTO TB_BC_CHANGEINVOICE_ITEM_EXCHANGED([TRANSACTION],ITEM_CODE,QUANTITY,CREATED_DATE,CREATED_BY,UNIT_PRICE,DB_CODE)
                VALUES (@TRANSACTION,@ITEM_CODE,@QUANTITY,@CREATED_DATE,@CREATED_BY,@UNIT_PRICE,@DB_CODE)";
            var param = new
            {
                TRANSACTION = transaction,
                ITEM_CODE = itemCode,
                QUANTITY = quantity,
                CREATED_DATE = DateTime.Today,
                CREATED_BY = userName,
                UNIT_PRICE = unitPrice,
                DB_CODE = dbCode
            };
            await _sqlDataAccess.ExecuteAsync(sql, param);
        }

        public async Task<bool> IsAllItemRequestCompletedByRequestIdAsync(int requestId)
        {
            const string sql = @"SELECT CAST(CASE WHEN COUNT(*) > 0 THEN 1 ELSE 0 END AS BIT) 
                FROM TB_BC_CHANGEINVOICE_DETAIL WHERE CHANGE_INVOICE_ID = @REQUEST_ID AND (IS_RECEIVED <> 'Completed' OR (TYPE = 'EXCHANGE' AND IS_RECEIVED <> 'Completed'))
                ";
            var param = new
            {
                REQUEST_ID = requestId
            };
            var result = await _sqlDataAccess.LoadSingleData<bool,dynamic>(sql, param);
            return result;
        }

        public async Task<bool> UpdateReceivedToCompletedByIdAsync(String dbCode,int requestId)
        {
            const string sql =
                @"UPDATE TB_BC_CHANGEINVOICE SET IS_RECEIVED = 'Completed' WHERE ID = @REQUEST_ID AND DB_CODE = @DB_CODE";
            var param = new
            {
                REQUEST_ID = requestId,
                DB_CODE = dbCode
            };
            var result = await _sqlDataAccess.ExecuteAsync(sql, param);
            return result == 1;
        }

        public Task<int> UpdateValue6ToZero(List<(string newTransaction, string TransLine, string itemCode, string oldTransaction)> tupleValues)
        {
            throw new NotImplementedException();
        }

        #endregion


        #endregion

    }
}
