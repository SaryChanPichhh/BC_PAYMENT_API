namespace BC.PAYMENT.INFRASTRUCTURE.Repository.General
{
    public class InvoiceClosingEntryRepository(ISqlDataAccess sqlDataAccess) : IInvoiceClosingEntryRepository
    {
        public async Task<bool> CheckIsEntriesIsAlreadyOpenAsync(string dbCode)
        {
            var param = new
            {
                DB_CODE = dbCode
            };
            return await sqlDataAccess.ExecuteScalarAsync<bool, dynamic>(InvoiceClosingEntryQueries.CheckIsEntriesIsAlreadyOpen, param);
        }
        public async Task<int> CreateClosingEntryAsync(InvoiceClosingEntriesModel closingEntry)
        {
            var code = await  GenerateOpeningEntryCodeAsync(closingEntry.DbCode);
            var param = new
            {
                CODE = code,
                DB_CODE = closingEntry.DbCode,
                DESCRIPTION = closingEntry.Description,
                CREATED_BY = closingEntry.CreatedBy,
                CREATED_DATE = DateTime.Now,
                IS_ACTIVE = true
            };
            return await sqlDataAccess.ExecuteAsync(InvoiceClosingEntryQueries.AddNew, param);
        }
        public async Task<string> GenerateOpeningEntryCodeAsync(string dbCode)
        {
            var param = new
            {
                DB_CODE = dbCode
            };
            var results = await sqlDataAccess.ExecuteScalarAsync<int, dynamic>(InvoiceClosingEntryQueries.GenerateEntriesCode, param);
            results++;
            var generateCode = $@"{dbCode}{DateTime.Now.Month:D2}{DateTime.Now.Year.ToString().Substring(2, 2)}{results:D3}";
            return generateCode;
        }
        public async Task<string> GetOpeningEntryCodeByDbCodeAsync(string dbCode)
        {
            const string sql = @"SELECT CODE FROM PAYMENT_CLOSING_ENTRY_INVOICE WHERE DB_CODE = @DB_CODE AND IS_ACTIVE = 1";
            var param = new
            {
                DB_CODE = dbCode
            };
            var results = await sqlDataAccess.ExecuteScalarAsync<string, dynamic>(sql, param);
            return results;
        }
    }
}
