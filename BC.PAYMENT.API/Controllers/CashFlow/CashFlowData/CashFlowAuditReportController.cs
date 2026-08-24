using System.ComponentModel.DataAnnotations;
using System.Net;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.Entities.CashFlow.CashFlowData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BC.PAYMENT.API.Controllers.CashFlow.CashFlowData
{
    public class CashFlowAuditReportController(IUnitOfWork unitOfWork) : BaseApiController
    {
        [HttpGet]
        [Route("getallpaymentcashflowreportbydate/{fromDate}/{toDate}")]
        public async Task<ApiResponse<List<PaymentCashFlowModel>>> GetPaymentCashFlowReportByDateAsync([Required] string fromDate, [Required] string toDate)
        {
            try
            {
                var credential = Common.DecodeJwt(User);
                var result = await unitOfWork.CashFlowDataReport.GetPaymentCashFlowReportByDateAsync(credential.DbCode!, Convert.ToDateTime(fromDate), Convert.ToDateTime(toDate));
                
                return ApiResponse<List<PaymentCashFlowModel>>.Builder()
                    .WithMessage("Payment cash flow report fetched successfully")
                    .WithStatusCode(result.Any() ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest)
                    .WithResult(result.Any() ? result : new List<PaymentCashFlowModel>())
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<PaymentCashFlowModel>>(ex.Message);
            }
        }

        [HttpGet]
        [Route("getpaymentcashflowauditcompletedbydate/{fromDate}/{toDate}")]
        public async Task<ApiResponse<List<PaymentCashFlowSubmittedModel>>> GetCashFlowAuditCompletedByDateAsync([Required] string fromDate, [Required] string toDate)
        {
            try
            {
                var credential = Common.DecodeJwt(User);
                const string status = "Completed";
                var execute = await unitOfWork.CashFlowAuditReport.GetPaymentCashFlowAuditReportByDateAsync(credential.DbCode!, Convert.ToDateTime(fromDate), Convert.ToDateTime(toDate), status);
                
                return ApiResponse<List<PaymentCashFlowSubmittedModel>>.Builder()
                    .WithMessage(execute.Any() ? "Payment cash flow audit report completed fetched successfully" : "Payment cash flow audit report completed fetched unsuccessfully")
                    .WithStatusCode(execute.Any() ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest)
                    .WithResult(execute.Any() ? execute : new List<PaymentCashFlowSubmittedModel>())
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<PaymentCashFlowSubmittedModel>>(ex.Message);
            }
        }
        
        [HttpGet]
        [Route("getpaymentcashflowauditrejectedbydate/{fromDate}/{toDate}")]
        public async Task<ApiResponse<List<PaymentCashFlowSubmittedModel>>> GetCashFlowAuditRejectedByDateAsync([Required] string fromDate, [Required] string toDate)
        {
            try
            {
                var credential = Common.DecodeJwt(User);
                const string status = "Rejected";
                var execute = await unitOfWork.CashFlowAuditReport.GetPaymentCashFlowAuditReportByDateAsync(credential.DbCode!, Convert.ToDateTime(fromDate), Convert.ToDateTime(toDate), status);

                return ApiResponse<List<PaymentCashFlowSubmittedModel>>.Builder()
                    .WithMessage(execute.Any() ? "Payment cash flow audit report rejected fetched successfully" : "Payment cash flow audit report rejected fetched unsuccessfully")
                    .WithStatusCode(execute.Any() ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest)
                    .WithResult(execute.Any() ? execute : new List<PaymentCashFlowSubmittedModel>())
                    .Build();
            }
            catch (SqlException ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<PaymentCashFlowSubmittedModel>>(ex.Message);
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<PaymentCashFlowSubmittedModel>>(ex.Message);
            }
        }
    }
}
