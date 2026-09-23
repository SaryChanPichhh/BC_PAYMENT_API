using BC.PAYMENT.CORE.Contracts.Criteria;
using BC.PAYMENT.CORE.Contracts.Invoice;
using BC.PAYMENT.CORE.Contracts.Request.Invoice;
using BC.PAYMENT.CORE.Contracts.Response.Invoice;
using BC.PAYMENT.CORE.Contracts.TablesType;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Invoice;

public class OldInvoiceRepository(ISqlDataAccess sqlDataAccess) : IOldInvoiceRepository
{
    public async Task<List<OldInvoiceResponse>> GetOldInvoiceAsync(string dbCode, int offset, int pageSize,List<string>? transCodes = null)
    {
        var param = new
        {
            DB_CODE = dbCode,
            OFF_SET = offset,
            PAGESIZE = pageSize
        };
        var result = await sqlDataAccess.LoadData<OldInvoiceResponse, dynamic>
            (OldInvoiceQueries.SelectOldInv, param, CommandType.StoredProcedure);
        return result.ToList();
    }
    public async Task<List<OldInvoiceResponse>> GetAllOldInvoicesFromLedgerAsync(OldInvoiceCriteria crit)
    {
        var parameter = new
        {
            DB_CODE = crit.DbCode,
            BY_DATE = crit.Date,
            FROM_ACC = crit.FromAccount,
            TO_ACC = crit.ToAccount,
            ACC_TYPE = "D",
            T = "",
            FROM_ANAL = crit.FromAnal,
            TO_ANAL = crit.ToAnal,
            crit.T0,
            crit.T1,
            crit.T2,
            crit.T3,
            crit.T4,
            crit.T5,
            crit.T6,
            crit.T7,
            crit.T8,
            crit.T9,
        };
        var oldInvoiceList = await sqlDataAccess.LoadData<OldInvoiceResponse, dynamic>(OldInvoiceQueries.SelectAging(crit.DbCode), parameter, CommandType.StoredProcedure);
        return oldInvoiceList.ToList();
    }
    public async Task<int> SaveOldInvoice(List<OldInvoiceTableType> req)
    {
        var dataTable = AppExtension.ConvertToDataTable(req);
        var param = new DynamicParameters();
        param.Add("@OldInvoice", dataTable.AsTableValuedParameter("OLD_INVOICES")); 
        var rowAffected = await sqlDataAccess.ExecuteAsync(OldInvoiceQueries.InsertOldInvoice, param, CommandType.StoredProcedure);
        return rowAffected;
    }   
    public async Task<int> RecreateOldInvoiceAsync(InvoiceDTO invoiceDto)
    {
        var arguments = new { DB_CODE = invoiceDto.DbCode, USER_CREATED = invoiceDto.CreatedBy, CODE = invoiceDto.TransactionCode, ENTRIES_CODE = invoiceDto.EntryCode };
        var rowAffected = await sqlDataAccess.ExecuteAsync(OldInvoiceQueries.RecreateOldInvoice, arguments);
        return rowAffected;
    }
    public async Task<OldInvoiceResponse> GetOldInvoiceByTransactionCode(string dbCode, string transactionCode)
    {
        var param = new { DB_CODE = dbCode, CODE = transactionCode };
        var result = await sqlDataAccess.LoadSingleData<OldInvoiceResponse, dynamic>(OldInvoiceQueries.GetOldInvoiceByInvoiceCode, param);
        return result;
    }
    public async Task<int> UpdateStatus(string dbCode, string transaction)
    {
        var param = new { CODE = transaction, DB_CODE = dbCode, CREATED_DATE = DateTime.Today };
        var result = await sqlDataAccess.ExecuteAsync(OldInvoiceQueries.UpdateStatus, param);
        return result;
    }
    public async Task<int> DeleteOldInvoiceByIdAsync(int id)
    {
        var param = new { ID = id };
        var result = await sqlDataAccess.ExecuteAsync(OldInvoiceQueries.DeleteOldInvoiceById, param);
        return result;
    }

}