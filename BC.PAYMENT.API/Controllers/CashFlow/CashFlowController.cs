using BC.PAYMENT.CORE.Entities.CashFlow;

namespace BC.PAYMENT.API.Controllers.CashFlow;

[Produces("application/json")]
[Route("api/v2/cash-flow")]
public class CashFlowController(IUnitOfWork unitOfWork) : BaseApiController
{
    [HttpGet]
    public async Task<ApiResponse<List<CashFlowModel>>> GetPaymentCashFlowsAsync(
        [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
    {
        if (!TryResolveDateRange(fromDate, toDate, out var from, out var to, out var error))
            return ApiResponseFactory.ErrorResponse(new List<CashFlowModel>(), error);
        try
        {
            var credential = Common.DecodeJwt(User);
            var result = await unitOfWork.CashFlow.GetPaymentCashFlowsAsync(credential.DbCode!, from, to);
            return ApiResponseFactory.SuccessResponse(result, "Payment cash flows fetched successfully");
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<CashFlowModel>>(ex.Message);
        }
    }

    [HttpGet("entry/{entryCode}")]
    public async Task<ApiResponse<List<CashFlowModel>>> GetPaymentsByEntryCodeAsync(string entryCode,
        [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
    {
        if (string.IsNullOrWhiteSpace(entryCode))
            return ApiResponseFactory.ErrorResponse(new List<CashFlowModel>(), "Entry code is required");
        if (!TryResolveDateRange(fromDate, toDate, out var from, out var to, out var error))
            return ApiResponseFactory.ErrorResponse(new List<CashFlowModel>(), error);
        try
        {
            var credential = Common.DecodeJwt(User);
            var result = await unitOfWork.CashFlow.GetPaymentsByDbCodeAndEntryCodeAsync(credential.DbCode!,
                entryCode.Trim(), from, to);
            return ApiResponseFactory.SuccessResponse(result, "Payment cash flows fetched successfully");
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<CashFlowModel>>(ex.Message);
        }
    }

    [HttpPost("entries")]
    public async Task<ApiResponse<List<CashFlowModel>>> GetPaymentsByEntryCodesAsync(
        [FromBody] List<string> entryCodes)
    {
        var codes = entryCodes?
            .Where(code => !string.IsNullOrWhiteSpace(code))
            .Select(code => code.Trim())
            .Distinct()
            .ToList() ?? [];
        if (codes.Count == 0)
            return ApiResponseFactory.ErrorResponse(new List<CashFlowModel>(), "Entry codes are required");
        try
        {
            var credential = Common.DecodeJwt(User);
            var result =
                await unitOfWork.CashFlow.GetPaymentsByDbCodeAndMultiEntryCodesAsync(credential.DbCode!, codes);
            return ApiResponseFactory.SuccessResponse(result, "Payment cash flows fetched successfully");
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<CashFlowModel>>(ex.Message);
        }
    }

    internal static bool TryResolveDateRange(DateTime? fromDate, DateTime? toDate,
        out DateTime from, out DateTime to, out string error)
    {
        from = (fromDate ?? toDate ?? DateTime.Today).Date;
        to = (toDate ?? fromDate ?? DateTime.Today).Date;
        error = string.Empty;
        if (from <= to) return true;
        error = "fromDate must be earlier than or equal to toDate";
        return false;
    }
}
