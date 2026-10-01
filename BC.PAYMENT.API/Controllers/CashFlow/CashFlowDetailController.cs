using BC.PAYMENT.CORE.Contracts.CashFlow;

namespace BC.PAYMENT.API.Controllers.CashFlow;

[Produces("application/json")]
[Route("api/v2/cash-flow/details")]
public class CashFlowDetailController(IUnitOfWork unitOfWork) : BaseApiController
{
    [HttpPost]
    public async Task<ApiResponse<int>> AddAsync([FromBody] CashFlowDetailRequest request)
    {
        if (!TryValidateRequest(request, out var error))
            return ApiResponseFactory.ErrorResponse(0, error);
        try
        {
            var credential = Common.DecodeJwt(User);
            var id = await unitOfWork.CashFlowDetail.AddAsync(credential.DbCode!, request, credential.Username!,
                credential.InvoiceEntryCode!);
            if (id <= 0)
            {
                var status = await unitOfWork.CashFlowDetail.GetHeaderStatusAsync(credential.DbCode!,
                    request.HeaderId, request.Date);
                return ApiResponseFactory.ErrorResponse(0, LockedMessage(status, request.Date));
            }

            // The entered date was locked, so the row was saved on the next open date; tell the user.
            var savedDate = await unitOfWork.CashFlowDetail.GetDetailDateAsync(credential.DbCode!, id);
            if (savedDate == null || savedDate.Value.Date == request.Date.Date)
                return ApiResponseFactory.SuccessResponse(id, "Cash flow detail added successfully");

            var lockedStatus = await unitOfWork.CashFlowDetail.GetHeaderStatusAsync(credential.DbCode!, 0,
                request.Date);
            return ApiResponseFactory.SuccessResponse(id,
                $"លំហូរសាច់ប្រាក់ថ្ងៃ {request.Date:MM/dd/yyyy} {StatusText(lockedStatus)}រួចហើយ។\n" +
                $"ទិន្នន័យត្រូវបានបញ្ចូលទៅថ្ងៃ {savedDate.Value:MM/dd/yyyy} ជំនួសវិញ។");
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ApiResponse<int>> UpdateAsync(int id, [FromBody] CashFlowDetailRequest request)
    {
        if (id <= 0)
            return ApiResponseFactory.ErrorResponse(0, "Cash flow detail id is required");
        if (!TryValidateRequest(request, out var error))
            return ApiResponseFactory.ErrorResponse(0, error);
        try
        {
            var credential = Common.DecodeJwt(User);
            var affectedRows =
                await unitOfWork.CashFlowDetail.UpdateAsync(credential.DbCode!, id, request, credential.Username!);
            if (affectedRows > 0)
                return ApiResponseFactory.SuccessResponse(id, "Cash flow detail updated successfully");

            var status = await unitOfWork.CashFlowDetail.GetHeaderStatusAsync(credential.DbCode!, request.HeaderId,
                request.Date);
            return ApiResponseFactory.ErrorResponse(0, status is null or "Pending"
                ? "Cash flow detail not found"
                : LockedMessage(status, request.Date));
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }

    private static bool TryValidateRequest(CashFlowDetailRequest? request, out string error)
    {
        error = request switch
        {
            null => "Request body is required",
            { Date: var date } when date == default => "Date is required",
            { Name: var name } when string.IsNullOrWhiteSpace(name) => "Name is required",
            { Amount: <= 0 } => "Amount must be greater than 0",
            { ExchangeRate: <= 0 } => "Exchange rate must be greater than 0",
            _ => string.Empty
        };
        return error.Length == 0;
    }

    private static string LockedMessage(string? status, DateTime date) => status == null
        ? "ថ្ងៃលំហូរសាច់ប្រាក់ដែលបានជ្រើសរើស មិនមានទៀតទេ។ សូមជ្រើសរើសថ្ងៃម្តងទៀត។"
        : $"លំហូរសាច់ប្រាក់ថ្ងៃ {date:MM/dd/yyyy} {StatusText(status)}រួចហើយ មិនអាចបញ្ចូលទិន្នន័យបានទេ។";

    // Same wording as the WinForms status column.
    private static string StatusText(string? status) => status switch
    {
        "Submitted" => "បានដាក់ស្នើរ",
        "Completed" or "Approved" => "បានអនុម័ត",
        "Rejected" => "ត្រូវបានបដិសេធ",
        _ => "មិនអាចកែប្រែបាន"
    };
}
