namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Report.OthersReport;

public class CustomerReportRepository : ICustomerReportRepository
{
    private readonly ISqlDataAccess _sqlDataAccess;

    public CustomerReportRepository(ISqlDataAccess sqlDataAccess)
    {
        _sqlDataAccess = sqlDataAccess;
    }

    public async Task<List<CustomerReportModel>> GetCustomerReportByDateAsync(string dbCode, DateTime fromDate,
        DateTime toDate)
    {
        var sql =
            $@"SELECT DbCode, DbName, CustomerCode, CustomerName, AreaNameKhmer Area, MarketNameKhmer Market, Store, LAST_NAME + ' ' + FIRST_NAME Fullname, DATE_CHECK DateCheck, C.MEET IsMet, C.[ORDER] IsOrdered, C.DESCRIPTION Description,
	            C.LATITUDE Lat2,C.LONGITUDE Lon2, S.Lat1, S.Lon1,
                CONCAT(CAST(ROUND(geography::Point(S.Lat1, S.Lon1, 4326).STDistance(geography::Point(C.LATITUDE, C.LONGITUDE, 4326)), 2) AS DECIMAL(10,2)), 'm') as Distance
				FROM FN_GETCUSTOMERS(@DB_CODE) CUS 
	            INNER JOIN MB_CUSTOMER_CHECKUP C ON CUS.CustomerCode = C.CUST_CODE
	            INNER JOIN BCUSERS U ON C.USER_ID = U.USER_ID 
				INNER JOIN (SELECT 
				LEFT(GOOGLE_MAP, CHARINDEX(',', GOOGLE_MAP) - 1) AS Lat1,
				RIGHT(GOOGLE_MAP, LEN(GOOGLE_MAP) - CHARINDEX(',', GOOGLE_MAP)) AS Lon1,
				ADD_CODE CustCode
			FROM SIADD
			WHERE DB_CODE = @DB_CODE) S ON S.CustCode = CUS.CustomerCode
               WHERE DATE_CHECK BETWEEN @FROM_DATE AND @TO_DATE AND C.DB_CODE = @DB_CODE AND C.[STATUS] = 1
			   ORDER BY AreaNameKhmer";
        var param = new
        {
            DB_CODE = dbCode,
            FROM_DATE = fromDate,
            TO_DATE = toDate
        };
        var execute = await _sqlDataAccess.LoadData<CustomerReportModel, dynamic>(sql, param);
        return execute.ToList();
    }
}