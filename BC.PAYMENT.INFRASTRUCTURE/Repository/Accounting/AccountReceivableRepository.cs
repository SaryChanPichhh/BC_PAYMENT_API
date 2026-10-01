using BC.PAYMENT.CORE.Contracts.Request.AccountReceivable;
using static BC.PAYMENT.CORE.Entities.Accounting.AccountReceivableModel;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Accounting;

public class AccountReceivableRepository(IDbConnection dbConnection, ISqlDataAccess sqlDataAccess)
    : IAccountReceivableRepository
{
    public async Task<List<AccountReceivablePatternModel>> GetAccountReceivablePatterns(string dbCode)
    {
        const string sql = @"SELECT 
        DATA_CODE AccountCode,
        DATA_NAME CreditDebitType,
        DATA_DESC Description,
        DATA_TYPE DataType,
        FIELD_1 Field1,
        FIELD_2 Field2,
        FIELD_3 Field3,
        FIELD_4 Field4,
        FIELD_5 Field5,
        FIELD_6 Field6,
        FIELD_7 Field7,
        FIELD_8 Field8,
        FIELD_9 Field9,
        CREATED_DATE CreatedDate,
        CREATED_BY CreatedBy
        FROM BCDATA WHERE DB_CODE = @DB_CODE AND DATA_TYPE = @DATA_TPYE";
        var param = new { DB_CODE = dbCode, DATA_TPYE = "ACCOUNT RECEIVABLE" };
        var results = await sqlDataAccess.LoadData<AccountReceivablePatternModel, dynamic>(sql, param);
        return results.ToList();
    }

    public async Task<dynamic> GetJournalTypesByDbCode(string dbCode)
    {
        const string sql = @"SELECT JNLT_CODE,JNLT_DESC FROM SIJNLTYP WHERE DB_CODE = @DB_CODE";
        var param = new { DB_CODE = dbCode };
        var results = await sqlDataAccess.LoadData<dynamic, dynamic>(sql, param);
        return results;
    }

    public async Task<int> GetJournalIdByDbCode(string dbCode)
    {
        var sql = $@"SELECT MAX(JRNAL_NO) FROM {dbCode}SILEDG";
        var results = await sqlDataAccess.ExecuteScalarAsync<int, dynamic>(sql, new { });
        return results;
    }

    public async Task<int> GetAllocReferenceByDbCode(string dbCode)
    {
        var sql = $@"SELECT MAX(ALLOC_REF) FROM {dbCode}SILEDG";
        var results = await sqlDataAccess.ExecuteScalarAsync<int, dynamic>(sql, new { });
        return results;
    }

    public async Task<bool> SplitAccountReceivable(string customerCode, string referenceNo, string dbCode,
        double currentAmount,
        double splitAmount)
    {
        var sql =
            @$"SELECT * FROM {dbCode}SILEDG WHERE REFERENCE = @REFERENCE AND ACC_CODE = @ACC_CODE AND ALLOCATION = ''";
        var param = new
        {
            ACC_CODE = customerCode,
            REFERENCE = referenceNo
        };
        // Check connection status
        if (dbConnection.State != ConnectionState.Open) dbConnection.Open();

        // Get the current account receivable record from database
        var results = (await dbConnection.QueryAsync<SILEDG>(sql, param)).ToList();
        // Check if the account receivable record is found or has been allocated
        if (results.Count is 0 or > 1) return false;

        // Get first record from the list
        var firstOrDefault = results.FirstOrDefault();
        // Check if first record is null
        if (firstOrDefault is null) return false;

        // Start transaction
        using var transaction = dbConnection.BeginTransaction();
        var rowAffected = 0;
        try
        {
            // Delete the current account receivable record
            var sqlDelete =
                @$"DELETE FROM {dbCode}SILEDG WHERE ACC_CODE = @ACC_CODE AND REFERENCE = @REFERENCE AND ALLOCATION = ''";
            var paramDelete = new
            {
                ACC_CODE = customerCode,
                REFERENCE = referenceNo
            };
            rowAffected += await dbConnection.ExecuteAsync(sqlDelete, paramDelete, transaction);
            sql =
                @$"INSERT INTO {dbCode}SILEDG (ACC_CODE, ACC_PERIOD, TRANS_DATE, JRNAL_NO, JRNAL_LINE, AMOUNT, D_C, JRNAL_TYPE, REFERENCE,
                       DESCRIPTN, ENTRY_DATE, ENTRY_PRD, DUE_DATE, ALLOCATION, ALLOC_REF, ALLOC_DATE, ALLOC_PERIOD,
                       ALLOC_USER, ASSET_CODE, ASSET_UPDT, CONV_CODE, CONV_SIGN, CONV_RATE, OTHER_AMT, LOSS_GAIN,
                       ANAL_T0, ANAL_T1, ANAL_T2, ANAL_T3, ANAL_T4, ANAL_T5, ANAL_T6, ANAL_T7, ANAL_T8, ANAL_T9,
                       TRAN_DESC1, TRAN_DESC2, TRAN_DESC3, TRAN_DESC4, TRAN_DESC5, TRAN_DESC6, ALLOC_IN_PROG, HOLD_REF,
                       HOLD_USER_CODE, USER_CREA, USER_UPDT, DATE_UPDT)
                VALUES (@ACC_CODE,@ACC_PERIOD,@TRANS_DATE,@JRNAL_NO,@JRNAL_LINE,@AMOUNT,@D_C,@JRNAL_TYPE,@REFERENCE,@DESCRIPTN,@ENTRY_DATE,@ENTRY_PRD,@DUE_DATE,@ALLOCATION,@ALLOC_REF,@ALLOC_DATE,@ALLOC_PERIOD,@ALLOC_USER,@ASSET_CODE,@ASSET_UPDT,@CONV_CODE,@CONV_SIGN,@CONV_RATE,@OTHER_AMT,@LOSS_GAIN,
                    @ANAL_T0,@ANAL_T1,@ANAL_T2,@ANAL_T3,@ANAL_T4,@ANAL_T5,@ANAL_T6,@ANAL_T7,@ANAL_T8,@ANAL_T9,@TRAN_DESC1,@TRAN_DESC2,@TRAN_DESC3,@TRAN_DESC4,@TRAN_DESC5,@TRAN_DESC6,@ALLOC_IN_PROG,
                    @HOLD_REF,@HOLD_USER_CODE,@USER_CREA,@USER_UPDT,@DATE_UPDT);";
            // Split the current account receivable record
            for (var i = 1; i <= 2; i++)
                if (i == 1)
                {
                    firstOrDefault.AMOUNT = currentAmount;
                    var parameters = new DynamicParameters(firstOrDefault);
                    rowAffected += await dbConnection.ExecuteAsync(sql, parameters, transaction);
                }
                else
                {
                    var maxJournalLine = await dbConnection.ExecuteScalarAsync<int>(
                        $@"SELECT MAX(JRNAL_LINE) FROM {dbCode}SILEDG WHERE REFERENCE = @REFERENCE AND ACC_CODE = @ACC_CODE",
                        new { ACC_CODE = customerCode, REFERENCE = referenceNo }, transaction);
                    firstOrDefault.AMOUNT = splitAmount;
                    firstOrDefault.ALLOCATION = "A";
                    firstOrDefault.JRNAL_LINE = maxJournalLine + 1;
                    var parameters = new DynamicParameters(firstOrDefault);
                    rowAffected += await dbConnection.ExecuteAsync(sql, parameters, transaction);
                }

            if (rowAffected == 3)
                transaction.Commit();
            else
                transaction.Rollback();

            // return true if the account receivable record is successfully split
            return rowAffected == 3;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            transaction.Rollback();
            return false;
        }
    }

    public async Task<int> InsertAccountReceivable(SiLedgerRequest request, bool isAccountsReceivableCompleted = false)
    {
        if (dbConnection.State == ConnectionState.Closed) dbConnection.Open();
        var results = await GetAccountReceivablePatterns(request.DbCode);
        if (results.Count != 2) return 0;
        var maxJournalId = await GetJournalIdByDbCode(request.DbCode);
        var maxAllocReference = await GetAllocReferenceByDbCode(request.DbCode);
        maxJournalId++;
        maxAllocReference++;
        var rowAffected = 0;
        var journalLine = 0;
        using var transaction = dbConnection.BeginTransaction();
        var procedureName = $"{request.DbCode}SI_INSERT_SILEDG";
        foreach (var accountReceivablePatternModel in results)
        {
            journalLine++;
            if (accountReceivablePatternModel.CreditDebitType == CreditDebitType.Credit)
            {
                var param = new
                {
                    ACC_CODE_1 = request.ACC_CODE,
                    request.ACC_PERIOD_2,
                    request.TRANS_DATE_3,
                    JRNAL_NO_4 = maxJournalId,
                    JRNAL_LINE_5 = journalLine,
                    AMOUNT_6 = request.AMOUNT_6 * -1,
                    D_C_7 = "C",
                    JRNAL_TYPE_8 = accountReceivablePatternModel.JournalType,
                    request.REFERENCE_9,
                    request.DESCRIPTN_10,
                    request.ENTRY_DATE_11,
                    request.ENTRY_PRD_12,
                    request.DUE_DATE_13,
                    ALLOCATION_14 = "A",
                    ALLOC_REF_15 = maxAllocReference,
                    ALLOC_DATE_16 = DateTime.Today.ToString("MM/dd/yyyy"),
                    request.ALLOC_PERIOD_17,
                    request.ALLOC_USER_18,
                    request.ASSET_CODE_19,
                    ASSET_UPDT_20 = "N",
                    request.CONV_CODE_21,
                    request.CONV_SIGN_22,
                    CONV_RATE_23 = "0.00000",
                    OTHER_AMT_24 = "0.00000",
                    request.LOSS_GAIN_25,
                    request.ANAL_T0_26,
                    request.ANAL_T1_27,
                    request.ANAL_T2_28,
                    request.ANAL_T3_29,
                    request.ANAL_T4_30,
                    request.ANAL_T5_31,
                    request.ANAL_T6_32,
                    request.ANAL_T7_33,
                    request.ANAL_T8_34,
                    request.ANAL_T9_35,
                    request.TRAN_DESC1_36,
                    request.TRAN_DESC2_37,
                    request.TRAN_DESC3_38,
                    request.TRAN_DESC4_39,
                    request.TRAN_DESC5_40,
                    request.TRAN_DESC6_41,
                    request.ALLOC_IN_PROG_42,
                    HOLD_REF_43 = "0",
                    request.HOLD_USER_CODE_44,
                    request.USER_CREA_45,
                    request.USER_UPDT_46,
                    request.DATE_UPDT_47
                };
                rowAffected +=
                    await dbConnection.ExecuteAsync(procedureName, param, transaction,
                        commandType: CommandType.StoredProcedure);
            }
            else
            {
                var param = new
                {
                    ACC_CODE_1 = accountReceivablePatternModel.AccountCode,
                    request.ACC_PERIOD_2,
                    request.TRANS_DATE_3,
                    JRNAL_NO_4 = maxJournalId,
                    JRNAL_LINE_5 = journalLine,
                    request.AMOUNT_6,
                    D_C_7 = "D",
                    JRNAL_TYPE_8 = accountReceivablePatternModel.JournalType,
                    request.REFERENCE_9,
                    request.DESCRIPTN_10,
                    request.ENTRY_DATE_11,
                    request.ENTRY_PRD_12,
                    request.DUE_DATE_13,
                    ALLOCATION_14 = "",
                    request.ALLOC_REF_15,
                    ALLOC_DATE_16 = "",
                    ALLOC_PERIOD_17 = "0",
                    request.ALLOC_USER_18,
                    ASSET_CODE_19 = "",
                    ASSET_UPDT_20 = "N",
                    request.CONV_CODE_21,
                    request.CONV_SIGN_22,
                    request.CONV_RATE_23,
                    request.OTHER_AMT_24,
                    request.LOSS_GAIN_25,
                    request.ANAL_T0_26,
                    request.ANAL_T1_27,
                    request.ANAL_T2_28,
                    request.ANAL_T3_29,
                    request.ANAL_T4_30,
                    request.ANAL_T5_31,
                    request.ANAL_T6_32,
                    request.ANAL_T7_33,
                    request.ANAL_T8_34,
                    request.ANAL_T9_35,
                    request.TRAN_DESC1_36,
                    request.TRAN_DESC2_37,
                    request.TRAN_DESC3_38,
                    request.TRAN_DESC4_39,
                    request.TRAN_DESC5_40,
                    request.TRAN_DESC6_41,
                    request.ALLOC_IN_PROG_42,
                    request.HOLD_REF_43,
                    request.HOLD_USER_CODE_44,
                    request.USER_CREA_45,
                    request.USER_UPDT_46,
                    request.DATE_UPDT_47
                };
                rowAffected +=
                    await dbConnection.ExecuteAsync(procedureName, param, transaction,
                        commandType: CommandType.StoredProcedure);
            }
        }

        if (isAccountsReceivableCompleted)
        {
            var sql =
                @$"UPDATE {request.DbCode}SILEDG SET ALLOCATION=@ALLOCATION,ALLOC_REF=@ALLOC_REF,ALLOC_DATE=@ALLOC_DATE,ALLOC_PERIOD=@ALLOC_PERIOD WHERE ACC_CODE = @ACCOUNT_CODE AND REFERENCE = @REFERENCE AND ALLOCATION = ''";
            var paramUpdate = new
            {
                ALLOCATION = "A",
                ALLOC_REF = maxAllocReference,
                ALLOC_DATE = DateTime.Today.ToString("MM/dd/yyyy"),
                ALLOC_PERIOD = request.ALLOC_PERIOD_17,
                ACCOUNT_CODE = request.ACC_CODE,
                REFERENCE = request.REFERENCE_9
            };
            rowAffected += await dbConnection.ExecuteAsync(sql, paramUpdate, transaction);
        }

        if (rowAffected is 2 or 3)
            transaction.Commit();
        else
            transaction.Rollback();
        return rowAffected;
    }
}