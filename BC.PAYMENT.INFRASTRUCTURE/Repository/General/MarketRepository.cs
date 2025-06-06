using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
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

        public async Task<List<Market>> LoadMarketBySaleTypesAsync(string dbCode, List<string> saleTypes, int fromMov, int toMov)
        {
            var saleTypeCondition = string.Empty;
            if (saleTypes.Any())
            {
                var saleType = string.Join(",", saleTypes.Select(s => $@"'{s}'"));
                saleTypeCondition = $@"AND HRSALE.TRANS_CODE IN ({saleType})";
            }

            string sql =
                $@"SELECT DISTINCT ANAD_CODE MarketName,
                                CASE 
                                    WHEN TRIM(ANAD_COM) COLLATE Latin1_General_BIN = N'' THEN ANAD_CODE 
                                    ELSE ANAD_COM
                                END MarketNameKhmer
                              FROM {dbCode}SISOHDR HRSALE
	                          INNER JOIN (SELECT ANAD_CODE, ANAD_COM FROM SIANALD WHERE DB_CODE =  @DB_CODE AND ANAM_CODE = 'M9') AS ANAN ON HRSALE.ANAL_M9 = ANAN.ANAD_CODE
                              WHERE HRSALE.CUST_CODE NOT LIKE 'PV%' AND HRSALE.INV_PRD BETWEEN @FROM_MOV AND @TO_MOV {saleTypeCondition}
                              ORDER BY ANAD_CODE;"; 
            var param = new
            {
                DB_CODE = dbCode,
                FROM_MOV = fromMov,
                TO_MOV = toMov,
            };
            var execute = await _sqlDataAccess.LoadData<Market, dynamic>(sql, param);
            return execute.ToList();
        }
    }
}
