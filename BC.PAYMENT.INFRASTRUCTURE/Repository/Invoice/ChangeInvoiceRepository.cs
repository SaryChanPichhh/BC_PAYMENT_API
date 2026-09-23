using BC.PAYMENT.CORE.Contracts.Response.Invoice;
using BC.PAYMENT.SQL.Queries;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Invoice
{
    public class ChangeInvoiceRepository(ISqlDataAccess sqlDataAccess) : IChangeInvoiceRepository
    {
        public async Task<List<ChangeInvoiceResponse>> GetChangeInvoicesAsync(string dbCode, DateTime createDate)
        {
            var param = new
            {
                DB_CODE = dbCode,
                CREATED_DATE = createDate.Date,
                STATUS = "C",
            };
            var invoicesModels = await sqlDataAccess.LoadData<ChangeInvoiceResponse, dynamic>(ChangeInvoiceQueries.GetChangeInvoices, param);
            return invoicesModels.ToList();
        }

        public async Task<bool> CheckExistInvoiceAsync(string transaction, string dbCode)
        {
            var param = new { TRANSACTION = transaction, DB_CODE = dbCode };
            return await sqlDataAccess.LoadSingleData<bool,dynamic>(ChangeInvoiceQueries.CheckExistInvoice, param);
        }

        public async Task<List<ChangeInvoiceResponse>> GetLocalInvoiceAsync(string dbCode, DateTime fromDate, DateTime toDate)
        {
            var sql = ChangeInvoiceQueries.GetLocalInvoice;
            var param = new
            {
                DB_CODE = dbCode,
                FROM_DATE = fromDate,
                TO_DATE = toDate
            };
            var results = await sqlDataAccess.LoadData<ChangeInvoiceResponse, dynamic>(sql, param);
            return results.ToList();
        }

        public async Task<List<ChangeInvoiceResponse>> GetOtherBranchInvoiceAsync(string dbCode, DateTime fromDate, DateTime toDate)
        {
            var sql = ChangeInvoiceQueries.GetOtherBranchInvoice;
            var param = new
            {
                DB_CODE = dbCode,
                FROM_DATE = fromDate,
                TO_DATE = toDate
            };
            var results = await sqlDataAccess.LoadData<ChangeInvoiceResponse, dynamic>(sql, param);
            return results.ToList();
        }

        public async Task<int> AddChangeInvoiceAsync(ChangeInvoiceModel model)
        {
            var sql = model.IsExists ? ChangeInvoiceQueries.UpdateExistInvoice : ChangeInvoiceQueries.InsertIfNotExistsInvoice;
            var rowAffected = await sqlDataAccess.ExecuteAsync(
                sql,
                new
                {
                    DB_CODE = model.DbCode,
                    TRANSACTION = model.Transaction,
                    CUSTOMER_CODE = model.CustomerCode,
                    CUSTOMER_NAME = model.CustomerName,
                    VALUE = model.InvoiceValue,
                    STATUS = "C",
                    ENTRIES_CODE = model.EntriesCode,
                    CREATED_BY = model.UserName
                });
            return rowAffected;
        }
    }
}
