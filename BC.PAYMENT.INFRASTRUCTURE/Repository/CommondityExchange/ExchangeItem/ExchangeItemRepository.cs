namespace BC.PAYMENT.INFRASTRUCTURE.Repository.CommondityExchange.ExchangeItem
{
    
    public  class ExchangeItemRepository : IExchangeItemRepository
    {
        private readonly ISqlDataAccess _sqlDataAccess;
        private readonly IConfiguration _configuration;
        public ExchangeItemRepository(ISqlDataAccess sqlDataAccess, IConfiguration configuration)
        {
            _sqlDataAccess = sqlDataAccess;
            _configuration = configuration;
        }

        public async Task<List<CustomerDto>> GetAllCustomerHasExchangeGoods(string dbCode)
        {
            const string sql = @"
                            	 SELECT ID MasterId,TAB1.ReceivedId,TAB1.RequestDetailId,CUST_CODE CustomerCode,ADD_LINE_1KH CustomerName,MARKET_KHMER_NAME Market,AREA_NAME_KHMER Area,Phone,USER_CODE UserCode,TAB1.FIRST_NAME FirstName,TAB1.LAST_NAME LastName FROM(
                        (SELECT DISTINCT M.ID,R.ID ReceivedId,D.ID RequestDetailId, CUST_CODE,M.USER_CODE,S.FIRST_NAME, S.LAST_NAME FROM TB_BC_CHANGEINVOICE M INNER JOIN TB_BC_CHANGEINVOICE_DETAIL D ON D.CHANGE_INVOICE_ID = M.ID
                        INNER JOIN TB_BC_CHANGEINVOICE_RECEIVED R ON R.CHANGE_INVOICE_DETAIL_ID = D.ID
                                    INNER JOIN dbo.BCUSERS S ON S.USER_ID = M.USER_CODE
                        WHERE M.DB_CODE = @DB_CODE AND D.IS_RECEIVED IN ('Yes','CreditNote') AND M.IS_RECEIVED <> 'Rejected' AND [TYPE] = 'EXCHANGE') TAB1
                        LEFT JOIN(
                        SELECT ADD_CODE,ADD_LINE_1KH,MARKET_KHMER_NAME,AREA_NAME_KHMER,S.PICTURE,ADD_TEL Phone FROM 
                        SIADD S INNER JOIN TB_BCMARKET M ON M.MARKET_ID = S.MARKET_ID 
                        INNER JOIN TB_AREAS A ON A.AREA_ID = S.AREA_ID
                        WHERE S.DB_CODE = @DB_CODE AND M.DB_CODE = @DB_CODE AND A.DB_CODE = @DB_CODE) TAB2 ON TAB1.CUST_CODE = TAB2.ADD_CODE) 
                ";
            var param = new
            {
                DB_CODE = dbCode,
            };
            var execute = await _sqlDataAccess.LoadData<CustomerDto, dynamic>(sql, param);
            return execute.ToList();
        }

        public async Task<List<ItemExchangeDto>> GetAllItemExchangeByCustomerCodeAsync(string dbCode, int requestId)
        {
            const string sql =
                @"SELECT TBCD.ID RequestExchangeDetailId,TBCR.ID ReceivedId, ITEM.ITEM_CODE ItemCode, ITEM_NAME ItemName, TBCD.QUANTITY Quantity, UNIT_PRICE UnitPrice, 
                UNIT_PRICE * TBCD.QUANTITY Total, TBCD.DESCRIPION   
                Description, TBC.CREATED_DATE RequestDate
                    FROM TB_BC_CHANGEINVOICE TBC INNER JOIN
                        TB_BC_CHANGEINVOICE_DETAIL TBCD ON TBC.ID = TBCD.CHANGE_INVOICE_ID
                        INNER JOIN TB_BC_CHANGEINVOICE_RECEIVED TBCR ON TBCR.CHANGE_INVOICE_DETAIL_ID = TBCD.ID
                        INNER JOIN (SELECT ITEM_CODE, CASE WHEN ITEM_CUS10_KH = '' THEN ITEM_DESC ELSE ITEM_CUS10_KH END 'ITEM_NAME',
						ITEM_PRICE4 UNIT_PRICE
                        FROM SIITEMS
                        WHERE DB_CODE = @DB_CODE)
                                     ITEM ON ITEM.ITEM_CODE = TBCD.ITEM_CODE
                    WHERE TBC.ID = @REQUEST_ID AND TBC.DB_CODE = @DB_CODE AND TYPE = 'EXCHANGE' AND TBCR.STATUS = 0 AND TBCD.IS_RECEIVED <> 'Rejected' AND TBC.IS_RECEIVED <> 'Rejected'";
            var param = new
            {
                REQUEST_ID = requestId,
                DB_CODE = dbCode
            };
            var execute = await _sqlDataAccess.LoadData<ItemExchangeDto, dynamic>(sql,param);
            return execute.ToList();
        }

        public async Task<List<ItemExchangeDto>> GetAllItemCreditByCustomerCodeAsync(string dbCode,int requestId)
        {
            const string sql =
                @"SELECT TBCD.ID RequestExchangeDetailId,TBCR.ID ReceivedId, ITEM.ITEM_CODE ItemCode, ITEM_NAME ItemName, TBCD.QUANTITY Quantity, UNIT_PRICE UnitPrice, 
                UNIT_PRICE * TBCD.QUANTITY Total, TBCD.DESCRIPION 
                Description, TBC.CREATED_DATE RequestDate
                    FROM TB_BC_CHANGEINVOICE TBC INNER JOIN
                        TB_BC_CHANGEINVOICE_DETAIL TBCD ON TBC.ID = TBCD.CHANGE_INVOICE_ID
                        INNER JOIN TB_BC_CHANGEINVOICE_RECEIVED TBCR ON TBCR.CHANGE_INVOICE_DETAIL_ID = TBCD.ID
                        INNER JOIN (SELECT ITEM_CODE, CASE WHEN ITEM_CUS10_KH = '' THEN ITEM_DESC ELSE ITEM_CUS10_KH END 'ITEM_NAME',    
						ITEM_PRICE1 UNIT_PRICE
                        FROM SIITEMS
                        WHERE DB_CODE = @DB_CODE) ITEM ON ITEM.ITEM_CODE = TBCD.ITEM_CODE
                    WHERE TBC.ID = @ID AND TBC.DB_CODE = @DB_CODE AND TYPE = 'EXCHANGE' AND TBCR.STATUS = 1 AND TBC.IS_RECEIVED <> 'Rejected'";
            var param = new
            {
                ID = requestId,
                DB_CODE = dbCode,
            };
            var execute = await _sqlDataAccess.LoadData<ItemExchangeDto, dynamic>(sql,param);

            return execute.ToList();
        }

        public async Task<List<InvoiceForExchangeDto>> GetInvoiceForExchangeByCustomerCodeAndItemCodeIn6MonthsAsync(string dbCode,string customerCode, string itemCode)
        {
            var sql =
                $@"SELECT M.TRANS_DATE TransactionDate,D.TRANS_LINE MovLine,ITEM_CODE ItemCode,D.LINE_REF ExpiredDate,D.TRANS_REF [Transaction],TRANS_LINE TransactionLine,D.VALUE_1 Quantity FROM {dbCode}SISODET D INNER JOIN {dbCode}SISOHDR M ON M.TRANS_REF = D.TRANS_REF
					WHERE CUST_CODE = @CustomerCode AND ITEM_CODE = @ItemCode AND D.REC_TYPE = 'D' AND D.CREDIT_STATUS = '' AND D.CREDIT_STATUS = 0 AND VALUE_3 != '0.00000' AND M.TRANS_DATE >= DATEADD(MONTH, -6, GETDATE()) AND M.STATUS = '80'
                GROUP BY M.TRANS_DATE,ITEM_CODE,D.TRANS_REF,TRANS_LINE,D.VALUE_1,D.LINE_REF
					ORDER BY CONVERT(DATE,M.TRANS_DATE)";
            var param = new
            {
                ItemCode = itemCode,
                CustomerCode = customerCode
            };
            var execute = await _sqlDataAccess.LoadData<InvoiceForExchangeDto, dynamic>(sql,param);
            return execute.ToList();
        }

        public async Task<List<InvoiceForExchangeDto>> GetInvoiceForExchangeByCustomerCodeAndItemCodeAsync(string dbCode,string customerCode, string itemCode)
        {
            var sql =
                $@"SELECT M.TRANS_DATE TransactionDate,D.TRANS_LINE MovLine,ITEM_CODE ItemCode,D.LINE_REF ExpiredDate,D.TRANS_REF [Transaction],TRANS_LINE TransactionLine,D.VALUE_1 Quantity FROM {dbCode}SISODET D INNER JOIN {dbCode}SISOHDR M ON M.TRANS_REF = D.TRANS_REF
					WHERE CUST_CODE = @CustomerCode AND ITEM_CODE = @ItemCode AND D.REC_TYPE = 'D' AND D.CREDIT_STATUS = '' AND D.CREDIT_STATUS = 0 AND M.STATUS = '80' --AND VALUE_3 != '0.00000' 
                GROUP BY M.TRANS_DATE,ITEM_CODE,D.TRANS_REF,TRANS_LINE,D.VALUE_1,D.LINE_REF
					ORDER BY CONVERT(DATE,M.TRANS_DATE)";
            var param = new
            {
                ItemCode = itemCode,
                CustomerCode = customerCode
            };
            var execute = await _sqlDataAccess.LoadData<InvoiceForExchangeDto, dynamic>(sql, param);
            return execute.ToList();
        }

        public async Task<int> AddNewCreditNote(CreditNoteItemModel model)
        {
            var affectedRow = 0;
            var connection = new SqlConnection(_configuration.GetConnectionString("DBConnection"));
            if(connection.State == ConnectionState.Closed) connection.Open();
            var transaction = connection.BeginTransaction();

            try
            {
                var creditInvoiceDetail = new CreditInvoiceDetailsDto
                {
                    NewTransaction = model.NewTransaction,
                    OldLine = model.OldTranLine,
                    OldTransaction = model.OldTransaction,
                    CreditPeriod = model.Period,
                    UserCreated = model.UserName,
                };
                var sql =
                    @$"SELECT TRANS_LINE [Key],VALUE_1 [Value] FROM {model.DbCode}SISODET WHERE TRANS_REF = @TransactionCode AND ITEM_CODE = @ItemCode AND CREDIT_STATUS = '' AND TRANS_LINE = @TRANS_LINE --AND VALUE_3 != '0.00000'";
                var param = new
                {
                    TransactionCode = model.OldTransaction,
                    ItemCode = model.ItemCode,
                    TRANS_LINE = model.OldTranLine,
                };
                var result = (await connection.QueryAsync<KeyValuePair<string,double>>(sql,param,transaction)).ToDictionary(x=>x.Key,x=>x.Value);

                foreach (var item in result)
                {
                    var transactionLine = item.Key;
                    var sqlGetMaxCode =
                        $@"SELECT MAX(TRANS_LINE) FROM {model.DbCode}SISODET WHERE TRANS_REF = @TransactionCode AND ITEM_CODE = @ItemCode";
                    var maxCodeParam = new
                    {
                        ItemCode = model.ItemCode,
                        TransactionCode = model.OldTransaction,
                    };
                    var maxTranLine =
                        await connection.QueryFirstOrDefaultAsync<string>(sqlGetMaxCode, maxCodeParam, transaction);
                        maxTranLine = maxTranLine.Replace(" ", "");
                        if (maxTranLine.Length >= 5)
                    {
                        var lastIndex = int.Parse(maxTranLine[maxTranLine.Length - 1] + "");
                        var remove = maxTranLine.Remove(maxTranLine.Length - 1);
                        var newText = remove.Insert(maxTranLine.Length - 1, lastIndex + 1 + "");
                        //  await SplitInvoice(oldTransaction, itemCode, transactionLine, newText, itemQty);
                        var insertDetailQuery = $@"INSERT INTO {model.DbCode}SISODET
                            SELECT REC_TYPE,DETAIL_ID,TRANS_TYPE,TRANS_REF,@LINE,TRANS_CD,LOCATION,ITEM_CODE,DESCRIPTN,DUE_DATE,STATUS,@QTY,@QTY,VALUE_3,@QTY * VALUE_3,VALUE_5,VALUE_6,@QTY * VALUE_3,VALUE_8,VALUE_9,VALUE_10,
                            VALUE_11,VALUE_12,@QTY * VALUE_3,VALUE_14,VALUE_15,VALUE_16,VALUE_17,VALUE_18,VALUE_19,VALUE_20,UNIT_SALE,ORD_PRD,DEL_DATE,INV_DATE,INV_NO,INV_PRD,ACCNT_CODE,ANAL_M0,ANAL_M1,ANAL_M2,ANAL_M3,ANAL_M4,ANAL_M5,ANAL_M6,ANAL_M7,ANAL_M8,
                            ANAL_M9,ASSEMBLY_IND,SPLIT_VAL,CREDIT_STATUS,PRICE_BOOK,SALE_QTY_VALUE,STK_QTY_VALUE,TOT_VALUE,DISP_VAL_1,DISP_VAL_2,FIXED_VAL,LINE_REF,USER_CODE,USER_INVOICED,ALLOC_REF,UPDATE_STOCK,FIXED_VAL_2,FIXED_VAL_3
                            FROM {model.DbCode}SISODET WHERE TRANS_REF = @TransactionCode AND ITEM_CODE = @ItemCode AND CREDIT_STATUS = '' AND TRANS_LINE = @OLD_LINE
                            IF @@ROWCOUNT > 0
                            UPDATE {model.DbCode}SISODET SET VALUE_1 = VALUE_1 - @QTY,VALUE_2 = VALUE_2 - @QTY,VALUE_4 = (VALUE_2 - @QTY)  * VALUE_3,VALUE_7 = (VALUE_2 - @QTY)  * VALUE_3,VALUE_13 = (VALUE_2 - @QTY)  * VALUE_3 WHERE TRANS_REF = @TransactionCode AND ITEM_CODE = @ItemCode AND TRANS_LINE = @OLD_LINE";
                        var insertDetailParam = new
                        {
                            TransactionCode = model.OldTransaction,
                            ItemCode = model.ItemCode,
                            OLD_LINE = transactionLine,
                            LINE = newText,
                            QTY = model.Quantity
                        };
                        affectedRow += await connection.ExecuteAsync(insertDetailQuery,
                            insertDetailParam, transaction
                        );
                    }
                    else
                    {
                        var newText = maxTranLine.Insert(maxTranLine.Length, 1.ToString("D2"));
                        var insertDetailQuery = $@"INSERT INTO {model.DbCode}SISODET
                            SELECT REC_TYPE,DETAIL_ID,TRANS_TYPE,TRANS_REF,@LINE,TRANS_CD,LOCATION,ITEM_CODE,DESCRIPTN,DUE_DATE,STATUS,@QTY,@QTY,VALUE_3,@QTY * VALUE_3,VALUE_5,VALUE_6,@QTY * VALUE_3,VALUE_8,VALUE_9,VALUE_10,
                            VALUE_11,VALUE_12,@QTY * VALUE_3,VALUE_14,VALUE_15,VALUE_16,VALUE_17,VALUE_18,VALUE_19,VALUE_20,UNIT_SALE,ORD_PRD,DEL_DATE,INV_DATE,INV_NO,INV_PRD,ACCNT_CODE,ANAL_M0,ANAL_M1,ANAL_M2,ANAL_M3,ANAL_M4,ANAL_M5,ANAL_M6,ANAL_M7,ANAL_M8,
                            ANAL_M9,ASSEMBLY_IND,SPLIT_VAL,CREDIT_STATUS,PRICE_BOOK,SALE_QTY_VALUE,STK_QTY_VALUE,TOT_VALUE,DISP_VAL_1,DISP_VAL_2,FIXED_VAL,LINE_REF,USER_CODE,USER_INVOICED,ALLOC_REF,UPDATE_STOCK,FIXED_VAL_2,FIXED_VAL_3
                            FROM {model.DbCode}SISODET WHERE TRANS_REF = @TransactionCode AND ITEM_CODE = @ItemCode AND CREDIT_STATUS = '' AND TRANS_LINE = @OLD_LINE
                            IF @@ROWCOUNT > 0
                            UPDATE {model.DbCode}SISODET SET VALUE_1 = VALUE_1 - @QTY,VALUE_2 = VALUE_2 - @QTY,VALUE_4 = (VALUE_2 - @QTY)  * VALUE_3,VALUE_7 = (VALUE_2 - @QTY)  * VALUE_3,VALUE_13 = (VALUE_2 - @QTY)  * VALUE_3 WHERE TRANS_REF = @TransactionCode AND ITEM_CODE = @ItemCode AND TRANS_LINE = @OLD_LINE";
                        var insertDetailParam = new
                        {
                            TransactionCode = model.OldTransaction,
                            ItemCode = model.ItemCode,
                            OLD_LINE = transactionLine,
                            LINE = newText,
                            QTY = model.Quantity
                        };
                        affectedRow += await connection.ExecuteAsync(insertDetailQuery,
                            insertDetailParam, transaction
                        );
                    }
                    var insertHeaderQuery =
                        $@"INSERT INTO {model.DbCode}SISOHDR SELECT 'C', @NEW_TRANS_REF, HEADER_ID, CUST_CODE, DELIV_ADD, @CREDIT_DATE, '80', 'C', TRANS_CODE, @TRANS_REF, CONVERT(NVARCHAR,TRANS_DATE,101), PRN_DATE, DEL_DATE, @INV_DATE, @CREDIT_PRD, @CUST_REF, @DEL_REF, @COM, @TRANS_VAL, PAY_DATE, ANAL_M0, ANAL_M1, ANAL_M2, ANAL_M3, ANAL_M4, ANAL_M5, ANAL_M6, ANAL_M7, ANAL_M8, ANAL_M9, QUOTE_CONVERT, QUOTE_PRINT, QUOTE_EXPIRY, QUOTED_PRD, QUOTATION_REF, DATE_QUOTED, VOID_STATUS, USER_CODE FROM {model.DbCode}SISOHDR WHERE REC_TYPE='I' AND TRANS_REF=@TRANS_REF AND VOID_STATUS='N'";
                    var insertHeaderParam = new
                    {
                        TRANS_REF = model.OldTransaction,
                        CREDIT_DATE = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        INV_DATE = DateTime.Now.ToString("MM/dd/yyyy"),
                        NEW_TRANS_REF = model.NewTransaction,
                        CREDIT_PRD = model.Period,
                        CUST_REF = model.CustomerCode,
                        DEL_REF = "",
                        COM = "",
                        USER_INVOICED = model.UserName,
                        TRANS_VAL = model.Total
                    };
                    affectedRow = await connection.ExecuteAsync(insertHeaderQuery, insertHeaderParam, transaction);
                    var sqlInvoiceDetails =
                        @$"DECLARE @REC_TYPE CHAR(1), @DETAIL_ID CHAR(1), @TRANS_TYPE NVARCHAR(10), @TRANS_CD CHAR(1), @LOCATION NVARCHAR(15), @ITEM_CODE NVARCHAR(15), @DESCRIPTN NVARCHAR(50), @DUE_DATE CHAR(10), @STATUS CHAR(2), @VALUE_1 NUMERIC(18,5), @VALUE_2 NUMERIC(18,5), @VALUE_3 NUMERIC(18,5), @VALUE_4 NUMERIC(18,5), @VALUE_5 NUMERIC(18,5), @VALUE_6 NUMERIC(18,5), @VALUE_7 NUMERIC(18,5), @VALUE_8 NUMERIC(18,5), @VALUE_9 NUMERIC(18,5), @VALUE_10 NUMERIC(18,5), @VALUE_11 NUMERIC(18,5), @VALUE_12 NUMERIC(18,5), @VALUE_13 NUMERIC(18,5), @VALUE_14 NUMERIC(18,5), @VALUE_15 NUMERIC(18,5), @VALUE_16 NUMERIC(18,5), @VALUE_17 NUMERIC(18,5), @VALUE_18 NUMERIC(18,5), @VALUE_19 NUMERIC(18,5), @VALUE_20 NUMERIC(18,5), @UNIT_SALE NVARCHAR(5), @ORD_PRD INT, @DEL_DATE CHAR(10), @INV_DATE CHAR(10), @INV_NO CHAR(15), @INV_PRD INT, @ACCNT_CODE NVARCHAR(15), @ANAL_M0 NVARCHAR(15), @ANAL_M1 NVARCHAR(15), @ANAL_M2 NVARCHAR(15), @ANAL_M3 NVARCHAR(15), @ANAL_M4 NVARCHAR(15), @ANAL_M5 NVARCHAR(15), @ANAL_M6 NVARCHAR(15), @ANAL_M7 NVARCHAR(15), @ANAL_M8 NVARCHAR(15), @ANAL_M9 NVARCHAR(15), @ASSEMBLY_IND CHAR(1), @SPLIT_VAL INT, @CREDIT_STATUS CHAR(1), @PRICE_BOOK NVARCHAR(10), @SALE_QTY_VALUE INT, @STK_QTY_VALUE INT, @TOT_VALUE INT, @DISP_VAL_1 INT, @DISP_VAL_2 INT, @FIXED_VAL INT, @LINE_REF NVARCHAR(20), @USER_CODE NVARCHAR(15), @ALLOC_REF CHAR(20), @UPDATE_STOCK_STATUS CHAR(1),@FIXED_VAL_2 INT,@FIXED_VAL_3 INT
                        SELECT @REC_TYPE='C',@DETAIL_ID='D', @TRANS_TYPE=TRANS_TYPE, @TRANS_CD='C', @LOCATION=LOCATION, @ITEM_CODE=ITEM_CODE, @DESCRIPTN=DESCRIPTN, @DUE_DATE=DUE_DATE, @STATUS='80', @VALUE_1= VALUE_1, @VALUE_2=VALUE_2, @VALUE_3= VALUE_3, @VALUE_4= VALUE_4 , @VALUE_5= VALUE_5 , @VALUE_6= VALUE_6 , @VALUE_7= VALUE_4 , @VALUE_8= VALUE_8 ,@VALUE_9= VALUE_9 ,@VALUE_10= VALUE_10 ,@VALUE_11= VALUE_11 ,@VALUE_12= VALUE_12 ,@VALUE_13= VALUE_4 ,@VALUE_14= VALUE_14 ,@VALUE_15= VALUE_15 ,@VALUE_16= VALUE_16 ,@VALUE_17= VALUE_17 ,@VALUE_18= VALUE_18 ,@VALUE_19= VALUE_19 ,@VALUE_20= VALUE_20 ,@UNIT_SALE=UNIT_SALE, @ORD_PRD=@CREDIT_PRD, @DEL_DATE=DEL_DATE, @INV_DATE=INV_DATE, @INV_NO=INV_NO, @INV_PRD=INV_PRD, @ACCNT_CODE=ACCNT_CODE, @ANAL_M0=ANAL_M0, @ANAL_M1=ANAL_M1, @ANAL_M2=ANAL_M2, @ANAL_M3=ANAL_M3, @ANAL_M4=ANAL_M4, @ANAL_M5=ANAL_M5, @ANAL_M6=ANAL_M6, @ANAL_M7=ANAL_M7, @ANAL_M8=ANAL_M8, @ANAL_M9=ANAL_M9, @ASSEMBLY_IND=ASSEMBLY_IND, @SPLIT_VAL=SPLIT_VAL, @CREDIT_STATUS='0', @PRICE_BOOK=PRICE_BOOK, @SALE_QTY_VALUE=SALE_QTY_VALUE, @STK_QTY_VALUE=STK_QTY_VALUE, @TOT_VALUE=TOT_VALUE, @DISP_VAL_1=DISP_VAL_1, @DISP_VAL_2=DISP_VAL_2, @FIXED_VAL=FIXED_VAL, @LINE_REF=LINE_REF, @USER_CODE=USER_CODE, @ALLOC_REF=ALLOC_REF, @UPDATE_STOCK_STATUS=UPDATE_STOCK , @FIXED_VAL_2=FIXED_VAL_2,@FIXED_VAL_3=FIXED_VAL_3 FROM {model.DbCode}SISODET WHERE REC_TYPE='D' AND INV_NO=@TRANS_REF AND TRANS_LINE=@TRANS_LINE
                        INSERT INTO {model.DbCode}SISODET VALUES(@REC_TYPE, @DETAIL_ID, @TRANS_TYPE, @NEW_TRANS_REF, @TRANS_LINE, @TRANS_CD, @LOCATION , @ITEM_CODE, @DESCRIPTN, @DUE_DATE, @STATUS, @VALUE_1, @VALUE_2, @VALUE_3, @VALUE_4, @VALUE_5, @VALUE_6, @VALUE_7, @VALUE_8, @VALUE_9, @VALUE_10, @VALUE_11, @VALUE_12, @VALUE_13, @VALUE_14, @VALUE_15, @VALUE_16, @VALUE_17, @VALUE_18, @VALUE_19, @VALUE_20, @UNIT_SALE, @ORD_PRD, @DEL_DATE, @INV_DATE, @INV_NO, @INV_PRD, @ACCNT_CODE, @ANAL_M0, @ANAL_M1, @ANAL_M2, @ANAL_M3, @ANAL_M4, @ANAL_M5, @ANAL_M6, @ANAL_M7, @ANAL_M8, @ANAL_M9, @ASSEMBLY_IND, @SPLIT_VAL , @CREDIT_STATUS, @PRICE_BOOK, @SALE_QTY_VALUE, @STK_QTY_VALUE, @TOT_VALUE, @DISP_VAL_1, @DISP_VAL_2, @FIXED_VAL, @LINE_REF, @USER_CODE, @USER_INVOICED, @ALLOC_REF, @UPDATE_STOCK_STATUS,@FIXED_VAL_2,@FIXED_VAL_3)
                        --SET OLD INVOICE LINE TO CREDITED
                        UPDATE {model.DbCode}SISODET SET CREDIT_STATUS='Y' WHERE REC_TYPE='D' AND INV_NO=@TRANS_REF AND TRANS_LINE=@TRANS_LINE AND CREDIT_STATUS=''";
                    var invocieDetailParam = new
                    {
                        TRANS_REF = creditInvoiceDetail.OldTransaction,
                        TRANS_LINE = creditInvoiceDetail.OldLine,
                        CREDIT_DATE = creditInvoiceDetail.CreditDate.ToString("yyyy-MM-dd HH:mm:ss"),
                        NEW_TRANS_REF = creditInvoiceDetail.NewTransaction,
                        CREDIT_PRD = creditInvoiceDetail.CreditPeriod,
                        USER_INVOICED = creditInvoiceDetail.UserCreated,
                    };
                    var rowAffected  = affectedRow += await connection.ExecuteAsync(sqlInvoiceDetails,
                        invocieDetailParam
                        , transaction);
                    if (rowAffected <= 0)
                    {
                        transaction.Rollback();
                        return 0;
                    }
                    affectedRow += await connection.ExecuteAsync(
                        "UPDATE TB_BC_CHANGEINVOICE_RECEIVED SET STATUS = @STATUS WHERE ID = @ID",
                        new { ID = model.ReceivedId, STATUS = 1 }, transaction);
                    affectedRow += await connection.ExecuteAsync(
                        "INSERT INTO TB_BC_CHANGEINVOICE_CREDIT_NOTE(CHANGEINVOICE_RECEIVED_ID,DB_CODE,OLD_TRANSACTION,NEW_TRANSACTION,ITEM_CODE,CREATED_DATE,CREATED_BY)" +
                        " VALUES (@RECEIVED_ID,@DB_CODE,@OLD_TRANSACTION,@NEW_TRANSACTION,@ITEM_CODE,@CREATED_DATE,@CREATED_BY)",
                        new
                        {
                            RECEIVED_ID = model.ReceivedId,
                            DB_CODE = model.DbCode,
                            OLD_TRANSACTION = model.OldTransaction,
                            NEW_TRANSACTION = model.NewTransaction,
                            ITEM_CODE = model.ItemCode,
                            CREATED_DATE = DateTime.Today,
                            CREATED_BY = model.UserName
                        }, transaction);
                    affectedRow += await connection.ExecuteAsync(
                        "UPDATE TB_BC_CHANGEINVOICE_DETAIL SET IS_RECEIVED = @STATUS WHERE ID = (SELECT CHANGE_INVOICE_DETAIL_ID FROM TB_BC_CHANGEINVOICE_RECEIVED WHERE ID = @ID)",
                        new
                        {
                            STATUS = Enum.GetName(typeof(ExchangeStatus), ExchangeStatus.CreditNote),
                            ID = model.ReceivedId
                        },
                        transaction);
                }
                if (affectedRow > 6)
                {
                    transaction.Commit();
                    return 1;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                transaction.Rollback();
            }
            return 0;
        }

        public Task<int> SaveRecordItemExchanged(string transaction, string itemCode, int quantity, double unitPrice)
        {


            throw new NotImplementedException();
        }

        public Task<bool> CheckIfInvoiceAlreadyPostInvoiceAsync(int masterId)
        {
            throw new NotImplementedException();
        }

        public async Task<string> CreateInvoice(string dbCode,string userName,int requestId, string customerCode, string transactionCode, double subTotal, List<ItemForExchaneDto> items,
            List<int> receivedId)
        {
            var sql =
            @"INSERT INTO TB_BC_CHANGEINVOICE_EXCHANGE_INVOICE(DB_CODE,REQUEST_EXCHANGE_ID,CUSTOMER_CODE,INVOICE_DATE,INVOICE_BY,TRANSACTION_CODE,SUB_TOTAL)
                OUTPUT inserted.ID
                VALUES(@DB_CODE,@REQUEST_ID,@CUSTOMER_CODE,@INVOICE_DATE,@INVOICE_BY,@TRANSACTION_CODE,@SUB_TOTAL)";
            var param = new
            {
                DB_CODE = dbCode,
                CUSTOMER_CODE = customerCode,
                INVOICE_DATE = DateTime.Now,
                INVOICE_BY = userName,
                TRANSACTION_CODE = transactionCode,
                REQUEST_ID = requestId,
                SUB_TOTAL = subTotal
            };
            using var connection = new SqlConnection(_configuration.GetConnectionString("DBConnection"));
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            using var transaction = connection.BeginTransaction();
            var rowAffected = await connection.QueryFirstOrDefaultAsync<int>(sql, param, transaction);
            if (rowAffected <= 0) return string.Empty;
            sql =
                @"INSERT INTO TB_BC_CHANGEINVOICE_EXCHANGE_ITEM(INVOICE_ID,ITEM_CODE,ITEM_DESCRIPTION,QUANTITY,UNIT_PRICE)
                         VALUES(@INVOICE_ID,@ITEM_CODE,@ITEM_DESCRIPTION,@QUANTITY,@UNIT_PRICE)";
            foreach (var paramDetail in items.Select(itemDto => new
            {
                INVOICE_ID = rowAffected,
                ITEM_CODE = itemDto.ItemCode,
                ITEM_DESCRIPTION = itemDto.ItemName,
                QUANTITY = itemDto.Quantity,
                UNIT_PRICE = itemDto.UnitPrice,
                RECEIVED_ID = itemDto.ReceivedId
            }))
                await connection.ExecuteAsync(sql, paramDetail, transaction);

            foreach (var t in receivedId)
            {
                sql = @"UPDATE TB_BC_CHANGEINVOICE_DETAIL SET IS_RECEIVED = 'Completed'
            FROM TB_BC_CHANGEINVOICE_DETAIL D INNER JOIN TB_BC_CHANGEINVOICE_RECEIVED R ON R.CHANGE_INVOICE_DETAIL_ID = D.ID
            WHERE R.ID = @RECEIVED_ID";
                await connection.ExecuteAsync(sql, new { RECEIVED_ID = t }, transaction);
                sql = @"UPDATE TB_BC_CHANGEINVOICE_RECEIVED SET STATUS = 1 WHERE ID = @ID";
                await connection.ExecuteAsync(sql, new { ID = t }, transaction);
            }

            var invoiceId = await connection.QueryFirstOrDefaultAsync<string>(
                @"SELECT INVOICE_NUMBER FROM TB_BC_CHANGEINVOICE_EXCHANGE_INVOICE WHERE ID = @ID",
                new { ID = rowAffected }, transaction);
            transaction.Commit();
            return invoiceId;
        }
    }
}
