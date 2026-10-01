using BC.PAYMENT.APPLICATION.Interfaces.CashFlow;
using BC.PAYMENT.CORE.Contracts.CashFlow;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.CashFlow;

public class CashFlowDetailRepository : ICashFlowDetailRepository
{
    private readonly ISqlDataAccess _sqlDataAccess;

    public CashFlowDetailRepository(ISqlDataAccess sqlDataAccess)
    {
        _sqlDataAccess = sqlDataAccess;
    }

    public async Task<int> AddAsync(string dbCode, CashFlowDetailRequest request, string createdBy,
        string entriesCode)
    {
        const string sql = @"
            SET XACT_ABORT ON;
            BEGIN TRANSACTION;

            DECLARE @HEADER_ID INT = NULLIF(@REQUEST_HEADER_ID, 0);
            DECLARE @ROW_DATE DATE = @DATE;
            IF @HEADER_ID IS NULL
            BEGIN
                DECLARE @HEADER_STATUS VARCHAR(50);
                SELECT TOP 1 @HEADER_ID = ID, @HEADER_STATUS = STATUS
                FROM BC_PAYMENT_MONEY_CASH_FLOW_HEADER WITH (UPDLOCK, HOLDLOCK)
                WHERE DB_CODE = @DB_CODE AND CASH_FLOW_DATE = @DATE;

                IF @HEADER_ID IS NOT NULL AND @HEADER_STATUS <> 'Pending'
                BEGIN
                    DECLARE @LOCKED_ID INT = @HEADER_ID;
                    SELECT TOP 1 @HEADER_ID = ID, @ROW_DATE = CASH_FLOW_DATE
                    FROM BC_PAYMENT_MONEY_CASH_FLOW_HEADER WITH (UPDLOCK, HOLDLOCK)
                    WHERE DB_CODE = @DB_CODE AND STATUS = 'Pending' AND CASH_FLOW_DATE >= @DATE
                    ORDER BY CASH_FLOW_DATE, ID;

                    IF @HEADER_ID = @LOCKED_ID
                    BEGIN
                        SET @HEADER_ID = NULL;
                        SET @ROW_DATE = DATEADD(DAY, 1, @DATE);
                        WHILE EXISTS (
                            SELECT 1 FROM BC_PAYMENT_MONEY_CASH_FLOW_HEADER WITH (UPDLOCK, HOLDLOCK)
                            WHERE DB_CODE = @DB_CODE AND CASH_FLOW_DATE = @ROW_DATE
                        )
                            SET @ROW_DATE = DATEADD(DAY, 1, @ROW_DATE);
                    END
                END

                IF @HEADER_ID IS NULL
                BEGIN
                    INSERT INTO BC_PAYMENT_MONEY_CASH_FLOW_HEADER
                        (DB_CODE, CASH_FLOW_DATE, STATUS, CREATED_BY, CREATED_DATE, ENTRIES_CODE)
                    VALUES
                        (@DB_CODE, @ROW_DATE, 'Pending', @CREATED_BY, GETDATE(), @ENTRIES_CODE);
                    SET @HEADER_ID = CAST(SCOPE_IDENTITY() AS INT);
                END
            END

            DECLARE @ID INT = 0;
            IF EXISTS (
                SELECT 1 FROM BC_PAYMENT_MONEY_CASH_FLOW_HEADER
                WHERE ID = @HEADER_ID AND DB_CODE = @DB_CODE AND STATUS = 'Pending'
            )
            BEGIN
                INSERT INTO BC_PAYMENT_MONEY_CASH_FLOW
                    (DB_CODE, HEADER_ID, DATE, NAME, AMOUNT, CURRENCY_FORMAT, EXCHANGE_RATE, STATUS, CREATED_DATE, CREATED_BY)
                VALUES
                    (@DB_CODE, @HEADER_ID, @ROW_DATE, @NAME, @AMOUNT, @CURRENCY_FORMAT, @EXCHANGE_RATE, 'Pending', GETDATE(), @CREATED_BY);
                SET @ID = CAST(SCOPE_IDENTITY() AS INT);
            END

            IF @ID > 0 COMMIT TRANSACTION; ELSE ROLLBACK TRANSACTION;
            SELECT @ID;";

        var param = new
        {
            DB_CODE = dbCode,
            REQUEST_HEADER_ID = request.HeaderId,
            ENTRIES_CODE = entriesCode,
            DATE = request.Date.Date,
            NAME = request.Name.Trim(),
            AMOUNT = request.Amount,
            CURRENCY_FORMAT = request.CurrencyFormat.ToString(),
            EXCHANGE_RATE = request.ExchangeRate,
            CREATED_BY = createdBy
        };
        return await _sqlDataAccess.ExecuteScalarAsync<int, dynamic>(sql, param);
    }

    public async Task<string?> GetHeaderStatusAsync(string dbCode, int headerId, DateTime date)
    {
        // Same header AddAsync targets: the given id, or the header of the date when the id is 0.
        const string sql = @"
            SELECT TOP 1 STATUS
            FROM BC_PAYMENT_MONEY_CASH_FLOW_HEADER
            WHERE DB_CODE = @DB_CODE
              AND ((@HEADER_ID > 0 AND ID = @HEADER_ID) OR (@HEADER_ID = 0 AND CASH_FLOW_DATE = @DATE))";

        var param = new { DB_CODE = dbCode, HEADER_ID = headerId, DATE = date.Date };
        return await _sqlDataAccess.ExecuteScalarAsync<string?, dynamic>(sql, param);
    }

    public async Task<DateTime?> GetDetailDateAsync(string dbCode, int id)
    {
        const string sql = "SELECT DATE FROM BC_PAYMENT_MONEY_CASH_FLOW WHERE ID = @ID AND DB_CODE = @DB_CODE";
        return await _sqlDataAccess.ExecuteScalarAsync<DateTime?, dynamic>(sql, new { DB_CODE = dbCode, ID = id });
    }

    public async Task<int> UpdateAsync(string dbCode, int id, CashFlowDetailRequest request, string updatedBy)
    {
        const string sql = @"
            UPDATE BC_PAYMENT_MONEY_CASH_FLOW
            SET DATE = @DATE, NAME = @NAME, AMOUNT = @AMOUNT, CURRENCY_FORMAT = @CURRENCY_FORMAT,
                EXCHANGE_RATE = @EXCHANGE_RATE, UPDATED_BY = @UPDATED_BY, UPDATED_DATE = GETDATE()
            WHERE ID = @ID AND HEADER_ID = @HEADER_ID AND DB_CODE = @DB_CODE AND STATUS = 'Pending'";

        var param = new
        {
            DB_CODE = dbCode,
            ID = id,
            HEADER_ID = request.HeaderId,
            DATE = request.Date.Date,
            NAME = request.Name.Trim(),
            AMOUNT = request.Amount,
            CURRENCY_FORMAT = request.CurrencyFormat.ToString(),
            EXCHANGE_RATE = request.ExchangeRate,
            UPDATED_BY = updatedBy
        };
        return await _sqlDataAccess.ExecuteAsync(sql, param);
    }
}