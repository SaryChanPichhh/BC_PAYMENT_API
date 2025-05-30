using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.Entities.CashFlow.CashFlowData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.Net;
using BC.PAYMENT.API.Helper;

namespace BC.PAYMENT.API.Controllers.CashFlow.CashFlowData
{
    public class CashFlowSubmittedController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public CashFlowSubmittedController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("getallpaymentcashflowsubmitted")]
        public async Task<ApiResponse<List<PaymentCashFlowSubmittedModel>>> GetPaymentCashFlowModelSubmittedAsync()
        {
            var credential = Common.DecodeJwt(User);
            var response = new ApiResponse<List<PaymentCashFlowSubmittedModel>>();
            try
            {
                var result = await _unitOfWork.CashFlowSubmitted.GetPaymentCashFlowModelSubmittedAsync(credential.DbCode!);
                if (result.Any())
                {
                    response.Result = result;
                    response.Message = "Payment cash flow submitted fetched successfully";
                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.Success = true;
                }
                else
                {
                    response.Result = new List<PaymentCashFlowSubmittedModel>();
                    response.Message = "Payment cash flow submitted fetched successfully";
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromDate">2025-05-26</param>
        /// <param name="toDate">2025-05-26</param>
        /// <returns></returns>
        [HttpGet]
        [Route("getpaymentcashflowsubmittedbydate/{fromDate}/{toDate}")]
        public async Task<ApiResponse<List<PaymentCashFlowSubmittedModel>>> GetPaymentCashFlowModelSubmittedByDateAsync([Required] string fromDate, [Required] string toDate)
        {
            var credential = Common.DecodeJwt(User);
            var response = new ApiResponse<List<PaymentCashFlowSubmittedModel>>();
            try
            {
                var result = await _unitOfWork.CashFlowSubmitted.GetPaymentCashFlowModelSubmittedByDateAsync(credential.DbCode!, Convert.ToDateTime(fromDate), Convert.ToDateTime(toDate));
                if (result.Any())
                {
                    response.Result = result;
                    response.Message = "Payment cash flow report fetched successfully";
                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.Success = true;
                }
                else
                {
                    response.Result = new List<PaymentCashFlowSubmittedModel>();
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
    }
}
