namespace BC.PAYMENT.CORE.Entities.General
{
    public class Delivery
    {
        public string? DeliveryId { get; set; }
        public string? DeliveryName { get; set; }
        public string? ImagePath { get; set; }

        public class DeliveryImage
        {
            public byte[]? Image { get; set; }  
        }
        
        


    }
}
