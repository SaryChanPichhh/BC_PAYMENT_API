using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.Transaction.ProvincialPayment.ReviewReport;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BC.PAYMENT.API.Controllers.Transaction.Provincial_Payment.StockCarPayment
{
    public class ReviewReportController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        public ReviewReportController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region Requestion Action

        [HttpGet]
        [Route("getallrequestionaction")]

        public async Task<ApiResponse<List<RequestionActionDto>>> GetRequestActionAsync()
        {
            var requestAction = new ApiResponse<List<RequestionActionDto>>();
            try
            {
                var result = await _unitOfWork.ReviewReport.GetAllActionAsync();
                if (result.Any())
                {
                    requestAction.Result = result;
                    requestAction.StatusCode = StatusCodes.Status200OK;
                    requestAction.Message = "Request action fetched successfully";
                    requestAction.Success = true;
                }
                else
                {
                    requestAction.Result = result;
                    requestAction.StatusCode = StatusCodes.Status400BadRequest;
                    requestAction.Message = "Request action fetched unsuccessfully";
                    requestAction.Success = false;
                }
                
            }catch (SqlException ex)
            {
                requestAction.StatusCode = StatusCodes.Status500InternalServerError;
                requestAction.Message = $"Sql Exception : {ex.Message}";
            }
            catch (Exception ex)
            {
                requestAction.StatusCode = StatusCodes.Status500InternalServerError;
                requestAction.Message = $"Error Exception : {ex.Message}";
            }
            return requestAction;
        }
        
        #endregion
    }
}
