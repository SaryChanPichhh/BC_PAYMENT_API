namespace BC.PAYMENT.APPLICATION.Interfaces.General
{
    public interface IMarketRepository
    {
        Task<List<MarketResponse>> GetMarket(string dbCode);
        Task<Market.MarketImage> GetDeliveryImage(string deliveryId);
        Task<List<Market>> LoadMarketBySaleTypesAsync(string dbCode, List<string> saleTypes,int fromMov,int toMov);
        Task<List<MarketResponse>> GetMarketByDbCodeAsync(string dbCode);
        Task<MarketResponse> GetMarketByMarketIdAsync(string dbCode,string marketId);
        Task<List<MarketResponse>> GetMarketByAreaIdAsync(string dbCode, string areaId);
        Task<int> AddNewMarketAsync(MarketModel model);
        Task<int> UpdateMarketAsync(MarketModel model);
        Task<int> DeleteMarketAsync(string marketId);
        Task<byte[]> GetMarketImageAsync(string marketId);
        Task<string> GenerateMarketIdAsync();
    }
}
