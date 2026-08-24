namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Report.OthersReport
{
    public class BillsOwedRepository : IBillsOwedRepository
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public BillsOwedRepository(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<List<Dictionary<string, object>>> GetBillOwedByDateAsync(string dbCode, string month)
        {
            var storeProcedure = $@"PAYMENT_ACCOUNT_RECEIVABLE";
            var param = new
            {
                DB_CODE = dbCode,
                MONTH = month,
            };
            
            var execute = await _sqlDataAccess.LoadData<dynamic, dynamic>(storeProcedure, param);
            var dictionary = execute.Select( row => (IDictionary<string,object> )row).Select(dict => dict.ToDictionary(k => k.Key, v => v.Value))
                .ToList();
            return dictionary;
        }
    }
}
