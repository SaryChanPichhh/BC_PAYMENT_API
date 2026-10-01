using System;

namespace BC.PAYMENT.CORE.Contracts.Request.Area;

public class UpdateAreaRequest
{
    public string? Id { get; set; }
    public string? AreaName { get; set; }
    public string? AreaNameKhmer { get; set; }
    public string? Other { get; set; }
    public bool? Status { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}