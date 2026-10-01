using BC.PAYMENT.API.Mapper.Invoice;
using BC.PAYMENT.CORE.Contracts.Request.Invoice;
using BC.PAYMENT.CORE.Contracts.Response.Invoice;
using BC.PAYMENT.CORE.Entities;
using BC.PAYMENT.CORE.Enums;

namespace BC.PAYMENT.API.Controllers.Invoice;

public class NewInvoiceController(IUnitOfWork unitOfWork) : BaseApiController
{
    [HttpGet]
    [Route("")]
    public async Task<ApiResponse<List<NewInvoiceResponse>>> GetInvoicesAsync()
    {
        var credential = Common.DecodeJwt(HttpContext.User);
        try
        {
            var execute =
                await unitOfWork.NewInvoice.GetInvoices(InvoiceStatus.NewInvoice, credential.DbCode!, DateTime.Today);
            if (execute.Count > 0)
                return ApiResponse<List<NewInvoiceResponse>>.Builder()
                    .WithMessage("New Invoices fetched successfully")
                    .WithSuccess(true)
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(execute)
                    .Build();
            return ApiResponse<List<NewInvoiceResponse>>.Builder()
                .WithMessage("New Invoices fetched unsuccessfully")
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .WithResult(new List<NewInvoiceResponse>())
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<NewInvoiceResponse>>(ex.Message);
        }
    }

    [HttpGet]
    [Route("by-sale-types")]
    public async Task<ApiResponse<List<NewInvoiceResponse>>> GetInvoicesByInvoiceCodeAsync
    ([FromQuery] string invoiceCode1, [FromQuery] string invoiceCode2, [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate, [FromQuery] bool isAutoSave
    )
    {
        var credential = Common.DecodeJwt(HttpContext.User);
        try
        {
            var data = await unitOfWork.NewInvoice.GetInvoicesByInvoiceCode(invoiceCode1, invoiceCode2, fromDate,
                toDate, credential.DbCode!);
            data.ForEach(invoice =>
            {
                invoice.DbCode = credential.DbCode;
                invoice.CreatedBy = credential.Username;
                invoice.InvoiceType = InvoiceStatus.NewInvoice;
                invoice.EntriesCode = credential.InvoiceEntryCode;
            });
            var req = data.Select(NewInvoiceMapper.FromResponseToModel).ToList();
            var affectedRow = 0;
            if (isAutoSave)
                affectedRow = await unitOfWork.NewInvoice.SaveInvoices(req);
            if (affectedRow > 0 || !isAutoSave)
                return ApiResponse<List<NewInvoiceResponse>>.Builder()
                    .WithMessage("New Invoices added successfully")
                    .WithSuccess(true)
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(data)
                    .Build();
            return ApiResponse<List<NewInvoiceResponse>>.Builder()
                .WithMessage("New Invoices added unsuccessfully")
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .WithResult([])
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<NewInvoiceResponse>>(ex.Message);
        }
    }

    [HttpPost]
    [Route("")]
    public async Task<ApiResponse<int>> AddNewInvoiceAsync([FromBody] List<NewInvoiceRequest> req
    )
    {
        var credential = Common.DecodeJwt(HttpContext.User);
        try
        {
            var data = req.Select(NewInvoiceMapper.FromRequestToModel).ToList();
            data.ForEach(invoice =>
            {
                invoice.DbCode = credential.DbCode;
                invoice.CreatedBy = credential.Username;
                invoice.InvoiceType = InvoiceStatus.NewInvoice;
                invoice.EntriesCode = credential.InvoiceEntryCode;
            });
            var affectedRow = await unitOfWork.NewInvoice.SaveInvoices(data);
            if (affectedRow > 0)
                return ApiResponse<int>.Builder()
                    .WithMessage("New Invoices added successfully")
                    .WithSuccess(true)
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(affectedRow)
                    .Build();
            return ApiResponse<int>.Builder()
                .WithMessage("New Invoices added unsuccessfully")
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .WithResult(affectedRow)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }

    [HttpDelete]
    [Route("{invoiceId:int}")]
    public async Task<ApiResponse<bool>> DeleteInvoiceAsync(int invoiceId)
    {
        try
        {
            var affectedRow = await unitOfWork.NewInvoice.DeleteInvoiceAsync(invoiceId);
            var isSuccess = affectedRow > 0;
            return ApiResponse<bool>.Builder()
                .WithMessage(isSuccess ? "New Invoices deleted successfully" : "New Invoices deleted unsuccessfully")
                .WithSuccess(isSuccess)
                .WithStatusCode(isSuccess ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest)
                .WithResult(isSuccess)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<bool>(ex.Message);
        }
    }

    [HttpPut]
    [Route("update-trans-value")]
    public async Task<ApiResponse<bool>> UpdateHeaderValueAsync([FromBody] UpdateHeaderValueRequest request)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var data = new PcEditDividedInvoice
            {
                DividedId = request.DividedId,
                CreatedBy = credential.Username,
                NewAmount = request.NewAmount,
                OldAmount = request.OldAmount,
                DbCode = credential.DbCode,
                CreateDate = credential.CurrectDate,
                Description = request.Description
            };
            var affectedRow = await unitOfWork.NewInvoice.UpdateHeaderValueAsync(data);
            if (affectedRow > 0)
                return ApiResponse<bool>.Builder()
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithMessage("Header value updated successfully")
                    .WithResult(affectedRow > 0)
                    .Build();

            return ApiResponse<bool>.Builder()
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .WithMessage("Header value updated unsuccessfully")
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<bool>(ex.Message);
        }
    }
}