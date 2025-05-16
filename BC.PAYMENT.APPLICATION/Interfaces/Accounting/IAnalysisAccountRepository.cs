using BC.PAYMENT.CORE.DTO.Accounting;
using BC.PAYMENT.CORE.Entities;
using BC.PAYMENT.CORE.Entities.Accounting;

namespace BC.PAYMENT.APPLICATION.Interfaces.Accounting
{
    public interface IAnalysisAccountRepository
    {
        Task<Dictionary<string, List<AnalysisCode>>> GetAnalysisByDetailDictionary(string dbCode);
        Task<List<AnalysisCode>> GetAnalysisByDetail(AnalysisCodeDTO analysisCodeDto);
        Task<List<AnalysisCode>> GetAnalysisByRange(AnalysisCodeDTO analysisCodeDto);
        Task<List<AccountCode>> GetAccountCode(string dbCode);
        Task<List<AccountCode>> GetAccountCode(string dbCode, int offset, int pageSize);
        Task<List<AnalysisCodeType>> GetAnalysisType(string dbCode);
    }
}
