using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.CommondityExchange.ItemRepairReport
{
    public class ItemExchangeReportRepository : IItemExchangeReportRepository
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public ItemExchangeReportRepository(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }
        public async Task<List<ExchangeItemReportDto>> GetExchangeItemInvoiceAsync(string dbCode,DateTime fromDate,DateTime toDate)
        {
            string sql =
                @"SELECT S.LAST_NAME +' '+ S.FIRST_NAME [Seller],DB.DB_NAME DbName,CREATED_DATE SubmittedDate,INVOICE.INVOICE_DATE CompletedDate,ITEM_CODE ItemCode,DESCRIPION [Description],QUANTITY Quantity,
                D.IS_RECEIVED [Status],CUS.*,INVOICE.TRANSACTION_CODE [Transaction],INVOICE.SUB_TOTAL Total FROM TB_BC_CHANGEINVOICE C INNER JOIN TB_BC_CHANGEINVOICE_DETAIL D 
                ON D.CHANGE_INVOICE_ID = C.ID 
                LEFT JOIN (SELECT ADD_CODE CustomerCode,ADD_LINE_1KH CustomerName,M.MARKET_KHMER_NAME Market,R.AREA_NAME_KHMER Area,STORE Store 
		        FROM SIADD S INNER JOIN TB_BCMARKET M ON M.MARKET_ID 
                = S.MARKET_ID INNER JOIN TB_AREAS R ON R.AREA_ID = S.AREA_ID
                ) CUS ON CUS.CustomerCode = C.CUST_CODE
                INNER JOIN SIDBINFO DB ON DB.DB_CODE = C.DB_CODE
                INNER JOIN dbo.BCUSERS S ON S.USER_ID = C.USER_CODE
                LEFT JOIN TB_BC_CHANGEINVOICE_EXCHANGE_INVOICE INVOICE ON INVOICE.REQUEST_EXCHANGE_ID = C.ID
                WHERE TYPE = 'EXCHANGE' AND C.DB_CODE = @DB_CODE AND CONVERT(DATE,INVOICE.INVOICE_DATE) BETWEEN @FROM_DATE AND @TO_DATE";
            var param = new
            {
                DB_CODE =  dbCode,
                FROM_DATE = fromDate,
                TO_DATE = toDate
            };
            var execute = await _sqlDataAccess.LoadData<ExchangeItemReportDto,dynamic>(sql,param);
            return execute.ToList();
        }

        public async Task<List<ItemReceivedDto>> GetReceivedExchangeItemInvoiceAsync(string dbCode, DateTime fromDate, DateTime toDate)
        {
            var sql = @"
               SELECT DetailId,MasterId,ReceivedDate,TransactionCode,Area,Market,ITEM_CODE ItemCode,Customer CustomerCode,CustomerName,QUANTITY Quantity,
                Received Received,[Status] FROM (
                        SELECT TBCD.ID DetailId,TBC.ID MasterId,
	                        REC.CREATED_DATE ReceivedDate,
							REC.ITEM_CODE,
	                        TRANSACTION_CODE TransactionCode,
	                        AREA_NAME_KHMER Area,
	                        MARKET_KHMER_NAME Market,
	                        ADD_CODE Customer,
	                        ADD_LINE_1 CustomerName,
	                        TBCD.QUANTITY,
	                        CASE WHEN TBCD.IS_RECEIVED NOT IN('Pending','Reject') THEN TBCD.QUANTITY END 'Received',
                            TBCD.IS_RECEIVED Status
                        FROM 
                        TB_BC_CHANGEINVOICE_DETAIL TBCD INNER JOIN TB_BC_CHANGEINVOICE TBC ON TBC.ID = TBCD.CHANGE_INVOICE_ID
                        LEFT JOIN (SELECT ADD_CODE,ADD_LINE_1,MARKET_KHMER_NAME,AREA_NAME_KHMER 
                        FROM SIADD CUS INNER JOIN TB_BCMARKET M ON M.MARKET_ID = 
                        CUS.MARKET_ID INNER JOIN TB_AREAS R ON R.AREA_ID = CUS.AREA_ID
                        WHERE CUS.DB_CODE = @DB_CODE AND M.DB_CODE = @DB_CODE AND R.DB_CODE = @DB_CODE)
						CUS ON CUS.ADD_CODE = CUST_CODE
						INNER JOIN TB_BC_CHANGEINVOICE_RECEIVED REC ON REC.CHANGE_INVOICE_DETAIL_ID = TBCD.ID
                        INNER JOIN TB_BC_CHANGEINVOICE_EXCHANGE_INVOICE INV ON INV.REQUEST_EXCHANGE_ID = TBC.ID 
						WHERE CONVERT(DATE,REC.CREATED_DATE) BETWEEN @FROM_DATE AND @TO_DATE AND TBCD.[TYPE] = 'EXCHANGE'
						AND TBC.DB_CODE = @DB_CODE
						) TAB";
            var param = new
            {
                DB_CODE = dbCode,
                FROM_DATE = fromDate,
                TO_DATE = toDate,
            };
            var execute = await _sqlDataAccess.LoadData<ItemReceivedDto, dynamic>(sql,param);
            return execute.ToList();
        }

        public async Task<List<ReportExchangePendingItemDto>> GetPendingExchangeItemInvoiceAsync(string dbCode)
        {
            var sql = $@"SELECT CUS.*,DETAIL.ITEM_CODE ItemCode,DETAIL.DESCRIPION [Description],QUANTITY Quantity,S.FIRST_NAME + S.LAST_NAME [Seller],HEADER.CREATED_DATE CreatedDate,DETAIL.IS_RECEIVED [Status] FROM TB_BC_CHANGEINVOICE_DETAIL 
                DETAIL INNER JOIN TB_BC_CHANGEINVOICE HEADER ON HEADER.ID = DETAIL.CHANGE_INVOICE_ID
                INNER JOIN GET_CUSTOMERS_BY_DB_CODE() CUS ON CUS.CustomerCode = HEADER.CUST_CODE
                INNER JOIN BCUSERS S ON S.USER_ID = HEADER.USER_CODE
                WHERE DETAIL.IS_RECEIVED IN ('Yes','CreditNote') AND TYPE = 'EXCHANGE' AND HEADER.DB_CODE = @DB_CODE";
            var param = new
            {
                DB_CODE = dbCode
            };
            var execute = await _sqlDataAccess.LoadData<ReportExchangePendingItemDto, dynamic>(sql, param);
            return execute.ToList();
        }

        public async Task<List<ReportExchangeDto>> GetExchangeInvoiceNotYetSendToCustomerReportAsync(string dbCode, DateTime fromDate, DateTime toDate)
        {
            var sql = @"SELECT EXCHANGE.INVOICE_DATE [InvoiceDate],CUS.*,EXCHANGE.TRANSACTION_CODE [InvoiceNumber],ITEM.ITEM_CODE ItemCode,QUANTITY Quantity,UNIT_PRICE UnitPrice,TOTAL Total FROM TB_BC_CHANGEINVOICE_EXCHANGE_INVOICE EXCHANGE 
				INNER JOIN TB_BC_CHANGEINVOICE_EXCHANGE_ITEM ITEM ON ITEM.INVOICE_ID = EXCHANGE.ID
				LEFT JOIN GET_CUSTOMERS_BY_DB_CODE() CUS ON CUS.CustomerCode = EXCHANGE.CUSTOMER_CODE
				WHERE EXCHANGE.DB_CODE = @DB_CODE AND CAST(EXCHANGE.INVOICE_DATE AS DATE) BETWEEN @FROM_DATE AND @TO_DATE
				AND EXCHANGE.TRANSACTION_CODE NOT IN(SELECT TRANSACTION_REF FROM(
                SELECT CREATE_DATE,TRANSACTION_REF,Status Id,StatusText,CASE WHEN InvoiceAmount = Paid THEN 'Completed' ELSE '' END [Status] FROM(
                SELECT TOP 1 WITH TIES CREATE_DATE,TRANSACTION_REF,Status,StatusText,InvoiceAmount,Paid FROM (
                SELECT D.CREATE_DATE,N.TRANSACTION_REF,CASE WHEN R.RETURN_ID IS NOT NULL THEN R.RETURN_ID ELSE P.PAYMENT_ID END [Status]
                ,CASE WHEN R.RETURN_ID IS NOT NULL THEN 'Return' ELSE 'Payment' END [StatusText],N.HEADER_TRANSACTION_VALUES [InvoiceAmount],P.AMOUNT [Paid]
                FROM NEW_INVOICE N INNER JOIN PC_DIVIDED_INVOICE D ON D.INVOICE_ID = N.ID
                LEFT JOIN PC_PAYMENT_INVOICE P ON P.DIVDIE_INVOICE_ID = D.DIVIDED_INVOICE_ID
                LEFT JOIN PC_RETURN_INVOICE R ON R.DIVIDED_INIOVICE_ID = D.DIVIDED_INVOICE_ID
                WHERE  D.DB_CODE = @DB_CODE AND N.DB_CODE = @DB_CODE) T ORDER BY ROW_NUMBER() OVER (PARTITION BY TRANSACTION_REF ORDER BY CREATE_DATE DESC)) T
                ) TAB WHERE TAB.Status = 'Completed')
				ORDER BY TRANSACTION_CODE DESC";
            var param = new
            {
                DB_CODE = dbCode,
                FROM_DATE = fromDate,
                TO_DATE = toDate
            };
            var execute = await _sqlDataAccess.LoadData<ReportExchangeDto, dynamic>(sql, param);
            return execute.ToList();
        }

        public async Task<List<ReportExchangeDto>> GetExchangeInvoiceAlreadySendToCustomerReportAsync(string dbCode, DateTime fromDate, DateTime toDate)
        {
            var sql = @"SELECT COMPLETED.CREATE_DATE [AllocationDate],EXCHANGE.INVOICE_DATE [InvoiceDate],CUS.*,EXCHANGE.TRANSACTION_CODE [InvoiceNumber],ITEM.ITEM_CODE ItemCode,QUANTITY Quantity,UNIT_PRICE UnitPrice,TOTAL Total FROM TB_BC_CHANGEINVOICE_EXCHANGE_INVOICE EXCHANGE INNER JOIN(SELECT TRANSACTION_REF,CREATE_DATE,Status FROM(
                SELECT CREATE_DATE,TRANSACTION_REF,Status Id,StatusText,CASE WHEN InvoiceAmount = Paid THEN 'Completed' ELSE '' END [Status] FROM(
                SELECT TOP 1 WITH TIES CREATE_DATE,TRANSACTION_REF,Status,StatusText,InvoiceAmount,Paid FROM (
                SELECT D.CREATE_DATE,N.TRANSACTION_REF,CASE WHEN R.RETURN_ID IS NOT NULL THEN R.RETURN_ID ELSE P.PAYMENT_ID END [Status]
                ,CASE WHEN R.RETURN_ID IS NOT NULL THEN 'Return' ELSE 'Payment' END [StatusText],N.HEADER_TRANSACTION_VALUES [InvoiceAmount],P.AMOUNT [Paid]
                FROM NEW_INVOICE N INNER JOIN PC_DIVIDED_INVOICE D ON D.INVOICE_ID = N.ID
                LEFT JOIN PC_PAYMENT_INVOICE P ON P.DIVDIE_INVOICE_ID = D.DIVIDED_INVOICE_ID
                LEFT JOIN PC_RETURN_INVOICE R ON R.DIVIDED_INIOVICE_ID = D.DIVIDED_INVOICE_ID
                WHERE  D.DB_CODE = @DB_CODE AND N.DB_CODE = @DB_CODE AND D.CREATE_DATE BETWEEN @FROM_DATE AND @TO_DATE) T ORDER BY ROW_NUMBER() OVER (PARTITION BY TRANSACTION_REF ORDER BY CREATE_DATE DESC)) T
                ) TAB WHERE TAB.Status = 'Completed') COMPLETED ON COMPLETED.TRANSACTION_REF = EXCHANGE.TRANSACTION_CODE
				INNER JOIN TB_BC_CHANGEINVOICE_EXCHANGE_ITEM ITEM ON ITEM.INVOICE_ID = EXCHANGE.ID
				LEFT JOIN GET_CUSTOMERS_BY_DB_CODE() CUS ON CUS.CustomerCode = EXCHANGE.CUSTOMER_CODE
				WHERE EXCHANGE.DB_CODE = @DB_CODE ORDER BY TRANSACTION_CODE";
            var parameters = new
            {
                DB_CODE = dbCode,
                FROM_DATE = fromDate,
                TO_DATE = toDate
            };
            var execute = await _sqlDataAccess.LoadData<ReportExchangeDto, dynamic>(sql, parameters);
            return execute.ToList();
        }
    }
}
