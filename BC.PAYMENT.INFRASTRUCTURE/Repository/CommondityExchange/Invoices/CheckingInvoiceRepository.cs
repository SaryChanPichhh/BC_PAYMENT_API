namespace BC.PAYMENT.INFRASTRUCTURE.Repository.CommondityExchange.Invoices
{
    public class CheckingInvoiceRepository : ICheckingInvoiceRepository
    {

        private readonly ISqlDataAccess _sqlDataAccess;

        public CheckingInvoiceRepository(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<List<CheckingInvoiceDto>> LoadOldInvoicesAsync(string dbCode, InvoiceType request,int page,int pageSize)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 1;
            var offSet = (page - 1) * pageSize;
            var sql =
                @$"SELECT * FROM(SELECT TRANSACTION_CODE [Transaction],CONVERT(DATE,H.TRANS_DATE) Date,E.CUSTOMER_CODE CustomerCode,'Exchange' [STATUS] FROM TB_BC_CHANGEINVOICE_EXCHANGE_INVOICE E INNER JOIN {dbCode}SISOHDR H ON H.TRANS_REF = E.TRANSACTION_CODE
		        WHERE E.DB_CODE = @DB_CODE
                UNION
                SELECT DISTINCT INVOICE_NUMBER,CONVERT(DATE,INVOICE_DATE) Date,I.CUSTOMER_CODE CustomerCode,'Repair' [STATUS] FROM TB_BC_CHANGEINVOICE_REPAIR_INVOICE I INNER JOIN TB_BC_CHANGEINVOICE_REPAIR_INVOICE_DETAILS DETAILS
				ON DETAILS.INVOICE_ID = I.INVOICE_NUMBER
				WHERE DETAILS.REPAIR_COMPLETED_ID IS NOT NULL AND DETAILS.REPAIR_COMPLETED_ID >0 
				AND I.DB_CODE_1 = @DB_CODE
				) T WHERE T.STATUS = @STATUS ORDER BY Date DESC OFFSET @OFFSET ROWS FETCH NEXT @PAGESIZE ROWS ONLY";
            var param = new
            {
                DB_CODE = dbCode,
                STATUS = request.GetDescription(),
                OFFSET = offSet,
                PAGESIZE = pageSize,
            };
            var execute = await _sqlDataAccess.LoadData<CheckingInvoiceDto, dynamic>(sql, param);
            return execute.ToList();

        }

        public async Task<List<CheckingInvoiceDto>> LoadNewInvoicesAsync(string dbCode,int page,int pageSize, InvoiceType request)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 1;
            var offset = (page - 1) * pageSize;

            string sql = @$"SELECT INVOICE_NUMBER [Transaction],CONVERT(DATE,INVOICE_DATE) [Date],T.CUST_CODE CustomerCode,[STATUS] STATUS FROM(
            SELECT INVOICE.INVOICE_NUMBER,INVOICE.INVOICE_DATE,HEADER.CUST_CODE,'Repair' [STATUS] FROM TB_BC_CHANGEINVOICE HEADER INNER JOIN TB_BC_CHANGEINVOICE_DETAIL DETAIL ON DETAIL.CHANGE_INVOICE_ID = HEADER.ID
            INNER JOIN TB_BC_CHANGEINVOICE_RECEIVED RECEIVED ON RECEIVED.CHANGE_INVOICE_DETAIL_ID = DETAIL.ID
            INNER JOIN TB_BC_CHANGEINVOICE_REPAIR REPAIR ON REPAIR.RECEIVED_ID = RECEIVED.ID
            INNER JOIN TB_BC_CHANGEINVOICE_REPAIR_COMPLETED COMPLETED ON COMPLETED.TRAN_REF = REPAIR.TRAN_REF
            INNER JOIN TB_BC_CHANGEINVOICE_REPAIR_INVOICE_DETAILS INVOICE_DETAIL ON INVOICE_DETAIL.TRANS_REF = COMPLETED.TRAN_REF
            INNER JOIN TB_BC_CHANGEINVOICE_REPAIR_INVOICE INVOICE ON INVOICE.INVOICE_NUMBER = INVOICE_DETAIL.INVOICE_ID
            WHERE INVOICE.DB_CODE_1 = @DB_CODE
            UNION ALL
            SELECT INVOICE.INVOICE_NUMBER,INVOICE.INVOICE_DATE,HEADER.CUST_CODE,'Repair' [STATUS] FROM TB_BC_CHANGEINVOICE HEADER INNER JOIN TB_BC_CHANGEINVOICE_DETAIL DETAIL ON DETAIL.CHANGE_INVOICE_ID = HEADER.ID 
            INNER JOIN TB_BC_CHANGEINVOICE_RECEIVED RECEIVED ON RECEIVED.CHANGE_INVOICE_DETAIL_ID = DETAIL.ID 
            INNER JOIN TB_BC_CHANGEINVOICE_REPAIR_TRANSFER [TRANSFER] ON [TRANSFER].RECEIVED_ID = RECEIVED.ID
            INNER JOIN TB_BC_CHANGEINVOICE_REPAIR REPAIR ON REPAIR.TRAN_REF = [TRANSFER].TRAN_REF
            INNER JOIN TB_BC_CHANGEINVOICE_REPAIR_COMPLETED COMPLETED ON COMPLETED.TRAN_REF = REPAIR.TRAN_REF
            INNER JOIN TB_BC_CHANGEINVOICE_REPAIR_INVOICE_DETAILS INVOICE_DETAIL ON INVOICE_DETAIL.TRANS_REF = COMPLETED.TRAN_REF
            INNER JOIN TB_BC_CHANGEINVOICE_REPAIR_INVOICE INVOICE ON INVOICE.INVOICE_NUMBER = INVOICE_DETAIL.INVOICE_ID
            WHERE INVOICE.DB_CODE_1 = @DB_CODE
            UNION ALL
            SELECT * FROM(SELECT TRANSACTION_CODE [Transaction],CONVERT(DATE,H.TRANS_DATE) Date,H.CUST_CODE,'Exchange' [STATUS] FROM TB_BC_CHANGEINVOICE_EXCHANGE_INVOICE E INNER JOIN {dbCode}SISOHDR H ON 
            H.TRANS_REF = E.TRANSACTION_CODE WHERE E.DB_CODE = @DB_CODE)T)T WHERE T.STATUS = @STATUS ORDER BY INVOICE_DATE DESC OFFSET @OFFSET ROWS FETCH NEXT @PAGESIZE ROWS ONLY";
            var param = new
            {
                DB_CODE = dbCode,
                OFFSET = offset,
                PAGESIZE = pageSize,
                STATUS = request.GetDescription(),
            };
            var execute = await _sqlDataAccess.LoadData<CheckingInvoiceDto, dynamic>(sql, param);
            return execute.ToList();
        }

        public async Task<ExchangeInvoiceDetailRespondDto> GetExchangeNewInvoiceDetailByTransactionCode(string dbCode, string transactionCode)
        {
            ExchangeInvoiceDetailRespondDto exchangeInvoiceDetail = new();
            var customerQuery =
                @"SELECT * FROM (
                        SELECT T.*,TRANSACTION_CODE [Transaction],S.LAST_NAME LastName, S.FIRST_NAME FirstName,E.INVOICE_DATE Date,E.INVOICE_BY UserCode
                        FROM TB_BC_CHANGEINVOICE_EXCHANGE_INVOICE E LEFT JOIN (SELECT ADD_CODE CustomerCode,ADD_LINE_1KH CustomerName,
		                MARKET_KHMER_NAME Market,AREA_NAME_KHMER Area,ADD_TEL Phone,GOOGLE_MAP Map,S.PICTURE Image 
                        FROM SIADD S LEFT JOIN TB_BCMARKET M ON M.MARKET_ID = S.MARKET_ID LEFT JOIN TB_AREAS A ON A.AREA_ID = S.AREA_ID
                        WHERE S.DB_CODE = @DB_CODE AND M.DB_CODE = @DB_CODE AND A.DB_CODE = @DB_CODE) T ON T.CustomerCode = E.CUSTOMER_CODE
                        INNER JOIN TB_BC_CHANGEINVOICE C ON C.ID = E.REQUEST_EXCHANGE_ID
                        INNER JOIN dbo.BCUSERS S ON S.USER_ID = C.USER_CODE 
		                WHERE C.DB_CODE = @DB_CODE  AND E.DB_CODE = @DB_CODE) T WHERE T.[Transaction] = @Transaction";
            var customerParam = new
            {
                DB_CODE = dbCode,
                Transaction = transactionCode,
            };
            var execute = await _sqlDataAccess.LoadSingleData<CustomerInvoiceDto, dynamic>(customerQuery, customerParam);

            var exchangeItemQuery = @"SELECT ITEM_CODE ItemCode,QUANTITY Quantity,UNIT_PRICE UnitPrice FROM TB_BC_CHANGEINVOICE_EXCHANGE_ITEM D INNER JOIN 
                        TB_BC_CHANGEINVOICE_EXCHANGE_INVOICE M ON M.ID = D.INVOICE_ID
                        WHERE M.TRANSACTION_CODE = @Transaction AND M.DB_CODE = @DB_CODE";
            var exchangeItemParam = new
            {
                DB_CODE = dbCode,
                Transaction = transactionCode,
            };
            var exchangeItem = await _sqlDataAccess.LoadData<ItemExchangeInvoiceDto, dynamic>(exchangeItemQuery, exchangeItemParam);

            var itemExchangedQuery = @"SELECT ITEM_CODE ItemCode,QUANTITY Quantity,UNIT_PRICE UnitPrice FROM TB_BC_CHANGEINVOICE_ITEM_EXCHANGED
                        WHERE DB_CODE = @DB_CODE AND [TRANSACTION] = @Transaction";
            var itemExchangedParam = new
            {
                DB_CODE = dbCode,
                Transaction = transactionCode,
            };
            var itemExchanged = await _sqlDataAccess.LoadData<ItemExchangeInvoiceDto, dynamic>(itemExchangedQuery, itemExchangedParam);


            exchangeInvoiceDetail.Customer = execute;
            exchangeInvoiceDetail.OutBoundExchangeItems = exchangeItem.ToList();
            exchangeInvoiceDetail.InBoundExchangeItems = itemExchanged.ToList();

            return exchangeInvoiceDetail;
        }

        public async Task<RepairInvoiceDetailRespondDto> GetRepairNewInvoiceDetailByTransactionCode(string dbCode, string transactionCode)
        {
            var invoiceDetail = new RepairInvoiceDetailRespondDto();
            var sql = @"SELECT INVOICE_NUMBER [Transaction],CONVERT(DATE,INVOICE_DATE) [Date],T.CustomerCode,T.CustomerName,T.Store,T.Market,T.Area,T.Phone,T.UserCode,FirstName,LastName,Description,T.TRAN_REF FROM(
                    SELECT INVOICE.INVOICE_NUMBER,INVOICE.INVOICE_DATE,CUS.*,S.FIRST_NAME FirstName,S.LAST_NAME LastName,HEADER.USER_CODE UserCode,INVOICE.DESCRIPTION [Description],REPAIR.TRAN_REF FROM TB_BC_CHANGEINVOICE HEADER INNER JOIN TB_BC_CHANGEINVOICE_DETAIL DETAIL ON DETAIL.CHANGE_INVOICE_ID = HEADER.ID
                    INNER JOIN TB_BC_CHANGEINVOICE_RECEIVED RECEIVED ON RECEIVED.CHANGE_INVOICE_DETAIL_ID = DETAIL.ID
                    INNER JOIN TB_BC_CHANGEINVOICE_REPAIR REPAIR ON REPAIR.RECEIVED_ID = RECEIVED.ID
                    INNER JOIN TB_BC_CHANGEINVOICE_REPAIR_COMPLETED COMPLETED ON COMPLETED.TRAN_REF = REPAIR.TRAN_REF 
                    INNER JOIN TB_BC_CHANGEINVOICE_REPAIR_INVOICE_DETAILS INVOICE_DETAIL ON INVOICE_DETAIL.TRANS_REF = REPAIR.TRAN_REF
                    INNER JOIN TB_BC_CHANGEINVOICE_REPAIR_INVOICE INVOICE ON INVOICE.INVOICE_NUMBER = INVOICE_DETAIL.INVOICE_ID AND INVOICE.DB_CODE_1 = LEFT(INVOICE_DETAIL.TRANS_REF,3)
                    LEFT JOIN (SELECT S.ADD_CODE CustomerCode,S.ADD_LINE_1KH CustomerName,STORE Store,M.MARKET_KHMER_NAME Market,A.AREA_NAME_KHMER Area,S.ADD_TEL Phone FROM 
                                        SIADD S INNER JOIN TB_BCMARKET M ON M.MARKET_ID = S.MARKET_ID INNER JOIN TB_AREAS A ON A.AREA_ID = S.AREA_ID WHERE S.DB_CODE = @DB_CODE AND A.DB_CODE = @DB_CODE AND M.DB_CODE = @DB_CODE) 
                                        CUS ON CUS.CustomerCode = HEADER.CUST_CODE
					                    INNER JOIN dbo.BCUSERS S ON S.USER_ID = HEADER.USER_CODE
                    WHERE INVOICE.DB_CODE_1 = @DB_CODE AND SUBSTRING(INVOICE_DETAIL.TRANS_REF,1,3) = @DB_CODE
                    UNION ALL
                    SELECT INVOICE.INVOICE_NUMBER,INVOICE.INVOICE_DATE,CUS.*,S.FIRST_NAME FirstName, S.LAST_NAME LastName,HEADER.USER_CODE UserCode,INVOICE.DESCRIPTION [Description],REPAIR.TRAN_REF  FROM TB_BC_CHANGEINVOICE HEADER INNER JOIN TB_BC_CHANGEINVOICE_DETAIL DETAIL ON DETAIL.CHANGE_INVOICE_ID = HEADER.ID 
                    INNER JOIN TB_BC_CHANGEINVOICE_RECEIVED RECEIVED ON RECEIVED.CHANGE_INVOICE_DETAIL_ID = DETAIL.ID 
                    INNER JOIN TB_BC_CHANGEINVOICE_REPAIR_TRANSFER [TRANSFER] ON [TRANSFER].RECEIVED_ID = RECEIVED.ID
                    INNER JOIN TB_BC_CHANGEINVOICE_REPAIR REPAIR ON REPAIR.TRAN_REF = [TRANSFER].TRAN_REF
                    INNER JOIN TB_BC_CHANGEINVOICE_REPAIR_COMPLETED COMPLETED ON COMPLETED.TRAN_REF = REPAIR.TRAN_REF
                    INNER JOIN TB_BC_CHANGEINVOICE_REPAIR_INVOICE_DETAILS INVOICE_DETAIL ON INVOICE_DETAIL.TRANS_REF = REPAIR.TRAN_REF
                    INNER JOIN TB_BC_CHANGEINVOICE_REPAIR_INVOICE INVOICE ON INVOICE.INVOICE_NUMBER = INVOICE_DETAIL.INVOICE_ID  AND INVOICE.DB_CODE_1 = LEFT(INVOICE_DETAIL.TRANS_REF,3)
                    LEFT JOIN (SELECT S.ADD_CODE CustomerCode,S.ADD_LINE_1KH CustomerName,STORE Store,M.MARKET_KHMER_NAME Market,A.AREA_NAME_KHMER Area,S.ADD_TEL Phone FROM 
                                        SIADD S INNER JOIN TB_BCMARKET M ON M.MARKET_ID = S.MARKET_ID INNER JOIN TB_AREAS A ON A.AREA_ID = S.AREA_ID WHERE S.DB_CODE = @DB_CODE AND A.DB_CODE = @DB_CODE AND M.DB_CODE = @DB_CODE) 
                                        CUS ON CUS.CustomerCode = HEADER.CUST_CODE
                    INNER JOIN BCUSERS S ON S.USER_ID = HEADER.USER_CODE
                    WHERE INVOICE.DB_CODE_1 = @DB_CODE )T WHERE T.INVOICE_NUMBER = @Transaction  ORDER BY INVOICE_DATE DESC";
            var param = new
            {
                DB_CODE = dbCode,
                Transaction = transactionCode,
            };

            var execute = await _sqlDataAccess.LoadSingleData<CustomerInvoiceDto, dynamic>(sql, param);

            sql = @"SELECT  UNIT_PRICE UnitPrice,R.DESCRIPTION Description,DETAILS.QUANTITY Quantity,R.ITEM_CODE ItemCode FROM TB_BC_CHANGEINVOICE_REPAIR_INVOICE_DETAILS DETAILS 
						INNER JOIN TB_BC_CHANGEINVOICE_REPAIR R ON R.TRAN_REF = DETAILS.TRANS_REF
						INNER JOIN (SELECT DISTINCT TRAN_REF,STATUS FROM(SELECT STATUS,TRAN_REF,STATUS_REPAIR FROM TB_BC_CHANGEINVOICE_REPAIR_COMPLETED )RC GROUP BY STATUS,TRAN_REF) RC ON RC.TRAN_REF = R.TRAN_REF
						INNER JOIN TB_BC_CHANGEINVOICE_REPAIR_INVOICE RI ON RI.INVOICE_NUMBER = DETAILS.INVOICE_ID
						WHERE INVOICE_ID = @Transaction AND RI.DB_CODE_1 = @DB_CODE AND RC.STATUS = 'Completed'
                        AND RI.INVOICE_NUMBER = @Transaction 
                        AND SUBSTRING(DETAILS.TRANS_REF,1,3) = RI.DB_CODE_1 AND DETAILS.QUANTITY>0";
            var itemRepairParam = new
            {
                Transaction = transactionCode,
                DB_CODE = dbCode
            };
            var itemRepair = await _sqlDataAccess.LoadData<ItemRepairInvoiceDto, dynamic>(sql,itemRepairParam);
            invoiceDetail.Customer = execute;
            invoiceDetail.RepairItems = itemRepair.ToList();

            return invoiceDetail;
        }

        public async Task<ExchangeInvoiceDetailRespondDto> GetExchangeOldInvoiceDetailByTransactionCode(string dbCode, string transactionCode)
        {
            ExchangeInvoiceDetailRespondDto exchangeInvoiceDetail = new();
            var customerQuery =
                @"SELECT * FROM (
                        SELECT T.*,TRANSACTION_CODE [Transaction],S.LAST_NAME LastName, S.FIRST_NAME FirstName,E.INVOICE_DATE Date,E.INVOICE_BY UserCode
                        FROM TB_BC_CHANGEINVOICE_EXCHANGE_INVOICE E LEFT JOIN (SELECT ADD_CODE CustomerCode,ADD_LINE_1KH CustomerName,
		                MARKET_KHMER_NAME Market,AREA_NAME_KHMER Area,ADD_TEL Phone,GOOGLE_MAP Map,S.PICTURE Image 
                        FROM SIADD S LEFT JOIN TB_BCMARKET M ON M.MARKET_ID = S.MARKET_ID LEFT JOIN TB_AREAS A ON A.AREA_ID = S.AREA_ID
                        WHERE S.DB_CODE = @DB_CODE AND M.DB_CODE = @DB_CODE AND A.DB_CODE = @DB_CODE) T ON T.CustomerCode = E.CUSTOMER_CODE
                        INNER JOIN TB_BC_CHANGEINVOICE C ON C.ID = E.REQUEST_EXCHANGE_ID
                        INNER JOIN dbo.BCUSERS S ON S.USER_ID = C.USER_CODE 
		                WHERE C.DB_CODE = @DB_CODE  AND E.DB_CODE = @DB_CODE) T WHERE T.[Transaction] = @Transaction";
            var customerParam = new
            {
                DB_CODE = dbCode,
                Transaction = transactionCode,
            };
            var execute = await _sqlDataAccess.LoadSingleData<CustomerInvoiceDto, dynamic>(customerQuery, customerParam);

            var exchangeItemQuery = @"SELECT ITEM_CODE ItemCode,QUANTITY Quantity,UNIT_PRICE UnitPrice FROM TB_BC_CHANGEINVOICE_EXCHANGE_ITEM D INNER JOIN 
                        TB_BC_CHANGEINVOICE_EXCHANGE_INVOICE M ON M.ID = D.INVOICE_ID
                        WHERE M.TRANSACTION_CODE = @Transaction AND M.DB_CODE = @DB_CODE";
            var exchangeItemParam = new
            {
                DB_CODE = dbCode,
                Transaction = transactionCode,
            };
            var exchangeItem = await _sqlDataAccess.LoadData<ItemExchangeInvoiceDto, dynamic>(exchangeItemQuery, exchangeItemParam);

            var itemExchangedQuery = @"SELECT ITEM_CODE ItemCode,QUANTITY Quantity,UNIT_PRICE UnitPrice FROM TB_BC_CHANGEINVOICE_ITEM_EXCHANGED
                        WHERE DB_CODE = @DB_CODE AND [TRANSACTION] = @Transaction";
            var itemExchangedParam = new
            {
                DB_CODE = dbCode,
                Transaction = transactionCode,
            };
            var itemExchanged = await _sqlDataAccess.LoadData<ItemExchangeInvoiceDto,dynamic>(itemExchangedQuery,itemExchangedParam);


            exchangeInvoiceDetail.Customer = execute;
            exchangeInvoiceDetail.OutBoundExchangeItems = exchangeItem.ToList();
            exchangeInvoiceDetail.InBoundExchangeItems = itemExchanged.ToList();

            return exchangeInvoiceDetail;
        }

        public async Task<RepairInvoiceDetailRespondDto> GetRepairOldInvoiceDetailByTransactionCode(string dbCode, string transactionCode)
        {
            RepairInvoiceDetailRespondDto repairInvoiceDetail = new();
            var customerQuery =
                    @"SELECT INVOICE.DB_CODE_1 DbCode,S.FIRST_NAME FirstName,S.LAST_NAME LastName,CUS.*,INVOICE.CREATED_DATE Date,INVOICE.CREATED_BY UserCode,INVOICE.INVOICE_NUMBER [Transaction],
                INVOICE.DESCRIPTION [Description]
                FROM TB_BC_CHANGEINVOICE HEADER 
                INNER JOIN TB_BC_CHANGEINVOICE_DETAIL DETAIL ON DETAIL.CHANGE_INVOICE_ID = HEADER.ID
                INNER JOIN TB_BC_CHANGEINVOICE_RECEIVED RECEIVED ON RECEIVED.CHANGE_INVOICE_DETAIL_ID = DETAIL.ID
                INNER JOIN TB_BC_CHANGEINVOICE_REPAIR_TRANSFER TRANS ON TRANS.RECEIVED_ID = RECEIVED.ID
                INNER JOIN TB_BC_CHANGEINVOICE_REPAIR_RECEIVED RC ON RC.TRAN_REF = TRANS.TRAN_REF
                INNER JOIN TB_BC_CHANGEINVOICE_REPAIR REPAIR ON REPAIR.TRAN_REF = RC.TRAN_REF
                INNER JOIN TB_BC_CHANGEINVOICE_REPAIR_COMPLETED COMPLETED ON COMPLETED.TRAN_REF = REPAIR.TRAN_REF
                INNER JOIN TB_BC_CHANGEINVOICE_REPAIR_INVOICE_DETAILS ID ON ID.TRANS_REF = COMPLETED.TRAN_REF
                INNER JOIN dbo.BCUSERS S ON S.USER_ID = HEADER.USER_CODE
                INNER JOIN TB_BC_CHANGEINVOICE_REPAIR_INVOICE INVOICE ON INVOICE.INVOICE_NUMBER = ID.INVOICE_ID and INVOICE.DB_CODE_1 = LEFT(ID.TRANS_REF,3)
                INNER JOIN (SELECT S.ADD_CODE CustomerCode,S.ADD_LINE_1KH CustomerName,M.MARKET_KHMER_NAME Market,A.AREA_NAME_KHMER Area,S.ADD_TEL Phone FROM 
                                    SIADD S INNER JOIN TB_BCMARKET M ON M.MARKET_ID = S.MARKET_ID INNER JOIN TB_AREAS A ON A.AREA_ID = S.AREA_ID WHERE S.DB_CODE = @DB_CODE AND A.DB_CODE = @DB_CODE AND M.DB_CODE = @DB_CODE) CUS ON CUS.CustomerCode = HEADER.CUST_CODE
                WHERE INVOICE.DB_CODE_1 = @DB_CODE AND INVOICE.INVOICE_NUMBER = @Transaction
                UNION
                SELECT INVOICE.DB_CODE_1,S.FIRST_NAME,S.LAST_NAME,CUS.*,INVOICE.CREATED_DATE Date,INVOICE.CREATED_BY UserCode,INVOICE.INVOICE_NUMBER [Transaction],
                INVOICE.DESCRIPTION [Description] FROM TB_BC_CHANGEINVOICE HEADER 
                INNER JOIN TB_BC_CHANGEINVOICE_DETAIL DETAIL ON DETAIL.CHANGE_INVOICE_ID = HEADER.ID
                INNER JOIN TB_BC_CHANGEINVOICE_RECEIVED RECEIVED ON RECEIVED.CHANGE_INVOICE_DETAIL_ID = DETAIL.ID
                INNER JOIN TB_BC_CHANGEINVOICE_REPAIR REPAIR ON REPAIR.RECEIVED_ID = RECEIVED.ID
                INNER JOIN TB_BC_CHANGEINVOICE_REPAIR_COMPLETED COMPLETED ON COMPLETED.TRAN_REF = REPAIR.TRAN_REF
                INNER JOIN TB_BC_CHANGEINVOICE_REPAIR_INVOICE_DETAILS ID ON ID.TRANS_REF = COMPLETED.TRAN_REF
                INNER JOIN TB_BC_CHANGEINVOICE_REPAIR_INVOICE INVOICE ON INVOICE.INVOICE_NUMBER = ID.INVOICE_ID and INVOICE.DB_CODE_1 = LEFT(ID.TRANS_REF,3)
                INNER JOIN dbo.BCUSERS S ON S.USER_ID = HEADER.USER_CODE
                INNER JOIN (SELECT S.ADD_CODE CustomerCode,S.ADD_LINE_1KH CustomerName,M.MARKET_KHMER_NAME Market,A.AREA_NAME_KHMER Area,S.ADD_TEL Phone FROM 
                                    SIADD S INNER JOIN TB_BCMARKET M ON M.MARKET_ID = S.MARKET_ID INNER JOIN TB_AREAS A ON A.AREA_ID = S.AREA_ID WHERE S.DB_CODE = @DB_CODE AND A.DB_CODE = @DB_CODE AND M.DB_CODE = @DB_CODE) CUS ON CUS.CustomerCode = HEADER.CUST_CODE
                WHERE INVOICE.DB_CODE_1 = @DB_CODE AND INVOICE.INVOICE_NUMBER = @Transaction";

            var customerParam = new
            {
                DB_CODE = dbCode,
                Transaction = transactionCode,
            };
            var customer = await _sqlDataAccess.LoadSingleData<CustomerInvoiceDto, dynamic>(customerQuery,customerParam);

            var itemRepairQuery =
                @"SELECT D.ITEM_CODE ItemCode,D.DESCRIPION [Description],ID.QUANTITY Quantity,ID.UNIT_PRICE UnitPrice FROM TB_BC_CHANGEINVOICE M INNER JOIN TB_BC_CHANGEINVOICE_DETAIL D ON D.CHANGE_INVOICE_ID = M.ID
                        INNER JOIN TB_BC_CHANGEINVOICE_RECEIVED C ON C.CHANGE_INVOICE_DETAIL_ID = D.ID
                        INNER JOIN TB_BC_CHANGEINVOICE_REPAIR R ON R.RECEIVED_ID = C.ID
                        INNER JOIN TB_BC_CHANGEINVOICE_REPAIR_COMPLETED Completed on Completed.REPAIR_ID = R.ID
                        INNER JOIN TB_BC_CHANGEINVOICE_REPAIR_INVOICE_DETAILS ID ON ID.REPAIR_COMPLETED_ID = Completed.ID
                        INNER JOIN TB_BC_CHANGEINVOICE_REPAIR_INVOICE I ON I.INVOICE_NUMBER = ID.INVOICE_ID
                        WHERE I.DB_CODE = @DB_CODE AND M.DB_CODE = @DB_CODE  AND I.INVOICE_NUMBER = @Transaction
                        ";
            var itemRepairParam = new
            {
                DB_CODE = dbCode,   
                Transaction = transactionCode,
            };
            var itemRepairs = await _sqlDataAccess.LoadData<ItemRepairInvoiceDto, dynamic>(itemRepairQuery,itemRepairParam);


            repairInvoiceDetail.Customer = customer;
            repairInvoiceDetail.RepairItems = itemRepairs.ToList();

            return repairInvoiceDetail;
        }
    }
}
