using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.Accounting;
using BC.PAYMENT.CORE.Entities.Accounting;
using BC.PAYMENT.LOGGING;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Net;
using BC.PAYMENT.CORE.Entities;

namespace BC.PAYMENT.API.Controllers
{
    [Authorize]
    public class AnalysisAccountsController : BaseApiController
    {
        #region ===[ Private Members ]=============================================================

        private readonly IUnitOfWork _unitOfWork;
        private readonly AppSettings _appSettings;

        #endregion

        #region ===[ Constructor ]=================================================================
        public AnalysisAccountsController(IUnitOfWork unitOfWork, IOptions<AppSettings> appSettings)
        {
            this._unitOfWork = unitOfWork;
            this._appSettings = appSettings.Value;
        }
        #endregion

        [HttpPut("details")]
        public async Task<ApiResponse<List<AnalysisCode>>> GetAnalysisByDetail([FromBody]AnalysisCodeDTO analysisCodeDto)
        {
            var apiResponse = new ApiResponse<List<AnalysisCode>>();

            try
            {
                var claim = Common.DecodeJwt(HttpContext.User);
                analysisCodeDto.DbCode = claim.DbCode;
                var data = await _unitOfWork.AnalysisAccounts.GetAnalysisByDetail(analysisCodeDto);
                if (!data.Any())
                {
                    apiResponse.Success = false;
                    apiResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                    apiResponse.Message = "No analysis account code";
                    apiResponse.Result = new List<AnalysisCode>();
                    return apiResponse;
                }
                
                apiResponse.Success = true;
                apiResponse.Message = "Analysis account code fetched successfully.";
                apiResponse.StatusCode = (int)HttpStatusCode.OK;
                apiResponse.Result = data;

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

        [HttpGet("alldetails")]
        public async Task<ApiResponse<Dictionary<string, List<AnalysisCode>>>> GetAllAnalysisByDetail()
        {
            var apiResponse = new ApiResponse<Dictionary<string, List<AnalysisCode>>>();

            try
            {
                var claim = Common.DecodeJwt(HttpContext.User);
                var result = await _unitOfWork.AnalysisAccounts.GetAnalysisByDetailDictionary(claim.DbCode!);

                if (!result.Any())
                {
                    apiResponse.Success = false;
                    apiResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                    apiResponse.Message = "No analysis account code";
                    apiResponse.Result = new Dictionary<string, List<AnalysisCode>>();
                    return apiResponse;
                }

               
                // Set the API response
                apiResponse.Success = true;
                apiResponse.Message = "Analysis account code fetched successfully.";
                apiResponse.StatusCode = (int)HttpStatusCode.OK;
                apiResponse.Result = result;
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

        [HttpGet("accountcodes")]
        public async Task<ApiResponse<List<AccountCode>>> GetAllAccountCode()
        {
            var apiResponse = new ApiResponse<List<AccountCode>>();

            try
            {
                var claim = Common.DecodeJwt(HttpContext.User);
                var result = await _unitOfWork.AnalysisAccounts.GetAccountCode(claim.DbCode!);

                if (!result.Any())
                {
                    apiResponse.Success = false;
                    apiResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                    apiResponse.Message = "No account code";
                    apiResponse.Result = new List<AccountCode>();
                    return apiResponse;
                }


                // Set the API response
                apiResponse.Success = true;
                apiResponse.Message = "Account code fetched successfully.";
                apiResponse.StatusCode = (int)HttpStatusCode.OK;
                apiResponse.Result = result;
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

        [HttpGet("accountcodespaged")]
        public async Task<ApiResponse<PaginatedResponse<AccountCode>>> GetAllAccountCode([FromQuery] int page = 1, int pageSize = 10)
        {
            var apiResponse = new ApiResponse<PaginatedResponse<AccountCode>>();

            try
            {
                var claim = Common.DecodeJwt(HttpContext.User);

                // Fetch all records
                var allRecords = await _unitOfWork.AnalysisAccounts.GetAccountCode(claim.DbCode!,page,pageSize);

                if (!allRecords.Any())
                {
                    apiResponse.Success = false;
                    apiResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                    apiResponse.Message = "No account code";
                    apiResponse.Result = new PaginatedResponse<AccountCode>(new List<AccountCode>(), 0, page, pageSize);
                    return apiResponse;
                }

                // Calculate pagination details
                var totalRecords = allRecords.Count;
                //var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

                // Paginate the data
                var paginatedData = allRecords
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // Set the API response
                apiResponse.Success = true;
                apiResponse.Message = "Account code fetched successfully.";
                apiResponse.StatusCode = (int)HttpStatusCode.OK;
                apiResponse.Result = new PaginatedResponse<AccountCode>(paginatedData, totalRecords, page, pageSize);
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

        [HttpGet("analysistypes")]
        public async Task<ApiResponse<List<AnalysisCodeType>>> GetAnalysisType()
        {
            var apiResponse = new ApiResponse<List<AnalysisCodeType>>();

            try
            {
                var claim = Common.DecodeJwt(HttpContext.User);
                var result = await _unitOfWork.AnalysisAccounts.GetAnalysisType(claim.DbCode!);

                if (!result.Any())
                {
                    apiResponse.Success = false;
                    apiResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                    apiResponse.Message = "No analysis type";
                    apiResponse.Result = new List<AnalysisCodeType>();
                    return apiResponse;
                }


                // Set the API response
                apiResponse.Success = true;
                apiResponse.Message = "Analysis code fetched successfully.";
                apiResponse.StatusCode = (int)HttpStatusCode.OK;
                apiResponse.Result = result;
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
    }
}
