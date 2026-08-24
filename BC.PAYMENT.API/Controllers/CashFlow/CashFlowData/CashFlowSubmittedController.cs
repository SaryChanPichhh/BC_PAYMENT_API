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
    public class CashFlowSubmittedController(IUnitOfWork unitOfWork) : BaseApiController
    {

        [HttpGet]
        [Route("getallpaymentcashflowsubmitted")]
        public async Task<ApiResponse<List<PaymentCashFlowSubmittedModel>>> GetPaymentCashFlowModelSubmittedAsync()
        {
            try
            {
                var credential = Common.DecodeJwt(User);
                var result = await unitOfWork.CashFlowSubmitted.GetPaymentCashFlowModelSubmittedAsync(credential.DbCode!);
                
                return ApiResponse<List<PaymentCashFlowSubmittedModel>>.Builder()
                    .WithMessage("Payment cash flow submitted fetched successfully")
                    .WithStatusCode(result.Any() ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest)
                    .WithResult(result.Any() ? result : new List<PaymentCashFlowSubmittedModel>())
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<PaymentCashFlowSubmittedModel>>(ex.Message);
            }
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
            try
            {
                var credential = Common.DecodeJwt(User);
                var result = await unitOfWork.CashFlowSubmitted.GetPaymentCashFlowModelSubmittedByDateAsync(credential.DbCode!, Convert.ToDateTime(fromDate), Convert.ToDateTime(toDate));
                
                return ApiResponse<List<PaymentCashFlowSubmittedModel>>.Builder()
                    .WithMessage("Payment cash flow report fetched successfully")
                    .WithStatusCode(result.Any() ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest)
                    .WithResult(result.Any() ? result : new List<PaymentCashFlowSubmittedModel>())
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<PaymentCashFlowSubmittedModel>>(ex.Message);
            }
        }
    }
}
