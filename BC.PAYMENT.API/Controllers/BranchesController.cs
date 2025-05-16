using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.General;
using BC.PAYMENT.LOGGING;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Net;

namespace BC.PAYMENT.API.Controllers
{
    [Helper.Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BranchesController : ControllerBase
    {
        #region ===[ Private Members ]=============================================================

        private readonly IUnitOfWork _unitOfWork;
        private readonly AppSettings _appSettings;

        #endregion

        #region ===[ Constructor ]=================================================================
        public BranchesController(IUnitOfWork unitOfWork, IOptions<AppSettings> appSettings)
        {
            this._unitOfWork = unitOfWork;
            this._appSettings = appSettings.Value;
        }
        #endregion

        #region ===[ Public Methods ]==============================================================
        [AllowAnonymous]
        [HttpGet("dbcodes")]
        public async Task<ApiResponse<List<BranchDTO>>> GetLoginBranch([FromQuery]string username,string appCode)
        {
            var apiResponse = new ApiResponse<List<BranchDTO>>();

            try
            {
                var data = await _unitOfWork.Branches.GetLoginBranchAsync(username,appCode);
                if (!data.Any())
                {
                    apiResponse.Success = true;
                    apiResponse.Message = "No branches";
                    apiResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                    apiResponse.Result = new List<BranchDTO>();
                    return apiResponse;
                }

                apiResponse.Success = true;
                apiResponse.Message = "Branches fetched successfully.";
                apiResponse.StatusCode = (int)HttpStatusCode.OK;
                apiResponse.Result = data.ToList();
            }
            catch (SqlException ex)
            {
                apiResponse.Success = false;
                apiResponse.Message = ex.Message;
                apiResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                Logger.Instance.Error("SQL Exception:", ex);
            }
            catch (Exception ex)
            {
                apiResponse.Success = false;
                apiResponse.Message = ex.Message;
                apiResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                Logger.Instance.Error("Exception:", ex);
            }

            return apiResponse;
        }
        #endregion
    }
}
