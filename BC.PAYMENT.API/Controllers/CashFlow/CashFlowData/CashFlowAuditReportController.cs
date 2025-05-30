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
    public class CashFlowAuditReportController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public CashFlowAuditReportController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        [Route("getallpaymentcashflowreportbydate/{fromDate}/{toDate}")]
        public async Task<ApiResponse<List<PaymentCashFlowModel>>> GetPaymentCashFlowReportByDateAsync([Required] string fromDate, [Required] string toDate)
        {
            var credential = Common.DecodeJwt(User);
            var response = new ApiResponse<List<PaymentCashFlowModel>>();
            try
            {
                var result = await _unitOfWork.CashFlowDataReport.GetPaymentCashFlowReportByDateAsync(credential.DbCode!, Convert.ToDateTime(fromDate), Convert.ToDateTime(toDate));
                if (result.Any())
                {
                    response.Result = result;
                    response.Message = "Payment cash flow report fetched successfully";
                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.Success = true;
                }
                else
                {
                    response.Result = new List<PaymentCashFlowModel>();
                    response.Message = "Payment cash flow report fetched successfully";
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                }

            }
            catch (SqlException ex)
            {
                response.Message = $"Sql Exception : {ex.Message}";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            catch (Exception ex)
            {
                response.Message = $"Error Exception : {ex.Message}";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            return response;
        }

        [HttpGet]
        [Route("getpaymentcashflowauditcompletedbydate/{fromDate}/{toDate}")]
        public async Task<ApiResponse<List<PaymentCashFlowSubmittedModel>>> GetCashFlowAuditCompletedByDateAsync([Required] string fromDate, [Required] string toDate)
        {
            var credential = Common.DecodeJwt(User);
            var auditReport = new ApiResponse<List<PaymentCashFlowSubmittedModel>>();
            try
            
            {
                const string status = "Completed";
                var execute = await _unitOfWork.CashFlowAuditReport.GetPaymentCashFlowAuditReportByDateAsync(credential.DbCode!, Convert.ToDateTime(fromDate), Convert.ToDateTime(toDate), status);
                if (execute.Any())
                {
                    auditReport.Result = execute;
                    auditReport.Message = "Payment cash flow audit report completed fetched successfully";
                    auditReport.StatusCode = (int)HttpStatusCode.OK;
                    auditReport.Success = true;
                }
                else
                {
                    auditReport.Result = new List<PaymentCashFlowSubmittedModel>();
                    auditReport.Message = "Payment cash flow audit report completed fetched unsuccessfully";
                    auditReport.StatusCode = (int)HttpStatusCode.BadRequest;
                }
            }catch(SqlException ex)
            {
                auditReport.Message = $"Sql Exception : {ex.Message}";
                auditReport.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            catch(Exception ex)
            {
                auditReport.Message = $"Error Exception : {ex.Message}";
                auditReport.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            return auditReport;
        }
        
        [HttpGet]
        [Route("getpaymentcashflowauditrejectedbydate/{fromDate}/{toDate}")]
        public async Task<ApiResponse<List<PaymentCashFlowSubmittedModel>>> GetCashFlowAuditRejectedByDateAsync([Required] string fromDate, [Required] string toDate)
        {
            var credential = Common.DecodeJwt(User);
            var auditReport = new ApiResponse<List<PaymentCashFlowSubmittedModel>>();
            try
            
            {
                const string status = "Rejected";
                var execute = await _unitOfWork.CashFlowAuditReport.GetPaymentCashFlowAuditReportByDateAsync(credential.DbCode!, Convert.ToDateTime(fromDate), Convert.ToDateTime(toDate), status);
                if (execute.Any())
                {
                    auditReport.Result = execute;
                    auditReport.Message = "Payment cash flow audit report rejected fetched successfully";
                    auditReport.StatusCode = (int)HttpStatusCode.OK;
                    auditReport.Success = true;
                }
                else
                {
                    auditReport.Result = new List<PaymentCashFlowSubmittedModel>();
                    auditReport.Message = "Payment cash flow audit report rejected fetched unsuccessfully";
                    auditReport.StatusCode = (int)HttpStatusCode.BadRequest;
                }
            }catch(SqlException ex)
            {
                auditReport.Message = $"Sql Exception : {ex.Message}";
                auditReport.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            catch(Exception ex)
            {
                auditReport.Message = $"Error Exception : {ex.Message}";
                auditReport.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            return auditReport;
        }
    }
}
