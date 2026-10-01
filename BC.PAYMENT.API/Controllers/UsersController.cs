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

namespace BC.PAYMENT.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(IUnitOfWork unitOfWork, IOptions<AppSettings> appSettings) : ControllerBase
{
    #region ===[ Public Methods ]==============================================================

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
                    InvoiceEntryCode = data.InvoiceEntryCode ?? ""
                };
                Common.GenerateJwtToken(claimsDto, appSettings.Value);
                var loginResponse = new LoginResponseDTO
                {
                    Token = Common.GenerateJwtToken(claimsDto, appSettings.Value),
                    UserId = data.UserId,
                    Username = data.Username!,
                    DbCode = data.DbCode!
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