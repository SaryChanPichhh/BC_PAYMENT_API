using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using BC.PAYMENT.API.Helper;

namespace BC.PAYMENT.API.Controllers.Report.OthersReport;

public class BillsOwedController : BaseApiController
{
    private readonly IUnitOfWork _unitOfWork;

    public BillsOwedController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    [Route("getaccountreceviable/{period}")]
    public async Task<ApiResponse<List<Dictionary<string, object>>>> GetMonthlyHistoryPaidInvoiceByDateAsync(
        [Required] string period)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var execute = await _unitOfWork.billsOwed.GetBillOwedByDateAsync(credential.DbCode, period);
            if (execute.Any())
                return ApiResponse<List<Dictionary<string, object>>>.Builder()
                    .WithResult(execute)
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithMessage("Bills owed fetched successfully")
                    .Build();
            else
                return ApiResponse<List<Dictionary<string, object>>>.Builder()
                    .WithStatusCode(StatusCodes.Status400BadRequest)
                    .WithMessage("Bills owed fetched unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<Dictionary<string, object>>>(ex.Message);
        }
    }
}