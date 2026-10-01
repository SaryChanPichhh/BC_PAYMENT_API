using BC.PAYMENT.CORE.Contracts.Request.Invoice;
using BC.PAYMENT.CORE.Contracts.Response.Invoice;
using BC.PAYMENT.CORE.Entities;
using BC.PAYMENT.SQL.Queries;
using ReturnInvoiceModel =
    BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DividingInvoices.Invoice.ReturnInvoiceModel;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Invoice;

public class ReturnInvoiceRepository(ISqlDataAccess sqlDataAccess) : IReturnInvoiceRepository
{
    public async Task<List<ReturnInvoiceResponse>> GetReturnInvoiceAsync(string dbCode)
    {
        var param = new
        {
            DB_CODE = dbCode
        };
        var results =
            await sqlDataAccess.LoadData<ReturnInvoiceResponse, dynamic>(ReturnInvoiceQueries.GetReturnInvoice, param);
        return results.ToList();
    }

    public async Task<List<ReturnInvoiceResponse>> GetReturnInvoiceByDateAsync(string dbCode, DateTime fromDate,
        DateTime toDate)
    {
        var param = new
        {
            DB_CODE = dbCode,
            FROM_DATE = fromDate,
            TO_DATE = toDate
        };
        var criteria = $@"AND P.CREATE_DATE BETWEEN @FROM_DATE AND @TO_DATE";
        var results =
            await sqlDataAccess.LoadData<ReturnInvoiceResponse, dynamic>(
                ReturnInvoiceQueries.GetReturnInvoiceByDate(string.Empty, criteria), param);
        return results.ToList();
    }

    public async Task<int> SaveReturnInvoiceAsync(List<ReturnInvoiceRequest> requests, string dbCode, string username,
        string entryCode)
    {
        var rowAffected = 0;
        foreach (var item in requests)
        {
            var param = new
            {
                DB_CODE = dbCode,
                TRANSACTION = item.TransactionCode,
                CUSTOMER_CODE = item.CustomerCode,
                CUSTOMER_NAME = item.CustomerName,
                VALUE = item.InvoiceValue,
                STATUS = "C",
                CREATED_BY = username,
                ENTRIES_CODE = entryCode
            };
            rowAffected += await sqlDataAccess.ExecuteAsync(ReturnInvoiceQueries.InsertReturnInvoice, param);
        }

        return rowAffected;
    }

    public async Task<int> CreatePcReturnInvoiceAsync(string dbCode, int dividedInvoiceId, int paymentHeaderId,
        string description, string createdBy)
    {
        var param = new
        {
            DB = dbCode,
            DDID = dividedInvoiceId,
            PAYMENT_HEADER_ID = paymentHeaderId,
            DESC = description,
            CB = createdBy
        };
        return await sqlDataAccess.ExecuteAsync(ReturnInvoiceQueries.CreatePcReturnInvoice, param);
    }

    public async Task<int> CreatePcReturnInvoiceAsync(PcReturnInvoice returnInvoice)
    {
        return await CreatePcReturnInvoiceAsync(
            returnInvoice.DbCode,
            returnInvoice.DividedId,
            returnInvoice.HeaderId,
            returnInvoice.Description,
            returnInvoice.CreatedBy);
    }

    public async Task<int> CreatePcReturnInvoiceAsync(List<PcReturnInvoice> returnInvoices)
    {
        var rowAffected = 0;
        foreach (var item in returnInvoices) rowAffected += await CreatePcReturnInvoiceAsync(item);
        return rowAffected;
    }

    public async Task<int> CreatePcReturnInvoiceAsync(List<PcReturnInvoiceCreateRequest> requests, string dbCode,
        int paymentHeaderId, string createdBy)
    {
        var rowAffected = 0;
        foreach (var item in requests)
            rowAffected +=
                await CreatePcReturnInvoiceAsync(dbCode, item.DividedId, paymentHeaderId, item.Description, createdBy);
        return rowAffected;
    }

    public async Task<List<PcReturnInvoice>> GetPcReturnInvoiceAsync(string dbCode)
    {
        var param = new
        {
            DB_CODE = dbCode
        };
        var results =
            await sqlDataAccess.LoadData<PcReturnInvoice, dynamic>(ReturnInvoiceQueries.GetPcReturnInvoices, param);
        return results.ToList();
    }

    public async Task<List<ReturnInvoiceResponse>> GetPcReturnInvoiceByDateAsync(string dbCode, DateTime fromDate,
        DateTime toDate)
    {
        var addOnFields =
            $@"D.DIVIDED_INVOICE_ID DividedId,1.00 [Return],R.DESCRIPTION Description,R.RETURN_ID ReturnId";
        var param = new
        {
            DB_CODE = dbCode,
            FROM_DATE = fromDate,
            TO_DATE = toDate
        };
        var criteria = $@"AND P.CREATE_DATE BETWEEN @FROM_DATE AND @TO_DATE";
        var execute =
            await sqlDataAccess.LoadData<ReturnInvoiceResponse, dynamic>(
                ReturnInvoiceQueries.GetReturnInvoiceByDate(addOnFields, criteria), param);
        return execute.ToList();
    }

    public async Task<List<ReturnInvoiceResponse>> GetPcReturnInvoiceByPeriodAsync(string dbCode, string month,
        string year)
    {
        var addOnFields =
            $@"D.DIVIDED_INVOICE_ID DividedId,1.00 [Return],R.DESCRIPTION Description,R.RETURN_ID ReturnId";
        var param = new
        {
            DB_CODE = dbCode,
            MONTH = month,
            YEAR = year
        };
        var criteria = $@"AND YEAR(P.CREATE_DATE) = @YEAR AND MONTH(P.CREATE_DATE)= @MONTH";
        var execute =
            await sqlDataAccess.LoadData<ReturnInvoiceResponse, dynamic>(
                ReturnInvoiceQueries.GetReturnInvoiceByDate(addOnFields, criteria), param);
        return execute.ToList();
    }

    public async Task<int> DeletePcReturnInvoiceAsync(int dividedId)
    {
        var param = new
        {
            DIVIDED_ID = dividedId
        };
        var count = await sqlDataAccess.ExecuteAsync(ReturnInvoiceQueries.DeletePcReturnInvoice, param);
        if (count > 0)
            await sqlDataAccess.ExecuteAsync(ReturnInvoiceQueries.UpdateDividedInvoiceStatusAfterReturnDelete, param);
        return count;
    }

    public async Task<List<PcReturnInvoiceAuditResponse>> GetReturnChangeInvoiceByDateAsync(string dbCode,
        DateTime fromDate, DateTime toDate)
    {
        var param = new
        {
            DB_CODE = dbCode,
            FROM_DATE = fromDate,
            TO_DATE = toDate
        };
        var criteria = $@"AND P.CREATE_DATE BETWEEN @FROM_DATE AND @TO_DATE";
        var execute =
            await sqlDataAccess.LoadData<PcReturnInvoiceAuditResponse, dynamic>(
                ReturnInvoiceQueries.GetPcReturnInvoiceAudit(criteria), param);
        return execute.ToList();
    }

    public async Task<List<PcReturnInvoiceAuditResponse>> GetReturnChangeInvoiceByPeriodAsync(string dbCode, int month,
        int year)
    {
        var param = new
        {
            DB_CODE = dbCode,
            YEAR = year,
            MONTH = month
        };
        var criteria = $@"AND YEAR(P.CREATE_DATE) = @YEAR AND MONTH(P.CREATE_DATE) = @MONTH";
        var execute =
            await sqlDataAccess.LoadData<PcReturnInvoiceAuditResponse, dynamic>(
                ReturnInvoiceQueries.GetPcReturnInvoiceAudit(criteria), param);
        return execute.ToList();
    }

    public async Task<int> InsertGetReturnInvoice(PcReturningInvoiceAudit model)
    {
        var param = new
        {
            RETURN_ID = model.ReturnId,
            PROCESSING_STATUS = 1,
            APPROVAL_STATUS = model.ApprovalStatus,
            LAST_UPDATED_DATE = model.LastUpdatedDate,
            LAST_UPDATED_BY = model.LastUpdatedBy
        };
        var affectedRow = await sqlDataAccess.ExecuteAsync(ReturnInvoiceQueries.InsertPcReturnInvoiceAudit, param);
        return affectedRow;
    }
}