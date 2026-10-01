using BC.PAYMENT.CORE.Entities.CashFlow;
using ICashFlowSubmittedRepository = BC.PAYMENT.APPLICATION.Interfaces.CashFlow.ICashFlowSubmittedRepository;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.CashFlow;

public class CashFlowSubmittedRepository : ICashFlowSubmittedRepository
{
    private readonly ISqlDataAccess _sqlDataAccess;

    public CashFlowSubmittedRepository(ISqlDataAccess sqlDataAccess)
    {
        _sqlDataAccess = sqlDataAccess;
    }

    public async Task<IEnumerable<CashFlowModelSubmittedModel>> GetSubmittedPaymentCashFlowsAsync(string dbCode,
        DateTime fromDate, DateTime toDate, SubmittedStatus? status)
    {
        const string sql = @"
            SELECT
                P.DATE AS Date, S.SUBMITTED_ID AS SubmittedId, S.NAME AS Name, S.AMOUNT AS Amount,
                S.CURRENCY_FORMAT AS CurrencyFormat, S.EXCHANGE_RATE AS ExchangeRate,
                S.CREATED_DATE AS CreatedDate, S.CREATED_BY AS CreatedBy,
                S.UPDATED_DATE AS UpdatedDate, S.UPDATED_BY AS UpdatedBy, S.STATUS AS Status
            FROM BC_PAYMENT_MONEY_CASH_FLOW_SUBMITTED S
            INNER JOIN BC_PAYMENT_MONEY_CASH_FLOW P ON P.ID = S.CASH_FLOW_ID AND P.DB_CODE = S.DB_CODE
            WHERE S.DB_CODE = @DB_CODE
              AND P.DATE >= @FROM_DATE AND P.DATE < @TO_DATE
              AND ((@STATUS IS NULL AND P.STATUS != 'Cancel') OR S.STATUS = @STATUS)
            ORDER BY P.DATE, S.SUBMITTED_ID";

        var param = new
        {
            DB_CODE = dbCode,
            FROM_DATE = fromDate.Date,
            TO_DATE = toDate.Date.AddDays(1),
            STATUS = status?.ToString()
        };
        return await _sqlDataAccess.LoadData<CashFlowModelSubmittedModel, dynamic>(sql, param);
    }

    public async Task<int> UpdateSubmittedStatusAsync(string dbCode, int id, SubmittedStatus submittedStatus,
        string updatedBy)
    {
        const string sql = @"
            UPDATE BC_PAYMENT_MONEY_CASH_FLOW_SUBMITTED
            SET STATUS = @STATUS,
                UPDATED_BY = @UPDATED_BY,
                UPDATED_DATE = GETDATE()
            WHERE SUBMITTED_ID = @ID AND DB_CODE = @DB_CODE";

        var param = new
        {
            DB_CODE = dbCode,
            STATUS = submittedStatus.ToString(),
            ID = id,
            UPDATED_BY = updatedBy
        };
        return await _sqlDataAccess.ExecuteAsync(sql, param);
    }

    public async Task<int> DeleteSubmittedAsync(string dbCode, int id)
    {
        const string sql = @"
            DELETE FROM BC_PAYMENT_MONEY_CASH_FLOW_SUBMITTED
            WHERE SUBMITTED_ID = @ID AND DB_CODE = @DB_CODE";

        var param = new { ID = id, DB_CODE = dbCode };
        return await _sqlDataAccess.ExecuteAsync(sql, param);
    }

    public async Task<IEnumerable<CashFlowModelSubmittedModel>> GetSubmittedPaymentCashFlowReportAsync(string dbCode,
        DateTime fromDate, DateTime toDate)
    {
        const string sql = @"
            SELECT
                S.DATE AS Date, S.SUBMITTED_ID AS SubmittedId, S.NAME AS Name, S.AMOUNT AS Amount,
                S.CURRENCY_FORMAT AS CurrencyFormat, S.EXCHANGE_RATE AS ExchangeRate,
                S.CREATED_DATE AS CreatedDate, S.CREATED_BY AS CreatedBy,
                S.UPDATED_DATE AS UpdatedDate, S.UPDATED_BY AS UpdatedBy, S.STATUS AS Status
            FROM BC_PAYMENT_MONEY_CASH_FLOW_SUBMITTED S
            WHERE S.DB_CODE = @DB_CODE
              AND S.DATE >= @FROM_DATE AND S.DATE < @TO_DATE
            ORDER BY S.DATE, S.SUBMITTED_ID";

        var param = new
        {
            DB_CODE = dbCode,
            FROM_DATE = fromDate.Date,
            TO_DATE = toDate.Date.AddDays(1)
        };
        return await _sqlDataAccess.LoadData<CashFlowModelSubmittedModel, dynamic>(sql, param);
    }
}