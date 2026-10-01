namespace BC.PAYMENT.CORE.DTO.Preset.ExchangeItemAnalysis;

public class DailySaleReportValueDto
{
    public DateTime Date { get; set; }
    public Dictionary<string, string> Branches { get; set; }
}