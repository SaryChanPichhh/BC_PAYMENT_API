namespace BC.PAYMENT.INFRASTRUCTURE.Repository.General;

public class GeneralRepository(ISqlDataAccess sqlDataAccess) : IGeneralRepository
{
    public async Task<List<string>> GetSaleTypes(string dbCode)
    {
        return (await sqlDataAccess.LoadData<string,dynamic>(GeneralQueries.GetSaleTypes,new {DB_CODE = dbCode})).ToList();
    }

    public async Task<List<BC.PAYMENT.CORE.Contracts.Response.General.AccountCodeResponse>> LoadAccountCode(string dbCode)
    {
        return (await sqlDataAccess.LoadData<BC.PAYMENT.CORE.Contracts.Response.General.AccountCodeResponse, dynamic>(GeneralQueries.LoadAccountCode, new { DB_CODE = dbCode })).ToList();
    }

    public async Task<List<BC.PAYMENT.CORE.Contracts.Response.General.AnalysisRangeDetailResponse>> LoadAnalysisByRangeDetails(string dbCode, string type)
    {
        return (await sqlDataAccess.LoadData<BC.PAYMENT.CORE.Contracts.Response.General.AnalysisRangeDetailResponse, dynamic>(GeneralQueries.LoadAnalysisByRangeDetails, new { DB_CODE = dbCode, ANAM_CODE = type })).ToList();
    }

    public async Task<List<BC.PAYMENT.CORE.Contracts.Response.General.AnalysisAllDetailResponse>> LoadAnalysisByAllDetail(string dbCode, string type)
    {
        return (await sqlDataAccess.LoadData<BC.PAYMENT.CORE.Contracts.Response.General.AnalysisAllDetailResponse, dynamic>(GeneralQueries.LoadAnalysisByAllDetail, new { DB_CODE = dbCode, ANAM_CODE = type })).ToList();
    }

    public async Task<List<BC.PAYMENT.CORE.Contracts.Response.General.AnalysisTypeResponse>> LoadAnalysisType(string dbCode)
    {
        return (await sqlDataAccess.LoadData<BC.PAYMENT.CORE.Contracts.Response.General.AnalysisTypeResponse, dynamic>(GeneralQueries.LoadAnalysisType, new { DB_CODE = dbCode })).ToList();
    }

    public async Task<string> GetPeriod(string dbCode)
    {
        return await sqlDataAccess.LoadSingleData<string, dynamic>(GeneralQueries.GetPeriod, new { DB_CODE = dbCode });
    }
}