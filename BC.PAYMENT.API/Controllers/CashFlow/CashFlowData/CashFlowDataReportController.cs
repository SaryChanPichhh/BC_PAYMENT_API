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

namespace BC.PAYMENT.API.Controllers.CashFlow.CashFlowData;

public class CashFlowDataReportController(IUnitOfWork unitOfWork) : BaseApiController
{
    [HttpGet]
    [Route("getallpaymentcashflowreport")]
    public async Task<ApiResponse<List<PaymentCashFlowModel>>> GetAllPaymentCashFlowReportAsync()
    {
        try
        {
            var credential = Common.DecodeJwt(User);
            var result = await unitOfWork.CashFlowDataReport.GetPaymentCashFlowReportAsync(credential.DbCode!);

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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fromDate">2025-05-26</param>
    /// <param name="toDate">2025-05-26</param>
    /// <returns></returns>
    [HttpGet]
    [Route("getpaymentcashflowreportbydate/{fromDate}/{toDate}")]
    public async Task<ApiResponse<List<PaymentCashFlowModel>>> GetPaymentCashFlowReportByDateAsync(
        [Required] string fromDate, [Required] string toDate)
    {
        try
        {
            var credential = Common.DecodeJwt(User);
            var result = await unitOfWork.CashFlowDataReport.GetPaymentCashFlowReportByDateAsync(credential.DbCode!,
                Convert.ToDateTime(fromDate), Convert.ToDateTime(toDate));

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
}