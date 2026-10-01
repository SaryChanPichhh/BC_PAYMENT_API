namespace BC.PAYMENT.API.Controllers;

[Microsoft.AspNetCore.Authorization.Authorize]
public class AnalysisAccountsController(IUnitOfWork unitOfWork, IOptions<AppSettings> appSettings) : BaseApiController
{
    [HttpPut("details")]
    public async Task<ApiResponse<List<AnalysisCode>>> GetAnalysisByDetail(
        [FromBody] AnalysisCodeCreateRequest analysisCodeCreateRequest)
    {
        try
        {
            var claim = Common.DecodeJwt(HttpContext.User);
            analysisCodeCreateRequest.DbCode = claim.DbCode;
            var data = await unitOfWork.AnalysisAccounts.GetAnalysisByDetail(analysisCodeCreateRequest);
            return ApiResponse<List<AnalysisCode>>.Builder()
                .WithMessage(data.Any() ? "Analysis account code fetched successfully." : "No analysis account code")
                .WithStatusCode(data.Any() ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest)
                .WithResult(data.Any() ? data : new List<AnalysisCode>())
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<AnalysisCode>>(ex.Message);
        }
    }

    [HttpGet("alldetails")]
    public async Task<ApiResponse<Dictionary<string, List<AnalysisCode>>>> GetAllAnalysisByDetail()
    {
        try
        {
            var claim = Common.DecodeJwt(HttpContext.User);
            var result = await unitOfWork.AnalysisAccounts.GetAnalysisByDetailDictionary(claim.DbCode!);

            return ApiResponse<Dictionary<string, List<AnalysisCode>>>.Builder()
                .WithMessage(result.Any() ? "Analysis account code fetched successfully." : "No analysis account code")
                .WithStatusCode(result.Any() ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest)
                .WithResult(result.Any() ? result : new Dictionary<string, List<AnalysisCode>>())
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<Dictionary<string, List<AnalysisCode>>>(ex.Message);
        }
    }

    [HttpGet("accountcodes")]
    public async Task<ApiResponse<List<AccountCode>>> GetAllAccountCode()
    {
        try
        {
            var claim = Common.DecodeJwt(HttpContext.User);
            var result = await unitOfWork.AnalysisAccounts.GetAccountCode(claim.DbCode!);

            return ApiResponse<List<AccountCode>>.Builder()
                .WithMessage(result.Any() ? "Account code fetched successfully." : "No account code")
                .WithStatusCode(result.Any() ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest)
                .WithResult(result.Any() ? result : new List<AccountCode>())
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<AccountCode>>(ex.Message);
        }
    }

    [HttpGet("accountcodespaged")]
    public async Task<ApiResponse<PaginatedResponse<AccountCode>>> GetAllAccountCode([FromQuery] int page = 1,
        int pageSize = 10)
    {
        try
        {
            var claim = Common.DecodeJwt(HttpContext.User);

            // Fetch all records
            var allRecords = await unitOfWork.AnalysisAccounts.GetAccountCode(claim.DbCode!, page, pageSize);

            if (!allRecords.Any())
                return ApiResponse<PaginatedResponse<AccountCode>>.Builder()
                    .WithMessage("No account code")
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .WithResult(new PaginatedResponse<AccountCode>(new List<AccountCode>(), 0, page, pageSize))
                    .Build();

            // Calculate pagination details
            var totalRecords = allRecords.Count;

            // Paginate the data
            var paginatedData = allRecords
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return ApiResponse<PaginatedResponse<AccountCode>>.Builder()
                .WithMessage("Account code fetched successfully.")
                .WithStatusCode((int)HttpStatusCode.OK)
                .WithResult(new PaginatedResponse<AccountCode>(paginatedData, totalRecords, page, pageSize))
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<PaginatedResponse<AccountCode>>(ex.Message);
        }
    }

    [HttpGet("analysistypes")]
    public async Task<ApiResponse<List<AnalysisCodeType>>> GetAnalysisType()
    {
        try
        {
            var claim = Common.DecodeJwt(HttpContext.User);
            var result = await unitOfWork.AnalysisAccounts.GetAnalysisType(claim.DbCode!);

            return ApiResponse<List<AnalysisCodeType>>.Builder()
                .WithMessage(result.Any() ? "Analysis code fetched successfully." : "No analysis type")
                .WithStatusCode(result.Any() ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest)
                .WithResult(result.Any() ? result : new List<AnalysisCodeType>())
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<AnalysisCodeType>>(ex.Message);
        }
    }
}