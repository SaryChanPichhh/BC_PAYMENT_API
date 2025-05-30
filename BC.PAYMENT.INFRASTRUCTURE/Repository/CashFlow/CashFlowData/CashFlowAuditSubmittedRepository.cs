
using System.Data;
using System.Diagnostics;
using BC.PAYMENT.APPLICATION.Interfaces.CashFlow.CashFlowData;
using BC.PAYMENT.CORE.Entities.CashFlow.CashFlowData;
using BC.PAYMENT.INFRASTRUCTURE.DBAccess;
using Dapper;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.CashFlow.CashFlowData
{
    public class CashFlowAuditSubmittedRepository : ICashFlowAuditSubmittedRepository
    {
        private readonly ISqlDataAccess _sqlDataAccess;
        private readonly IDbConnection _dbConnection;
        public CashFlowAuditSubmittedRepository(ISqlDataAccess sqlDataAccess, IDbConnection dbConnection)
        {
            _sqlDataAccess = sqlDataAccess;
            _dbConnection = dbConnection;
        }

        public async Task<List<CashFlowDataHeader>> GetCashFlowHeaderSubmittedAsync(string dbCode)
        {
            const string sql =
                @"SELECT ID Id,CASH_FLOW_DATE [Date],STATUS [Status] FROM BC_PAYMENT_MONEY_CASH_FLOW_HEADER WHERE DB_CODE = @DB_CODE AND STATUS = 'Submitted";
            var param = new
            {
                DB_CODE = dbCode,
            };
            var execute = await _sqlDataAccess.LoadData<CashFlowDataHeader, dynamic>(sql, param);
            return execute.ToList();
        }

        public async Task<int> AuditCashFlowAsync(string createBy, int headerId, string status)
        {
            if(_dbConnection.State == ConnectionState.Open) 
                _dbConnection.Open();
            var transaction = _dbConnection.BeginTransaction();

            try
            {
                const string cashFlowHeader =
                    @"UPDATE BC_PAYMENT_MONEY_CASH_FLOW_HEADER SET STATUS = @STATUS,UPDATED_BY = @UPDATED_BY,UPDATED_DATE = @UPDATED_DATE WHERE ID = @ID";
                var param = new
                {
                    ID = headerId,
                    STATUS = status,
                    UPDATED_BY = createBy,
                    UPDATED_DATE = DateTime.Today,
                };
                var affectedRow = await _dbConnection.ExecuteAsync(cashFlowHeader, param);
                if (affectedRow > 0)
                {
                    const string cashFlowDetail =
                        @"UPDATE BC_PAYMENT_MONEY_CASH_FLOW SET STATUS = @STATUS,UPDATED_BY = @UPDATED_BY,UPDATED_DATE = @UPDATED_DATE WHERE HEADER_ID = @ID";
                    await _dbConnection.ExecuteAsync(cashFlowDetail, param, transaction);

                    const string cashFlowSubmitted =
                        @"UPDATE BC_PAYMENT_MONEY_CASH_FLOW_SUBMITTED SET STATUS = @STATUS, UPDATED_BY = @UPDATED_BY,UPDATED_DATE = @UPDATED_DATE WHERE CASH_FLOW_HEADER_ID = @ID AND STATUS = 'Pending';";
                    await _dbConnection.ExecuteAsync(cashFlowDetail, param, transaction);

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

        public async Task<List<PaymentCashFlowModel>> GetCashFlowDetailPendingAsync(string dbCode, int headerId)
        {
            const string sql =
                @"SELECT SUBMITTED_ID Id,P.DATE Date,P.NAME Name,P.AMOUNT Amount,P.CURRENCY_FORMAT CurrencyFormat,P.EXCHANGE_RATE ExchangeRate,
                P.CREATED_DATE CreatedDate,P.CREATED_BY CreatedBy,STATUS Status
                FROM BC_PAYMENT_MONEY_CASH_FLOW_SUBMITTED P  WHERE P.DB_CODE = @DB_CODE AND CASH_FLOW_HEADER_ID = @HEADER_ID AND STATUS = 'Pending'";
            var param = new { DB_CODE = dbCode, HEADER_ID = headerId };
            var execute = await _sqlDataAccess.LoadData<PaymentCashFlowModel, dynamic>(sql,param);
            return execute.ToList();
        }
    }
}
