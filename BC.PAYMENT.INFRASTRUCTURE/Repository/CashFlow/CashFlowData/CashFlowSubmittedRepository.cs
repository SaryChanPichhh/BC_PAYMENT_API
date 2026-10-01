namespace BC.PAYMENT.INFRASTRUCTURE.Repository.CashFlow.CashFlowData;

public class CashFlowSubmittedRepository : ICashFlowSubmittedRepository
{
    private readonly ISqlDataAccess _sqlDataAccess;

    public CashFlowSubmittedRepository(ISqlDataAccess sqlDataAccess)
    {
        _sqlDataAccess = sqlDataAccess;
    }

    public async Task<List<PaymentCashFlowSubmittedModel>> GetPaymentCashFlowModelSubmittedAsync(string dbCode)
    {
        const string sql =
            @"SELECT TOP 100 P.DATE Date,SUBMITTED_ID SubmittedId,S.NAME Name,S.AMOUNT Amount,S.CURRENCY_FORMAT CurrencyFormat,S.EXCHANGE_RATE ExchangeRate,S.CREATED_DATE CreatedDate,S.CREATED_BY CreatedBy,
            P.UPDATED_DATE UpdatedDate,P.UPDATED_BY UpdatedBy,S.STATUS Status
            FROM BC_PAYMENT_MONEY_CASH_FLOW_SUBMITTED S INNER JOIN BC_PAYMENT_MONEY_CASH_FLOW P ON P.ID = S.CASH_FLOW_ID WHERE S.DB_CODE = @DB_CODE AND P.STATUS != 'Cancel'";
        var param = new
        {
            DB_CODE = dbCode
        };
        var results = await _sqlDataAccess.LoadData<PaymentCashFlowSubmittedModel, dynamic>(sql, param);
        return results.ToList();
    }

    public async Task<List<PaymentCashFlowSubmittedModel>> GetPaymentCashFlowModelSubmittedByDateAsync(string dbCode,
        DateTime fromDate, DateTime toDate)
    {
        const string sql =
            @"SELECT S.DATE Date,SUBMITTED_ID SubmittedId,S.NAME Name,S.AMOUNT Amount,S.CURRENCY_FORMAT CurrencyFormat,S.EXCHANGE_RATE ExchangeRate,S.CREATED_DATE CreatedDate,S.CREATED_BY CreatedBy,
             P.UPDATED_DATE UpdatedDate,P.UPDATED_BY UpdatedBy,S.STATUS Status
            FROM BC_PAYMENT_MONEY_CASH_FLOW_SUBMITTED S INNER JOIN BC_PAYMENT_MONEY_CASH_FLOW P ON P.ID = S.CASH_FLOW_ID WHERE S.DB_CODE = @DB_CODE AND P.STATUS != 'Cancel'
			AND P.DATE BETWEEN @FROM_DATE AND @TO_DATE";
        var param = new
        {
            DB_CODE = dbCode,
            FROM_DATE = fromDate,
            TO_DATE = toDate
        };
        var results = await _sqlDataAccess.LoadData<PaymentCashFlowSubmittedModel, dynamic>(sql, param);
        return results.ToList();
    }
}