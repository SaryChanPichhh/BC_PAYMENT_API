using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.CORE.Entities.Report.ProvincialPayment;

namespace BC.PAYMENT.API.Controllers.Report.ProvincialPayment
{
    public class CarPaymentReportController(IUnitOfWork unitOfWork) : ControllerBase
    {
        [HttpGet]
        [Route("getcarpaymentreportbyperiod/{period}")]
        public async Task<ApiResponse<List<CarPaymentReportModel>>> GetMonthlyHistoryPaidInvoiceByDateAsync([Required] int period)
        {
            try
            {
                var execute = await unitOfWork.CarPaymentReport.GetCarPaymentReportByPeriodAsync(period);
                if (execute.Any())
                {
                    return ApiResponse<List<CarPaymentReportModel>>.Builder()
                        .WithResult(execute)
                        .WithStatusCode(StatusCodes.Status200OK)
                        .WithMessage("Car payment report fetched successfully")
                        .Build();
                }
                else
                {
                    return ApiResponse<List<CarPaymentReportModel>>.Builder()
                        .WithStatusCode(StatusCodes.Status400BadRequest)
                        .WithMessage("Car payment report fetched unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<CarPaymentReportModel>>(ex.Message);
            }
        }
        
        [HttpGet]
        [Route("getcarpaymentreportbydate/{fromDate}/{toDate}")]
        public async Task<ApiResponse<List<CarPaymentReportModel>>> GetCarPaymentReportByDateAsync([Required] DateTime fromDate, [Required] DateTime toDate)
        {
            try
            {
                var execute = await unitOfWork.CarPaymentReport.GetCarPaymentReportByDateAsync(fromDate, toDate);
                if (execute.Any())
                {
                    return ApiResponse<List<CarPaymentReportModel>>.Builder()
                        .WithResult(execute)
                        .WithStatusCode(StatusCodes.Status200OK)
                        .WithMessage("Car payment report fetched successfully")
                        .Build();
                }
                else
                {
                    return ApiResponse<List<CarPaymentReportModel>>.Builder()
                        .WithStatusCode(StatusCodes.Status400BadRequest)
                        .WithMessage("Car payment report fetched unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<CarPaymentReportModel>>(ex.Message);
            }
        }
    }
}
