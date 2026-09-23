namespace BC.PAYMENT.CORE.Contracts.Response.Market;

public class MarketResponse
{
    public string MarketId { get; set; }
    public string MarketName { get; set; }
    public string MarketNameKhmer { get; set; }
    public string AreaId { get; set; }
    public string AreaName { get; set; }
    public string District { get; set; }
    public string Province { get; set; }
    public bool Status { get; set; }
    public string Image { get; set; }
    public string Other { get; set; }
}