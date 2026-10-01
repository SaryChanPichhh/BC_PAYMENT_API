namespace BC.PAYMENT.CORE.DTO.Preset.ExchangeItemAnalysis;

public class ReportSalesPerYearRequestDto
{
    public string FromPeriod { get; set; }
    public string ToPeriod { get; set; }
    public string SaleType { get; set; }
}

public class SaleTypeDto
{
    public string FromPeriod { get; set; }
    public string ToPeriod { get; set; }
    public List<string> SaleType { get; set; }
}

public class ReportSaleBySaleType
{
    public SaleTypeDto SaleTypes { get; set; }
    public List<string> Markets { get; set; }
    public List<string> CustomerCode { get; set; }
}