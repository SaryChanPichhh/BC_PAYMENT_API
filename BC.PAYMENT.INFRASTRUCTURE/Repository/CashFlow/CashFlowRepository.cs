using BC.PAYMENT.APPLICATION.Interfaces.CashFlow;
using BC.PAYMENT.CORE.Entities.CashFlow;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.CashFlow;

public class CashFlowRepository(ISqlDataAccess sqlDataAccess) : ICashFlowRepository
{
    private const string SelectColumns = @"
                P.ID AS Id, P.DATE AS Date, P.NAME AS Name, P.AMOUNT AS Amount,
                P.CURRENCY_FORMAT AS CurrencyFormat, P.EXCHANGE_RATE AS ExchangeRate,
                P.CREATED_DATE AS CreatedDate, P.CREATED_BY AS CreatedBy";

    public async Task<List<CashFlowModel>> GetPaymentCashFlowsAsync(string dbCode, DateTime fromDate, DateTime toDate)
    {
        const string sql = $@"
            SELECT {SelectColumns}, S.STATUS AS Status
            FROM BC_PAYMENT_MONEY_CASH_FLOW P
            OUTER APPLY (
                SELECT TOP 1 X.STATUS
                FROM BC_PAYMENT_MONEY_CASH_FLOW_SUBMITTED X
                WHERE X.CASH_FLOW_ID = P.ID
                  AND X.DB_CODE = @DB_CODE
                  AND X.STATUS != 'Cancel'
                ORDER BY X.SUBMITTED_ID DESC
            ) S
            WHERE P.DB_CODE = @DB_CODE
              AND P.DATE >= @FROM_DATE AND P.DATE < @TO_DATE
            ORDER BY P.DATE, P.ID";

        var param = new { DB_CODE = dbCode, FROM_DATE = fromDate.Date, TO_DATE = toDate.Date.AddDays(1) };
        return await QueryAsync(sql, param);
    }

    public async Task<List<CashFlowModel>> GetPaymentsByDbCodeAndEntryCodeAsync(string dbCode, string entryCode,
        DateTime fromDate, DateTime toDate)
    {
        const string sql = $@"
            SELECT {SelectColumns}, P.STATUS AS Status
            FROM BC_PAYMENT_MONEY_CASH_FLOW P
            INNER JOIN BC_PAYMENT_MONEY_CASH_FLOW_HEADER H ON H.ID = P.HEADER_ID
            WHERE P.DB_CODE = @DB_CODE
              AND H.ENTRIES_CODE = @ENTRIES_CODE
              AND P.DATE >= @FROM_DATE AND P.DATE < @TO_DATE
            ORDER BY P.DATE, P.ID";

        var param = new
        {
            DB_CODE = dbCode, ENTRIES_CODE = entryCode, FROM_DATE = fromDate.Date, TO_DATE = toDate.Date.AddDays(1)
        };
        return await QueryAsync(sql, param);
    }

    public async Task<List<CashFlowModel>> GetPaymentsByDbCodeAndMultiEntryCodesAsync(string dbCode,
        List<string> entryCodes)
    {
        const string sql = $@"
            SELECT {SelectColumns}, P.STATUS AS Status
            FROM BC_PAYMENT_MONEY_CASH_FLOW P
            INNER JOIN BC_PAYMENT_MONEY_CASH_FLOW_HEADER H ON H.ID = P.HEADER_ID
            WHERE P.DB_CODE = @DB_CODE
              AND H.ENTRIES_CODE IN @ENTRIES_CODE
            ORDER BY P.DATE, P.ID";

        var param = new { DB_CODE = dbCode, ENTRIES_CODE = entryCodes };
        return await QueryAsync(sql, param);
    }

    // Dapper buffers into a List<T> already; AsList() reuses it instead of copying like ToList().
    private async Task<List<CashFlowModel>> QueryAsync(string sql, object param)
    {
        return (await sqlDataAccess.LoadData<CashFlowModel, object>(sql, param)).AsList();
    }
}