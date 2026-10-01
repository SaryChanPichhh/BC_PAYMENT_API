namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Accounting;

public class AnalysisAccountRepository : IAnalysisAccountRepository
{
    private readonly ISqlDataAccess _sqlDataAccess;

    public AnalysisAccountRepository(ISqlDataAccess sqlDataAccess)
    {
        _sqlDataAccess = sqlDataAccess;
    }

    public async Task<Dictionary<string, List<AnalysisCode>>> GetAnalysisByDetailDictionary(string dbCode)

    {
        const string sql = @"
                SELECT ANAD_CODE AnadCode, ANAD_DESC AnadDesc, ANAD_COM AnadCom, ANAM_CODE AnamCode
                FROM SIANALD
                WHERE ANAM_CODE IN @ANAM_CODES AND DB_CODE = @DB_CODE";

        var anamCodes = new List<string> { "M0", "M1", "M2", "M3", "M4", "M5", "M6", "M7", "M8", "M9" };
        var param = new
        {
            ANAM_CODES = anamCodes,
            DB_CODE = dbCode
        };

        var result = await _sqlDataAccess.LoadData<AnalysisCode, dynamic>(sql, param);

        return result
            .GroupBy(x => x.AnamCode)
            .ToDictionary(
                g => g.Key, // Use AnamCode as the key
                g => g.ToList() // Use a list of all records for duplicate keys
            );
    }

    public async Task<List<AnalysisCode>> GetAnalysisByDetail(AnalysisCodeCreateRequest analysisCodeCreateRequest)
    {
        const string sql =
            @"SELECT ANAD_CODE AnadCode,ANAD_DESC AnadDesc,ANAD_COM AnadCom FROM SIANALD WHERE ANAM_CODE = @ANAM_CODE AND DB_CODE = @DB_CODE";
        var param = new
        {
            ANAM_CODE = analysisCodeCreateRequest.AnamCode,
            DB_CODE = analysisCodeCreateRequest.DbCode
        };
        var result = await _sqlDataAccess.LoadData<AnalysisCode, dynamic>(sql, param);
        return result.ToList();
    }

    public async Task<List<AnalysisCode>> GetAnalysisByRange(AnalysisCodeCreateRequest analysisCodeCreateRequest)
    {
        const string sql =
            @"SELECT ANAD_CODE AnadCode,ANAD_COM AnadCom FROM SIANALD WHERE ANAM_CODE = @ANAM_CODE AND DB_CODE = @DB_CODE";
        var param = new
        {
            ANAM_CODE = analysisCodeCreateRequest.AnamCode,
            DB_CODE = analysisCodeCreateRequest.DbCode
        };
        var result = await _sqlDataAccess.LoadData<AnalysisCode, dynamic>(sql, param);
        return result.ToList();
    }

    public async Task<List<AccountCode>> GetAccountCode(string dbCode)
    {
        const string sql =
            @"SELECT ACC_CODE AccCode FROM SIACCNT WHERE DB_CODE = @DB_CODE AND ACC_TYPE= 'D' AND ACC_BF = 'B' AND ACC_STAT = 'A'";
        var param = new
        {
            DB_CODE = dbCode
        };
        var result = await _sqlDataAccess.LoadData<AccountCode, dynamic>(sql, param);
        return result.ToList();
    }

    public async Task<List<AccountCode>> GetAccountCode(string dbCode, int offset, int pageSize)
    {
        const string sql = @"
                SELECT ACC_CODE AccCode, ACC_COM1 AccCom 
                FROM SIACCNT 
                WHERE DB_CODE = @DB_CODE 
                  AND ACC_TYPE= 'D' 
                  AND ACC_BF = 'B'
                  AND ACC_STAT = 'A'
                ORDER BY ACC_CODE
                OFFSET @OFF_SET ROWS
                FETCH NEXT @PAGE_SIZE ROWS ONLY";

        var param = new
        {
            DB_CODE = dbCode,
            OFF_SET = offset,
            PAGE_SIZE = pageSize
        };

        var result = await _sqlDataAccess.LoadData<AccountCode, dynamic>(sql, param);
        return result.ToList();
    }

    public async Task<List<AnalysisCodeType>> GetAnalysisType(string dbCode)
    {
        const string sql =
            @"SELECT ANAM_CODE AnadCode,ANAM_DESC AnadDesc FROM SIANALM WHERE DB_CODE = @DB_CODE
                                 AND ANAM_CODE LIKE 'T%' GROUP BY ANAM_CODE,ANAM_DESC";
        var param = new
        {
            DB_CODE = dbCode
        };
        var result = await _sqlDataAccess.LoadData<AnalysisCodeType, dynamic>(sql, param);
        return result.ToList();
    }
}