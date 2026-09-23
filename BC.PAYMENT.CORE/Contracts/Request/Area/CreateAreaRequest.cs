using System;

namespace BC.PAYMENT.CORE.Contracts.Request.Area
{
    public class CreateAreaRequest
    {
        public string? Id { get; set; }
        public string? AreaName { get; set; }
        public string? AreaNameKhmer { get; set; }
        public string? Other { get; set; }
        public bool? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
    }
}
