namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Preset.AnnualPurchase
{
    public class AnnualPurchaseRepository : IAnnualPurchaseRepository
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public AnnualPurchaseRepository(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<List<AnnualPurchaseModel>> GetSalesReportListAsync(string dbCode,List<int> year,int page,int pageSize )
        {
            if (page == 0) page = 1;
            if (pageSize == 0) pageSize = 1;
            var offSet = (page-1)*pageSize;
            var sql = @$"SELECT CUST_CODE CustomerCode,[Year],AMOUNT Amount,CUS.CustomerName,CUS.AreaNameKhmer Area,CUS.MarketNameKhmer Market,CUS.Store FROM 
            (SELECT CUST_CODE,SUM(TRANS_VAL) AMOUNT,LEFT(INV_PRD, LEN(INV_PRD) - 2) [Year] FROM {dbCode}SISOHDR WHERE LEFT(INV_PRD,4) IN @YEAR
			 AND CUST_CODE  NOT LIKE 'PV%' GROUP BY INV_PRD,CUST_CODE) AS  HRSALE 
			 LEFT JOIN (SELECT S.DB_CODE DbCode,DB.DB_NAME DbName, S.ADD_CODE CustomerCode,S.ADD_LINE_1KH CustomerName,M.MARKET_KHMER_NAME MarketNameKhmer,S.STORE Store,A.AREA_NAME_KHMER AreaNameKhmer, A.AREA_ID AreaId, M.MARKET_ID MarketId,A.ANAL_M0,ADD_TEL [PhonNumber],
				ADD_STAT,S.GOOGLE_MAP [Maps]
				FROM SIADD S INNER JOIN TB_BCMARKET M ON M.MARKET_ID = S.MARKET_ID INNER JOIN TB_AREAS A ON A.AREA_ID = S.AREA_ID INNER JOIN SIDBINFO DB ON S.DB_CODE = DB.DB_CODE
				WHERE  S.ADD_TYPE = '0' )  CUS ON CUS.CustomerCode = CUST_CODE
			 ORDER BY AMOUNT DESC OFFSET @OFFSET ROWS FETCH NEXT @PAGE_SIZE ROWS ONLY";
            var param = new
            {
                YEAR = year,
                OFFSET = offSet,
                PAGE_SIZE = pageSize,
            };
            var results = await _sqlDataAccess.LoadData<AnnualPurchaseModel, dynamic>(sql, param);
            return results.ToList();
        }

        public async Task<List<AnnualPurchaseModel>> GetAllReportSalesPerYearsAsync(string dbCode, List<string> marketCode, List<string> customerCode,
            SaleTypeDto reportSalesPerYearRequestDto, params List<int>[] year)
        {
            string sql = string.Empty;
            string sqlCondition = string.Empty;
            string periodCondition = string.Empty;
            string marketCodes = string.Join(", ", marketCode.Select(x => $"'{x}'"));
            string customerCodes = string.Join(", ", customerCode.Select(x => $"'{x}'"));
            string lsYear = string.Join(", ", year.Select(x => $"'{x}'"));
            //string saleType = string.Join(" ", year.Select(x => $"'{x}'"));
            //string saleCode = string.Join(" " ,reportSalesPerYearRequestDto)
            if (marketCode.Any() && customerCode.Any())
            {
                sqlCondition = $@"WHERE CUST_CODE IN ({customerCodes}) AND ANAD_CODE IN ({marketCodes})";
            }
            else if (marketCode.Any())
            {
                sqlCondition = $@"WHERE ANAD_CODE IN ({marketCodes})";
            }
            else if (customerCode.Any())
            {
                sqlCondition = $@"WHERE CUST_CODE IN ({customerCodes})";
            }
            if (year.Any()) periodCondition = @$"WHERE LEFT(INV_PRD, LEN(INV_PRD) - 2) IN ({lsYear})";
            if (reportSalesPerYearRequestDto.FromPeriod != string.Empty && reportSalesPerYearRequestDto.ToPeriod != string.Empty)
                periodCondition = @$"WHERE INV_PRD BETWEEN {reportSalesPerYearRequestDto.FromPeriod} AND {reportSalesPerYearRequestDto.ToPeriod}";
            sql = $@"SELECT ANAD_COM Market,CUST_CODE CustomerCode,CustomerName ,AMOUNT Amount,TAB2.Store,AreaNameKhmer Area,[Year] FROM 
                                      (SELECT CUST_CODE, ANAL_M9, SUM(TRANS_VAL) AMOUNT,LEFT(INV_PRD, LEN(INV_PRD) - 2) Year FROM
                                      {dbCode}SISOHDR {periodCondition} 
                                     AND TRANS_CODE IN @SALE_TYPE 
                                      AND CUST_CODE  NOT LIKE 'PV%' GROUP BY (INV_PRD),CUST_CODE, ANAL_M9) AS  HRSALE INNER JOIN
                                      (SELECT ANAD_CODE, ANAD_COM FROM SIANALD WHERE DB_CODE = @DB_CODE AND ANAM_CODE = 'M9') AS ANAN ON HRSALE.ANAL_M9 = ANAN.ANAD_CODE
                                      LEFT JOIN(SELECT S.DB_CODE DbCode,DB.DB_NAME DbName, S.ADD_CODE CustomerCode,S.ADD_LINE_1KH CustomerName,M.MARKET_KHMER_NAME MarketNameKhmer,S.STORE Store,A.AREA_NAME_KHMER AreaNameKhmer, A.AREA_ID AreaId, M.MARKET_ID MarketId,A.ANAL_M0,ADD_TEL [PhonNumber],
										ADD_STAT,S.GOOGLE_MAP [Maps]
										FROM SIADD S INNER JOIN TB_BCMARKET M ON M.MARKET_ID = S.MARKET_ID INNER JOIN TB_AREAS A ON A.AREA_ID = S.AREA_ID INNER JOIN SIDBINFO DB ON S.DB_CODE = DB.DB_CODE
									    WHERE  S.ADD_TYPE = '0' ) TAB2 ON TAB2.CustomerCode = HRSALE.CUST_CODE
                                      {sqlCondition}
                                      ORDER BY ANAD_CODE, AMOUNT DESC";
            var param = new
            {
                DB_CODE = dbCode,
                SALE_TYPE = reportSalesPerYearRequestDto.SaleType
            };
            var result = await _sqlDataAccess.LoadData<AnnualPurchaseModel, dynamic>(sql, param);
            return result.ToList();
        }

        public async Task<List<string>> SaleCodesAsync(string dbCode)
        {
            var sql = $@"SELECT CODE FROM SIDATA WHERE DB_CODE = @DB_CODE AND  SI_TYPE = 'SALES'";
            var param = new
            {
                DB_CODE = dbCode,
            };
            var execute = await _sqlDataAccess.LoadData<string, dynamic>(sql, param);
            return execute.ToList();
        }

        public async Task<List<DailySaleReportValueDetailsModel>> GetDailySaleReportDetailsValueAsync(DateTime date, string branchCode)
        {
            var dateFilter = date.ToString("M/dd/yyyy");
            var sql = $@"SELECT ITEM_CODE ItemCode, SUM(POST) Invoiced, SUM(HOLD) Ordered, SUM(RELEASE) Posted, SUM(TOTAL) Total
            FROM(
            SELECT ITEM_CODE , POST, HOLD, RELEASE, (ISNULL(POST, 0) + ISNULL(HOLD, 0) + ISNULL(RELEASE, 0)) TOTAL
            FROM(
            SELECT DETAIL.ITEM_CODE,
            ISNULL(CASE WHEN HEADER.STATUS IN('80', '85') THEN SUM(VALUE_4) END, 0) 'POST',
            ISNULL(CASE WHEN HEADER.STATUS = '00' THEN SUM(VALUE_4) END, 0)          'HOLD',
            ISNULL(CASE WHEN HEADER.STATUS = '10' THEN SUM(VALUE_4) END, 0)          'RELEASE'
            FROM {branchCode}SISOHDR HEADER
            INNER JOIN {branchCode}SISODET DETAIL ON HEADER.TRANS_REF = DETAIL.TRANS_REF
            WHERE HEADER.ORDER_DATE BETWEEN @FROMDATE AND @TODATE
            GROUP BY ITEM_CODE, HEADER.STATUS) TAB1
            GROUP BY ITEM_CODE, POST, HOLD, RELEASE) TAB
            GROUP BY ITEM_CODE
            ORDER BY ITEM_CODE
            ";
            var param = new { FROMDATE = dateFilter, TODATE = dateFilter, };
            var dailySaleReportDetailsValueResults =
                await _sqlDataAccess.LoadData<DailySaleReportValueDetailsModel, dynamic>(sql, param);
            return dailySaleReportDetailsValueResults.ToList();
        }

        public async Task<List<DailySaleReportValueModel>> GetDailySaleReportValueAsync(DateTime date, Dictionary<string, string> branches)
        {
            var dailySaleReportValues = new List<DailySaleReportValueModel>();
            var dateFilter = date.ToString("M/dd/yyyy");
            foreach (var branch in branches)
            {
                var branchCode = branch.Key;
                var branchName = branch.Value;
                var sql = $@"SELECT SUM(POST) Invoiced, SUM(HOLD) [Ordered],SUM(RELEASE) Posted, SUM(TOTAL) Total
                FROM(
                SELECT ITEM_CODE, POST, HOLD,RELEASE, (ISNULL(POST, 0) + ISNULL(HOLD, 0) + ISNULL(RELEASE,0)) TOTAL
                FROM(
                SELECT DETAIL.ITEM_CODE,
                ISNULL(CASE WHEN HEADER.STATUS IN('80', '85') THEN SUM(VALUE_4) END, 0) 'POST',
                ISNULL(CASE WHEN HEADER.STATUS = '00' THEN SUM(VALUE_4) END, 0)          'HOLD',
				ISNULL(CASE WHEN HEADER.STATUS = '10' THEN SUM(VALUE_4) END, 0)          'RELEASE'
				FROM {branchCode}SISOHDR HEADER
				INNER JOIN {branchCode}SISODET DETAIL ON HEADER.TRANS_REF = DETAIL.TRANS_REF
				WHERE HEADER.ORDER_DATE BETWEEN @FROMDATE AND @TODATE
				GROUP BY ITEM_CODE, HEADER.STATUS) TAB1
				GROUP BY ITEM_CODE, POST, HOLD,RELEASE
				UNION ALL
				SELECT ITEM_CODE, '0', '0','0', SUM(QUANTITY)
				FROM {branchCode}SIINVMOV
				WHERE MOV_DATE BETWEEN @FROMDATE AND @TODATE
				AND IR_STAT = 'I'
				AND REC_TYPE = 'T'
				GROUP BY ITEM_CODE, MOV_DATE) TAB";
                var dailySaleReportResult =
                    await _sqlDataAccess.LoadSingleData<DailySaleReportValueModel, dynamic>(sql, new { FROMDATE = dateFilter, TODATE = dateFilter });
                dailySaleReportResult.BranchName = branchName;
                dailySaleReportResult.BranchCode = branchCode;
                dailySaleReportValues.Add(dailySaleReportResult);
            }
            return dailySaleReportValues;
        }
    }
}
