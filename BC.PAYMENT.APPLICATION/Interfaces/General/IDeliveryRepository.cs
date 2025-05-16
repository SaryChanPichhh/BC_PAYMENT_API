using BC.PAYMENT.CORE.Entities.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BC.PAYMENT.APPLICATION.Interfaces.General
{
    public interface IDeliveryRepository
    {
        Task<List<Delivery>> GetDelivery(string dbCode);
        Task<Delivery.DeliveryImage> GetDeliveryImage(string deliveryId);
    }
}
