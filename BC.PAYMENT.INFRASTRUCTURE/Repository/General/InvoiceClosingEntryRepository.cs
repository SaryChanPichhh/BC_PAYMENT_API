using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.Entities.General;
using BC.PAYMENT.INFRASTRUCTURE.DBAccess;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.General
{
    public class InvoiceClosingEntryRepository : IInvoiceClosingEntryRepository
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public InvoiceClosingEntryRepository(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<bool> CheckIsEntriesIsAlreadyOpenAsync(string dbCode)
        {
            const string sql = @"SELECT COUNT(*) FROM PAYMENT_CLOSING_ENTRY_INVOICE WHERE IS_ACTIVE=1 AND DB_CODE=@DB_CODE";
            var param = new
            {
                DB_CODE = dbCode
            };
            return await _sqlDataAccess.ExecuteScalarAsync<bool, dynamic>(sql, param);
        }
        public async Task<int> CreateClosingEntryAsync(InvoiceClosingEntriesModel closingEntry, string dbCode)
        {
            const string sql =
                @"INSERT INTO PAYMENT_CLOSING_ENTRY_INVOICE(CODE,DB_CODE,DESCRIPTION,CREATED_BY,CREATED_DATE,IS_ACTIVE)
                VALUES(@CODE,@DB_CODE,@DESCRIPTION,@CREATED_BY,@CREATED_DATE,@IS_ACTIVE)";
            var param = new
            {
                CODE = closingEntry.Code,
                DB_CODE = dbCode,
                DESCRIPTION = closingEntry.Description,
                CREATED_BY = closingEntry.CreatedBy,
                CREATED_DATE = DateTime.Now,
                IS_ACTIVE = true
            };
            return await _sqlDataAccess.ExecuteAsync(sql, param);
        }
        public async Task<string> GenerateOpeningEntryCodeAsync(string dbCode)
        {
            const string sql = @"SELECT ISNULL(MAX(SUBSTRING(CODE,8,10)),0) FROM PAYMENT_CLOSING_ENTRY_INVOICE
         WHERE SUBSTRING(CODE,4,2) = MONTH(GETDATE()) AND DB_CODE = @DB_CODE";
            var param = new
            {
                DB_CODE = dbCode
            };
            var results = await _sqlDataAccess.ExecuteScalarAsync<int, dynamic>(sql, param);
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
            var results = await _sqlDataAccess.ExecuteScalarAsync<string, dynamic>(sql, param);
            return results;
        }
    }
}
