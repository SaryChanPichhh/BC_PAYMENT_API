using BC.PAYMENT.CORE.Contracts.CashFlow;
using BC.PAYMENT.CORE.Entities.CashFlow;

namespace BC.PAYMENT.API.Controllers.CashFlow;

[Produces("application/json")]
[Route("api/v2/cash-flow/headers")]
public class CashFlowHeaderController(IUnitOfWork unitOfWork) : BaseApiController
{
    [HttpGet]
    public async Task<ApiResponse<List<CashFlowHeaderModel>>> GetAllAsync()
    {
        try
        {
            var dbCode = Common.DecodeJwt(User).DbCode!;
            var result = await unitOfWork.CashFlowHeader.GetAllAsync(dbCode);
            return ApiResponseFactory.SuccessResponse(result, "Cash flow headers fetched successfully");
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<CashFlowHeaderModel>>(ex.Message);
        }
    }

    [HttpPost]
    public async Task<ApiResponse<int>> AddAsync([FromBody] CashFlowHeaderRequest request)
    {
        if (request.Date == default)
            return ApiResponseFactory.ErrorResponse(0, "Date is required");
        try
        {
            var credential = Common.DecodeJwt(User);
            var id = await unitOfWork.CashFlowHeader.AddAsync(credential.DbCode!, request.Date,
                credential.Username!, credential.InvoiceEntryCode!);
            return id > 0
                ? ApiResponseFactory.SuccessResponse(id, "Cash flow header added successfully")
                : ApiResponseFactory.ErrorResponse(0, $"Cash flow of {request.Date:MM/dd/yyyy} already exists");
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }

    [HttpPut("{id:int}/submit")]
    public async Task<ApiResponse<bool>> SubmitAsync(int id)
    {
        try
        {
            var credential = Common.DecodeJwt(User);
            var affectedRows =
                await unitOfWork.CashFlowHeader.SubmitAsync(credential.DbCode!, id, credential.Username!);
            return affectedRows > 0
                ? ApiResponseFactory.SuccessResponse(true, "Cash flow submitted successfully")
                : ApiResponseFactory.ErrorResponse(false,
                    "Cash flow header not found, already submitted or has no detail");
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<bool>(ex.Message);
        }
    }

    [HttpPut("{id:int}/cancel-submit")]
    public async Task<ApiResponse<bool>> CancelSubmitAsync(int id)
    {
        try
        {
            var credential = Common.DecodeJwt(User);
            var affectedRows =
                await unitOfWork.CashFlowHeader.CancelSubmitAsync(credential.DbCode!, id, credential.Username!);
            return affectedRows > 0
                ? ApiResponseFactory.SuccessResponse(true, "Cash flow submission cancelled successfully")
                : ApiResponseFactory.ErrorResponse(false, "Submitted cash flow header not found");
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<bool>(ex.Message);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<ApiResponse<bool>> DeleteAsync(int id)
    {
        try
        {
            var dbCode = Common.DecodeJwt(User).DbCode!;
            var affectedRows = await unitOfWork.CashFlowHeader.DeleteAsync(dbCode, id);
            return affectedRows > 0
                ? ApiResponseFactory.SuccessResponse(true, "Cash flow header deleted successfully")
                : ApiResponseFactory.ErrorResponse(false, "Cash flow header not found or already submitted");
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<bool>(ex.Message);
        }
    }
}
