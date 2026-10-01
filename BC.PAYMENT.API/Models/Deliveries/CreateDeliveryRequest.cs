using Microsoft.AspNetCore.Http;

namespace BC.PAYMENT.API.Models.Deliveries;

public class CreateDeliveryRequest
{
    public string? DeliveryId { get; set; }
    public string? DbCode { get; set; }
    public string? DeliveryName { get; set; }
    public string? DeliveryNameKhmer { get; set; }
    public string? Others { get; set; }
    public IFormFile? Image { get; set; }
    public string? Status { get; set; }
}