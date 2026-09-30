using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.General;
using BC.PAYMENT.LOGGING;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net;
using BC.PAYMENT.CORE.Contracts.General;
using BC.PAYMENT.CORE.Contracts.Login;
using BC.PAYMENT.CORE.Contracts.Response.User;

namespace BC.PAYMENT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(IUnitOfWork unitOfWork, IOptions<AppSettings> appSettings) : ControllerBase
    {
        #region ===[ Public Methods ]==============================================================

        [HttpGet]
        public async Task<ApiResponse<List<UserResponse>>> GetAllUsersAsync()
        {
            try
            {
                var users = await unitOfWork.Users.GetAllAsync();
                var response = users.Select(u => new UserResponse
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    DbCode = u.DbCode,
                    Branches = u.Branches,
                    UserPass = u.UserPass,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Name = u.Name,
                    CompanyCode = u.CompanyCode,
                    AppCode = u.AppCode,
                    CurrentDate = u.CurrentDate,
                    RowNumber = u.RowNumber,
                    InvoiceEntryCode = u.InvoiceEntryCode,
                    Role = u.Role
                }).ToList();

                return ApiResponse<List<UserResponse>>.Builder()
                    .WithMessage(response.Count != 0 ? "Users fetched successfully." : "No users found.")
                    .WithStatusCode(response.Count != 0 ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest)
                    .WithResult(response.Count != 0 ? response : new List<UserResponse>())
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<UserResponse>>(ex.Message);
            }
        }

        [HttpPost("credentials")]
        public async Task<ApiResponse<LoginResponseDTO>> GetBcUserCredentialAsync([FromBody] LoginRequestDTO requestDto)
        {
            try
            {
                var data = await unitOfWork.Users.GetBcUserCredential(requestDto);
                if (BCrypt.Net.BCrypt.Verify(requestDto.Password, data.UserPass))
                {
                    var claimsDto = new ClaimDTO
                    {
                        UserId = data.UserId,
                        Username = data.Username!,
                        CompanyCode = requestDto.CompanyCode!,
                        AppCode = requestDto.AppCode!,
                        DbCode = requestDto.DbCode!,
                        CurrectDate = data.CurrentDate,
                        InvoiceEntryCode = data.InvoiceEntryCode ?? "",
                    };
                    Common.GenerateJwtToken(claimsDto, appSettings.Value);
                    var loginResponse = new LoginResponseDTO
                    {
                        Token = Common.GenerateJwtToken(claimsDto, appSettings.Value),
                        UserId = data.UserId,
                        Username = data.Username!,
                        DbCode = data.DbCode!,
                    };       
                
                    return ApiResponse<LoginResponseDTO>.Builder()
                        .WithMessage("User fetched successfully!")
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithResult(loginResponse)
                        .Build();
                }
                else
                {
                    return ApiResponse<LoginResponseDTO>.Builder()
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithMessage("Username or password is incorrect")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<LoginResponseDTO>(ex.Message);
            }
        }
        #endregion
        
        
    }
}
