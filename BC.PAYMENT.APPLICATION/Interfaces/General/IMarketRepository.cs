using BC.PAYMENT.CORE.Entities.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BC.PAYMENT.APPLICATION.Interfaces.General
{
    public interface IMarketRepository
    {
        Task<List<Market>> GetMarket(string dbCode);
        Task<Market.MarketImage> GetDeliveryImage(string deliveryId);
        Task<List<Market>> LoadMarketBySaleTypesAsync(string dbCode, List<string> saleTypes,int fromMov,int toMov);
    }
}
