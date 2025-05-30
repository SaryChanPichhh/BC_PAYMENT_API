using System.ComponentModel.DataAnnotations;
using System.Net;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.Entities.CashFlow.CashFlowData;
using BC.PAYMENT.INFRASTRUCTURE.Repository.CashFlow.CashFlowData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BC.PAYMENT.API.Controllers.CashFlow.CashFlowData
{
    public class CashFlowDataReportController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public CashFlowDataReportController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("getallpaymentcashflowreport")]
        public async Task<ApiResponse<List<PaymentCashFlowModel>>> GetAllPaymentCashFlowReportAsync()
        {
            var credential = Common.DecodeJwt(User);
            var response = new ApiResponse<List<PaymentCashFlowModel>>();
            try
            {
                var result = await _unitOfWork.CashFlowDataReport.GetPaymentCashFlowReportAsync(credential.DbCode!);
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
            }catch (Exception ex)
            {
                response.Message = $"Error Exception : {ex.Message}";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromDate">2025-05-26</param>
        /// <param name="toDate">2025-05-26</param>
        /// <returns></returns>
        [HttpGet]
        [Route("getpaymentcashflowreportbydate/{fromDate}/{toDate}")]
        public async Task<ApiResponse<List<PaymentCashFlowModel>>> GetPaymentCashFlowReportByDateAsync([Required] string fromDate, [Required] string toDate)
        {
            var credential = Common.DecodeJwt(User);
            var response = new ApiResponse<List<PaymentCashFlowModel>>();
            try
            {
                var result = await _unitOfWork.CashFlowDataReport.GetPaymentCashFlowReportByDateAsync(credential.DbCode!,Convert.ToDateTime( fromDate),Convert.ToDateTime(toDate));
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
            }catch (Exception ex)
            {
                response.Message = $"Error Exception : {ex.Message}";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            return response;
        }
    }
}
