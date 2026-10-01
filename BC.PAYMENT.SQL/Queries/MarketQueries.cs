namespace BC.PAYMENT.SQL.Queries;

public static class MarketQueries
{
    public static string GenerateMarketId => "SELECT dbo.GenerateID_Market()";

    public static string GetMarketByMarketId => @"SELECT 
		                    MARKET_ID MarketId, 
		                    MARKET_NAME MarketName, 
		                    MARKET_KHMER_NAME MarketKhmerName,
		                    R.AREA_ID AreaId,
		                    R.AREA_NAME_KHMER AreaName,
		                    D.D_NAME District,
		                    P.PRO_NAME Province,
		                    M.IMAGE Image,
		                    M.STATUS Status,
		                    M.OTHER,
		                    P.PROID,
		                    D.D_ID 
                        FROM TB_BCMARKET M 
	                    LEFT JOIN TB_AREAS R ON R.AREA_ID = M.AREA_ID 
	                    INNER JOIN DISTRICT D ON D.D_ID = M.DISTRICT 
	                    INNER JOIN PROVINCE P ON P.PROID = M.CITY
                        WHERE M.DB_CODE = @DB_CODE AND MARKET_ID = @MARKET_ID";

    public static string GetMarketByAreaId => @"SELECT 
		                    MARKET_ID MarketId, 
		                    MARKET_NAME MarketName, 
		                    MARKET_KHMER_NAME MarketKhmerName,
		                    R.AREA_ID AreaId,
		                    R.AREA_NAME_KHMER AreaName,
		                    D.D_NAME District,
		                    P.PRO_NAME Province,
		                    M.IMAGE Image,
		                    M.STATUS Status,
		                    M.OTHER,
		                    P.PROID,
		                    D.D_ID 
                        FROM TB_BCMARKET M 
	                    LEFT JOIN TB_AREAS R ON R.AREA_ID = M.AREA_ID 
	                    INNER JOIN DISTRICT D ON D.D_ID = M.DISTRICT 
	                    INNER JOIN PROVINCE P ON P.PROID = M.CITY
                        WHERE M.DB_CODE = @DB_CODE AND M.AREA_ID = @AREA_ID";

    public static string GetMarket => @"SELECT 
		                    MARKET_ID MarketId, 
		                    MARKET_NAME MarketName, 
		                    MARKET_KHMER_NAME MarketKhmerName,
		                    R.AREA_ID AreaId,
		                    R.AREA_NAME_KHMER AreaName,
		                    D.D_NAME District,
		                    P.PRO_NAME Province,
		                    M.STATUS Status,
		                    M.OTHER,
		                    P.PROID,
		                    D.D_ID 
                        FROM TB_BCMARKET M 
	                    LEFT JOIN TB_AREAS R ON R.AREA_ID = M.AREA_ID 
	                    INNER JOIN DISTRICT D ON D.D_ID = M.DISTRICT 
	                    INNER JOIN PROVINCE P ON P.PROID = M.CITY
                        WHERE M.DB_CODE = @DB_CODE";

    public static string GetMarketByDbCode =>
        @"SELECT MARKET_ID MarketId,MARKET_NAME MarketName,MARKET_KHMER_NAME MarketNameKhmer
            ,R.AREA_ID AreaId,R.AREA_NAME_KHMER AreaName,D.D_NAME DistrictName,P.PRO_NAME ProvinceName,
             M.STATUS Status,M.OTHER Other,P.PROID,D.D_ID 
                        FROM TB_BCMARKET M LEFT JOIN TB_AREAS R ON R.AREA_ID = M.AREA_ID INNER JOIN DISTRICT D ON D.D_ID = M.DISTRICT INNER JOIN PROVINCE P ON P.PROID = M.CITY
                         WHERE M.DB_CODE = @DB_CODE AND R.DB_CODE = @DB_CODE;";

    public static string AddNewMarket =>
        @"INSERT INTO TB_BCMARKET (MARKET_ID, DB_CODE, MARKET_NAME, MARKET_KHMER_NAME, AREA_ID, CITY, DISTRICT, OTHER, IMAGE, STATUS, USER_CREATE, CREATE_DATE, USER_UPDATE, UPDATE_DATE, MAP, ANAD_CODE)
                VALUES (@MARKET_ID, @DB_CODE, @MARKET_NAME, @MARKET_KHMER_NAME, @AREA_ID, @CITY, @DISTRICT, @OTHER, @IMAGE, @STATUS, @USER_CREATE, @CREATE_DATE, N'', NULL, N'', N'');";

    public static string UpdateMarket => @"UPDATE TB_BCMARKET SET 
                        MARKET_NAME = @MARKET_NAME, 
                        MARKET_KHMER_NAME = @MARKET_KHMER_NAME, 
                        AREA_ID = @AREA_ID, 
                        DISTRICT = @DISTRICT, 
                        CITY = @CITY, 
                        STATUS = @STATUS, 
                        OTHER = @OTHER, 
                        IMAGE = @IMAGE,
                        USER_UPDATE = @USER_UPDATE,
                        UPDATE_DATE = @UPDATE_DATE
                        WHERE MARKET_ID = @MARKET_ID AND DB_CODE = @DB_CODE";

    public static string DeleteMarket => @"DELETE FROM TB_BCMARKET WHERE MARKET_ID = @MARKET_ID";

    public static string GetMarketImage => @"SELECT IMAGE FROM TB_BCMARKET WHERE MARKET_ID = @MARKET_ID";

    public static string LoadMarketBySaleTypes(string dbCode, string saleTypeCondition)
    {
        return $@"SELECT DISTINCT ANAD_CODE MarketName,
                                CASE 
                                    WHEN TRIM(ANAD_COM) COLLATE Latin1_General_BIN = N'' THEN ANAD_CODE 
                                    ELSE ANAD_COM
                                END MarketNameKhmer
                              FROM {dbCode}SISOHDR HRSALE
	                          INNER JOIN (SELECT ANAD_CODE, ANAD_COM FROM SIANALD WHERE DB_CODE =  @DB_CODE AND ANAM_CODE = 'M9') AS ANAN ON HRSALE.ANAL_M9 = ANAN.ANAD_CODE
                              WHERE HRSALE.CUST_CODE NOT LIKE 'PV%' AND HRSALE.INV_PRD BETWEEN @FROM_MOV AND @TO_MOV {saleTypeCondition}
                              ORDER BY ANAD_CODE;";
    }
}