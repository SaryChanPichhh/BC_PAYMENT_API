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
    public class CashFlowAuditSubmittedController(IUnitOfWork unitOfWork) : BaseApiController
    {

        [HttpGet]
        [Route("getallcashflowsubmitted")]
        public async Task<ApiResponse<List<CashFlowDataHeader>>> GetCashFlowHeaderSubmittedAsync()
        {
            try
            {
                var credential = Common.DecodeJwt(User);
                var result = await unitOfWork.CashFlowAuditSubmitted.GetCashFlowHeaderSubmittedAsync(credential.DbCode!);
                return ApiResponse<List<CashFlowDataHeader>>.Builder()
                    .WithMessage("Payment cash flow submitted fetched successfully")
                    .WithStatusCode(result.Any() ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest)
                    .WithResult(result.Any() ? result : new List<CashFlowDataHeader>())
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<CashFlowDataHeader>>(ex.Message);
            }
        }
        
        [HttpPut]
        [Route("auditcashflowheadertocompleted/{headerId}")]
        public async Task<ApiResponse<int>> AuditCashFlowHeaderToCompletedAsync([Required] int headerId)
        {
            try
            {
                var credential = Common.DecodeJwt(User);
                const string status = "Completed";
                var affectedRow = await unitOfWork.CashFlowAuditSubmitted.AuditCashFlowAsync(credential.Username!,headerId,status);
                
                return ApiResponse<int>.Builder()
                    .WithMessage("Payment cash flow completed updated successfully")
                    .WithStatusCode(affectedRow > 0 ? (int)HttpStatusCode.NoContent : (int)HttpStatusCode.BadRequest)
                    .WithResult(affectedRow > 0 ? affectedRow : 0)
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
            }
        }
        
        [HttpPut]
        [Route("auditcashflowheadertorejected/{headerId}")]
        public async Task<ApiResponse<int>> AuditCashFlowHeaderToRejectedAsync([Required] int headerId)
        {
            try
            {
                var credential = Common.DecodeJwt(User);
                const string status = "Rejected";
                var affectedRow = await unitOfWork.CashFlowAuditSubmitted.AuditCashFlowAsync(credential.Username!,headerId,status);
                
                return ApiResponse<int>.Builder()
                    .WithMessage("Payment cash flow rejected updated successfully")
                    .WithStatusCode(affectedRow > 0 ? (int)HttpStatusCode.NoContent : (int)HttpStatusCode.BadRequest)
                    .WithResult(affectedRow > 0 ? affectedRow : 0)
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
            }
        }
        
        [HttpGet]
        [Route("getcashflowdetailpending/{headerId}")]
        public async Task<ApiResponse<List<PaymentCashFlowModel>>> GetCashFlowDetailPendingAsync([Required] int headerId)
        {
            try
            {
                var credential = Common.DecodeJwt(User);
                var execute = await unitOfWork.CashFlowAuditSubmitted.GetCashFlowDetailPendingAsync(credential.DbCode!,headerId);
                
                return ApiResponse<List<PaymentCashFlowModel>>.Builder()
                    .WithMessage("Payment cash flow pending fetched successfully")
                    .WithStatusCode(execute.Any() ? (int)HttpStatusCode.NoContent : (int)HttpStatusCode.BadRequest)
                    .WithResult(execute.Any() ? execute : new List<PaymentCashFlowModel>())
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<PaymentCashFlowModel>>(ex.Message);
            }
        }
    }
}

