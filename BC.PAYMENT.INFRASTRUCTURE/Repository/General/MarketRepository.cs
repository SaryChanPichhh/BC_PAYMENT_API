namespace BC.PAYMENT.INFRASTRUCTURE.Repository.General
{
    public class MarketRepository(ISqlDataAccess sqlDataAccess, IConfiguration settings) : IMarketRepository
    {
        private readonly IConfiguration _settings = settings;

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
            var result = await sqlDataAccess.LoadData<Market, dynamic>(sql, param);
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
            var execute = await sqlDataAccess.LoadData<Market, dynamic>(sql, param);
            return execute.ToList();
        }

        public async Task<List<MarketResponse>> GetMarketByDbCodeAsync(string dbCode)
        {
            var sql = $@"SELECT MARKET_ID MarketId,MARKET_NAME MarketName,MARKET_KHMER_NAME MarketNameKhmer
            ,R.AREA_ID AreaId,R.AREA_NAME_KHMER AreaName,D.D_NAME DistrictName,P.PRO_NAME ProvinceName,
             M.STATUS Status,M.OTHER Other,P.PROID,D.D_ID 
                        FROM TB_BCMARKET M LEFT JOIN TB_AREAS R ON R.AREA_ID = M.AREA_ID INNER JOIN DISTRICT D ON D.D_ID = M.DISTRICT INNER JOIN PROVINCE P ON P.PROID = M.CITY
                         WHERE M.DB_CODE = @DB_CODE AND R.DB_CODE = @DB_CODE;";
            var execute = await sqlDataAccess.LoadData<MarketResponse, dynamic>(sql,new {DB_CODE = dbCode}); 
            return execute.ToList();
        }
        public async Task<int> AddNewMarketAsync(MarketModel model)
        {
            const string sql = @"INSERT INTO TB_BCMARKET (MARKET_ID, DB_CODE, MARKET_NAME, MARKET_KHMER_NAME, AREA_ID, CITY, DISTRICT, OTHER, IMAGE, STATUS, USER_CREATE, CREATE_DATE, USER_UPDATE, UPDATE_DATE, MAP, ANAD_CODE)
                VALUES (@MARKET_ID, @DB_CODE, @MARKET_NAME, @MARKET_KHMER_NAME, @AREA_ID, @CITY, @DISTRICT, @OTHER, @IMAGE, @STATUS, @USER_CREATE, @CREATE_DATE, N'', NULL, @MAP,@ANAD_CODE);";
            var param = new
            {
                MARKET_ID = model.MarketId,
                DB_CODE = model.DbCode,
                MARKET_NAME = model.MarketName,
                MARKET_KHMER_NAME = model.MarketNameKhmer,
                AREA_ID = model.AreaId,
                CITY = model.ProvinceId,
                DISTRICT = model.DistrictId, 
                STATUS = model.Status ,
                OTHER = model.Other,
                IMAGE = model.Image,
                USER_CREATE = model.CreatedBy,
                CREATE_DATE = model.CreatedAt,
                MAP = model.Map,
                ANAD_CODE = model.AnadCode
            };
            return await sqlDataAccess.ExecuteAsync(sql, param);
        }
        public async Task<int> UpdateMarketAsync(MarketModel model)
        {
            var sql = @"UPDATE TB_BCMARKET SET 
                        MARKET_NAME = @MARKET_NAME, 
                        MARKET_KHMER_NAME = @MARKET_KHMER_NAME, 
                        AREA_ID = @AREA_ID, 
                        DISTRICT = @DISTRICT, 
                        CITY = @CITY, 
                        STATUS = @STATUS, 
                        OTHER = @OTHER, 
                        IMAGE = @IMAGE,
                        USER_UPDATE = @USER_UPDATE,
                        UPDATE_DATE = @UPDATE_DATE,
                        MAP = @MAP,
                        ANAD_CODE = @ANAD_CODE
                        WHERE MARKET_ID = @MARKET_ID AND DB_CODE = @DB_CODE";
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
                MAP = model.Map,
                ANAD_CODE = model.AnadCode,
                MARKET_ID = model.MarketId,
                DB_CODE = model.DbCode
            };
            return await sqlDataAccess.ExecuteAsync(sql, param);
        }
        public async Task<int> DeleteMarketAsync(string marketId)
        {
            var sql = @"DELETE FROM TB_BCMARKET WHERE MARKET_ID = @MARKET_ID";
            return await sqlDataAccess.ExecuteAsync(sql, new { MARKET_ID = marketId });
        }

        public Task<byte[]> GetMarketImageAsync(string marketId)
        {
            var sql = $@"SELECT IMAGE FROM TB_BCMARKET WHERE MARKET_ID = @MARKET_ID ";
            return sqlDataAccess.LoadSingleData<byte[],dynamic>(sql, new { MARKET_ID = marketId });
        }
    }
}
