using BC.PAYMENT.CORE.Entities.CashFlow;
using BC.PAYMENT.CORE.Enums;

namespace BC.PAYMENT.API.Controllers.CashFlow;

[Produces("application/json")]
[Route("api/v2/cash-flow/submitted")]
public class CashFlowSubmittedController(IUnitOfWork unitOfWork) : BaseApiController
{
    [HttpGet]
    public async Task<ApiResponse<List<CashFlowModelSubmittedModel>>> GetSubmittedPaymentCashFlowsAsync(
        [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate, [FromQuery] SubmittedStatus? status)
    {
        if (!CashFlowController.TryResolveDateRange(fromDate, toDate, out var from, out var to, out var error))
            return ApiResponseFactory.ErrorResponse(new List<CashFlowModelSubmittedModel>(), error);
        try
        {
            var dbCode = Common.DecodeJwt(User).DbCode!;
            var result =
                await unitOfWork.CashFlowPaymentSubmitted.GetSubmittedPaymentCashFlowsAsync(dbCode, from, to, status);
            return ApiResponseFactory.SuccessResponse(result.ToList(),
                "Submitted payment cash flows fetched successfully");
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<CashFlowModelSubmittedModel>>(ex.Message);
        }
    }
    
    [HttpGet("report")]
    public async Task<ApiResponse<List<CashFlowModelSubmittedModel>>> GetSubmittedPaymentCashFlowReportAsync(
        [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
    {
        if (!CashFlowController.TryResolveDateRange(fromDate, toDate, out var from, out var to, out var error))
            return ApiResponseFactory.ErrorResponse(new List<CashFlowModelSubmittedModel>(), error);
        try
        {
            var dbCode = Common.DecodeJwt(User).DbCode!;
            var result =
                await unitOfWork.CashFlowPaymentSubmitted.GetSubmittedPaymentCashFlowReportAsync(dbCode, from, to);
            return ApiResponseFactory.SuccessResponse(result.ToList(),
                "Submitted payment cash flow report fetched successfully");
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<CashFlowModelSubmittedModel>>(ex.Message);
        }
    }

    [HttpPut("{id:int}/status")]
    public async Task<ApiResponse<bool>> UpdateSubmittedStatusAsync(int id, [FromQuery] SubmittedStatus status)
    {
        try
        {
            var credential = Common.DecodeJwt(User);
            var affectedRows = await unitOfWork.CashFlowPaymentSubmitted.UpdateSubmittedStatusAsync(
                credential.DbCode!, id, status, credential.Username!);
            return affectedRows > 0
                ? ApiResponseFactory.SuccessResponse(true, "Submitted status updated successfully")
                : ApiResponseFactory.ErrorResponse(false, "Submitted cash flow not found");
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<bool>(ex.Message);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<ApiResponse<bool>> DeleteSubmittedAsync(int id)
    {
        try
        {
            var dbCode = Common.DecodeJwt(User).DbCode!;
            var affectedRows = await unitOfWork.CashFlowPaymentSubmitted.DeleteSubmittedAsync(dbCode, id);
            return affectedRows > 0
                ? ApiResponseFactory.SuccessResponse(true, "Submitted cash flow deleted successfully")
                : ApiResponseFactory.ErrorResponse(false, "Submitted cash flow not found");
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<bool>(ex.Message);
        }
    }
}
