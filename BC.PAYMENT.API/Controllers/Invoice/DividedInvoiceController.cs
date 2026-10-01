using System.ComponentModel.DataAnnotations;
using BC.PAYMENT.CORE.Contracts.Invoice;
using BC.PAYMENT.CORE.Contracts.Request.Invoice;
using BC.PAYMENT.CORE.Contracts.Response.Invoice;

namespace BC.PAYMENT.API.Controllers.Invoice;

public class DividedInvoiceController(IUnitOfWork unitOfWork) : BaseApiController
{
    [HttpGet]
    [Route("by-area-and-transcode")]
    public async Task<ApiResponse<List<InvoiceResponse>>> GetInvoiceByAreaAndTransCodeAsync([FromQuery] string areaId,
        [FromQuery] string transCode = "")
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var execute =
                await unitOfWork.Invoices.GetInvoiceByAreaAndTransCodeAsync(credential.DbCode!, areaId, transCode);
            if (execute.Count != 0)
                return ApiResponse<List<InvoiceResponse>>.Builder()
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithMessage("Invoices fetched successfully")
                    .WithSuccess(true)
                    .WithResult(execute)
                    .Build();
            return ApiResponse<List<InvoiceResponse>>.Builder()
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .WithSuccess(true)
                .WithMessage("Invoices fetched unsuccessfully")
                .WithResult([])
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<InvoiceResponse>>(ex.Message);
        }
    }

    [HttpPost]
    [Route("")]
    public async Task<ApiResponse<int>> SaveDividedInvoiceAsync([FromBody] List<CreateDividedInvoiceRequest> request)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var rowsAffected = await unitOfWork.DividedInvoice.SaveDividedInvoiceAsync(
                request,
                credential.DbCode!,
                credential.Username!);
            if (rowsAffected <= 0)
                return ApiResponse<int>.Builder()
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .WithMessage("No divided invoice saved.")
                    .Build();
            return ApiResponse<int>.Builder()
                .WithStatusCode((int)HttpStatusCode.OK)
                .WithMessage("Divided invoices saved successfully.")
                .Success()
                .WithResult(rowsAffected)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }

    [HttpGet]
    [Route("by-deliveryId-and-date")]
    public async Task<ApiResponse<List<DividedInvoiceResponse>>> GetDividedInvoiceByDeliverIdAndDate(
        [Required] [FromQuery] string deliverId, [Required] [FromQuery] DateTime date)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var execute =
                await unitOfWork.DividedInvoice.GetDividedInvoicesByDeliveryIdAndDateAsync(credential.DbCode!,
                    deliverId, date);
            if (execute.Count != 0)
                return ApiResponse<List<DividedInvoiceResponse>>.Builder()
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithMessage($@"Divided Invoice fetched successfully")
                    .Success()
                    .WithResult(execute)
                    .Build();

            return ApiResponse<List<DividedInvoiceResponse>>.Builder()
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .WithMessage($@"Divided Invoice fetched unsuccessfully")
                .WithResult([])
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<DividedInvoiceResponse>>(ex.Message);
        }
    }

    [HttpGet]
    [Route("detail-invoice-by-date")]
    public async Task<ApiResponse<List<DividedInvoiceDetailResponse>>> GetDividedInvoiceDetailByDateAsync(
        [Required] [FromQuery] DateTime fromDate, [Required] [FromQuery] DateTime toDate)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var execute =
                await unitOfWork.DividedInvoice.GetDividedInvoiceDetailByDateAsync(credential.DbCode!,
                    fromDate, toDate);
            if (execute.Count != 0)
                return ApiResponse<List<DividedInvoiceDetailResponse>>.Builder()
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithMessage($@"Divided Invoice fetched successfully")
                    .Success()
                    .WithResult(execute)
                    .Build();
            return ApiResponse<List<DividedInvoiceDetailResponse>>.Builder()
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .WithMessage($@"Divided Invoice fetched unsuccessfully")
                .WithResult([])
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<DividedInvoiceDetailResponse>>(ex.Message);
        }
    }

    [HttpDelete]
    [Route("")]
    public async Task<ApiResponse<bool>> DeleteDividedInvoiceByDeliverIdAndDate(
        [FromBody] DeleteDividedInvoiceRequest model)
    {
        try
        {
            var affectedRow =
                await unitOfWork.DividedInvoice.DeleteDividedInvoiceAsync(model.InvoiceId, model.TransactionCode,
                    model.DeliveryId, model.Note);
            if (affectedRow > 0)
                return ApiResponse<bool>.Builder()
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithMessage($@"Divided Invoice deleted successfully")
                    .Success()
                    .WithResult(true)
                    .Build();
            return ApiResponse<bool>.Builder()
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .WithMessage($@"Divided Invoice deleted unsuccessfully")
                .WithResult(false)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<bool>(ex.Message);
        }
    }

    [HttpGet]
    [Route("divided-invoice-by-date")]
    public async Task<ApiResponse<List<DividedInvoiceSummaryResponse>>> GetInvoiceReportAsync([Required] DateTime date)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var execute =
                await unitOfWork.DividedInvoice.GetDividedInvoiceReportAsync(credential.DbCode!, date);
            if (execute.Count != 0)
                return ApiResponse<List<DividedInvoiceSummaryResponse>>.Builder()
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithMessage($@"Invoice fetched successfully")
                    .WithSuccess(true)
                    .WithResult(execute)
                    .Build();
            return ApiResponse<List<DividedInvoiceSummaryResponse>>.Builder()
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .WithMessage($@"Invoice fetched unsuccessfully")
                .WithResult([])
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<DividedInvoiceSummaryResponse>>(ex.Message);
        }
    }

    [HttpGet]
    [Route("invoice-status-by-date")]
    public async Task<ApiResponse<List<DividedInvoiceStatusResponse>>> GetDividedInvoiceStatusByDateAsync(
        [Required] DateTime date, [Required] string deliveryId)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var execute =
                await unitOfWork.DividedInvoice.GetDividedInvoiceStatusByDateAsync(credential.DbCode!, date,
                    deliveryId);
            if (execute.Count != 0)
                return ApiResponse<List<DividedInvoiceStatusResponse>>.Builder()
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithMessage($@"Invoice status fetched successfully")
                    .WithSuccess(true)
                    .WithResult(execute)
                    .Build();
            return ApiResponse<List<DividedInvoiceStatusResponse>>.Builder()
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .WithMessage($@"Invoice status fetched unsuccessfully")
                .WithResult([])
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<DividedInvoiceStatusResponse>>(ex.Message);
        }
    }
}