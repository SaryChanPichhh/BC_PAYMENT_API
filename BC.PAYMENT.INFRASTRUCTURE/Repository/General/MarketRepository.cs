using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.Entities.General;
using BC.PAYMENT.INFRASTRUCTURE.DBAccess;
using Microsoft.Extensions.Configuration;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.General
{
    public class MarketRepository:IMarketRepository
    {
        private readonly ISqlDataAccess _sqlDataAccess;
        private readonly IConfiguration _settings;

        public MarketRepository(ISqlDataAccess sqlDataAccess, IConfiguration settings)
        {
            _sqlDataAccess = sqlDataAccess;
            _settings = settings;
        }
        public async Task<List<Market>> GetMarket(string dbCode)
        {
            var marketDictionary = new Dictionary<string, Market>();
            var sql = @"SELECT 
		                    MARKET_ID MarketId, 
		                    MARKET_NAME MarketName, 
		                    MARKET_KHMER_NAME MarketKhmerName,
		                    R.AREA_ID AreaId,
		                    R.AREA_NAME_KHMER AreaNameKhmer,
		                    D.D_NAME DistrictId,
		                    P.PRO_NAME ProvinceId,
		                    --M.IMAGE Image,
		                    M.STATUS Status,
		                    M.OTHER,
		                    P.PROID,
		                    D.D_ID 
                        FROM TB_BCMARKET M 
	                    LEFT JOIN TB_AREAS R ON R.AREA_ID = M.AREA_ID 
	                    INNER JOIN DISTRICT D ON D.D_ID = M.DISTRICT 
	                    INNER JOIN PROVINCE P ON P.PROID = M.CITY
                        WHERE M.DB_CODE = @DB_CODE";

            var param = new
            {
                DB_CODE = dbCode,
            };
            var result = await _sqlDataAccess.LoadData<Market, dynamic>(sql, param);
           

            return result.ToList();
        }

        public Task<Market.MarketImage> GetDeliveryImage(string deliveryId)
        {
            throw new NotImplementedException();
        }
    }
}
