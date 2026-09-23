using BC.PAYMENT.CORE.Contracts.General;
using BC.PAYMENT.CORE.Contracts.Login;

namespace BC.PAYMENT.API.Controllers.Auth
{
    [Produces("application/json")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/v2/[controller]")]
    [AllowAnonymous]
    public class AuthController(IUnitOfWork unitOfWork, IOptions<AppSettings> appSettings) : ControllerBase
    {
        [HttpGet("username/exists")]
        public async Task<ApiResponse<bool>> UsernameExists([FromQuery] string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return ApiResponseFactory.ErrorResponse<bool>(false, "Username is required");
            var isExistsUserName = await unitOfWork.Users.IsExistsUserName(username);
            return ApiResponseFactory.SuccessResponse(isExistsUserName, isExistsUserName ? "username is exists" : "username is not exists");
        }

        [HttpPost("login")]
        public async Task<ApiResponse<LoginResponseDTO>> GetBcUserCredentialAsync([FromBody] LoginRequestDTO requestDto)
        {
            try
            {
                var data = await unitOfWork.Users.GetBcUserCredential(requestDto);
                
                if (data is not null)
                {
                    var period =await unitOfWork.GeneralRepository.GetPeriod(requestDto.DbCode);
                    ClaimDTO claimDto = new();
                    if (data.Role.Equals("ADMIN"))
                    {
                        claimDto = new ClaimDTO
                        {
                            Username = data.Username,
                            DbCode = data.DbCode,
                            Role = data.Role,
                            CompanyCode = data.CompanyCode,
                            AppCode = data.AppCode,
                            CurrectDate = data.CurrentDate,
                            InvoiceEntryCode = data.InvoiceEntryCode ?? "",
                            Period = period
                        };
                    }else if (BCrypt.Net.BCrypt.Verify(requestDto.Password, data.UserPass))
                    {
                        claimDto = new ClaimDTO
                        {
                            UserId = data.UserId,
                            Username = data.Username!,
                            CompanyCode = requestDto.CompanyCode!,
                            AppCode = requestDto.AppCode!,
                            DbCode = requestDto.DbCode!,
                            CurrectDate = data.CurrentDate,
                            InvoiceEntryCode = data.InvoiceEntryCode ?? "",
                            Role =  data.Role,
                            Period = period
                        };
                    }
                    Common.GenerateJwtToken(claimDto, appSettings.Value);
                    var loginResponse = new LoginResponseDTO
                    {
                        Token = Common.GenerateJwtToken(claimDto, appSettings.Value),
                        UserId = data.UserId,
                        Username = data.Username!,
                        DbCode = data.DbCode!,
                    };
                    return ApiResponseFactory.SuccessResponse(loginResponse,"Login Successful.");
                }
                return ApiResponse<LoginResponseDTO>.Builder()
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .WithMessage("Username or password is incorrect")
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<LoginResponseDTO>(ex.Message);
            }
        }
        
    }
}
