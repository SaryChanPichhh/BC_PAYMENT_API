namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Preset.ExchangeItemAnalysis
{
    public class ExchangeItemAnalysisRepository : IExchangeItemAnalysisRepository
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public ExchangeItemAnalysisRepository(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<List<ExchangeItemAnalysisModel>> GetAllByDateAsync(string type, string isReceived, string dbCode, DateTime fromDate, DateTime toDate)
        {
            string statusCondition = isReceived=="None"? "" : $@"AND TBC.IS_RECEIVED = {isReceived}";
            string sql = string.Empty;

                sql = @$"SELECT Date,Seller,ItemName,MarketNameKhmer,Store,CustomerCode,CustomerName,Reason,AreaId,AreaNameKhmer,ItemCode,DbCode,DbName,SUM(Quantity)AMOUNT FROM 
					(SELECT CAST(TBC.CREATED_DATE AS DATE) Date,TBCD.ITEM_CODE ItemCode,QUANTITY Quantity,CUSTOMER.DbCode,CUSTOMER.DbName,
					CUSTOMER.CustomerCode,CUSTOMER.CustomerName,TBCD.DESCRIPION Reason, CUSTOMER.AreaId,CUSTOMER.AreaNameKhmer,
					CUSTOMER.MarketNameKhmer,CUSTOMER.Store,ITEM.ITEM_DESC ItemName ,
					USERS.USER_ID SellerCode,CONCAT(USERS.FIRST_NAME,' ',USERS.LAST_NAME ) Seller
                         FROM TB_BC_CHANGEINVOICE_DETAIL TBCD 
                         INNER JOIN TB_BC_CHANGEINVOICE TBC ON TBCD.CHANGE_INVOICE_ID  = TBC.ID
                         INNER JOIN (SELECT S.DB_CODE DbCode,DB.DB_NAME DbName, S.ADD_CODE CustomerCode,S.ADD_LINE_1KH CustomerName,M.MARKET_KHMER_NAME MarketNameKhmer,
						 S.STORE Store,A.AREA_NAME_KHMER AreaNameKhmer, A.AREA_ID AreaId, M.MARKET_ID MarketId,A.ANAL_M0,ADD_TEL [PhonNumber],
                         ADD_STAT
                         FROM SIADD S INNER JOIN TB_BCMARKET M ON M.MARKET_ID = S.MARKET_ID INNER JOIN TB_AREAS A ON A.AREA_ID = S.AREA_ID INNER JOIN SIDBINFO DB ON S.DB_CODE = DB.DB_CODE
                         WHERE S.DB_CODE = @DB_CODE AND M.DB_CODE = @DB_CODE AND A.DB_CODE = @DB_CODE AND S.ADD_TYPE = '0'
                         ) CUSTOMER ON CUSTOMER.CustomerCode = CUST_CODE
                         INNER JOIN SIITEMS ITEM ON ITEM.ITEM_CODE = TBCD.ITEM_CODE
                         INNER JOIN BCUSERS USERS ON USERS.USER_ID = TBC.USER_CODE
                         WHERE TYPE =@TYPE {statusCondition} AND CAST(TBC.CREATED_DATE AS DATE) BETWEEN @STARTDATE AND @ENDDATE
                       GROUP BY CREATED_DATE,TBCD.ITEM_CODE,QUANTITY,CUSTOMER.DbCode,CUSTOMER.DbName,CUSTOMER.CustomerCode,CUSTOMER.CustomerName,TBCD.DESCRIPION,
					   CUSTOMER.AreaId,CUSTOMER.AreaNameKhmer,CUSTOMER.MarketNameKhmer,CUSTOMER.Store,ITEM.ITEM_DESC,
					   USERS.USER_ID,USERS.FIRST_NAME,USERS.LAST_NAME
						  ) C GROUP BY C.ItemCode,C.DbCode,C.DbName,C.AreaId,C.CustomerCode,C.CustomerName,C.AreaNameKhmer,C.Reason
						  ,C.MarketNameKhmer,C.Store,C.ItemName,C.Seller,C.Date ORDER BY Date
						  ;";

                var param = new
            {
                DB_CODE = dbCode,
                TYPE = type,
                STARTDATE = fromDate,
                ENDDATE = toDate,
            };
            var execute = await _sqlDataAccess.LoadData<ExchangeItemAnalysisModel, dynamic>(sql, param);
            return execute.ToList();
        }
        
        public async Task<List<ExchangeItemAnalysisModel>> GetAllByPeriodAsync(string type, string isReceived, string dbCode, int fromPeriod,
            int toPeriod)
        {
            string statusCondition = isReceived=="None"? "" : $@"AND TBC.IS_RECEIVED = {isReceived}";
            string sql = string.Empty;

                sql = @$"SELECT Date,Seller,ItemName,MarketNameKhmer,Store,CustomerCode,CustomerName,Reason,AreaId,AreaNameKhmer,ItemCode,DbCode,DbName,SUM(Quantity)AMOUNT FROM 
					(SELECT CAST(TBC.CREATED_DATE AS DATE) Date,TBCD.ITEM_CODE ItemCode,QUANTITY Quantity,CUSTOMER.DbCode,CUSTOMER.DbName,
					CUSTOMER.CustomerCode,CUSTOMER.CustomerName,TBCD.DESCRIPION Reason, CUSTOMER.AreaId,CUSTOMER.AreaNameKhmer,
					CUSTOMER.MarketNameKhmer,CUSTOMER.Store,ITEM.ITEM_DESC ItemName ,
					USERS.USER_ID SellerCode,CONCAT(USERS.FIRST_NAME,' ',USERS.LAST_NAME ) Seller
                         FROM TB_BC_CHANGEINVOICE_DETAIL TBCD 
                         INNER JOIN TB_BC_CHANGEINVOICE TBC ON TBCD.CHANGE_INVOICE_ID  = TBC.ID
                         INNER JOIN (SELECT S.DB_CODE DbCode,DB.DB_NAME DbName, S.ADD_CODE CustomerCode,S.ADD_LINE_1KH CustomerName,M.MARKET_KHMER_NAME MarketNameKhmer,
						 S.STORE Store,A.AREA_NAME_KHMER AreaNameKhmer, A.AREA_ID AreaId, M.MARKET_ID MarketId,A.ANAL_M0,ADD_TEL [PhonNumber],
                         ADD_STAT
                         FROM SIADD S INNER JOIN TB_BCMARKET M ON M.MARKET_ID = S.MARKET_ID INNER JOIN TB_AREAS A ON A.AREA_ID = S.AREA_ID INNER JOIN SIDBINFO DB ON S.DB_CODE = DB.DB_CODE
                         WHERE S.DB_CODE = @DB_CODE AND M.DB_CODE = @DB_CODE AND A.DB_CODE = @DB_CODE AND S.ADD_TYPE = '0'
                         ) CUSTOMER ON CUSTOMER.CustomerCode = CUST_CODE
                         INNER JOIN SIITEMS ITEM ON ITEM.ITEM_CODE = TBCD.ITEM_CODE
                         INNER JOIN BCUSERS USERS ON USERS.USER_ID = TBC.USER_CODE
                         WHERE TYPE =@TYPE {statusCondition} AND CONCAT(FORMAT(CREATED_DATE,'yyyy'),FORMAT(CREATED_DATE,'MM')) BETWEEN @FROMPERIOD AND @TOPERIOD 
                       GROUP BY CREATED_DATE,TBCD.ITEM_CODE,QUANTITY,CUSTOMER.DbCode,CUSTOMER.DbName,CUSTOMER.CustomerCode,CUSTOMER.CustomerName,TBCD.DESCRIPION,
					   CUSTOMER.AreaId,CUSTOMER.AreaNameKhmer,CUSTOMER.MarketNameKhmer,CUSTOMER.Store,ITEM.ITEM_DESC,
					   USERS.USER_ID,USERS.FIRST_NAME,USERS.LAST_NAME
						  ) C GROUP BY C.ItemCode,C.DbCode,C.DbName,C.AreaId,C.CustomerCode,C.CustomerName,C.AreaNameKhmer,C.Reason
						  ,C.MarketNameKhmer,C.Store,C.ItemName,C.Seller,C.Date ORDER BY Date
						  ;";

                var param = new
            {
                DB_CODE = dbCode,
                TYPE = type,
                FROMPERIOD = fromPeriod,
                TOPERIOD = toPeriod,
            };
            var execute = await _sqlDataAccess.LoadData<ExchangeItemAnalysisModel, dynamic>(sql, param);
            return execute.ToList();
        }
    }
}
