using System.ComponentModel.DataAnnotations;
using BC.PAYMENT.CORE.Contracts.Request.Invoice;
using BC.PAYMENT.CORE.Contracts.Response.Invoice;
using BC.PAYMENT.CORE.Entities;

namespace BC.PAYMENT.API.Controllers.Invoice;

public class ReturnInvoiceController(IUnitOfWork unitOfWork) : BaseApiController
{
    [HttpGet]
    [Route("")]
    public async Task<ApiResponse<List<ReturnInvoiceResponse>>> GetReturnInvoiceAsync()
    {
        var credential = Common.DecodeJwt(HttpContext.User);
        try
        {
            var execute = await unitOfWork.ReturnInvoice.GetReturnInvoiceAsync(credential.DbCode!);
            if (execute.Count > 0)
                return ApiResponse<List<ReturnInvoiceResponse>>.Builder()
                    .WithMessage("Return Invoices fetched successfully")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(execute)
                    .Build();

            return ApiResponse<List<ReturnInvoiceResponse>>.Builder()
                .WithMessage("Return Invoices fetched unsuccessfully")
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .WithResult(new List<ReturnInvoiceResponse>())
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<ReturnInvoiceResponse>>(ex.Message);
        }
    }

    [HttpPost]
    [Route("save")]
    public async Task<ApiResponse<int>> SaveReturnInvoiceAsync([FromBody] List<ReturnInvoiceRequest> request)
    {
        try
        {
            var claim = Common.DecodeJwt(HttpContext.User);
            var rowsAffected = await unitOfWork.ReturnInvoice.SaveReturnInvoiceAsync(
                request,
                claim.DbCode!,
                claim.Username!,
                claim.InvoiceEntryCode!);

            if (rowsAffected <= 0)
                return ApiResponse<int>.Builder()
                    .WithErrors($"BE-{(int)HttpStatusCode.BadRequest}")
                    .WithStatusCode((int)HttpStatusCode.NotFound)
                    .WithMessage("No invoice found to save.")
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

    [HttpGet]
    [Route("return-invoices")]
    public async Task<ApiResponse<List<PcReturnInvoice>>> GetPcReturnInvoicesAsync()
    {
        var credential = Common.DecodeJwt(HttpContext.User);
        try
        {
            var execute = await unitOfWork.ReturnInvoice.GetPcReturnInvoiceAsync(credential?.DbCode);
            if (execute.Count > 0)
                return ApiResponse<List<PcReturnInvoice>>.Builder()
                    .WithMessage("PC Return Invoices fetched successfully")
                    .WithSuccess(true)
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(execute)
                    .Build();
            return ApiResponse<List<PcReturnInvoice>>.Builder()
                .WithMessage("PC Return Invoices fetched unsuccessfully")
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .WithResult(new List<PcReturnInvoice>())
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<PcReturnInvoice>>(ex.Message);
        }
    }

    [HttpGet]
    [Route("by-date")]
    public async Task<ApiResponse<List<ReturnInvoiceResponse>>> GetPcReturnInvoiceByDateAsync(
        [Required] [FromQuery] DateTime fromDate,
        [Required] [FromQuery] DateTime toDate)
    {
        var credential = Common.DecodeJwt(HttpContext.User);
        try
        {
            var execute =
                await unitOfWork.ReturnInvoice.GetPcReturnInvoiceByDateAsync(credential.DbCode!, fromDate, toDate);
            if (execute.Count > 0)
                return ApiResponse<List<ReturnInvoiceResponse>>.Builder()
                    .WithMessage("PC Return Invoices fetched successfully")
                    .WithSuccess(true)
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(execute)
                    .Build();
            return ApiResponse<List<ReturnInvoiceResponse>>.Builder()
                .WithMessage("PC Return Invoices fetched unsuccessfully")
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .WithResult(new List<ReturnInvoiceResponse>())
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<ReturnInvoiceResponse>>(ex.Message);
        }
    }

    [HttpGet]
    [Route("by-period")]
    public async Task<ApiResponse<List<ReturnInvoiceResponse>>> GetPcReturnInvoiceByPeriodAsync(
        [Required] [FromQuery] string month,
        [Required] [FromQuery] string year)
    {
        var credential = Common.DecodeJwt(HttpContext.User);
        try
        {
            var execute =
                await unitOfWork.ReturnInvoice.GetPcReturnInvoiceByPeriodAsync(credential.DbCode!, month, year);
            if (execute.Count > 0)
                return ApiResponse<List<ReturnInvoiceResponse>>.Builder()
                    .WithMessage("PC Return Invoices fetched successfully")
                    .WithSuccess(true)
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(execute)
                    .Build();

            return ApiResponse<List<ReturnInvoiceResponse>>.Builder()
                .WithMessage("PC Return Invoices fetched unsuccessfully")
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .WithResult(new List<ReturnInvoiceResponse>())
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<ReturnInvoiceResponse>>(ex.Message);
        }
    }

    [HttpDelete]
    [Route("{dividedId:int}")]
    public async Task<ApiResponse<int>> DeletePcReturnInvoiceAsync(int dividedId)
    {
        try
        {
            var affectedRow = await unitOfWork.ReturnInvoice.DeletePcReturnInvoiceAsync(dividedId);
            if (affectedRow > 0)
                return ApiResponse<int>.Builder()
                    .WithMessage("PC Return Invoice deleted successfully")
                    .WithSuccess(true)
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(affectedRow)
                    .Build();

            return ApiResponse<int>.Builder()
                .WithMessage("PC Return Invoice deleted unsuccessfully")
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .WithResult(0)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }

    [HttpPost]
    [Route("PcReturningInvoiceAudit")]
    public async Task<ApiResponse<int>> PcReturningInvoiceAuditAsync(
        [FromBody] CreatePcReturnInvoiceAuditRequest request)
    {
        var credential = Common.DecodeJwt(HttpContext.User);
        try
        {
            var auditModel = new PcReturningInvoiceAudit
            {
                ReturnId = request.ReturnId,
                ProcessingStatus = request.ProcessingStatus != 0 ? request.ProcessingStatus : 1,
                ApprovalStatus = request.ApprovalStatus ?? string.Empty,
                LastUpdatedDate = request.LastUpdatedDate ?? DateTime.Now,
                LastUpdatedBy = !string.IsNullOrWhiteSpace(request.LastUpdatedBy)
                    ? request.LastUpdatedBy
                    : credential?.Username ?? string.Empty
            };
            var affectedRow = await unitOfWork.ReturnInvoice.InsertGetReturnInvoice(auditModel);
            if (affectedRow > 0)
                return ApiResponse<int>.Builder()
                    .WithMessage("PC Returning Invoice Audit inserted successfully")
                    .WithSuccess(true)
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(affectedRow)
                    .Build();

            return ApiResponse<int>.Builder()
                .WithMessage("PC Returning Invoice Audit inserted unsuccessfully")
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .WithResult(0)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }

    [HttpGet]
    [Route("PcReturnInvoiceAudit/by-date")]
    public async Task<ApiResponse<List<PcReturnInvoiceAuditResponse>>> GetReturnChangeInvoiceByDateAsync(
        [Required] [FromQuery] DateTime fromDate,
        [Required] [FromQuery] DateTime toDate)
    {
        var credential = Common.DecodeJwt(HttpContext.User);
        try
        {
            var execute =
                await unitOfWork.ReturnInvoice.GetReturnChangeInvoiceByDateAsync(credential.DbCode!, fromDate, toDate);
            if (execute.Count > 0)
                return ApiResponse<List<PcReturnInvoiceAuditResponse>>.Builder()
                    .WithMessage("PC Return Invoice Audit fetched successfully")
                    .WithSuccess(true)
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(execute)
                    .Build();

            return ApiResponse<List<PcReturnInvoiceAuditResponse>>.Builder()
                .WithMessage("PC Return Invoice Audit fetched unsuccessfully")
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .WithResult(new List<PcReturnInvoiceAuditResponse>())
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<PcReturnInvoiceAuditResponse>>(ex.Message);
        }
    }

    [HttpGet]
    [Route("PcReturnInvoiceAudit/by-period")]
    public async Task<ApiResponse<List<PcReturnInvoiceAuditResponse>>> GetReturnChangeInvoiceByPeriodAsync(
        [Required] [FromQuery] int month,
        [Required] [FromQuery] int year)
    {
        var credential = Common.DecodeJwt(HttpContext.User);
        try
        {
            var execute =
                await unitOfWork.ReturnInvoice.GetReturnChangeInvoiceByPeriodAsync(credential.DbCode!, month, year);
            if (execute.Count > 0)
                return ApiResponse<List<PcReturnInvoiceAuditResponse>>.Builder()
                    .WithMessage("PC Return Invoice Audit fetched successfully")
                    .WithSuccess(true)
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(execute)
                    .Build();

            return ApiResponse<List<PcReturnInvoiceAuditResponse>>.Builder()
                .WithMessage("PC Return Invoice Audit fetched unsuccessfully")
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .WithResult(new List<PcReturnInvoiceAuditResponse>())
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<PcReturnInvoiceAuditResponse>>(ex.Message);
        }
    }
}