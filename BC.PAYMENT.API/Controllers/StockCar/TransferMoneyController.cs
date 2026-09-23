using System.ComponentModel.DataAnnotations;
using BC.PAYMENT.CORE.Contracts.Request.StockCar;
using BC.PAYMENT.CORE.Contracts.Response.StockCar;
using BC.PAYMENT.CORE.Entities.StockCar;

namespace BC.PAYMENT.API.Controllers.StockCar;

public class TransferMoneyController(IUnitOfWork unitOfWork) : BaseApiController
{
    [HttpGet]
    [Route("by-template/{templateId:int}")]
    public async Task<ApiResponse<List<TransferMoneyResponse>>> GetTransferMoneyAsync([Required] int templateId)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var data = await unitOfWork.TransferMoney.GetTransferMoneyAsync(credential?.DbCode, templateId);
            if (data.Count > 0)
            {
                return ApiResponse<List<TransferMoneyResponse>>.Builder()
                    .WithMessage("data fetched successfully.")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(data)
                    .Build();
            }
            return ApiResponse<List<TransferMoneyResponse>>.Builder()
                .WithMessage("data fetched empty.")
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<TransferMoneyResponse>>(ex.Message);
        }
    }

    [HttpGet]
    [Route("by-template/{templateId:int}/{id:int}")]
    public async Task<ApiResponse<TransferMoneyResponse>> GetTransferMoneyByIdAsync([Required] int templateId, [Required] int id)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var data = await unitOfWork.TransferMoney.GetTransferMoneyByIdAsync(credential?.DbCode, templateId, id);
            if (data != null)
            {
                return ApiResponse<TransferMoneyResponse>.Builder()
                    .WithMessage("data fetched successfully.")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(data)
                    .Build();
            }
            return ApiResponse<TransferMoneyResponse>.Builder()
                .WithMessage("data fetched empty.")
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<TransferMoneyResponse>(ex.Message);
        }
    }

    [HttpGet]
    [Route("total/by-template/{templateId:int}")]
    public async Task<ApiResponse<TotalTransferMoneyResponse>> GetTotalTransferMoneyByTemplateIdAsync([Required] int templateId)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var data = await unitOfWork.TransferMoney.GetTotalTransferMoneyByTemplateIdAsync(credential?.DbCode, templateId);
            if (data != null)
            {
                return ApiResponse<TotalTransferMoneyResponse>.Builder()
                    .WithMessage("data fetched successfully.")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(data)
                    .Build();
            }
            return ApiResponse<TotalTransferMoneyResponse>.Builder()
                .WithMessage("data fetched empty.")
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<TotalTransferMoneyResponse>(ex.Message);
        }
    }

    [HttpPost]
    [Route("")]
    public async Task<ApiResponse<int>> AddNewTransferMoneyAsync([FromBody] CreateTransferMoneyRequest req)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var model = new TransferMoney
            {
                TransactionDate = req.TransactionDate,
                Description = req.Description,
                Amount = req.Amount,
                DollarFromEmployee = req.DollarFromEmployee,
                RielFromEmployee = req.RielFromEmployee,
                ExchangeRateEmployee = req.ExchangeRateEmployee,
                DepositDollar = req.DepositDollar,
                DepositRiel = req.DepositRiel,
                DepositExchange = req.DepositExchange,
                CreatedDate = credential?.CurrectDate ?? DateTime.Now,
                CreatedBy = credential?.Username ?? string.Empty,
                DbCode = credential?.DbCode ?? string.Empty,
                EmployeeId = req.EmployeeId,
                TemplateId = req.TemplateId
            };
            var data = await unitOfWork.TransferMoney.AddNewTransferMoneyAsync(model);
            if (data > 0)
            {
                return ApiResponse<int>.Builder()
                    .WithMessage("data added successfully.")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(data)
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
    public async Task<ApiResponse<int>> UpdateTransferMoneyAsync([FromBody] UpdateTransferMoneyRequest req)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var model = new TransferMoney
            {
                Id = req.Id,
                TransactionDate = req.TransactionDate,
                Description = req.Description,
                Amount = req.Amount,
                DollarFromEmployee = req.DollarFromEmployee,
                RielFromEmployee = req.RielFromEmployee,
                ExchangeRateEmployee = req.ExchangeRateEmployee,
                DepositDollar = req.DepositDollar,
                DepositRiel = req.DepositRiel,
                DepositExchange = req.DepositExchange
            };
            var data = await unitOfWork.TransferMoney.UpdateTransferMoneyAsync(model);
            if (data > 0)
            {
                return ApiResponse<int>.Builder()
                    .WithMessage("data updated successfully.")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(data)
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

    [HttpDelete]
    [Route("{transferId:int}")]
    public async Task<ApiResponse<int>> DeleteTransferMoneyAsync([Required] int transferId)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var data = await unitOfWork.TransferMoney.DeleteTransferMoneyAsync(transferId);
            if (data > 0)
            {
                return ApiResponse<int>.Builder()
                    .WithMessage("data deleted successfully.")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(data)
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
}
