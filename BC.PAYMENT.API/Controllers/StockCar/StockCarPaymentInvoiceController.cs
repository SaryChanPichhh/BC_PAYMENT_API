using System.ComponentModel.DataAnnotations;
using BC.PAYMENT.CORE.Contracts.Request.StockCar;
using BC.PAYMENT.CORE.Contracts.Response.StockCar;
using BC.PAYMENT.CORE.Entities.StockCar;

namespace BC.PAYMENT.API.Controllers.StockCar;

public class StockCarPaymentInvoiceController(IUnitOfWork unitOfWork) : BaseApiController
{
    [HttpGet]
    [Route("by-template/{templateId:int}")]
    public async Task<ApiResponse<List<PaymentInvoiceResponse>>> GetPaymentInvoicesByTemplateIdAsync(
        [Required] int templateId)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var data = await unitOfWork.StockCarPaymentInvoice.GetPaymentInvoicesByTemplateIdAsync(credential?.DbCode,
                templateId);
            if (data.Count > 0)
                return ApiResponse<List<PaymentInvoiceResponse>>.Builder()
                    .WithMessage("data fetched successfully.")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(data)
                    .Build();
            return ApiResponse<List<PaymentInvoiceResponse>>.Builder()
                .WithMessage("data fetched empty.")
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<PaymentInvoiceResponse>>(ex.Message);
        }
    }

    [HttpPost]
    [Route("")]
    [Route("payment-invoice")]
    public async Task<ApiResponse<int>> PaymentInvoiceAsync([FromBody] CreateStockCarPaymentInvoiceRequest req)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var createdBy = !string.IsNullOrWhiteSpace(req.CreatedBy)
                ? req.CreatedBy
                : credential?.Username ?? string.Empty;

            var data = await unitOfWork.StockCarPaymentInvoice.PaymentInvoiceAsync(req.InvoiceId, req.AmountPaid,
                createdBy);
            if (data > 0)
                return ApiResponse<int>.Builder()
                    .WithMessage("payment recorded successfully.")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(data)
                    .Build();
            return ApiResponse<int>.Builder()
                .WithMessage("payment recorded unsuccessfully.")
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }

    [HttpDelete]
    [Route("{invoiceId:int}")]
    [Route("by-invoice/{invoiceId:int}")]
    public async Task<ApiResponse<int>> DeletePaymentInvoiceByInvoiceIdAsync([Required] int invoiceId)
    {
        try
        {
            var affectedRows = await unitOfWork.StockCarPaymentInvoice.DeletePaymentInvoiceByInvoiceIdAsync(invoiceId);
            if (affectedRows > 0)
                return ApiResponse<int>.Builder()
                    .WithMessage("data deleted successfully.")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(affectedRows)
                    .Build();
            return ApiResponse<int>.Builder()
                .WithMessage("data deleted unsuccessfully.")
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }

    [HttpPut]
    [Route("")]
    [Route("by-id")]
    public async Task<ApiResponse<int>> UpdatePaymentInvoiceByIdAsync(
        [FromBody] UpdateStockCarPaymentInvoiceRequest req)
    {
        try
        {
            var affectedRows =
                await unitOfWork.StockCarPaymentInvoice.UpdatePaymentInvoiceByIdAsync(req.Id, req.Amount);
            if (affectedRows > 0)
                return ApiResponse<int>.Builder()
                    .WithMessage("data updated successfully.")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(affectedRows)
                    .Build();
            return ApiResponse<int>.Builder()
                .WithMessage("data updated unsuccessfully.")
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }

    [HttpGet]
    [Route("payment-history/{invoiceId:int}")]
    public async Task<ApiResponse<List<InvoicesPayment>>> GetPaymentHistoryByInvoiceIdAsync([Required] int invoiceId)
    {
        try
        {
            var data = await unitOfWork.StockCarPaymentInvoice.GetPaymentHistoryByInvoiceIdAsync(invoiceId);
            if (data.Count > 0)
                return ApiResponse<List<InvoicesPayment>>.Builder()
                    .WithMessage("data fetched successfully.")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(data)
                    .Build();
            return ApiResponse<List<InvoicesPayment>>.Builder()
                .WithMessage("data fetched empty.")
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<InvoicesPayment>>(ex.Message);
        }
    }
}