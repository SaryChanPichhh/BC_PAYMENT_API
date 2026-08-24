namespace BC.PAYMENT.CORE.Contracts.Request.Market;

public class MarketCreateRequest
{
    public string MarketId { get; set; }
    public string MarketName { get; set; }
    public string MarketNameKhmer { get; set; }
    public int AreaId { get; set; }
    public int DistrictId { get; set; }
    public int ProvinceId { get; set; }
    public byte[] Image { get; set; }
    public string Other { get; set; }   
    public string Map { get; set; }
    public string AnadCode { get; set; }
}