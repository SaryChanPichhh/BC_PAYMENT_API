using System.ComponentModel.DataAnnotations;
using BC.PAYMENT.CORE.Contracts.Request.StockCar;
using BC.PAYMENT.CORE.Contracts.Response.StockCar;
using BC.PAYMENT.CORE.Entities.StockCar;

namespace BC.PAYMENT.API.Controllers.StockCar;

public class StockCarExpenseController(IUnitOfWork unitOfWork) : BaseApiController
{
    [HttpGet]
    [Route("by-template/{templateId:int}")]
    public async Task<ApiResponse<List<StockCarExpenseResponse>>> GetAllByTemplateIdAsync([Required] int templateId)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var data = await unitOfWork.StockCarExpense.GetAllByTemplateIdAsync(credential?.DbCode, templateId);
            if (data.Count > 0)
                return ApiResponse<List<StockCarExpenseResponse>>.Builder()
                    .WithMessage("data fetched successfully.")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(data)
                    .Build();
            return ApiResponse<List<StockCarExpenseResponse>>.Builder()
                .WithMessage("data fetched empty.")
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<StockCarExpenseResponse>>(ex.Message);
        }
    }

    [HttpGet]
    [Route("by-template/{templateId:int}/{expenseId:int}")]
    public async Task<ApiResponse<StockCarExpenseResponse>> GetByTemplateIdAndExpenseIdAsync([Required] int templateId,
        [Required] int expenseId)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var data = await unitOfWork.StockCarExpense.GetByTemplateIdAndExpenseIdAsync(credential?.DbCode, templateId,
                expenseId);
            if (data != null)
                return ApiResponse<StockCarExpenseResponse>.Builder()
                    .WithMessage("data fetched successfully.")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(data)
                    .Build();
            return ApiResponse<StockCarExpenseResponse>.Builder()
                .WithMessage("data fetched empty.")
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<StockCarExpenseResponse>(ex.Message);
        }
    }

    [HttpGet]
    [Route("total/by-template/{templateId:int}")]
    public async Task<ApiResponse<TotalStockCarExpenseResponse>> GetTotalExpenseByTemplateIdAsync(
        [Required] int templateId)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var data = await unitOfWork.StockCarExpense.GetTotalExpenseByTemplateIdAsync(credential?.DbCode,
                templateId);
            if (data != null)
                return ApiResponse<TotalStockCarExpenseResponse>.Builder()
                    .WithMessage("data fetched successfully.")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(data)
                    .Build();
            return ApiResponse<TotalStockCarExpenseResponse>.Builder()
                .WithMessage("data fetched empty.")
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<TotalStockCarExpenseResponse>(ex.Message);
        }
    }

    [HttpPost]
    [Route("")]
    public async Task<ApiResponse<int>> SaveAsync([FromBody] CreateStockCarExpenseRequest req)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var model = new StockCarExpense
            {
                TemplateId = req.TemplateId,
                ExpenseTypeId = req.ExpenseTypeId,
                ProvinceId = req.ProvinceId,
                Quantity = req.Quantity,
                UnitPrice = req.UnitPrice,
                AmountDollar = req.AmountDollar,
                AmountRiel = req.AmountRiel,
                ExchangeRate = req.ExchangeRate,
                ExpenseDate = req.ExpenseDate,
                CreatedDate = credential?.CurrectDate ?? DateTime.Now,
                CreatedBy = credential?.Username ?? string.Empty,
                DbCode = credential?.DbCode ?? string.Empty
            };
            var data = await unitOfWork.StockCarExpense.SaveAsync(model);
            if (data > 0)
                return ApiResponse<int>.Builder()
                    .WithMessage("data added successfully.")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(data)
                    .Build();
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
    public async Task<ApiResponse<int>> UpdateAsync([FromBody] UpdateStockCarExpenseRequest req)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var model = new StockCarExpense
            {
                Id = req.Id,
                ExpenseTypeId = req.ExpenseTypeId,
                ProvinceId = req.ProvinceId,
                Quantity = req.Quantity,
                UnitPrice = req.UnitPrice,
                AmountDollar = req.AmountDollar,
                AmountRiel = req.AmountRiel,
                ExchangeRate = req.ExchangeRate,
                ExpenseDate = req.ExpenseDate
            };
            var data = await unitOfWork.StockCarExpense.UpdateAsync(model);
            if (data > 0)
                return ApiResponse<int>.Builder()
                    .WithMessage("data updated successfully.")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(data)
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

    [HttpDelete]
    [Route("{expenseId:int}")]
    public async Task<ApiResponse<int>> DeleteAsync([Required] int expenseId)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var data = await unitOfWork.StockCarExpense.DeleteAsync(expenseId);
            if (data > 0)
                return ApiResponse<int>.Builder()
                    .WithMessage("data deleted successfully.")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(data)
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
}