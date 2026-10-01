namespace BC.PAYMENT.API.Controllers;

public class GeneralController(IUnitOfWork unitOfWork) : BaseApiController
{
    [HttpGet]
    [Route("sale-types")]
    public async Task<ApiResponse<List<string>>> GetSaleTypesAsync()
    {
        var credential = Common.DecodeJwt(HttpContext.User);
        try
        {
            var execute = await unitOfWork.GeneralRepository.GetSaleTypes(credential.DbCode!);
            if (execute.Count > 0)
                return ApiResponse<List<string>>.Builder()
                    .WithMessage("Sale type fetched successfully")
                    .WithSuccess(true)
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(execute)
                    .Build();
            else
                return ApiResponse<List<string>>.Builder()
                    .WithMessage("Sale type fetched unsuccessfully")
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<string>>(ex.Message);
        }
    }

    [HttpGet]
    [Route("account-codes")]
    public async Task<ApiResponse<List<BC.PAYMENT.CORE.Contracts.Response.General.AccountCodeResponse>>>
        GetAccountCodesAsync()
    {
        var credential = Common.DecodeJwt(HttpContext.User);
        try
        {
            var execute = await unitOfWork.GeneralRepository.LoadAccountCode(credential.DbCode!);
            if (execute.Count > 0)
                return ApiResponse<List<BC.PAYMENT.CORE.Contracts.Response.General.AccountCodeResponse>>.Builder()
                    .WithMessage("Account codes fetched successfully")
                    .WithSuccess(true)
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(execute)
                    .Build();
            return ApiResponse<List<BC.PAYMENT.CORE.Contracts.Response.General.AccountCodeResponse>>.Builder()
                .WithMessage("Account codes fetched unsuccessfully")
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler
                .ExceptionError<List<BC.PAYMENT.CORE.Contracts.Response.General.AccountCodeResponse>>(ex.Message);
        }
    }

    [HttpGet]
    [Route("analysis-range-details/{type}")]
    public async Task<ApiResponse<List<BC.PAYMENT.CORE.Contracts.Response.General.AnalysisRangeDetailResponse>>>
        GetAnalysisRangeDetailsAsync(string type)
    {
        var credential = Common.DecodeJwt(HttpContext.User);
        try
        {
            var execute = await unitOfWork.GeneralRepository.LoadAnalysisByRangeDetails(credential.DbCode!, type);
            if (execute.Count > 0)
                return ApiResponse<List<BC.PAYMENT.CORE.Contracts.Response.General.AnalysisRangeDetailResponse>>
                    .Builder()
                    .WithMessage("Analysis range details fetched successfully")
                    .WithSuccess(true)
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(execute)
                    .Build();
            return ApiResponse<List<BC.PAYMENT.CORE.Contracts.Response.General.AnalysisRangeDetailResponse>>.Builder()
                .WithMessage("Analysis range details fetched unsuccessfully")
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler
                .ExceptionError<List<BC.PAYMENT.CORE.Contracts.Response.General.AnalysisRangeDetailResponse>>(
                    ex.Message);
        }
    }

    [HttpGet]
    [Route("analysis-all-details/{type}")]
    public async Task<ApiResponse<List<BC.PAYMENT.CORE.Contracts.Response.General.AnalysisAllDetailResponse>>>
        GetAnalysisAllDetailsAsync(string type)
    {
        var credential = Common.DecodeJwt(HttpContext.User);
        try
        {
            var execute = await unitOfWork.GeneralRepository.LoadAnalysisByAllDetail(credential.DbCode!, type);
            if (execute.Count > 0)
                return ApiResponse<List<BC.PAYMENT.CORE.Contracts.Response.General.AnalysisAllDetailResponse>>.Builder()
                    .WithMessage("Analysis all details fetched successfully")
                    .WithSuccess(true)
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(execute)
                    .Build();
            return ApiResponse<List<BC.PAYMENT.CORE.Contracts.Response.General.AnalysisAllDetailResponse>>.Builder()
                .WithMessage("Analysis all details fetched unsuccessfully")
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler
                .ExceptionError<List<BC.PAYMENT.CORE.Contracts.Response.General.AnalysisAllDetailResponse>>(ex.Message);
        }
    }

    [HttpGet]
    [Route("analysis-types")]
    public async Task<ApiResponse<List<BC.PAYMENT.CORE.Contracts.Response.General.AnalysisTypeResponse>>>
        GetAnalysisTypesAsync()
    {
        var credential = Common.DecodeJwt(HttpContext.User);
        try
        {
            var execute = await unitOfWork.GeneralRepository.LoadAnalysisType(credential.DbCode!);
            if (execute.Count > 0)
                return ApiResponse<List<BC.PAYMENT.CORE.Contracts.Response.General.AnalysisTypeResponse>>.Builder()
                    .WithMessage("Analysis types fetched successfully")
                    .WithSuccess(true)
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(execute)
                    .Build();
            return ApiResponse<List<BC.PAYMENT.CORE.Contracts.Response.General.AnalysisTypeResponse>>.Builder()
                .WithMessage("Analysis types fetched unsuccessfully")
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler
                .ExceptionError<List<BC.PAYMENT.CORE.Contracts.Response.General.AnalysisTypeResponse>>(ex.Message);
        }
    }
}