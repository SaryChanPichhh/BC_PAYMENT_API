using System.ComponentModel.DataAnnotations;
using BC.PAYMENT.CORE.Contracts.Request.Invoice;
using BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DividingInvoices.Invoice;
using BC.PAYMENT.CORE.Enums;
using BC.PAYMENT.CORE.Contracts.Response.Invoice;

namespace BC.PAYMENT.API.Controllers.Invoice;

public class ChangeInvoiceController(IUnitOfWork unitOfWork) : BaseApiController
{
    [HttpGet]
    [Route("")]
    public async Task<ApiResponse<List<ChangeInvoiceResponse>>> GetChangeInvoicesAsync()
    {
        var credential = Common.DecodeJwt(HttpContext.User);
        try
        {
            var execute = await unitOfWork.ChangeInvoice.GetChangeInvoicesAsync(credential.DbCode!, DateTime.Today);
            if (execute.Count > 0)
                return ApiResponse<List<ChangeInvoiceResponse>>.Builder()
                    .WithMessage("Invoices fetched successfully")
                    .WithSuccess(true)
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(execute)
                    .Build();
            return ApiResponse<List<ChangeInvoiceResponse>>.Builder()
                .WithMessage("Invoices fetched unsuccessfully")
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .WithResult([])
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<ChangeInvoiceResponse>>(ex.Message);
        }
    }

    [HttpGet]
    [Route("exists-invoice/{transactionCode}")]
    public async Task<ApiResponse<bool>> CheckExistInvoiceAsync([Required] string transactionCode)
    {
        var credential = Common.DecodeJwt(HttpContext.User);
        try
        {
            var execute = await unitOfWork.ChangeInvoice.CheckExistInvoiceAsync(transactionCode, credential.DbCode!);
            return ApiResponse<bool>.Builder()
                .WithMessage("Invoice check executed successfully")
                .WithSuccess(execute)
                .WithStatusCode((int)HttpStatusCode.OK)
                .WithResult(execute)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<bool>(ex.Message);
        }
    }

    [HttpGet]
    [Route("local-invoice")]
    public async Task<ApiResponse<List<ChangeInvoiceResponse>>> GetLocalInvoiceAsync(
        [Required] [FromQuery] DateTime fromDate, [Required] [FromQuery] DateTime toDate)
    {
        var credential = Common.DecodeJwt(HttpContext.User);
        try
        {
            var execute = await unitOfWork.ChangeInvoice.GetLocalInvoiceAsync(credential.DbCode!, fromDate, toDate);
            if (execute.Count > 0)
                return ApiResponse<List<ChangeInvoiceResponse>>.Builder()
                    .WithMessage("Invoices fetched successfully")
                    .WithSuccess(true)
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(execute)
                    .Build();

            return ApiResponse<List<ChangeInvoiceResponse>>.Builder()
                .WithMessage("Invoices fetched unsuccessfully")
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .WithResult(new List<ChangeInvoiceResponse>())
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<ChangeInvoiceResponse>>(ex.Message);
        }
    }

    [HttpGet]
    [Route("invoice-send-from-head-office")]
    public async Task<ApiResponse<List<ChangeInvoiceResponse>>> GetOtherBranchesInvoiceAsync(
        [Required] [FromQuery] DateTime fromDate, [Required] [FromQuery] DateTime toDate)
    {
        var credential = Common.DecodeJwt(HttpContext.User);
        try
        {
            var execute =
                await unitOfWork.ChangeInvoice.GetOtherBranchInvoiceAsync(credential.DbCode!, fromDate, toDate);
            if (execute.Count > 0)
                return ApiResponse<List<ChangeInvoiceResponse>>.Builder()
                    .WithMessage("Invoices fetched successfully")
                    .WithSuccess(true)
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(execute)
                    .Build();
            else
                return ApiResponse<List<ChangeInvoiceResponse>>.Builder()
                    .WithMessage("Invoices fetched unsuccessfully")
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .WithResult(new List<ChangeInvoiceResponse>())
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<ChangeInvoiceResponse>>(ex.Message);
        }
    }

    [HttpPost]
    [Route("")]
    public async Task<ApiResponse<bool>> AddChangeInvoiceAsync(ChangeInvoiceRequest model)
    {
        var credential = Common.DecodeJwt(HttpContext.User);
        try
        {
            var changeInvoiceModel = new ChangeInvoiceModel
            {
                IsExists = model.IsExists,
                DbCode = credential.DbCode,
                UserName = credential.Username,
                Transaction = model.TransactionCode,
                CustomerCode = model.CustomerCode,
                CustomerName = model.CustomerName,
                InvoiceValue = model.InvoiceValue,
                EntriesCode = credential.InvoiceEntryCode
            };
            var affectedRow = await unitOfWork.ChangeInvoice.AddChangeInvoiceAsync(changeInvoiceModel);
            if (affectedRow > 0)
                return ApiResponse<bool>.Builder()
                    .WithMessage("Invoices added successfully")
                    .WithSuccess(true)
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(true)
                    .Build();
            return ApiResponse<bool>.Builder()
                .WithMessage("Invoices added unsuccessfully")
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .WithResult(false)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<bool>(ex.Message);
        }
    }
}