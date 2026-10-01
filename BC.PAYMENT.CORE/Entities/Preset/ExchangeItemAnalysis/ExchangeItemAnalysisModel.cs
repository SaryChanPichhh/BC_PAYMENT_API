namespace BC.PAYMENT.CORE.Entities.Preset.ExchangeItemAnalysis;

public class ExchangeItemAnalysisModel : Customer
{
    public string DbCode { get; set; }
    public string DbName { get; set; }
    public string AreaId { get; set; }
    public string AreaNameKhmer { get; set; }
    public string MarketNameKhmer { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public string Seller { get; set; }
    public string SellerCode { get; set; }
    public double Amount { get; set; }
    public string Reason { get; set; }
    public DateTime Date { get; set; }
}