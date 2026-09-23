namespace BC.PAYMENT.CORE.Entities.General
{
    public class Delivery
    {
        public string? DeliveryId { get; set; }
        public string? DbCode { get; set; }
        public string? DeliveryName { get; set; }
        public string? DeliveryNameKhmer { get; set; }
        public string? Others { get; set; }
        public byte[]? Image { get; set; }
        public string? ImagePath { get; set; } // Kept for API response URL
        public string? Status { get; set; }
        public string? UserCreate { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? UserUpdate { get; set; }
        public DateTime? UpdateDate { get; set; }

        public class DeliveryImage
        {
            public byte[]? Image { get; set; }  
        }
    }
}
