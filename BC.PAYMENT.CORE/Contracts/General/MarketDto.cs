namespace BC.PAYMENT.CORE.Contracts.General;

public class MarketDto
{
    public string MarketName { get; set; }
    public string MarketNameKhmer { get; set; }
}

public class MarketFilterDto
{
    public List<string> SaleTypes { get; set; }
    public int FromMov { get; set; }
    public int ToMov { get; set; }
}