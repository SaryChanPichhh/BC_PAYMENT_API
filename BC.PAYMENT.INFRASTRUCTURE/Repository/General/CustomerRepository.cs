namespace BC.PAYMENT.INFRASTRUCTURE.Repository.General
{
    public class CustomerRepository:ICustomerRepository
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public CustomerRepository(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<List<Customer>> GetCustomer(int offset, int pageSize)
        {
            const string sql = @"SELECT * FROM PM_GET_CUSTOMERS_BY_DB_CODE(@OFF_SET, @PAGESIZE)";
            var param = new
            {
                OFF_SET = offset,
                PAGESIZE = pageSize
            };
            var results = await _sqlDataAccess.LoadData<Customer, dynamic>(sql, param);
            return results.ToList();
        }
        public async Task<List<Customer>> GetCustomer()
        {
            const string sql = @"SELECT * FROM GET_CUSTOMERS_BY_DB_CODE()";
            
            var results = await _sqlDataAccess.LoadData<Customer, dynamic>(sql, new {});
            return results.ToList();
        }

        public async Task<List<Customer>> GetCustomerByMarketCodeAsync(string dbCode, List<string> marketCode, List<string> saleTypes)
        {
            string marketCondition = string.Empty;
            string saleTypeCondition = string.Empty;
            if (marketCode.Any())
            {
                marketCondition = @"AND ANAD_CODE IN @MARKETCODE";
            }

            if (saleTypes.Any())
            {
                var saleType = string.Join(", ", saleTypes.Select(s => $@"'{s}'"));
                saleTypeCondition = $@"AND HRSALE.TRANS_CODE IN ({saleType})";
            }
            string sql =
                $@"SELECT DISTINCT HRSALE.CUST_CODE CustomerCode,CUST.ADD_LINE_1KH CustomerName FROM {dbCode}SISOHDR HRSALE
                INNER JOIN 
                (SELECT ANAD_CODE,ANAD_COM FROM SIANALD WHERE DB_CODE = @DB_CODE 
                AND ANAM_CODE = 'M9') AS ANAN ON HRSALE.ANAL_M9 = ANAN.ANAD_CODE
	            INNER JOIN SIADD CUST ON CUST.ADD_CODE = HRSALE.CUST_CODE
                WHERE HRSALE.CUST_CODE NOT LIKE 'PV%' AND CUST.DB_CODE = @DB_CODE {marketCondition} {saleTypeCondition}
                GROUP BY HRSALE.CUST_CODE,CUST.ADD_LINE_1KH ORDER BY HRSALE.CUST_CODE;";
            var param = new
            {
                DB_CODE = dbCode,
                MARKETCODE = marketCode,
            };
            var execute = await _sqlDataAccess.LoadData<Customer, dynamic>(sql, param);
            return execute.ToList();
        }
    }
}
