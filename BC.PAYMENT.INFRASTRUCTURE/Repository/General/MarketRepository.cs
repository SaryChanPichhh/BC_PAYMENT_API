namespace BC.PAYMENT.INFRASTRUCTURE.Repository.General
{
    public class MarketRepository(ISqlDataAccess sqlDataAccess, IConfiguration settings) : IMarketRepository
    {
        private readonly IConfiguration _settings = settings;

        public async Task<List<MarketResponse>> GetMarket(string dbCode)
        {
            var param = new
            {
                DB_CODE = dbCode,
            };
            var result = await sqlDataAccess.LoadData<MarketResponse, dynamic>(MarketQueries.GetMarket, param);
            return result.ToList();
        }
        public Task<Market.MarketImage> GetDeliveryImage(string deliveryId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Market>> LoadMarketBySaleTypesAsync(string dbCode, List<string> saleTypes, int fromMov, int toMov)
        {
            var saleTypeCondition = string.Empty;
            if (saleTypes.Any())
            {
                var saleType = string.Join(",", saleTypes.Select(s => $@"'{s}'"));
                saleTypeCondition = $@"AND HRSALE.TRANS_CODE IN ({saleType})";
            }

            var sql = MarketQueries.LoadMarketBySaleTypes(dbCode, saleTypeCondition); 
            var param = new
            {
                DB_CODE = dbCode,
                FROM_MOV = fromMov,
                TO_MOV = toMov,
            };
            var execute = await sqlDataAccess.LoadData<Market, dynamic>(sql, param);
            return execute.ToList();
        }

        public async Task<List<MarketResponse>> GetMarketByDbCodeAsync(string dbCode)
        {
            var execute = await sqlDataAccess.LoadData<MarketResponse, dynamic>(MarketQueries.GetMarketByDbCode,new {DB_CODE = dbCode}); 
            return execute.ToList();
        }

        public async Task<MarketResponse> GetMarketByMarketIdAsync(string dbCode, string marketId)
        {
            var param = new
            {
                DB_CODE = dbCode,
                MARKET_ID = marketId
            };
            var result = await sqlDataAccess.LoadSingleData<MarketResponse, dynamic>(MarketQueries.GetMarketByMarketId, param);
            return result;
        }

        public async Task<List<MarketResponse>> GetMarketByAreaIdAsync(string dbCode, string areaId)
        {
            var param = new
            {
                DB_CODE = dbCode,
                AREA_ID = areaId
            };
            var result = await sqlDataAccess.LoadData<MarketResponse, dynamic>(MarketQueries.GetMarketByAreaId, param);
            return result.ToList();
        }
        public async Task<int> AddNewMarketAsync(MarketModel model)
        {
            var param = new
            {
                MARKET_ID = model.MarketId,
                DB_CODE = model.DbCode,
                MARKET_NAME = model.MarketName,
                MARKET_KHMER_NAME = model.MarketNameKhmer,
                AREA_ID = model.AreaId,
                CITY = model.ProvinceId,
                DISTRICT = model.DistrictId.ToString(), 
                STATUS = model.Status,
                OTHER = model.Other,
                USER_CREATE = model.CreatedBy,
                CREATE_DATE = model.CreatedAt,IMAGE = model.Image
            };
            var json = JsonSerializer.Serialize(param,new JsonSerializerOptions { WriteIndented = true });
            Console.WriteLine(json);
            return await sqlDataAccess.ExecuteAsync(MarketQueries.AddNewMarket, param);
        }
        public async Task<int> UpdateMarketAsync(MarketModel model)
        {
            var param = new
            {
                MARKET_NAME = model.MarketName,
                MARKET_KHMER_NAME = model.MarketNameKhmer,
                AREA_ID = model.AreaId,
                DISTRICT = model.DistrictId,
                CITY = model.ProvinceId,
                STATUS = model.Status,
                OTHER = model.Other,
                IMAGE = model.Image,
                USER_UPDATE = model.UpdatedBy,
                UPDATE_DATE = model.UpdatedAt,
                MARKET_ID = model.MarketId,
                DB_CODE = model.DbCode
            };
            return await sqlDataAccess.ExecuteAsync(MarketQueries.UpdateMarket, param);
        }
        public async Task<int> DeleteMarketAsync(string marketId)
        {
            return await sqlDataAccess.ExecuteAsync(MarketQueries.DeleteMarket, new { MARKET_ID = marketId });
        }

        
        public Task<byte[]> GetMarketImageAsync(string marketId)
        {
            return sqlDataAccess.LoadSingleData<byte[],dynamic>(MarketQueries.GetMarketImage, new { MARKET_ID = marketId });
        }

        public async Task<string> GenerateMarketIdAsync()
        {
            return await sqlDataAccess.LoadSingleData<string, dynamic>(MarketQueries.GenerateMarketId,new {});
        }
    }
}
