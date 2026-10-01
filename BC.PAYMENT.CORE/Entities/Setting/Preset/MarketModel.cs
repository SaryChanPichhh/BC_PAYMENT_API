namespace BC.PAYMENT.CORE.Entities.Setting.Preset;

public class MarketModel : BasedEntity
{
    public string DbCode { get; set; }
    public string MarketId { get; set; }
    public string MarketName { get; set; }
    public string MarketNameKhmer { get; set; }
    public string AreaId { get; set; }
    public string AreaName { get; set; }
    public int DistrictId { get; set; }
    public string DistrictName { get; set; }
    public int ProvinceId { get; set; }
    public string ProvinceName { get; set; }
    public bool Status { get; set; }
    public byte[] Image { get; set; }
    public string Other { get; set; }
}