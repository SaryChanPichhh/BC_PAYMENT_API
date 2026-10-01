using System.ComponentModel.DataAnnotations;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.Entities.Transaction.Submitting.HistoryApproval;
using BC.PAYMENT.LOGGING;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BC.PAYMENT.API.Controllers.Transaction.Submitting.HistoryApproval;

public class SubmissionHistoryController(IUnitOfWork unitOfWork) : BaseApiController
{
    [HttpGet]
    [Route("gethistoryapprovalbydate/{fromDate}/{toDate}")]
    public async Task<ApiResponse<List<SubmissionHistoryModel>>> GetHistoryApprovalByDate([Required] string fromDate,
        [Required] string toDate)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var execute =
                await unitOfWork.HistoryApproval.GetHistoryApprovalByDateAsync(credential.DbCode!, fromDate,
                    toDate);
            if (execute.Any())
                return ApiResponse<List<SubmissionHistoryModel>>.Builder()
                    .WithResult(execute)
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithMessage("History Approval fetched successfully")
                    .Build();
            else
                return ApiResponse<List<SubmissionHistoryModel>>.Builder()
                    .WithStatusCode(StatusCodes.Status400BadRequest)
                    .WithMessage("History Approval fetched unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<SubmissionHistoryModel>>(ex.Message);
        }
    }
}