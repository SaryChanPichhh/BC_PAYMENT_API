namespace BC.PAYMENT.CORE.Contracts.Response.Market;

public class MarketResponse
{
    public int MarketId { get; set; }
    public string MarketName { get; set; }
    public string MarketNameKhmer { get; set; }
    public int AreaId { get; set; }
    public string AreaName { get; set; }
    public string DistrictName { get; set; }
    public string ProvinceName { get; set; }
    public bool Status { get; set; }
    public string Image { get; set; }
    public string Other { get; set; }
}