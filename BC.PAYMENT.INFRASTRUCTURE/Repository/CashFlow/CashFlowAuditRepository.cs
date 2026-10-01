using BC.PAYMENT.APPLICATION.Interfaces.CashFlow;
using BC.PAYMENT.CORE.Entities.CashFlow;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.CashFlow;

public class CashFlowAuditRepository : ICashFlowAuditRepository
{
    private readonly IDbConnection _dbConnection;
    private readonly ISqlDataAccess _sqlDataAccess;

    public CashFlowAuditRepository(ISqlDataAccess sqlDataAccess, IDbConnection dbConnection)
    {
        _sqlDataAccess = sqlDataAccess;
        _dbConnection = dbConnection;
    }

    public async Task<List<CashFlowHeaderModel>> GetSubmittedHeadersAsync(string dbCode)
    {
        const string sql = @"
            SELECT
                H.ID AS Id, H.CASH_FLOW_DATE AS Date, H.ENTRIES_CODE AS EntriesCode, H.STATUS AS Status,
                H.CREATED_BY AS CreatedBy, H.CREATED_DATE AS CreatedDate,
                ISNULL(S.PENDING_COUNT, 0) AS PendingCount
            FROM BC_PAYMENT_MONEY_CASH_FLOW_HEADER H
            LEFT JOIN (
                SELECT X.CASH_FLOW_HEADER_ID, COUNT(1) AS PENDING_COUNT
                FROM BC_PAYMENT_MONEY_CASH_FLOW_SUBMITTED X
                WHERE X.DB_CODE = @DB_CODE AND X.STATUS = 'Pending'
                GROUP BY X.CASH_FLOW_HEADER_ID
            ) S ON S.CASH_FLOW_HEADER_ID = H.ID
            WHERE H.DB_CODE = @DB_CODE
              AND H.STATUS = 'Submitted'
            ORDER BY H.CASH_FLOW_DATE, H.ID";

        var param = new { DB_CODE = dbCode };
        var results = await _sqlDataAccess.LoadData<CashFlowHeaderModel, dynamic>(sql, param);

        return results.ToList();
    }

    public async Task<int> UpdateStatusAsync(string dbCode, int headerId, SubmittedStatus status,
        string updatedBy)
    {
        const string headerSql = @"
            UPDATE BC_PAYMENT_MONEY_CASH_FLOW_HEADER
            SET STATUS = @STATUS, UPDATED_BY = @UPDATED_BY, UPDATED_DATE = GETDATE()
            WHERE ID = @HEADER_ID AND DB_CODE = @DB_CODE AND STATUS = 'Submitted'";

        const string detailSql = @"
            UPDATE BC_PAYMENT_MONEY_CASH_FLOW
            SET STATUS = @STATUS, UPDATED_BY = @UPDATED_BY, UPDATED_DATE = GETDATE()
            WHERE HEADER_ID = @HEADER_ID AND DB_CODE = @DB_CODE";

        const string submittedSql = @"
            UPDATE BC_PAYMENT_MONEY_CASH_FLOW_SUBMITTED
            SET STATUS = @STATUS, UPDATED_BY = @UPDATED_BY, UPDATED_DATE = GETDATE()
            WHERE CASH_FLOW_HEADER_ID = @HEADER_ID AND DB_CODE = @DB_CODE AND STATUS = 'Pending'";

        var param = new
        {
            DB_CODE = dbCode,
            HEADER_ID = headerId,
            STATUS = status.ToString(),
            UPDATED_BY = updatedBy
        };

        if (_dbConnection.State == ConnectionState.Closed)
            _dbConnection.Open();
        using var transaction = _dbConnection.BeginTransaction();
        try
        {
            var affectedRows = await _dbConnection.ExecuteAsync(headerSql, param, transaction);
            if (affectedRows == 0)
            {
                transaction.Rollback(); 
                return 0;
            }

            await _dbConnection.ExecuteAsync(detailSql, param, transaction);
            await _dbConnection.ExecuteAsync(submittedSql, param, transaction);
            transaction.Commit();
            return affectedRows;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<List<CashFlowModelSubmittedModel>> GetPendingDetailsAsync(string dbCode, int headerId)
    {
        const string sql = @"
            SELECT
                S.CASH_FLOW_ID AS Id, S.SUBMITTED_ID AS SubmittedId, S.DATE AS Date, S.NAME AS Name,
                S.AMOUNT AS Amount, S.CURRENCY_FORMAT AS CurrencyFormat, S.EXCHANGE_RATE AS ExchangeRate,
                S.CREATED_DATE AS CreatedDate, S.CREATED_BY AS CreatedBy,
                S.UPDATED_DATE AS UpdatedDate, S.UPDATED_BY AS UpdatedBy, S.STATUS AS Status
            FROM BC_PAYMENT_MONEY_CASH_FLOW_SUBMITTED S
            INNER JOIN BC_PAYMENT_MONEY_CASH_FLOW_HEADER H ON H.ID = S.CASH_FLOW_HEADER_ID AND H.DB_CODE = S.DB_CODE
            INNER JOIN BC_PAYMENT_MONEY_CASH_FLOW P ON P.ID = S.CASH_FLOW_ID AND P.DB_CODE = S.DB_CODE
            WHERE S.DB_CODE = @DB_CODE
              AND S.CASH_FLOW_HEADER_ID = @HEADER_ID
              AND S.STATUS = 'Pending'
            ORDER BY S.DATE, S.SUBMITTED_ID";

        var param = new { DB_CODE = dbCode, HEADER_ID = headerId };
        var results = await _sqlDataAccess.LoadData<CashFlowModelSubmittedModel, dynamic>(sql, param);
        return results.ToList();
    }
}