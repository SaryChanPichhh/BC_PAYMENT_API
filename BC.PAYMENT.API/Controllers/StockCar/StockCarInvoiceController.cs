using System.ComponentModel.DataAnnotations;
using BC.PAYMENT.CORE.Contracts.Request.StockCar;
using BC.PAYMENT.CORE.Contracts.Response.StockCar;
using BC.PAYMENT.CORE.Entities;
using BC.PAYMENT.CORE.Enums;

namespace BC.PAYMENT.API.Controllers.StockCar;

public class StockCarInvoiceController(IUnitOfWork unitOfWork) : BaseApiController
{
    [HttpGet]
    [Route("by-invoiceType")]
    public async Task<ApiResponse<List<StockCarInvoiceResponse>>> PostSubmittedInvoiceAsync([FromQuery] int templateId,[FromQuery] InvoiceStatus invoiceStatus)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var data = await unitOfWork.StockCarInvoice
                .GetStockCarInvoicesByInvoiceTypeAsync(credential?.DbCode, templateId, invoiceStatus);
            if (data.Count > 0)
            {
                return ApiResponse<List<StockCarInvoiceResponse>>.Builder()
                    .WithMessage("data fetched successfully.")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(data)
                    .Build();
            }
            return ApiResponse<List<StockCarInvoiceResponse>>.Builder()
                .WithMessage("data fetched empty.")
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<StockCarInvoiceResponse>>(ex.Message);
        }
    }

    [HttpGet]
    [Route("payment-invoices/by-template/{templateId:int}")]
    public async Task<ApiResponse<List<PaymentInvoiceResponse>>> GetPaymentInvoicesByTemplateIdAsync([Required] int templateId)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var data = await unitOfWork.StockCarInvoice
                .GetPaymentInvoicesByTemplateIdAsync(credential?.DbCode, templateId);
            if (data.Count > 0)
            {
                return ApiResponse<List<PaymentInvoiceResponse>>.Builder()
                    .WithMessage("data fetched successfully.")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(data)
                    .Build();
            }
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
    
    [HttpDelete]
    [Route("{id:int}")]
    public async Task<ApiResponse<int>> DeleteStockCarInvoiceByIdAsync([Required] int id)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var affectedRow = await unitOfWork.StockCarInvoice
                .DeleteStockCarInvoiceByIdAsync(id);
            if (affectedRow > 0)
            {
                return ApiResponse<int>.Builder()
                    .WithMessage("data deleted successfully.")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(affectedRow)
                    .Build();
            }
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
    
    [HttpPost]
    [Route("")]
    public async Task<ApiResponse<int>> AddNewStockCarInvoiceASync([FromBody] CreateBcStockCarRequest request)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var model = new BcStockCar
            {
                DbCode = credential.DbCode,
                CustomerCode = request.CustomerCode,
                CustomerName = request.CustomerName,
                Code = request.TransactionCode,
                Value = request.InvoiceValue,
                Type = Enum.GetName(typeof(InvoiceStatus), request.InvoiceType)[..1],
                Period = credential.Period,
                TransactionDate = request.TransactionDate,
                Status = true,
                CreatedDate = credential.CurrectDate,
                CreatedBy = credential.Username,
                TemplateId = request.TemplateId
            };
            var affectedRow = await unitOfWork.StockCarInvoice
                .AddNewStockCarInvoiceAsync(model);
            if (affectedRow > 0)
            {
                return ApiResponse<int>.Builder()
                    .WithMessage("data added successfully.")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(affectedRow)
                    .Build();
            }
            return ApiResponse<int>.Builder()
                .WithMessage("data added unsuccessfully.")
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
    public async Task<ApiResponse<int>> UpdateInvoiceByIdAsync([FromBody] UpdateStockCarInvoiceRequest request)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var model = new BcStockCar
            {
                Id = request.Id,
                CustomerCode = request.CustomerCode,
                CustomerName = request.CustomerName,
                Value = request.Value
            };
            var affectedRow = await unitOfWork.StockCarInvoice
                .UpdateInvoiceByIdAsync(model);
            if (affectedRow > 0)
            {
                return ApiResponse<int>.Builder()
                    .WithMessage("data updated successfully.")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(affectedRow)
                    .Build();
            }
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
}