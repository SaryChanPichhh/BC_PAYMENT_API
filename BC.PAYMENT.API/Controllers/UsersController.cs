using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.General;
using BC.PAYMENT.CORE.DTO.Login;
using BC.PAYMENT.LOGGING;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Net;

namespace BC.PAYMENT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        #region ===[ Private Members ]=============================================================

        private readonly IUnitOfWork _unitOfWork;
        private readonly AppSettings _appSettings;

        #endregion

        #region ===[ Constructor ]=================================================================

        /// <summary>
        /// Initialize UsersController by injecting an object type of IUnitOfWork
        /// </summary>
        public UsersController(IUnitOfWork unitOfWork, IOptions<AppSettings> appSettings)
        {
            this._unitOfWork = unitOfWork;
            this._appSettings = appSettings.Value;
        }

        #endregion

        #region ===[ Public Methods ]==============================================================

        //[HttpGet]
        //public async Task<ApiResponse<List<User>>> GetAll()
        //{
        //    var apiResponse = new ApiResponse<List<User>>();

        //    try
        //    {
        //        var data = await _unitOfWork.Users.GetAllAsync();
        //        apiResponse.Success = true;
        //        apiResponse.Result = data.ToList();
        //    }
        //    catch (SqlException ex)
        //    {
        //        apiResponse.Success = false;
        //        apiResponse.Message = ex.Message;
        //        Logger.Instance.Error("SQL Exception:", ex);
        //    }
        //    catch (Exception ex)
        //    {
        //        apiResponse.Success = false;
        //        apiResponse.Message = ex.Message;
        //        Logger.Instance.Error("Exception:", ex);
        //    }

        //    return apiResponse;
        //}

        [HttpPost("credentials")]
        public async Task<ApiResponse<LoginResponseDTO>> GetBcUserCredentialAsync([FromBody] LoginRequestDTO requestDto)
        {
            var apiResponse = new ApiResponse<LoginResponseDTO>();

            try
            {
                var data = await _unitOfWork.Users.GetBcUserCredential(requestDto);
                if (data == null)
                {
                    apiResponse.Success = false;
                    apiResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                    apiResponse.Message = "Username or password is incorrect";

                    return apiResponse;
                }

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
                    Common.GenerateJwtToken(claimsDto, _appSettings);
                    var loginResponse = new LoginResponseDTO
                    {
                        Token = Common.GenerateJwtToken(claimsDto, _appSettings),
                        UserId = data.UserId,
                        Username = data.Username!,
                        DbCode = data.DbCode!,
                    };       
                
                    apiResponse.Success = true;
                    apiResponse.Message = "User fetched successfully!";
                    apiResponse.StatusCode = (int)HttpStatusCode.OK;
                    apiResponse.Result = loginResponse;
                }
                else
                {
                    apiResponse.Success = false;
                    apiResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                    apiResponse.Message = "Username or password is incorrect";
                    return apiResponse;
                }
            }
            catch (SqlException ex)
            {
                apiResponse.Success = false;
                apiResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                apiResponse.Message = ex.Message;
                Logger.Instance.Error("SQL Exception:", ex);
            }
            catch (Exception ex)
            {
                apiResponse.Success = false;
                apiResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                apiResponse.Message = ex.Message;
                Logger.Instance.Error("Exception:", ex);
            }

            return apiResponse;
        }
        #endregion
    }
}
