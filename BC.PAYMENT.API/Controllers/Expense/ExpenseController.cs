using BC.PAYMENT.CORE.Contracts.Request.Expense;
using BC.PAYMENT.CORE.Contracts.Response.Expense;

namespace BC.PAYMENT.API.Controllers.Expense;

public class ExpenseController(IUnitOfWork unitOfWork) : BaseApiController
{
    [HttpGet]
    [Route("expense-detail-by-date-and-deliveryId")]
    public async Task<ApiResponse<List<BcPaymentDetailResponse>>> LoadBcPaymentDetailAsync(
        [FromQuery] DateTime date,
        [FromQuery] string deliveryId)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var execute = await unitOfWork.Expense.LoadBcPaymentDetailAsync(credential.DbCode, deliveryId, date);
            if (execute.Count > 0)
                return ApiResponse<List<BcPaymentDetailResponse>>.Builder()
                    .WithMessage("Expense details fetched successfully")
                    .WithSuccess(true)
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(execute)
                    .Build();

            return ApiResponse<List<BcPaymentDetailResponse>>.Builder()
                .WithMessage("Expense details fetched unsuccessfully")
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .WithResult(new List<BcPaymentDetailResponse>())
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<BcPaymentDetailResponse>>(ex.Message);
        }
    }

    [HttpGet]
    [Route("expense-detail-with-paid-by-date")]
    public async Task<ApiResponse<List<BcPaymentDetailResponse>>> LoadBcPaymentDetailWithPaidAsync(
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var execute =
                await unitOfWork.Expense.LoadBcPaymentDetailWithPaidAsync(credential.DbCode, fromDate, toDate);
            if (execute.Count > 0)
                return ApiResponse<List<BcPaymentDetailResponse>>.Builder()
                    .WithMessage("Expense details fetched successfully")
                    .WithSuccess(true)
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(execute)
                    .Build();
            return ApiResponse<List<BcPaymentDetailResponse>>.Builder()
                .WithMessage("Expense details fetched unsuccessfully")
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .WithResult(new List<BcPaymentDetailResponse>())
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<BcPaymentDetailResponse>>(ex.Message);
        }
    }

    [HttpGet]
    [Route("expense-detail-by-date")]
    public async Task<ApiResponse<List<BcPaymentDetailResponse>>> LoadBcPaymentDetailByDateRangeAsync(
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var execute = await unitOfWork.Expense.LoadBcPaymentDetailAsync(credential.DbCode, fromDate, toDate);
            if (execute.Count > 0)
                return ApiResponse<List<BcPaymentDetailResponse>>.Builder()
                    .WithMessage("Expense details fetched successfully")
                    .WithSuccess(true)
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(execute)
                    .Build();
            return ApiResponse<List<BcPaymentDetailResponse>>.Builder()
                .WithMessage("Expense details fetched unsuccessfully")
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .WithResult(new List<BcPaymentDetailResponse>())
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<BcPaymentDetailResponse>>(ex.Message);
        }
    }

    [HttpGet]
    [Route("expense-detail-by-period")]
    public async Task<ApiResponse<List<BcPaymentDetailResponse>>> LoadBcPaymentDetailByPeriodAsync(
        [FromQuery] int month,
        [FromQuery] int year)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var execute = await unitOfWork.Expense.LoadBcPaymentDetailAsync(credential.DbCode, month, year);
            if (execute.Count > 0)
                return ApiResponse<List<BcPaymentDetailResponse>>.Builder()
                    .WithMessage("Expense details fetched successfully")
                    .WithSuccess(true)
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(execute)
                    .Build();
            return ApiResponse<List<BcPaymentDetailResponse>>.Builder()
                .WithMessage("Expense details fetched unsuccessfully")
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .WithResult(new List<BcPaymentDetailResponse>())
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<BcPaymentDetailResponse>>(ex.Message);
        }
    }

    [HttpPut]
    [Route("detail")]
    public async Task<ApiResponse<bool>> UpdateBcPaymentDetailAsync([FromBody] UpdateBcPaymentDetailRequest request)
    {
        try
        {
            var affectedRows = await unitOfWork.Expense.UpdateBcPaymentDetailAsync(request);
            if (affectedRows > 0)
                return ApiResponse<bool>.Builder()
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithMessage("Payment detail updated successfully.")
                    .WithSuccess(true)
                    .WithResult(true)
                    .Build();

            return ApiResponse<bool>.Builder()
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .WithMessage("Payment detail updated unsuccessfully.")
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