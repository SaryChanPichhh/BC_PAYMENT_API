using BC.PAYMENT.CORE.Contracts.Response.General;

namespace BC.PAYMENT.APPLICATION.Interfaces.General;

public interface IGeneralRepository
{
    Task<List<string>> GetSaleTypes(string dbCode);
    Task<List<AccountCodeResponse>> LoadAccountCode(string dbCode);
    Task<List<AnalysisRangeDetailResponse>> LoadAnalysisByRangeDetails(string dbCode, string type);
    Task<List<AnalysisAllDetailResponse>> LoadAnalysisByAllDetail(string dbCode, string type);
    Task<List<AnalysisTypeResponse>> LoadAnalysisType(string dbCode);
    Task<string> GetPeriod(string dbCode);
}