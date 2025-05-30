using System.Data;
using System.Diagnostics;
using BC.PAYMENT.APPLICATION.Interfaces.CashFlow.CashFlowData;
using BC.PAYMENT.CORE.DTO.CashFlow.CashFlowData;
using BC.PAYMENT.CORE.Entities.CashFlow.CashFlowData;
using BC.PAYMENT.INFRASTRUCTURE.DBAccess;
using Dapper;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.CashFlow.CashFlowData
{
    public class CashFlowDataRepository : ICashFlowDataRepository
    {
        private readonly ISqlDataAccess _sqlDataAccess ;
        private readonly IDbConnection _dbConnection ;

        public CashFlowDataRepository(ISqlDataAccess sqlDataAccess, IDbConnection dbConnection)
        {
            _sqlDataAccess = sqlDataAccess;
            _dbConnection = dbConnection;
        }

        public async Task<List<CashFlowDataHeader>> GetCashFlowHeaderAsync(string dbCode)
        {
            var sql = $@"SELECT ID Id,CASH_FLOW_DATE [Date],LEFT (CONVERT (varchar, CASH_FLOW_DATE, 111), 7) [Month],
                        LEFT (CONVERT (varchar, CASH_FLOW_DATE, 111), 4) [Year],
                        STATUS [Status]  FROM BC_PAYMENT_MONEY_CASH_FLOW_HEADER WHERE DB_CODE = @DB_CODE";
            var param = new
            {
                DB_CODE = dbCode
            };
            var execute = await _sqlDataAccess.LoadData<CashFlowDataHeader, dynamic>(sql, param);
            return execute.ToList();
        }

        public async Task<int> AddNewCashFlowHeaderAsync(CashFlowDataModel model)
        {
            const string sql =
                @"
            IF NOT EXISTS(SELECT * FROM BC_PAYMENT_MONEY_CASH_FLOW_HEADER WHERE DB_CODE = @DB_CODE AND CASH_FLOW_DATE = @CASH_FLOW_DATE)
            INSERT INTO BC_PAYMENT_MONEY_CASH_FLOW_HEADER(DB_CODE,CASH_FLOW_DATE,STATUS,CREATED_BY,CREATED_DATE,ENTRIES_CODE)
            VALUES (@DB_CODE,@CASH_FLOW_DATE,@STATUS,@CREATED_BY,@CREATED_DATE,@ENTRIES_CODE)";
            var param = new
            {
                DB_CODE = model.DbCode,
                CASH_FLOW_DATE = model.Date,
                STATUS = "Pending",
                CREATED_DATE = DateTime.Today,
                CREATED_BY = model.CreateBy,
                ENTRIES_CODE = model.Period
            };
            var affectedRow = await _sqlDataAccess.ExecuteAsync(sql,param);
            return affectedRow;
        }

        public async Task<int> CancelCashFlowHeaderAsync(int id)
        {
            if( _dbConnection.State == ConnectionState.Closed)
                _dbConnection.Open();

            var transaction = _dbConnection.BeginTransaction();
            try
            {
                var sql = $@"UPDATE BC_PAYMENT_MONEY_CASH_FLOW_HEADER SET STATUS = @STATUS WHERE ID = @ID";
                var param = new
                {
                    STATUS = "Pending",
                    ID = id,
                };
                var affectedRow = await _dbConnection.ExecuteAsync(sql, param,transaction);
                if(affectedRow > 0)
                {
                    var updateCashFlowDetail = $@"UPDATE BC_PAYMENT_MONEY_CASH_FLOW SET STATUS = @STATUS WHERE HEADER_ID = @HEADER_ID";
                    var updateCashFlowDetailParam = new
                    {
                        STATUS = "Pending",
                        HEADER_ID = id,
                    };
                     await _dbConnection.ExecuteAsync(updateCashFlowDetail, updateCashFlowDetailParam, transaction);
                     
                    var updateCashFlowSubmitted = $@"UPDATE BC_PAYMENT_MONEY_CASH_FLOW_SUBMITTED SET STATUS = @STATUS WHERE CASH_FLOW_HEADER_ID = @HEADER_ID";
                    var updateCashFlowSubmittedParam = new
                    {
                        STATUS = "Cancel",
                        HEADER_ID = id,
                    };
                     await _dbConnection.ExecuteAsync(updateCashFlowSubmitted, updateCashFlowSubmittedParam, transaction);

                    transaction.Commit();
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

        public async Task<int> DeleteCashFlowHeaderAsync(int id)
        {
            var sql = $@"DELETE FROM BC_PAYMENT_MONEY_CASH_FLOW_HEADER WHERE ID=@ID;";
            var param = new
            {
                ID = id,
            };
            var affectedRow = await _sqlDataAccess.ExecuteAsync(sql, param);
            return affectedRow;
        }

        public async Task<int> UpdateCashFlowHeaderAsync(int id, string cashFlowDate)
        {
            var sql = $@"
                            IF NOT EXISTS(SELECT * FROM BC_PAYMENT_MONEY_CASH_FLOW_HEADER WHERE STATUS = 'Submitted' AND ID = @ID) 
                            UPDATE BC_PAYMENT_MONEY_CASH_FLOW_HEADER SET CASH_FLOW_DATE = @DATE WHERE ID = @ID";
            var param = new
            {
                DATE = cashFlowDate,
                ID = id,
            };
            var affectedRow = await _sqlDataAccess.ExecuteAsync(sql, param);
            return affectedRow;
        }

        public async Task<List<CashFlowDataDetailModel>> GetPaymentCashFlowDetailByIdAsync(string dbCode, int id)
        {
            const string sql =
                @"SELECT ID Id,P.DATE Date,P.NAME Name,P.AMOUNT Amount,P.CURRENCY_FORMAT CurrencyFormat,P.EXCHANGE_RATE ExchangeRate,
                P.CREATED_DATE CreatedDate,P.CREATED_BY CreatedBy,STATUS Status
                FROM BC_PAYMENT_MONEY_CASH_FLOW P WHERE P.DB_CODE = @DB_CODE AND HEADER_ID = @HEADER_ID";
            var param = new
            {
                DB_CODE = dbCode,
                HEADER_ID = id
            };
            var execute = await _sqlDataAccess.LoadData<CashFlowDataDetailModel, dynamic>(sql, param);
            return execute.ToList();
        }

        public async Task<int> UpdateCashFlowDetailAsync(CashFlowDataDetailModel model)
        {
            const string sql =
                @"UPDATE BC_PAYMENT_MONEY_CASH_FLOW 
                SET DATE = @DATE,NAME= @NAME,AMOUNT = @AMOUNT,CURRENCY_FORMAT = @FORMAT,
                EXCHANGE_RATE = @EXCHANGE,UPDATED_BY = @UPDATED_BY,UPDATED_DATE = GETDATE() 
                WHERE ID = @ID";
            var param = new
            {
                DATE = model.Date,
                NAME = model.Name,
                AMOUNT = model.Amount,
                FORMAT = model.CurrencyFormat,
                EXCHANGE = model.ExchangeRate,
                UPDATED_BY = model.CreatedBy,
                ID = model.Id
            };
            var affectedRow = await _sqlDataAccess.ExecuteAsync(sql, param);
            return affectedRow;
        }

        public async Task<int> AddNewCashFlowDetailAsync(CashFlowDataDetailModel model)
        {
            const string sql =
                @"INSERT INTO BC_PAYMENT_MONEY_CASH_FLOW(DB_CODE,HEADER_ID,DATE,NAME,AMOUNT,CURRENCY_FORMAT,EXCHANGE_RATE,CREATED_DATE,STATUS,CREATED_BY)
            VALUES (@DB_CODE,@HEADER_ID,@DATE,@NAME,@AMOUNT,@FORMAT,@EXCHANGE,@CREATED_DATE,@STATUS,@CREATED_BY)";
            var param = new
            {
                DB_CODE = model.DbCode,
                HEADER_ID = model.HeaderId,
                DATE = model.Date,
                NAME = model.Name,
                AMOUNT = model.Amount,
                FORMAT = model.CurrencyFormat,
                EXCHANGE = model.ExchangeRate,
                STATUS = "Pending",
                CREATED_DATE = DateTime.Today,
                CREATED_BY = model.CreatedBy
            };
            var affectedRow = await _sqlDataAccess.ExecuteAsync(sql, param);
            return affectedRow;
        }

        public async Task<int> AddNewCashFlowSubmittedAsync(List<CashFlowDataDetailModel> model)
        {
            var affectedRow = 0;
            if(_dbConnection.State==ConnectionState.Closed)
                _dbConnection.Open();

            var transaction = _dbConnection.BeginTransaction();
            const string sql =
                @"INSERT INTO BC_PAYMENT_MONEY_CASH_FLOW_SUBMITTED(DB_CODE,CASH_FLOW_HEADER_ID,CASH_FLOW_ID,NAME,DATE,AMOUNT,CURRENCY_FORMAT,EXCHANGE_RATE,STATUS,CREATED_DATE,CREATED_BY)
                VALUES (@DB_CODE,@CASH_FLOW_HEADER_ID,@CASH_FLOW_ID,@NAME,@DATE,@AMOUNT,@FORMAT,@EXCHANGE,@STATUS,@CREATED_DATE,@CREATED_BY)";
            try
            {
                foreach (var item in model)
                {
                    var submittedParam = new
                    {
                        DB_CODE = item.DbCode,
                        CASH_FLOW_HEADER_ID = item.HeaderId,
                        DATE = item.Date,
                        NAME = item.Name,
                        CASH_FLOW_ID = item.Id,
                        AMOUNT = item.Amount,
                        FORMAT = item.CurrencyFormat,
                        EXCHANGE = item.ExchangeRate,
                        STATUS = "Pending",
                        CREATED_DATE = DateTime.Today,
                        CREATED_BY = item.CreatedBy,
                    };
                    affectedRow += await _dbConnection.ExecuteAsync(sql, submittedParam, transaction);
                    affectedRow += await _dbConnection.ExecuteAsync(
                        @"UPDATE BC_PAYMENT_MONEY_CASH_FLOW SET STATUS = 'Submitted',UPDATED_DATE = @UPDATE_DATE,UPDATED_BY = @UPDATE_BY WHERE ID = @ID",
                        new { ID = item.Id, UPDATE_DATE = DateTime.Today, UPDATE_BY = item.CreatedBy },
                        transaction);
                    affectedRow += await _dbConnection.ExecuteAsync(
                        @"UPDATE BC_PAYMENT_MONEY_CASH_FLOW_HEADER SET STATUS = 'Submitted',UPDATED_DATE = @UPDATE_DATE,UPDATED_BY = @UPDATE_BY WHERE ID = @ID",
                        new
                        {
                            ID = item.HeaderId,
                            UPDATE_DATE = DateTime.Today,
                            UPDATE_BY = item.CreatedBy
                        }, transaction);
                }

                if (affectedRow > 2)
                {
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
                Debug.WriteLine(ex.Message);

            }
            return 0;
        }

        public async Task<int> DeleteCashFlowDetailAsync(int id)
        {
            const string sql = @"DELETE BC_PAYMENT_MONEY_CASH_FLOW WHERE ID = @ID";
            var param = new
            {
                ID = id
            };
            var affectedRow = await _sqlDataAccess.ExecuteAsync(sql, param);
            return affectedRow;
        }

        public async Task<List<string>> GetDescriptionCashFlowDetailAsync(string dbCode)
        {
            string sql = $@"SELECT DISTINCT NAME FROM BC_PAYMENT_MONEY_CASH_FLOW WHERE DB_CODE = @DB_CODE";
            var param = new
            {
                DB_CODE = dbCode,
            };
            var execute = await _sqlDataAccess.LoadData<string, dynamic>(sql, param);
            return execute.ToList();
        }

    }
}
