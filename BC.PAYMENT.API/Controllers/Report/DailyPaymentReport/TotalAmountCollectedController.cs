using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.INFRASTRUCTURE.DBAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DailyPayment;

namespace BC.PAYMENT.API.Controllers.Report.DailyPaymentReport;

public class TotalAmountCollectedController(IUnitOfWork unitOfWork) : BaseApiController
{
    [HttpGet]
    [Route("gettotalamountcollectedbydate/{fromDate}/{toDate}")]
    public async Task<ApiResponse<List<ExpenseDetailModel>>> GetCreditInvoiceReportByDateAsync(
        [Required] string fromDate, [Required] string toDate)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var execute = await unitOfWork.AmountCollected.GetAmountCollectedReportsByDateAsync(credential.DbCode,
                Convert.ToDateTime(fromDate), Convert.ToDateTime(toDate));
            if (execute.Any())
                return ApiResponse<List<ExpenseDetailModel>>.Builder()
                    .WithResult(execute)
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithMessage("Amount collected fetched successfully")
                    .Build();
            else
                return ApiResponse<List<ExpenseDetailModel>>.Builder()
                    .WithStatusCode(StatusCodes.Status400BadRequest)
                    .WithMessage("Amount collected fetched unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<ExpenseDetailModel>>(ex.Message);
        }
    }
}