namespace BC.PAYMENT.CORE.Contracts.Request.Market;

public class MarketUpdateRequest : MarketCreateRequest
{
    public bool Status { get; set; }
}