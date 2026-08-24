namespace BC.PAYMENT.APPLICATION.Interfaces.General
{
    public interface IDeliveryRepository
    {
        Task<List<Delivery>> GetDelivery(string dbCode);
        Task<Delivery.DeliveryImage> GetDeliveryImage(string deliveryId);
    }
}
