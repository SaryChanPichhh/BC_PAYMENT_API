using BC.PAYMENT.CORE.Contracts.Response.Delivery;

namespace BC.PAYMENT.APPLICATION.Interfaces.General
{
    public interface IDeliveryRepository
    {
        Task<List<Delivery>> GetDelivery(string dbCode);
        Task<List<DeliveryResponse>> GetDeliveryByPermissionAsync(string dbCode);
        Task<Delivery.DeliveryImage> GetDeliveryImage(string deliveryId);
        Task<bool> CreateDelivery(Delivery delivery);
        Task<bool> UpdateDelivery(Delivery delivery);
        Task<bool> DeleteDelivery(string deliveryId);
    }
}
