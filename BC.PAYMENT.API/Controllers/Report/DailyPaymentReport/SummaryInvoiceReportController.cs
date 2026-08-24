using BC.PAYMENT.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.Entities.Report.DailyPaymentReport.SummaryInvoice;

namespace BC.PAYMENT.API.Controllers.Report.DailyPaymentReport
{
    public class SummaryInvoiceReportController(IUnitOfWork unitOfWork) : BaseApiController
    {
        [HttpGet]
        [Route("getsummaryinvoicebydate/{fromDate}/{toDate}")]
        public async Task<ApiResponse<List<SummaryInvoiceReportModel>>> GetCreditInvoiceReportByDateAsync([Required] string fromDate, [Required] string toDate)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var execute = await unitOfWork.SummaryInvoice.GetSummaryInvoiceReportsByDateAsync(credential.DbCode, Convert.ToDateTime(fromDate), Convert.ToDateTime(toDate));
                if (execute.Any())
                {
                    return ApiResponse<List<SummaryInvoiceReportModel>>.Builder()
                        .WithResult(execute)
                        .WithStatusCode(StatusCodes.Status200OK)
                        .WithMessage("Summary Invoice fetched successfully")
                        .Build();
                }
                else
                {
                    return ApiResponse<List<SummaryInvoiceReportModel>>.Builder()
                        .WithStatusCode(StatusCodes.Status400BadRequest)
                        .WithMessage("Summary Invoice fetched unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<SummaryInvoiceReportModel>>(ex.Message);
            }
        }
    }
}
