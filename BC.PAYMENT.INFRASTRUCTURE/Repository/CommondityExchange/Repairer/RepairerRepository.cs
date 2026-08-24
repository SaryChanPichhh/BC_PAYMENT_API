namespace BC.PAYMENT.INFRASTRUCTURE.Repository.CommondityExchange.Repairer
{
    public class RepairerRepository : IRepairerRepository , ICompletedRepairRepository
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public RepairerRepository(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<List<ItemRepairReceivedModel>> LoadItemRepairedToReparation(string dbCode)
        {
            var sql = $@"SELECT REPAIR.ID Id,REPAIR.TRAN_REF [TransactionCode],REPAIR.CUSTOMER_CODE CustomerCode,CUS.CustomerName,CUS.Area,CUS.Store,
            CUS.Market,REPAIR.ITEM_CODE ItemCode,REPAIR.QUANTITY Quantity,REPAIR.DESCRIPTION Description,
            IS_RECEIVED [Status],REPAIR.CREATED_DATE CreatedDate FROM TB_BC_CHANGEINVOICE_REPAIR REPAIR  
            LEFT JOIN (SELECT S.ADD_CODE CustomerCode,S.ADD_LINE_1KH CustomerName,A.AREA_NAME_KHMER Area,STORE Store,M.MARKET_KHMER_NAME Market
                        FROM SIADD S INNER JOIN TB_BCMARKET M ON M.MARKET_ID = S.MARKET_ID INNER JOIN TB_AREAS A ON A.AREA_ID = S.AREA_ID) CUS 
                        ON CUS.CustomerCode = REPAIR.CUSTOMER_CODE
            WHERE REPAIR.DB_CODE = @DB_CODE AND IS_RECEIVED IN('Pending','Yes') AND TRAN_REF IS NOT NULL";
            var param = new
            {
                DB_CODE = dbCode,
            };
            var execute = await _sqlDataAccess.LoadData<ItemRepairReceivedModel, dynamic>(sql,param);
            return execute.ToList();
        }

        public async Task<int> UpdateAfterRepairerReceivedItemsAsync(string dbCode, string userName, string transactionCode)
        {
            const string sql =
                @"UPDATE TB_BC_CHANGEINVOICE_REPAIR SET IS_RECEIVED = 'Yes',RECEIVED_DATE = @DATE,RECEIVED_BY = @RECEIVED_BY WHERE TRAN_REF = @TRAN_REF AND DB_CODE = @DB_CODE";
            var param = new
            {
                DB_CODE = dbCode,
                RECEIVED_BY = userName,
                TRAN_REF = transactionCode,
                DATE = DateTime.Today,
            };
            var affectedRow = await _sqlDataAccess.ExecuteAsync(sql, param);
            return affectedRow;
        }

        public async Task<List<CompletedRepairItemDto>> GetAllCompletedRepairItemsAsync(string dbCode)
        {
            const string sql = @"SELECT REPAIR.CUSTOMER_CODE CustomerCode,REPAIR.TRAN_REF [TransactionCode],REPAIR.ITEM_CODE ItemCode,DESCRIPTION Description,
				ISNULL((REPAIR.QUANTITY-SUM(COMPLETED.QUANTITY)),REPAIR.QUANTITY) Quantity,RECEIVED_DATE CreatedDate,RECEIVED_BY CreatedBy FROM TB_BC_CHANGEINVOICE_REPAIR REPAIR LEFT JOIN
				TB_BC_CHANGEINVOICE_REPAIR_COMPLETED COMPLETED ON REPAIR.TRAN_REF = COMPLETED.TRAN_REF 
				WHERE IS_RECEIVED='Yes' AND REPAIR.DB_CODE=@DB_CODE GROUP BY REPAIR.QUANTITY,REPAIR.CUSTOMER_CODE,REPAIR.TRAN_REF,REPAIR.ITEM_CODE,REPAIR.DESCRIPTION
				,REPAIR.RECEIVED_DATE,REPAIR.RECEIVED_BY HAVING ISNULL((REPAIR.QUANTITY-SUM(COMPLETED.QUANTITY)),REPAIR.QUANTITY)>0 ORDER BY REPAIR.TRAN_REF;";
            var param = new
            {
                DB_CODE = dbCode,
            };
            var execute = await _sqlDataAccess.LoadData<CompletedRepairItemDto,dynamic>(sql,param);
            return execute.ToList();
        }

        public async Task<int> UpdatedAfterRepairerReceivedAsyc(ItemRepairReceivedModel model)
        {
            var sql =
                @"INSERT INTO TB_BC_CHANGEINVOICE_REPAIR_COMPLETED(DB_CODE,CUSTOMER_CODE,ITEM_CODE,TRAN_REF,QUANTITY,STATUS_REPAIR,NOTE,CREATED_DATE,CREATED_BY,STATUS,REPAIR_TOOL_CODE,ITEM_STATUS,ITEM_DESCRIPTION)
            VALUES(@DB_CODE,@CUSTOMER_CODE,@ITEM_CODE,@TRAN_REF,@QUANTITY,@STATUS_REPAIR,@NOTE,@CREATED_DATE,@CREATED_BY,'Pending',@REPAIR_TOOL_CODE,@ITEM_STATUS,@ITEM_DESCRIPTION)
            IF @@ROWCOUNT > 0
	            UPDATE TB_BC_CHANGEINVOICE_REPAIR SET IS_RECEIVED = 'Completed' WHERE TRAN_REF = @TRAN_REF AND DB_CODE = @DB_CODE";
            var param = new
            {
                DB_CODE = model.DbCode,
                CUSTOMER_CODE = model.CustomerCode,
                TRAN_REF = model.TransactionCode,
                QUANTITY = model.Quantity,
                STATUS_REPAIR = model.RepairStatus,
                NOTE = model.Reason,
                CREATED_DATE = DateTime.Today,
                CREATED_BY = model.CreateBy,
                ITEM_CODE = model.ItemCode,
                REPAIR_TOOL_CODE =model.RepairToolCode,
                ITEM_STATUS = model.ItemStatus,
                ITEM_DESCRIPTION = model.Description,
            };
            var affectedRow = await _sqlDataAccess.ExecuteAsync(sql, param);
            return affectedRow;
        }

        public async Task<ItemBeingRepaired> GetItemByTransactionCode(string dbCode, string barCode)
        {
            var sql = $@"SELECT SUM(COMPLETED.QUANTITY) CompletedQuantity ,REPAIR.QUANTITY RepairQuantity,REPAIR.CUSTOMER_CODE CustomerCode,REPAIR.TRAN_REF Transaction,
	                    REPAIR.ITEM_CODE ItemCode,DESCRIPTION Description,RECEIVED_DATE CreatedDate, COMPLETED.ITEM_STATUS ItemStatus
	                    FROM TB_BC_CHANGEINVOICE_REPAIR REPAIR 
	                    LEFT JOIN TB_BC_CHANGEINVOICE_REPAIR_COMPLETED COMPLETED
	                    ON REPAIR.TRAN_REF = COMPLETED.TRAN_REF WHERE  IS_RECEIVED = 'Yes' AND REPAIR.DB_CODE = @DB_CODE AND REPAIR.TRAN_REF = @Transaction
	                    GROUP BY REPAIR.QUANTITY,REPAIR.CUSTOMER_CODE ,REPAIR.TRAN_REF ,
	                    REPAIR.ITEM_CODE ,DESCRIPTION ,RECEIVED_DATE, COMPLETED.ITEM_STATUS;";
            var param = new
            {
                DB_CODE = dbCode,
                Transaction = barCode,
            };
            var execute = await _sqlDataAccess.LoadSingleData<ItemBeingRepaired, dynamic>(sql,param);
            return execute;
        }

        public async Task<int> InsertCompletedRepairItemAsync(ItemRepairReceivedModel model)
        {
            var sql= @"INSERT INTO TB_BC_CHANGEINVOICE_REPAIR_COMPLETED(DB_CODE,CUSTOMER_CODE,ITEM_CODE,TRAN_REF,QUANTITY,STATUS_REPAIR,NOTE,CREATED_DATE,CREATED_BY,STATUS,REPAIR_TOOL_CODE,ITEM_STATUS,ITEM_DESCRIPTION)
            VALUES(@DB_CODE,@CUSTOMER_CODE,@ITEM_CODE,@TRAN_REF,@QUANTITY,@STATUS_REPAIR,@NOTE,@CREATED_DATE,@CREATED_BY,'Pending',@REPAIR_TOOL_CODE,@ITEM_STATUS,@ITEM_DESCRIPTION)
            IF @@ROWCOUNT > 0
	            UPDATE TB_BC_CHANGEINVOICE_REPAIR SET IS_RECEIVED = 'Completed' WHERE TRAN_REF = @TRAN_REF AND DB_CODE = @DB_CODE";
            var param = new
            {
                DB_CODE = model.DbCode,
                CUSTOMER_CODE = model.CustomerCode,
                ITEM_CODE = model.ItemCode,
                TRAN_REF = model.TransactionCode,
                QUANTITY = model.Quantity,
                STATUS_REPAIR = model.RepairStatus,
                CREATED_DATE = DateTime.Today,
                CREATED_BY = model.CreateBy,
                NOTE = model.Description,
                REPAIR_TOOL_CODE = model.RepairToolCode,
                ITEM_STATUS = model.ItemStatus,
                ITEM_DESCRIPTION = model.ItemDescription
            };
            var affectedRow = await _sqlDataAccess.ExecuteAsync(sql,param);
            return affectedRow; 
        }
    }
}
