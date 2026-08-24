using BC.PAYMENT.API.Models;
using BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DailyPayment;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.Entities.Report.DailyPaymentReport.TotalMonthPayment;

namespace BC.PAYMENT.API.Controllers.Report.DailyPaymentReport
{
    public class TotalMonthlyPaymentController(IUnitOfWork unitOfWork) : BaseApiController
    {
        [HttpGet]
        [Route("getpaidinvoicebydate/{fromDate}/{toDate}")]
        public async Task<ApiResponse<List<MonthlyPaymentModel>>> GetAllPaidInvoiceByDateAsync([Required] string fromDate, [Required] string toDate)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var execute = await unitOfWork.TotalMonthlyPayment.GetAllPaidInvoiceByDateAsync(credential.DbCode, Convert.ToDateTime(fromDate), Convert.ToDateTime(toDate));
                if (execute.Any())
                {
                    return ApiResponse<List<MonthlyPaymentModel>>.Builder()
                        .WithResult(execute)
                        .WithStatusCode(StatusCodes.Status200OK)
                        .WithMessage("Paid invoice fetched successfully")
                        .Build();
                }
                else
                {
                    return ApiResponse<List<MonthlyPaymentModel>>.Builder()
                        .WithStatusCode(StatusCodes.Status400BadRequest)
                        .WithMessage("Paid invoice fetched unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<MonthlyPaymentModel>>(ex.Message);
            }
        }

        [HttpGet]
        [Route("gethistorypaidinvoicebydate/{fromDate}/{toDate}")]
        public async Task<ApiResponse<List<Dictionary<string, object>>>> GetMonthlyHistoryPaidInvoiceByDateAsync([Required] string fromDate, [Required] string toDate)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var execute = await unitOfWork.TotalMonthlyPayment.GetMonthlyHistoryPaidInvoiceByDateAsync(credential.DbCode, Convert.ToDateTime(fromDate), Convert.ToDateTime(toDate));
                if (execute.Any())
                {
                    return ApiResponse<List<Dictionary<string, object>>>.Builder()
                        .WithResult(execute)
                        .WithStatusCode(StatusCodes.Status200OK)
                        .WithMessage("History paid invoice fetched successfully")
                        .Build();
                }
                else
                {
                    return ApiResponse<List<Dictionary<string, object>>>.Builder()
                        .WithStatusCode(StatusCodes.Status400BadRequest)
                        .WithMessage("History paid invoice fetched unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<Dictionary<string, object>>>(ex.Message);
            }
        }

    }
}
