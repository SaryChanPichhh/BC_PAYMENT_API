using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.CommondityExchange.RepairItem
{
    public class RepairGoodsRepository : IRepairGoodsRepository
    {
        private readonly ISqlDataAccess _sqlDataAccess;
        private readonly IDbConnection _dbConnection;
        public RepairGoodsRepository(ISqlDataAccess sqlDataAccess, IDbConnection dbConnection)
        {
            _sqlDataAccess = sqlDataAccess;
            _dbConnection = dbConnection;
        }
        public async Task<List<RepairGoodsRespondDto>> GetReceivedRepairGoodsByBranchAsync(string dbCode)
        {
            const string sql =
                @"SELECT TBCR.ID Id,TBCR.CHANGE_INVOICE_DETAIL_ID DetailId,Store, TBC.CUST_CODE CustomerCode, CUS.ADD_LINE_1 CustomerName, MARKET_KHMER_NAME Market, AREA_NAME_KHMER Area, ITEM.ITEM_CODE ItemCode, ITEM_NAME ItemName, TBCR.QUANTITY Quantity, DESCRIPION Description,
                N'មិនទាន់បានផ្ញើរទៅជាង'
				              StatusRepair, Convert(date,TBC.CREATED_DATE) [Date],
                S.LAST_NAME + ' ' + S.FIRST_NAME Seller
            FROM TB_BC_CHANGEINVOICE_DETAIL TBCD
                INNER JOIN TB_BC_CHANGEINVOICE_RECEIVED TBCR ON TBCR.CHANGE_INVOICE_DETAIL_ID = TBCD.ID
                INNER JOIN (
                            SELECT ITEM_CODE, CASE WHEN ITEM_CUS10_KH = '' THEN ITEM_DESC ELSE ITEM_CUS10_KH END 'ITEM_NAME'
                FROM SIITEMS
                WHERE DB_CODE = @DB_CODE) ITEM ON ITEM.ITEM_CODE = TBCD.ITEM_CODE
                LEFT JOIN TB_BC_CHANGEINVOICE_REPAIR TBCRP ON TBCRP.RECEIVED_ID = TBCR.ID
                INNER JOIN TB_BC_CHANGEINVOICE TBC ON TBC.ID = TBCD.CHANGE_INVOICE_ID
                INNER JOIN dbo.BCUSERS S ON S.USER_ID = TBC.USER_CODE
                LEFT JOIN (SELECT ADD_CODE,STORE Store, AREA_NAME, MARKET_NAME, ADD_LINE_1, MARKET_KHMER_NAME, AREA_NAME_KHMER
                FROM SIADD CUS INNER JOIN TB_BCMARKET M ON M.MARKET_ID = CUS.MARKET_ID INNER JOIN TB_AREAS R ON R.AREA_ID = CUS.AREA_ID
                WHERE CUS.DB_CODE = @DB_CODE AND M.DB_CODE = @DB_CODE AND R.DB_CODE = @DB_CODE) CUS ON CUS.ADD_CODE = CUST_CODE
            WHERE TBCD.TYPE = 'REPAIR' AND TBCR.[STATUS] = 0";
            var param = new
            {
                DB_CODE = dbCode,
            };
            var execute = await _sqlDataAccess.LoadData<RepairGoodsRespondDto, dynamic>(sql, param);
            return execute.ToList();
        }
        public async Task<int> DeleteReceivedRepairGoodAsync(int receivedId, int detailId)
        {
            const string sql = @"DELETE FROM TB_BC_CHANGEINVOICE_RECEIVED WHERE ID = @ID
			IF @@ROWCOUNT >0
			UPDATE TB_BC_CHANGEINVOICE_DETAIL SET IS_RECEIVED = 'Pending' WHERE ID = @RequestDetailId";
            var param = new
            {
                ID = receivedId,
                RequestDetailId = detailId
            };
            var affectedRow = await _sqlDataAccess.ExecuteAsync(sql, param);
            return affectedRow;
        }
        public async Task<string> GenerateTransactionCode(string dbCode)
        {
            const string sql = @"SELECT MAX(SUBSTRING(TRAN_REF,13,4)) FROM TB_BC_CHANGEINVOICE_REPAIR WHERE 
				SUBSTRING(TRAN_TYPE,1,3) = SUBSTRING(TRAN_TYPE,5,3) AND DB_CODE = @DB_CODE AND MONTH(CREATED_DATE) = MONTH(GETDATE())";
            var param = new
            {
                DB_CODE = dbCode
            };
            var results = await _sqlDataAccess.LoadSingleData<string, dynamic>(sql, param);
            var month = await _sqlDataAccess.LoadSingleData<int,dynamic>(@"SELECT MONTH(GETDATE())", new {});
            if (string.IsNullOrEmpty(results))
            {
                return
                    $"{dbCode}-{dbCode}-{DateTime.Today.Year.ToString().Substring(2, 2)}{month:D2}{0:D4}";
            }
            var maxId = Convert.ToInt32(results);
            maxId++;
            return
                $"{dbCode}-{dbCode}-{DateTime.Today.Year.ToString().Substring(2, 2)}{month:D2}{maxId:D4}";
        }
        public async Task<int> TransferRepairGoodsAsync(RepairGoodsRespondDto repairGoodsDto)
        {
            if(_dbConnection.State == ConnectionState.Closed)
                _dbConnection.Open(); 
            var transaction = _dbConnection.BeginTransaction();
            var transactionCode = await GenerateTransactionCode(repairGoodsDto.DbCode);
            
            try
            {
                const string insertSql =
                    @"INSERT INTO TB_BC_CHANGEINVOICE_REPAIR(DB_CODE,RECEIVED_ID,CUSTOMER_CODE,TRAN_TYPE,TRAN_REF,ITEM_CODE,QUANTITY,DESCRIPTION,IS_RECEIVED,CREATED_DATE,CREATED_BY)
				VALUES(@DB_CODE,@ID,@CUSTOMER_CODE,@TRAN_TYPE,@TRAN_REF,@ITEM_CODE,@QUANTITY,@DESCRIPTION,@IS_RECEIVED,@CREATED_DATE,@CREATED_BY)";
                var param = new
                {
                    DB_CODE = repairGoodsDto.DbCode,
                    CUSTOMER_CODE = repairGoodsDto.CustomerCode,
                    TRAN_TYPE = $"{repairGoodsDto.DbCode}-{repairGoodsDto.DbCode}",
                    TRAN_REF = transactionCode,
                    ITEM_CODE = repairGoodsDto.ItemCode,
                    QUANTITY = repairGoodsDto.Quantity,
                    DESCRIPTION = repairGoodsDto.Description,
                    IS_RECEIVED = "Pending",
                    CREATED_DATE = DateTime.Today,
                    CREATED_BY = repairGoodsDto.CreateBy,
                    ID = repairGoodsDto.Id
                };
                var affectedRow = await _sqlDataAccess.ExecuteAsync(insertSql, param);
                if (affectedRow > 0)
                {
                    const string updateReceivedSql = $@"UPDATE TB_BC_CHANGEINVOICE_RECEIVED SET STATUS = 1 WHERE ID = @ID";
                    var receivedParam = new
                    {
                        ID = repairGoodsDto.Id
                    };
                    var updateAffected = await _dbConnection.ExecuteAsync(updateReceivedSql, receivedParam, transaction);
                    if (updateAffected > 0)
                    {
                        transaction.Commit();
                    }
                    else
                    {
                        transaction.Rollback();
                        return 0;
                    }
                }
                else
                {
                    transaction.Rollback();
                    return 0;
                }
                return affectedRow;
            }
            catch(Exception ex)
            {
                transaction.Rollback();
                Debug.WriteLine(ex.Message);
            }
            return 0;
        }
        #region Repairing Goods
        public async Task<List<ItemRepairInprogressDto>> GetReparingGoodsByDateAsync(string dbCode, DateTime fromDate, DateTime toDate)
        {
            const string sql = @"SELECT R.CREATED_DATE Date,R.ID Id,CUS.*,R.ITEM_CODE ItemCode,R.QUANTITY Quantity,CREATED_BY CreatedBy,R.DESCRIPTION [Description],R.IS_RECEIVED [Status],R.TRAN_REF [TransactionCode],R.TRAN_TYPE 
                [TransactionType]
                FROM TB_BC_CHANGEINVOICE_REPAIR R
                LEFT JOIN (SELECT S.ADD_CODE CustomerCode,S.ADD_LINE_1KH CustomerName,A.AREA_NAME_KHMER Area,STORE Store,M.MARKET_KHMER_NAME Market
                FROM SIADD S INNER JOIN TB_BCMARKET M ON M.MARKET_ID = S.MARKET_ID INNER JOIN TB_AREAS A ON A.AREA_ID = S.AREA_ID) CUS 
                ON CUS.CustomerCode = R.CUSTOMER_CODE
                WHERE R.CREATED_DATE BETWEEN @FROM_DATE AND @TO_DATE AND R.DB_CODE = @DB_CODE";
            var param = new
            {
                DB_CODE = dbCode,
                FROM_DATE = fromDate,
                TO_DATE = toDate,
            };
            var execute = await _sqlDataAccess.LoadData<ItemRepairInprogressDto, dynamic>(sql, param);
            return execute.ToList();
        }

        public async Task<int> DeleteRepairItem(int receivedId, int repairId)
        {
            if(_dbConnection.State == ConnectionState.Closed)
                _dbConnection.Open();

            var transaction = _dbConnection.BeginTransaction();
            try
            {
                var deleteSql = @"DELETE FROM TB_BC_CHANGEINVOICE_REPAIR WHERE ID = @ID";
                var deleteParam = new
                {
                    ID = repairId
                };
                var affectedRow = await _dbConnection.ExecuteAsync(deleteSql, deleteParam, transaction);
                if (affectedRow > 0)
                {
                    const string sql = @"UPDATE TB_BC_CHANGEINVOICE_RECEIVED SET STATUS = 0 WHERE ID = @RECEIVED_ID";
                    var param = new
                    {
                        RECEIVED_ID = receivedId,
                    };
                    var updateAffected = await _dbConnection.ExecuteAsync(sql, param, transaction);
                    if (updateAffected > 0)
                    {
                        transaction.Commit();
                    }
                    else
                    {
                        transaction.Rollback();
                        return 0;
                    }
                }
                else
                { 
                    transaction.Rollback();
                    return 0;
                }
                return affectedRow;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Debug.WriteLine(ex.Message);
            }
            return 0;
        }

        public async Task<List<CustomerDto>> GetAllCustomerHasCompletedRepair(string dbCode)
        {
            const string sql =
                @" SELECT DISTINCT S.FIRST_NAME FirstName,HEADER.USER_CODE UserCode,S.LAST_NAME LastName,COMPLETED.CUSTOMER_CODE CustomerCode,CUS.CustomerName,CUS.Store,CUS.Area,CUS.Market 
            FROM TB_BC_CHANGEINVOICE HEADER INNER JOIN TB_BC_CHANGEINVOICE_DETAIL DETAIL ON DETAIL.CHANGE_INVOICE_ID = HEADER.ID
            INNER JOIN TB_BC_CHANGEINVOICE_RECEIVED RECEIVED ON RECEIVED.CHANGE_INVOICE_DETAIL_ID = DETAIL.ID
            INNER JOIN TB_BC_CHANGEINVOICE_REPAIR REPAIR ON REPAIR.RECEIVED_ID = RECEIVED.ID
            INNER JOIN TB_BC_CHANGEINVOICE_REPAIR_COMPLETED COMPLETED ON COMPLETED.TRAN_REF = REPAIR.TRAN_REF
            LEFT JOIN (SELECT S.ADD_CODE CustomerCode,S.ADD_LINE_1KH CustomerName,A.AREA_NAME_KHMER Area,STORE Store,M.MARKET_KHMER_NAME Market
                                    FROM SIADD S INNER JOIN TB_BCMARKET M ON M.MARKET_ID = S.MARKET_ID INNER JOIN TB_AREAS A ON A.AREA_ID = S.AREA_ID WHERE S.DB_CODE = @DB_CODE) CUS 
                                    ON CUS.CustomerCode = COMPLETED.CUSTOMER_CODE
            INNER JOIN dbo.BCUSERS S ON S.USER_ID = HEADER.USER_CODE    
            WHERE COMPLETED.STATUS <> 'Completed' AND COMPLETED.DB_CODE = @DB_CODE ";
            var results = (await _sqlDataAccess.LoadData<CustomerDto,dynamic>(sql, new { DB_CODE = dbCode })).ToList();
            return results;
        }

        public async Task<List<ItemRepairCompletedDto>> GetAllItemHasCompletedRepairByCustomerCode(string dbCode, string customerCode)
        {
            const string sql =
                @"SELECT DETAIL.ID RequestDetailId,COMPLETED.ID RepairCompletedId,CUS.Market,REPAIR.DESCRIPTION [Description],CUS.Area,CUS.Store,COMPLETED.ITEM_CODE ItemCode,COMPLETED.QUANTITY Quantity,
            COMPLETED.STATUS_REPAIR RepairStatus,COMPLETED.CREATED_DATE CreatedDate,TOTAL_PRICE Total, HEADER.ID RequestRepairId,
            COMPLETED.TRAN_REF [Transaction],COMPLETED.NOTE [Note] FROM TB_BC_CHANGEINVOICE HEADER INNER JOIN TB_BC_CHANGEINVOICE_DETAIL DETAIL ON DETAIL.CHANGE_INVOICE_ID = HEADER.ID
            INNER JOIN TB_BC_CHANGEINVOICE_RECEIVED RECEIVED ON RECEIVED.CHANGE_INVOICE_DETAIL_ID = DETAIL.ID
            INNER JOIN TB_BC_CHANGEINVOICE_REPAIR REPAIR ON REPAIR.RECEIVED_ID = RECEIVED.ID
            INNER JOIN TB_BC_CHANGEINVOICE_REPAIR_COMPLETED COMPLETED ON COMPLETED.TRAN_REF = REPAIR.TRAN_REF
            LEFT JOIN (SELECT S.ADD_CODE CustomerCode,S.ADD_LINE_1KH CustomerName,A.AREA_NAME_KHMER Area,STORE Store,M.MARKET_KHMER_NAME Market
                                    FROM SIADD S INNER JOIN TB_BCMARKET M ON M.MARKET_ID = S.MARKET_ID INNER JOIN TB_AREAS A ON A.AREA_ID = S.AREA_ID WHERE S.DB_CODE = @DB_CODE) CUS 
                                    ON CUS.CustomerCode = COMPLETED.CUSTOMER_CODE
            INNER JOIN dbo.BCUSERS S ON S.USER_ID = HEADER.USER_CODE
            WHERE COMPLETED.STATUS <> 'Completed' AND COMPLETED.DB_CODE = @DB_CODE AND REPAIR.CUSTOMER_CODE = @CUSTOMER_CODE";
            var param = new
            {
                DB_CODE = dbCode,
                CUSTOMER_CODE = customerCode
            };
            var results = await _sqlDataAccess.LoadData<ItemRepairCompletedDto,dynamic>(sql, param);
            return results.ToList();
        }

        public async Task<int> SwitchItemType(string dbCode,string userName,int id, string description, string fromType, string toType)
        {
            if(_dbConnection.State == ConnectionState.Closed)
                _dbConnection.Open();

            var transaction = _dbConnection.BeginTransaction();
            try
            {
                const string sql =
                    @"INSERT INTO TB_BC_CHANGEINVOICE_SWITCH_ITEM (DB_CODE,CHANGEINVOICE_DETAIL_ID,FROM_TYPE,TO_TYPE,DESCRIPTION,CREATED_BY,CREATED_DATE)
                VALUES (@DB_CODE,@CHANGE_DETAIL_ID,@FROM_TYPE,@TO_DATE,@DESCRIPTION,@CREATED_BY,@CREATED_DATE)";
                var param = new
                {
                    CHANGE_DETAIL_ID = id,
                    FROM_TYPE = fromType,
                    TO_DATE = toType,
                    DESCRIPTION = description,
                    CREATED_BY = userName,
                    DB_CODE = dbCode,
                    CREATED_DATE = DateTime.Now
                };
                var affectedRow = await _dbConnection.ExecuteAsync(sql, param, transaction);
                if (affectedRow > 0)
                {
                    var updateDetail = $@"UPDATE TB_BC_CHANGEINVOICE_DETAIL SET TYPE = @TYPE WHERE ID = @ID";
                    var updateDetailParam = new
                    {
                        TYPE = toType,
                        ID = id
                    };
                    var updateAffected = await _dbConnection.ExecuteAsync(updateDetail, updateDetailParam, transaction);
                    if (updateAffected > 0)
                    {
                        transaction.Commit();
                        return updateAffected;
                    }
                    else
                    {
                        transaction.Rollback();
                        return 0;
                    }
                }
                else
                {
                    transaction.Rollback();
                    return 0;
                }
                return 1;
            }
            catch(Exception ex)
            {
                transaction.Rollback();
                Debug.WriteLine(ex.Message);
            }
            return 0;
        }

        public Task<int> IssuanceRepairGoodCompletedInvoiceAsync(List<IssuanceInvoiceDto> model,NewInvoiceCompletedDto newInvoiceCompleted)
        {

            throw new NotImplementedException();
        }


        public Task<int> SetRepairItemCompletedStatusToCompleted(int id, string transactionCode)
        {
            const string sql = @"UPDATE TB_BC_CHANGEINVOICE_REPAIR_COMPLETED SET STATUS = 'Completed' WHERE ID = @ID";
            var updateCompletedParam = new
            {
                ID = id
            };
            if (_dbConnection.State == ConnectionState.Closed)
                _dbConnection.Open();   
            throw new NotImplementedException();
        }

        public Task<int> PaidRepairItemAsync(string createBy, double totalPrice, int repairCompletedId)
        {
            const string sql = @"UPDATE TB_BC_CHANGEINVOICE_REPAIR_COMPLETED SET TOTAL_PRICE =
                @TotalPrice,UPDATED_BY = @UpdateBy,UPDATED_DATE = @UpdateDate,STATUS = 'Paid'
                WHERE ID = @ID";
            var param = new
            {
                TotalPrice = totalPrice,
                UpdateBy = createBy,
                UpdateDate = DateTime.Today,
                ID = repairCompletedId
            };
            var affectedRow = _sqlDataAccess.ExecuteAsync(sql, param);
            return affectedRow;
        }

        #endregion

    }
}
