using System.Drawing;

namespace BC.PAYMENT.CORE.Contracts.Response.Customer;

public class CustomerResponse
{
    public string CustomerCode { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerNameKhmer { get; set; } = string.Empty;
    public string Tel { get; set; } = string.Empty;
    public string Store { get; set; } = string.Empty;
    public string Area { get; set; } = string.Empty;
    public string AreaDesc { get; set; } = string.Empty;
    public string AreaDescKh { get; set; } = string.Empty;
    public string Market { get; set; } = string.Empty;
    public string MarketDesc { get; set; } = string.Empty;
    public string MarketDescKh { get; set; } = string.Empty;
    public string AreaOS { get; set; } = string.Empty;
    public string AreaDescOS { get; set; } = string.Empty;
    public string AreaDescKhOS { get; set; } = string.Empty;
    public string MarketOS { get; set; } = string.Empty;
    public string MarketDescOS { get; set; } = string.Empty;
    public string MarketDescKhOS { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsCorrectArea { get; set; }
    public bool IsCorrectMarket { get; set; }
}