namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Prepare.Account;

public class AccountReceivablePresetRepository : IAccountReceivablePresetRepository
{
    private readonly ISqlDataAccess _sqlDataAccess;

    public AccountReceivablePresetRepository(ISqlDataAccess sqlDataAccess)
    {
        _sqlDataAccess = sqlDataAccess;
    }

    public async Task<int> AddNewAsync(AccountReceivablePresetModel accountReceivable)
    {
        const string sql =
            @"INSERT INTO BCDATA(DB_CODE,DATA_CODE,DATA_NAME,DATA_DESC,DATA_TYPE,FIELD_1,FIELD_2,FIELD_3,FIELD_4,FIELD_5,FIELD_6,FIELD_7,FIELD_8,FIELD_9,CREATED_DATE,CREATED_BY)
            VALUES(@DB_CODE,@DATA_CODE,@DATA_NAME,@DATA_DESC,@DATA_TYPE,@FIELD_1,@FIELD_2,@FIELD_3,@FIELD_4,@FIELD_5,@FIELD_6,@FIELD_7,@FIELD_8,@FIELD_9,@CREATED_DATE,@CREATED_BY)";
        var param = new
        {
            DB_CODE = accountReceivable.DbCode,
            DATA_NAME = accountReceivable.CreditDebitType!,
            DATA_CODE = accountReceivable.AccountCode,
            DATA_DESC = accountReceivable.Description,
            DATA_TYPE = accountReceivable.BcDatatype, //"ACCOUNT RECEIVABLE",
            FIELD_1 = accountReceivable.Field1,
            FIELD_2 = accountReceivable.Field2,
            FIELD_3 = accountReceivable.Field3,
            FIELD_4 = accountReceivable.Field4,
            FIELD_5 = accountReceivable.Field5,
            FIELD_6 = accountReceivable.Field6,
            FIELD_7 = accountReceivable.Field7,
            FIELD_8 = accountReceivable.Field8,
            FIELD_9 = accountReceivable.Field9,
            CREATED_DATE = accountReceivable.CreatedDate,
            CREATED_BY = accountReceivable.CreatedBy
        };
        var rowAffected = await _sqlDataAccess.ExecuteAsync(sql, param);
        return rowAffected;
    }

    public Task<int> UpdateAsync(AccountReceivablePresetModel model)
    {
        throw new NotImplementedException();
    }

    public async Task<List<AccountReceivablePresetModel>> GetAsync(string dbCode)
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
                FROM BCDATA WHERE DB_CODE = @DB_CODE AND DATA_TYPE = @DATA_TYPE";
        var param = new { DB_CODE = dbCode, DATA_TYPE = "ACCOUNT RECEIVABLE" };
        var results = await _sqlDataAccess.LoadData<AccountReceivablePresetModel, dynamic>(sql, param);
        return results.ToList();
    }

    public Task<int> DeleteAsync(string code)
    {
        throw new NotImplementedException();
    }

    public async Task<int> DeleteAccountReceivableAsync(AccountReceivablePresetModel model)
    {
        var sql =
            $@"DELETE FROM BCDATA WHERE DATA_CODE = @DATA_CODE AND DATA_NAME = @DATA_NAME AND DATA_TYPE=@DATA_TYPE AND DB_CODE = @DB_CODE";
        var param = new
        {
            DATA_CODE = model.AccountCode,
            DATA_NAME = model.CreditDebitType,
            DB_CODE = model.DbCode,
            DATA_TYPE = "ACCOUNT RECEIVABLE" // ✅ Fix: was missing, caused SQL bind error
        };
        var affectedRow = await _sqlDataAccess.ExecuteAsync(sql, param);
        return affectedRow;
    }
}