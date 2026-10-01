using BC.PAYMENT.APPLICATION.Interfaces.CashFlow;
using BC.PAYMENT.CORE.Entities.CashFlow;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.CashFlow;

public class CashFlowHeaderRepository : ICashFlowHeaderRepository
{
    private readonly ISqlDataAccess _sqlDataAccess;

    public CashFlowHeaderRepository(ISqlDataAccess sqlDataAccess)
    {
        _sqlDataAccess = sqlDataAccess;
    }

    public async Task<List<CashFlowHeaderModel>> GetAllAsync(string dbCode)
    {
        const string sql = @"
            SELECT
                H.ID AS Id, H.CASH_FLOW_DATE AS Date, H.ENTRIES_CODE AS EntriesCode, H.STATUS AS Status,
                H.CREATED_BY AS CreatedBy, H.CREATED_DATE AS CreatedDate
            FROM BC_PAYMENT_MONEY_CASH_FLOW_HEADER H
            WHERE H.DB_CODE = @DB_CODE
            ORDER BY H.CASH_FLOW_DATE DESC, H.ID DESC";

        var param = new { DB_CODE = dbCode };
        var results = await _sqlDataAccess.LoadData<CashFlowHeaderModel, dynamic>(sql, param);

        return results.ToList();
    }

    public async Task<int> AddAsync(string dbCode, DateTime date, string createdBy, string entriesCode)
    {
        // Returns the new header id, or 0 when a header already exists for the date.
        const string sql = @"
            DECLARE @ID INT = 0;
            IF NOT EXISTS (
                SELECT 1 FROM BC_PAYMENT_MONEY_CASH_FLOW_HEADER WITH (UPDLOCK, HOLDLOCK)
                WHERE DB_CODE = @DB_CODE AND CASH_FLOW_DATE = @CASH_FLOW_DATE
            )
            BEGIN
                INSERT INTO BC_PAYMENT_MONEY_CASH_FLOW_HEADER
                    (DB_CODE, CASH_FLOW_DATE, STATUS, CREATED_BY, CREATED_DATE, ENTRIES_CODE)
                VALUES
                    (@DB_CODE, @CASH_FLOW_DATE, 'Pending', @CREATED_BY, GETDATE(), @ENTRIES_CODE);
                SET @ID = CAST(SCOPE_IDENTITY() AS INT);
            END
            SELECT @ID;";

        var param = new
        {
            DB_CODE = dbCode,
            CASH_FLOW_DATE = date.Date,
            CREATED_BY = createdBy,
            ENTRIES_CODE = entriesCode
        };
        return await _sqlDataAccess.ExecuteScalarAsync<int, dynamic>(sql, param);
    }

    public async Task<int> SubmitAsync(string dbCode, int id, string submittedBy)
    {
        // Only a Pending header with at least one detail can be submitted. Details are copied as stored,
        // so the currency text never goes through a JSON / enum conversion.
        const string sql = @"
            SET XACT_ABORT ON;
            BEGIN TRANSACTION;

            UPDATE H
            SET STATUS = 'Submitted', UPDATED_BY = @UPDATED_BY, UPDATED_DATE = GETDATE()
            FROM BC_PAYMENT_MONEY_CASH_FLOW_HEADER H
            WHERE H.ID = @ID AND H.DB_CODE = @DB_CODE AND H.STATUS = 'Pending'
              AND EXISTS (SELECT 1 FROM BC_PAYMENT_MONEY_CASH_FLOW D
                          WHERE D.HEADER_ID = H.ID AND D.DB_CODE = H.DB_CODE);

            DECLARE @AFFECTED INT = @@ROWCOUNT;
            IF @AFFECTED > 0
            BEGIN
                INSERT INTO BC_PAYMENT_MONEY_CASH_FLOW_SUBMITTED
                    (DB_CODE, CASH_FLOW_HEADER_ID, CASH_FLOW_ID, NAME, DATE, AMOUNT, CURRENCY_FORMAT, EXCHANGE_RATE,
                     STATUS, CREATED_DATE, CREATED_BY)
                SELECT D.DB_CODE, D.HEADER_ID, D.ID, D.NAME, D.DATE, D.AMOUNT, D.CURRENCY_FORMAT, D.EXCHANGE_RATE,
                       'Pending', GETDATE(), @UPDATED_BY
                FROM BC_PAYMENT_MONEY_CASH_FLOW D
                WHERE D.HEADER_ID = @ID AND D.DB_CODE = @DB_CODE;

                UPDATE BC_PAYMENT_MONEY_CASH_FLOW
                SET STATUS = 'Submitted', UPDATED_BY = @UPDATED_BY, UPDATED_DATE = GETDATE()
                WHERE HEADER_ID = @ID AND DB_CODE = @DB_CODE;
            END

            COMMIT TRANSACTION;
            SELECT @AFFECTED;";

        var param = new { DB_CODE = dbCode, ID = id, UPDATED_BY = submittedBy };
        return await _sqlDataAccess.ExecuteScalarAsync<int, dynamic>(sql, param);
    }

    public async Task<int> CancelSubmitAsync(string dbCode, int id, string updatedBy)
    {
        // Only a Submitted header can be reverted; its details go back to Pending, its submissions to Cancel.
        const string sql = @"
            SET XACT_ABORT ON;
            BEGIN TRANSACTION;

            UPDATE BC_PAYMENT_MONEY_CASH_FLOW_HEADER
            SET STATUS = 'Pending', UPDATED_BY = @UPDATED_BY, UPDATED_DATE = GETDATE()
            WHERE ID = @ID AND DB_CODE = @DB_CODE AND STATUS = 'Submitted';

            DECLARE @AFFECTED INT = @@ROWCOUNT;
            IF @AFFECTED > 0
            BEGIN
                UPDATE BC_PAYMENT_MONEY_CASH_FLOW
                SET STATUS = 'Pending', UPDATED_BY = @UPDATED_BY, UPDATED_DATE = GETDATE()
                WHERE HEADER_ID = @ID AND DB_CODE = @DB_CODE;

                UPDATE BC_PAYMENT_MONEY_CASH_FLOW_SUBMITTED
                SET STATUS = 'Cancel', UPDATED_BY = @UPDATED_BY, UPDATED_DATE = GETDATE()
                WHERE CASH_FLOW_HEADER_ID = @ID AND DB_CODE = @DB_CODE;
            END

            COMMIT TRANSACTION;
            SELECT @AFFECTED;";

        var param = new { DB_CODE = dbCode, ID = id, UPDATED_BY = updatedBy };
        return await _sqlDataAccess.ExecuteScalarAsync<int, dynamic>(sql, param);
    }

    public async Task<int> DeleteAsync(string dbCode, int id)
    {
        // Only a Pending header can be deleted, together with its details.
        const string sql = @"
            SET XACT_ABORT ON;
            BEGIN TRANSACTION;

            DELETE FROM BC_PAYMENT_MONEY_CASH_FLOW_HEADER
            WHERE ID = @ID AND DB_CODE = @DB_CODE AND STATUS = 'Pending';

            DECLARE @AFFECTED INT = @@ROWCOUNT;
            IF @AFFECTED > 0
                DELETE FROM BC_PAYMENT_MONEY_CASH_FLOW WHERE HEADER_ID = @ID AND DB_CODE = @DB_CODE;

            COMMIT TRANSACTION;
            SELECT @AFFECTED;";

        var param = new { DB_CODE = dbCode, ID = id };
        return await _sqlDataAccess.ExecuteScalarAsync<int, dynamic>(sql, param);
    }
}