namespace BC.PAYMENT.APPLICATION.Interfaces.Accounting;

public interface IAnalysisAccountRepository
{
    Task<Dictionary<string, List<AnalysisCode>>> GetAnalysisByDetailDictionary(string dbCode);
    Task<List<AnalysisCode>> GetAnalysisByDetail(AnalysisCodeCreateRequest analysisCodeCreateRequest);
    Task<List<AnalysisCode>> GetAnalysisByRange(AnalysisCodeCreateRequest analysisCodeCreateRequest);
    Task<List<AccountCode>> GetAccountCode(string dbCode);
    Task<List<AccountCode>> GetAccountCode(string dbCode, int offset, int pageSize);
    Task<List<AnalysisCodeType>> GetAnalysisType(string dbCode);
}