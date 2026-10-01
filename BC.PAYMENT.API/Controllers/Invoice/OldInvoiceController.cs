using BC.PAYMENT.CORE.Contracts.Criteria;
using BC.PAYMENT.CORE.Contracts.Invoice;
using BC.PAYMENT.CORE.Contracts.Request.Invoice;
using BC.PAYMENT.CORE.Contracts.Response.Invoice;
using BC.PAYMENT.CORE.Contracts.TablesType;
using BC.PAYMENT.CORE.Entities.Invoice;
using BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.AccountReceivable;

namespace BC.PAYMENT.API.Controllers.Invoice;

public class OldInvoiceController(IUnitOfWork unitOfWork) : BaseApiController
{
    [HttpPost]
    [Route("")]
    public async Task<ApiResponse<List<ConfirmAccountReceivableModel.ConfirmBalanceDetails>>>
        AddNewConfirmAccountReceivableDetailAsync(OldInvoiceCriteriaPost model)
    {
        try
        {
            var credential = Common.DecodeJwt(HttpContext.User);
            var oldInvoiceParameter = new OldInvoiceCriteria
            {
                DbCode = credential.DbCode,
                Date = model.Date,
                FromAccount = model.FromAccount,
                ToAccount = model.ToAccount,
                FromAnal = model.FromAnal,
                ToAnal = model.ToAnal,
                T0 = model.T0,
                T1 = model.T1,
                T2 = model.T2,
                T3 = model.T3,
                T4 = model.T4,
                T5 = model.T5,
                T6 = model.T6,
                T7 = model.T7,
                T8 = model.T8,
                T9 = model.T9
            };
            var oldInvoiceModels = await unitOfWork.OldInvoice.GetAllOldInvoicesFromLedgerAsync(oldInvoiceParameter);
            var oldInvoices = oldInvoiceModels.Select(oldInvoiceModel =>
                    new ConfirmAccountReceivableModel.ConfirmBalanceDetails
                    {
                        CustomerCode = oldInvoiceModel.CustomerCode,
                        CustomerName = oldInvoiceModel.CustomerName,
                        InvoicedCode = oldInvoiceModel.TransactionCode,
                        InvoicedAmount = oldInvoiceModel.InvoiceValue,
                        DbCode = credential.DbCode,
                        CreatedBy = credential.Username
                    })
                .ToList();
            var affectedRow =
                await unitOfWork.ConfirmAccountReceivable.AddConfirmAccountReceivableDetails(model.AccountReceivableId,
                    oldInvoices);

            return ApiResponse<List<ConfirmAccountReceivableModel.ConfirmBalanceDetails>>.Builder()
                .WithMessage(affectedRow > 0
                    ? "Confirm Account Receivables Detail added successfully"
                    : "Confirm Account Receivables Detail added unsuccessfully")
                .WithStatusCode(affectedRow > 0 ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest)
                .WithResult(affectedRow > 0 ? oldInvoices : [])
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler
                .ExceptionError<List<ConfirmAccountReceivableModel.ConfirmBalanceDetails>>(ex.Message);
        }
    }

    [HttpGet("paged")]
    public async Task<ApiResponse<PaginatedResponse<OldInvoiceResponse>>> GetOldInvoiceAsync([FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var claim = Common.DecodeJwt(HttpContext.User);
            var allRecords = await unitOfWork.OldInvoice.GetOldInvoiceAsync(claim.DbCode!, page, pageSize);
            if (allRecords.Count == 0)
                return ApiResponse<PaginatedResponse<OldInvoiceResponse>>.Builder()
                    .WithResult(new PaginatedResponse<OldInvoiceResponse>([], 0, page, pageSize))
                    .WithStatusCode((int)HttpStatusCode.BadRequest).WithSuccess(true)
                    .WithErrors("BE-404")
                    .WithMessage("Old invoices  not found.")
                    .Build();
            var totalRecords = allRecords.Count;
            return ApiResponse<PaginatedResponse<OldInvoiceResponse>>.Builder()
                .WithResult(new PaginatedResponse<OldInvoiceResponse>(allRecords, totalRecords, page, pageSize))
                .WithStatusCode((int)HttpStatusCode.OK).WithSuccess(true)
                .WithMessage("Old invoice fetched successfully.")
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<PaginatedResponse<OldInvoiceResponse>>(ex.Message);
        }
    }

    [HttpPost]
    [Route("from-ledger")]
    public async Task<ApiResponse<List<OldInvoiceResponse>>> GetOldInvoiceFromLedgerAsync(
        [FromBody] OldInvoiceCriteria dto)
    {
        try
        {
            var claim = Common.DecodeJwt(HttpContext.User);
            dto.DbCode = claim.DbCode;
            dto.Date = claim.CurrectDate;
            var rows = await unitOfWork.OldInvoice.GetAllOldInvoicesFromLedgerAsync(dto);
            if (rows.Count == 0)
                return ApiResponse<List<OldInvoiceResponse>>.Builder()
                    .WithResult([]).WithErrors("BE-404")
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .WithMessage("No old invoices.")
                    .Build();
            return ApiResponse<List<OldInvoiceResponse>>.Builder()
                .WithResult(rows)
                .WithStatusCode((int)HttpStatusCode.OK)
                .WithMessage("Old invoice fetched successfully.")
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<OldInvoiceResponse>>(ex.Message);
        }
    }

    [HttpGet]
    [Route("by-transcode/{transactionCode}")]
    public async Task<ApiResponse<OldInvoiceResponse>> GetOldInvoiceFromLedgerAsync(string transactionCode)
    {
        try
        {
            var claim = Common.DecodeJwt(HttpContext.User);
            var row = await unitOfWork.OldInvoice.GetOldInvoiceByTransactionCode(claim.DbCode, transactionCode);
            if (row is not null)
                return ApiResponse<OldInvoiceResponse>.Builder()
                    .WithResult(row).WithErrors("BE-404")
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .WithMessage("No old invoices.")
                    .Build();
            return ApiResponse<OldInvoiceResponse>.Builder()
                .WithResult(new OldInvoiceResponse())
                .WithStatusCode((int)HttpStatusCode.OK)
                .WithMessage("Old invoice fetched successfully.")
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<OldInvoiceResponse>(ex.Message);
        }
    }

    [HttpPost]
    [Route("save")]
    public async Task<ApiResponse<int>> SaveOldInvoiceAsync([FromBody] List<OldInvoiceRequest> request)
    {
        try
        {
            var claim = Common.DecodeJwt(HttpContext.User);
            var dataTableTypes = request.Select(x => new OldInvoiceTableType(claim.DbCode, x.Code, claim.Period.ToInt(),
                x.CustomerCode
                , x.CustomerName, x.InvoiceValue, x.Employee, claim?.Username)).ToList();
            var rowsAffected = await unitOfWork.OldInvoice.SaveOldInvoice(dataTableTypes);
            if (rowsAffected <= 0)
                return ApiResponse<int>.Builder().WithErrors($"BE-{(int)HttpStatusCode.BadRequest}")
                    .WithStatusCode((int)HttpStatusCode.NotFound)
                    .WithMessage("No invoice found to update.")
                    .Build();
            return ApiResponse<int>.Builder()
                .WithResult(rowsAffected)
                .WithStatusCode((int)HttpStatusCode.OK)
                .WithMessage("Invoices saved successfully.")
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }

    [HttpPost("create-from-old-invoice")]
    public async Task<ApiResponse<OldInvoiceResponse>> CreateNewInvoiceFromOldInvoiceAsync(
        [FromBody] string transactionCode)
    {
        try
        {
            var claim = Common.DecodeJwt(HttpContext.User);
            var invoiceDto = new InvoiceDTO
            {
                DbCode = claim.DbCode,
                CreatedBy = claim.Username,
                EntryCode = claim.InvoiceEntryCode,
                TransactionCode = transactionCode.ToUpper()
            };
            // Update the status
            var rowsAffected = await unitOfWork.OldInvoice.RecreateOldInvoiceAsync(invoiceDto);
            if (rowsAffected <= 0)
                return ApiResponse<OldInvoiceResponse>.Builder()
                    .WithStatusCode((int)HttpStatusCode.NotFound)
                    .WithMessage("No invoice found to update.")
                    .Build();
            // Get the old invoice
            var oldInvoice = await unitOfWork.OldInvoice.GetOldInvoiceByTransactionCode(claim?.DbCode, transactionCode);
            return ApiResponse<OldInvoiceResponse>.Builder()
                .WithResult(oldInvoice)
                .WithStatusCode((int)HttpStatusCode.OK)
                .WithMessage("Invoices updated and retrieved successfully.")
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<OldInvoiceResponse>(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ApiResponse<int>> DeleteOldInvoiceAsync(int id)
    {
        try
        {
            var rowsAffected = await unitOfWork.OldInvoice.DeleteOldInvoiceByIdAsync(id);
            if (rowsAffected <= 0)
                return ApiResponse<int>.Builder()
                    .WithStatusCode((int)HttpStatusCode.NotFound)
                    .WithMessage("No invoice found to delete.")
                    .Build();
            return ApiResponse<int>.Builder()
                .WithResult(rowsAffected)
                .WithStatusCode((int)HttpStatusCode.OK)
                .WithMessage("Invoice deleted successfully.")
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }
}