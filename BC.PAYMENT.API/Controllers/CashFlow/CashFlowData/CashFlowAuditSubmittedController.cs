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
    public class CashFlowAuditSubmittedController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public CashFlowAuditSubmittedController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("getallcashflowsubmitted")]
        public async Task<ApiResponse<List<CashFlowDataHeader>>> GetCashFlowHeaderSubmittedAsync()
        {
            var credential = Common.DecodeJwt(User);
            var response = new ApiResponse<List<CashFlowDataHeader>>();
            try
            {
                var result =
                    await _unitOfWork.CashFlowAuditSubmitted.GetCashFlowHeaderSubmittedAsync(credential.DbCode!);
                if (result.Any())
                {
                    response.Result = result;
                    response.Message = "Payment cash flow submitted fetched successfully";
                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.Success = true;
                }
                else
                {
                    response.Result = new List<CashFlowDataHeader>();
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
        
        [HttpPut]
        [Route("auditcashflowheadertocompleted/{headerId}")]
        public async Task<ApiResponse<int>> AuditCashFlowHeaderToCompletedAsync([Required] int headerId)
        {
            var credential = Common.DecodeJwt(User);
            var response = new ApiResponse<int>();
            try
            {
                const string status = "Completed";
                var affectedRow =
                    await _unitOfWork.CashFlowAuditSubmitted.AuditCashFlowAsync(credential.Username!,headerId,status);
                if (affectedRow > 0 )
                {
                    response.Result = affectedRow;
                    response.Message = "Payment cash flow completed updated successfully";
                    response.StatusCode = (int)HttpStatusCode.NoContent;
                    response.Success = true;
                }
                else
                {
                    response.Message = "Payment cash flow completed updated successfully";
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
        
        [HttpPut]
        [Route("auditcashflowheadertorejected/{headerId}")]
        public async Task<ApiResponse<int>> AuditCashFlowHeaderToRejectedAsync([Required] int headerId)
        {
            var credential = Common.DecodeJwt(User);
            var response = new ApiResponse<int>();
            try
            {
                const string status = "Rejected";
                var affectedRow =
                    await _unitOfWork.CashFlowAuditSubmitted.AuditCashFlowAsync(credential.Username!,headerId,status);
                if (affectedRow > 0 )
                {
                    response.Result = affectedRow;
                    response.Message = "Payment cash flow rejected updated successfully";
                    response.StatusCode = (int)HttpStatusCode.NoContent;
                    response.Success = true;
                }
                else
                {
                    response.Message = "Payment cash flow rejected updated successfully";
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
        [Route("getcashflowdetailpending/{headerId}")]
        public async Task<ApiResponse<List<PaymentCashFlowModel>>> GetCashFlowDetailPendingAsync([Required] int headerId)
        {
            var credential = Common.DecodeJwt(User);
            var response = new ApiResponse<List<PaymentCashFlowModel>>();
            try
            {
                var execute =
                    await _unitOfWork.CashFlowAuditSubmitted.GetCashFlowDetailPendingAsync(credential.DbCode!,headerId);
                if (execute.Any())
                {
                    response.Result = execute;
                    response.Message = "Payment cash flow pending fetched successfully";
                    response.StatusCode = (int)HttpStatusCode.NoContent;
                    response.Success = true;
                }
                else
                {
                    response.Result = new List<PaymentCashFlowModel>();
                    response.Message = "Payment cash flow pending fetched successfully";
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

