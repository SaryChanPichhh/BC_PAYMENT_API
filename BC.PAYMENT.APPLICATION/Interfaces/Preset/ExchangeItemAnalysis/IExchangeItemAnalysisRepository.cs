namespace BC.PAYMENT.APPLICATION.Interfaces.Preset.ExchangeItemAnalysis;

public interface IExchangeItemAnalysisRepository
{
    Task<List<ExchangeItemAnalysisModel>> GetAllByPeriodAsync(string type, string isReceived, string dbCode,
        int fromPeriod,
        int toPeriod);

    Task<List<ExchangeItemAnalysisModel>> GetAllByDateAsync(string type, string isReceived, string dbCode,
        DateTime fromDate, DateTime toDate);
}