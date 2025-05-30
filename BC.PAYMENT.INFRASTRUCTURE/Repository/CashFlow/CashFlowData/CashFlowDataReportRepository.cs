

using BC.PAYMENT.APPLICATION.Interfaces.CashFlow.CashFlowData;
using BC.PAYMENT.CORE.Entities.CashFlow.CashFlowData;
using BC.PAYMENT.INFRASTRUCTURE.DBAccess;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.CashFlow.CashFlowData
{
    public class CashFlowDataReportRepository : ICashFlowDataReportRepository
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public CashFlowDataReportRepository(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<List<PaymentCashFlowModel>> GetPaymentCashFlowReportAsync(string dbCode)
        {
            var sql =
                $@"SELECT TOP 100 ID Id,P.DATE Date,P.NAME Name,P.AMOUNT Amount,P.CURRENCY_FORMAT CurrencyFormat,P.EXCHANGE_RATE ExchangeRate,
                P.CREATED_DATE CreatedDate,P.CREATED_BY CreatedBy,S.STATUS Status
                FROM BC_PAYMENT_MONEY_CASH_FLOW P LEFT JOIN (SELECT STATUS,CASH_FLOW_ID FROM BC_PAYMENT_MONEY_CASH_FLOW_SUBMITTED WHERE STATUS != 'Cancel' AND DB_CODE = @DB_CODE) S 
				ON S.CASH_FLOW_ID = P.ID WHERE P.DB_CODE = @DB_CODE";
            var param = new
            {
                DB_CODE = dbCode
            };
            var execute = await _sqlDataAccess.LoadData<PaymentCashFlowModel, dynamic>(sql, param);
            return execute.ToList();
        }

        public async Task<List<PaymentCashFlowModel>> GetPaymentCashFlowReportByDateAsync(string dbCode, DateTime fromDate, DateTime toDate)
        {
            var sql =
                $@"SELECT ID Id,P.DATE Date,P.NAME Name,P.AMOUNT Amount,P.CURRENCY_FORMAT CurrencyFormat,P.EXCHANGE_RATE ExchangeRate,
                P.CREATED_DATE CreatedDate,P.CREATED_BY CreatedBy,S.STATUS Status
                FROM BC_PAYMENT_MONEY_CASH_FLOW P LEFT JOIN (SELECT STATUS,CASH_FLOW_ID FROM BC_PAYMENT_MONEY_CASH_FLOW_SUBMITTED WHERE STATUS != 'Cancel' AND DB_CODE = @DB_CODE) S 
				ON S.CASH_FLOW_ID = P.ID WHERE P.DB_CODE = @DB_CODE AND P.DATE BETWEEN @FROM_DATE AND @TO_DATE";
            var param = new
            {
                DB_CODE = dbCode,
                FROM_DATE = fromDate, 
                TO_DATE = toDate
            };
            var execute = await _sqlDataAccess.LoadData<PaymentCashFlowModel, dynamic>(sql, param);
            return execute.ToList();
        }
    }
}
