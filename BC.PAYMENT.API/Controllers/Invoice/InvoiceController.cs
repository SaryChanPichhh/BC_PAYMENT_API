using System.ComponentModel.DataAnnotations;
using BC.PAYMENT.CORE.Contracts.Request.Invoice;
using BC.PAYMENT.CORE.Contracts.Response.Invoice;
using BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DividingInvoices.Invoice;
using BC.PAYMENT.CORE.Enums;

namespace BC.PAYMENT.API.Controllers.Invoice;

public class InvoiceController(IUnitOfWork unitOfWork) : BaseApiController
{
    [HttpGet]
    [Route("invoice-detail-by-deliveryId-and-date")]
    public async Task<ApiResponse<List<DividedInvoiceTransactionResponse>>> GetInvoiceTransactionByDeliveryAndDateAsync(
        [Required] [FromQuery] string deliveryId, [Required] [FromQuery] DateTime date)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var execute =
                await unitOfWork.Invoices.GetInvoiceTransactionByDeliveryAndDateAsync(credential.DbCode!, deliveryId,
                    date);
            if (execute.Count != 0)
                return ApiResponse<List<DividedInvoiceTransactionResponse>>.Builder()
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithMessage("Invoice transaction fetched successfully")
                    .WithSuccess(true)
                    .WithResult(execute)
                    .Build();
            return ApiResponse<List<DividedInvoiceTransactionResponse>>.Builder()
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .WithMessage("Invoice transaction fetched unsuccessfully")
                .WithResult([])
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<DividedInvoiceTransactionResponse>>(ex.Message);
        }
    }

    [HttpPost]
    [Route("post-invoice")]
    public async Task<ApiResponse<bool>> PostInvoiceAsync([Required] string transactionCode,
        [Required] RequestType type)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var execute = await unitOfWork.Invoices.PostPrintInvoiceAsync(
                transactionCode,
                type,
                credential.DbCode,
                credential.Period,
                credential.Username);
            if (execute)
                return ApiResponse<bool>.Builder()
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithMessage("Invoice posted successfully")
                    .WithSuccess(true)
                    .WithResult(true)
                    .Build();
            return ApiResponse<bool>.Builder()
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .WithMessage("Invoice posted unsuccessfully")
                .WithSuccess(false)
                .WithResult(false)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<bool>(ex.Message);
        }
    }

    [HttpGet]
    [Route("is-already-posted")]
    public async Task<ApiResponse<bool>> PostInvoiceAsync([Required] string transactionCode)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var isAlreadyPosted = await unitOfWork.Invoices.IsAlreadyPosted(credential.DbCode, transactionCode);
            if (isAlreadyPosted)
                return ApiResponse<bool>.Builder()
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithMessage("Invoice posted successfully")
                    .WithSuccess(true)
                    .WithResult(true)
                    .Build();
            return ApiResponse<bool>.Builder()
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .WithMessage("Invoice posted unsuccessfully")
                .WithSuccess(false)
                .WithResult(false)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<bool>(ex.Message);
        }
    }

    [HttpPost]
    [Route("save-post-invoice")]
    public async Task<ApiResponse<bool>> SaveRecordPostInvoice([Required] int invoiceId,
        [Required] string transactionCode, [Required] string transCode)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var isAlreadyPosted = await unitOfWork.Invoices.SaveRecordPostInvoice(credential.DbCode,
                credential.Username, invoiceId, transactionCode, transCode);
            if (isAlreadyPosted)
                return ApiResponse<bool>.Builder()
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithMessage("Invoice posted successfully")
                    .WithSuccess(true)
                    .WithResult(true)
                    .Build();
            return ApiResponse<bool>.Builder()
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .WithMessage("Invoice posted unsuccessfully")
                .WithSuccess(false)
                .WithResult(false)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<bool>(ex.Message);
        }
    }
}