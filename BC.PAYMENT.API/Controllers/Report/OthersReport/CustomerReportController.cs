using BC.PAYMENT.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.Entities.Report.OthersReport;

namespace BC.PAYMENT.API.Controllers.Report.OthersReport
{
    public class CustomerReportController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public CustomerReportController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("getaccountreceviable/{fromDate}/{toDate}")]
        public async Task<ApiResponse<List<CustomerReportModel>>> GetMonthlyHistoryPaidInvoiceByDateAsync([Required] string fromDate, [Required] string toDate)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var execute = await _unitOfWork.CustomerReport.GetCustomerReportByDateAsync(credential.DbCode,Convert.ToDateTime(fromDate) , Convert.ToDateTime(toDate));
                if (execute.Any())
                {
                    return ApiResponse<List<CustomerReportModel>>.Builder()
                        .WithResult(execute)
                        .WithStatusCode(StatusCodes.Status200OK)
                        .WithMessage("Customers report fetched successfully")
                        .Build();
                }
                else
                {
                    return ApiResponse<List<CustomerReportModel>>.Builder()
                        .WithStatusCode(StatusCodes.Status400BadRequest)
                        .WithMessage("Customers report fetched unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<CustomerReportModel>>(ex.Message);
            }
        }
    }
}
