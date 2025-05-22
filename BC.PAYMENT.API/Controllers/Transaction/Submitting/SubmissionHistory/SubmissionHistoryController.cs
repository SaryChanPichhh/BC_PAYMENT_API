using System.ComponentModel.DataAnnotations;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.Entities.Transaction.Submitting.HistoryApproval;
using BC.PAYMENT.LOGGING;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BC.PAYMENT.API.Controllers.Transaction.Submitting.HistoryApproval
{
    public class SubmissionHistoryController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubmissionHistoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("gethistoryapprovalbydate/{fromDate}/{toDate}")]
        public async Task<ApiResponse<List<SubmissionHistoryModel>>> GetHistoryApprovalByDate([Required]string fromDate,
            [Required]string toDate)
        {
            var credential = Common.DecodeJwt(User);
            var historyApproval = new ApiResponse<List<SubmissionHistoryModel>>();

            try
            {
                var execute =
                    await _unitOfWork.HistoryApproval.GetHistoryApprovalByDateAsync(credential.DbCode!, fromDate,
                        toDate);
                if (execute.Any())
                {
                    historyApproval.Result = execute;
                    historyApproval.StatusCode = StatusCodes.Status200OK;
                    historyApproval.Message = "History Approval fetched successfully";
                    historyApproval.Success = true;
                }
                else
                {
                    historyApproval.StatusCode = StatusCodes.Status400BadRequest;
                    historyApproval.Message = "History Approval fetched unsuccessfully";
                    historyApproval.Success = false;
                }
            }
            catch (SqlException ex)
            {
                historyApproval.StatusCode = StatusCodes.Status500InternalServerError;
                historyApproval.Message = $"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql Exception", ex);
            }
            catch (Exception ex)
            {
                historyApproval.StatusCode = StatusCodes.Status500InternalServerError;
                historyApproval.Message = $"Error Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }
            return historyApproval;
        }
    }
}
