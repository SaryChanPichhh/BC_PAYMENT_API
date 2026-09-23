namespace BC.PAYMENT.CORE.Contracts.Response.Area
{
    public class AreaResponse
    {
        public string? AreaId { get; set; }
        public string? AreaName { get; init; }
        public string? AreaNameKhmer { get; init; }
        public string? DeliveryNameKhmer { get; set; }
        public string? DeliveryName { get; set; }
        public string? Other { get; set; }
        public bool Status { get; set; }
    }
}
