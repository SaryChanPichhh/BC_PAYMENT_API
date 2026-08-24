using BC.PAYMENT.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.Entities.Report.DailyPaymentReport.PaidInvoice;

namespace BC.PAYMENT.API.Controllers.Report.DailyPaymentReport
{
    public class PaidInvoiceController(IUnitOfWork unitOfWork) : BaseApiController
    {
        [HttpGet]
        [Route("getpaidinvoicebydate/{fromDate}/{toDate}")]
        public async Task<ApiResponse<List<PaidInvoiceReportModel>>> GetCreditInvoiceReportByDateAsync([Required] string fromDate, [Required] string toDate)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var execute = await unitOfWork.PaidInvoice.GetPaidInvoiceByDateAsync(credential.DbCode, Convert.ToDateTime(fromDate), Convert.ToDateTime(toDate));
                if (execute.Any())
                {
                    return ApiResponse<List<PaidInvoiceReportModel>>.Builder()
                        .WithMessage("Credit Invoice fetched successfully")
                        .WithStatusCode(StatusCodes.Status200OK)
                        .WithResult(execute)
                        .Build();
                }
                else
                {
                    return ApiResponse<List<PaidInvoiceReportModel>>.Builder()
                        .WithMessage("Credit Invoice fetched unsuccessfully")
                        .WithStatusCode(StatusCodes.Status400BadRequest)
                        .WithResult(new List<PaidInvoiceReportModel>())
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<PaidInvoiceReportModel>>(ex.Message);
            }
        }
    }
}
