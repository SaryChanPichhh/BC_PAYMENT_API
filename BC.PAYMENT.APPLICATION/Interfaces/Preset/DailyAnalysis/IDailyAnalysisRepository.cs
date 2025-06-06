

using BC.PAYMENT.CORE.Entities.Preset.DailySaleAnalysis;

namespace BC.PAYMENT.APPLICATION.Interfaces.Preset.DailyAnalysis
{
    public interface IDailyAnalysisRepository
    {
        Task<List<DailySaleAnalysisModel>> GetDailySaleAnalysisAsync(string fromPrd, string endPrd,
            string itemCode = null);

        Task<List<DailySaleAnalysisModel>> GetDailySaleAnalysisByItemCodeAndDateAsync(DateTime fromDate, DateTime endDate,
            List<string> itemCode);

        Task<List<DailySaleAnalysisModel>> GetDailySaleAnalysisByItemCodeAndPeriodAsync(string fromPrd, string endPrd,
            List<string> itemCode);
    }
}
