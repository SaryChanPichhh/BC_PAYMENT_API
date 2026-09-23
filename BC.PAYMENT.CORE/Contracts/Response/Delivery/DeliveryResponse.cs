namespace BC.PAYMENT.CORE.Contracts.Response.Delivery;

public class DeliveryResponse
{
    public string? DeliveryId { get; set; }
    public string? DbCode { get; set; }
    public string? DeliveryName { get; set; }
    public string? DeliveryNameKhmer { get; set; }
    public string? Others { get; set; }
    public string? Status { get; set; }
    public string? ImagePath { get; set; }
}