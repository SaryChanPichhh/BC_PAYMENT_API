using BC.PAYMENT.CORE.Entities.CashFlow;
using BC.PAYMENT.CORE.Enums;

namespace BC.PAYMENT.API.Controllers.CashFlow;

[Produces("application/json")]
[Route("api/v2/cash-flow/audit")]
public class CashFlowAuditController(IUnitOfWork unitOfWork) : BaseApiController
{
    private static readonly SubmittedStatus[] AuditStatuses = [SubmittedStatus.Completed, SubmittedStatus.Rejected];

    [HttpGet("headers")]
    public async Task<ApiResponse<List<CashFlowHeaderModel>>> GetSubmittedHeadersAsync()
    {
        try
        {
            var dbCode = Common.DecodeJwt(User).DbCode!;
            var result = await unitOfWork.CashFlowAudit.GetSubmittedHeadersAsync(dbCode);
            return ApiResponseFactory.SuccessResponse(result, "Submitted cash flow headers fetched successfully");
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<CashFlowHeaderModel>>(ex.Message);
        }
    }

    [HttpGet("headers/{headerId:int}/pending")]
    public async Task<ApiResponse<List<CashFlowModelSubmittedModel>>> GetPendingDetailsAsync(int headerId)
    {
        try
        {
            var dbCode = Common.DecodeJwt(User).DbCode!;
            var result = await unitOfWork.CashFlowAudit.GetPendingDetailsAsync(dbCode, headerId);
            return ApiResponseFactory.SuccessResponse(result, "Pending cash flow details fetched successfully");
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<CashFlowModelSubmittedModel>>(ex.Message);
        }
    }

    [HttpPut("headers/{headerId:int}/status")]
    public async Task<ApiResponse<bool>> UpdateStatusAsync(int headerId, [FromQuery] SubmittedStatus status)
    {
        if (!AuditStatuses.Contains(status))
            return ApiResponseFactory.ErrorResponse(false, "Status must be Completed or Rejected");
        try
        {
            var credential = Common.DecodeJwt(User);
            var affectedRows = await unitOfWork.CashFlowAudit.UpdateStatusAsync(credential.DbCode!, headerId,
                status, credential.Username!);
            return affectedRows > 0
                ? ApiResponseFactory.SuccessResponse(true, $"Cash flow {status} successfully")
                : ApiResponseFactory.ErrorResponse(false, "Submitted cash flow header not found");
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<bool>(ex.Message);
        }
    }
}