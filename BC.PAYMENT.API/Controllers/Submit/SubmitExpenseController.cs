using System.ComponentModel.DataAnnotations;
using BC.PAYMENT.CORE.Contracts.Request.Expense;
using BC.PAYMENT.CORE.Contracts.Response.Expense;
using BC.PAYMENT.CORE.Entities;

namespace BC.PAYMENT.API.Controllers.Submit;

public class SubmitExpenseController(IUnitOfWork unitOfWork) : BaseApiController
{
    [HttpPost]
    [Route("")]
    [Route("add-new-submit-expense")]
    public async Task<ApiResponse<int>> AddNewSubmitExpenseAsync([FromBody] CreateBcSubmittedPaidDetailRequest request)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var detail = new BcSubmittedPaidDetail
            {
                PaidDetailId = request.PaidDetailId,
                Dollar = request.Dollar,
                Riel = request.Riel,
                Exchange = request.Exchange,
                Total = request.Total,
                ExpenseRiel = request.ExpenseRiel,
                ExpenseDollar = request.ExpenseDollar,
                MoneyBais = request.MoneyBias,
                Status = request.Status,
                DbCode = credential.DbCode,
                SubmittedDate = credential.CurrectDate,
                SubmittedBy = credential.Username
            };

            var affectedRow = await unitOfWork.SubmitExpense.AddNewSubmitExpense(detail);
            if (affectedRow > 0)
                return ApiResponse<int>.Builder()
                    .WithMessage("Submit expense added successfully")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithSuccess(true)
                    .WithResult(affectedRow)
                    .Build();

            return ApiResponse<int>.Builder()
                .WithMessage("Submit expense added unsuccessfully")
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .WithSuccess(false)
                .WithResult(affectedRow)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }

    [HttpGet]
    [Route("by-date")]
    public async Task<ApiResponse<List<SubmitExpenseDetailResponse>>> GetSubmitExpenseDetailByDateAsync(
        [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var data = await unitOfWork.SubmitExpense.GetSubmitExpenseDetailByDateAsync(credential?.DbCode, fromDate,
                toDate);
            if (data.Count > 0)
                return ApiResponse<List<SubmitExpenseDetailResponse>>.Builder()
                    .WithMessage("Submit expense added successfully")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithSuccess(true)
                    .WithResult(data)
                    .Build();
            return ApiResponse<List<SubmitExpenseDetailResponse>>.Builder()
                .WithMessage("Submit expense added unsuccessfully")
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .WithSuccess(false)
                .WithResult([])
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<SubmitExpenseDetailResponse>>(ex.Message);
        }
    }

    [HttpGet]
    [Route("approved-expense-by-date")]
    public async Task<ApiResponse<List<ApprovedSubmitExpenseDetailResponse>>> GetApprovedExpenseDetailByDateAsync(
        [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var data = await unitOfWork.SubmitExpense.GetApprovedExpenseDetailByDateAsync(credential?.DbCode, fromDate,
                toDate);
            if (data.Count > 0)
                return ApiResponse<List<ApprovedSubmitExpenseDetailResponse>>.Builder()
                    .WithMessage("Submit expense added successfully")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithSuccess(true)
                    .WithResult(data)
                    .Build();
            return ApiResponse<List<ApprovedSubmitExpenseDetailResponse>>.Builder()
                .WithMessage("Submit expense added unsuccessfully")
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .WithSuccess(false)
                .WithResult([])
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<ApprovedSubmitExpenseDetailResponse>>(ex.Message);
        }
    }

    [HttpPut]
    [Route("update-expense-description")]
    public async Task<ApiResponse<int>> UpdateSubmitExpenseDescriptionAsync(
        [FromBody] UpdateSubmitExpenseDescriptionRequest request)
    {
        try
        {
            var affectedRow = await unitOfWork.SubmitExpense.UpdateSubmitExpenseDescriptionAsync(request);
            if (affectedRow > 0)
                return ApiResponse<int>.Builder()
                    .WithMessage("Submit expense description updated successfully")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithSuccess(true)
                    .WithResult(affectedRow)
                    .Build();
            return ApiResponse<int>.Builder()
                .WithMessage("Submit expense description updated unsuccessfully")
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .WithSuccess(false)
                .WithResult(affectedRow)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }

    [HttpDelete]
    [Route("{submittedId}")]
    public async Task<ApiResponse<int>> DeleteSubmitExpenseAsync([Required] int submittedId)
    {
        try
        {
            var affectedRow = await unitOfWork.SubmitExpense.DeleteSubmitExpenseAsync(submittedId);
            if (affectedRow > 0)
                return ApiResponse<int>.Builder()
                    .WithMessage("Submit expense deleted successfully")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithSuccess(true)
                    .WithResult(affectedRow)
                    .Build();

            return ApiResponse<int>.Builder()
                .WithMessage("Submit expense deleted unsuccessfully")
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .WithSuccess(false)
                .WithResult(affectedRow)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }
}