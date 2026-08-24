using System.ComponentModel.DataAnnotations;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Net;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DividingInvoices.Invoice;

namespace BC.PAYMENT.API.Controllers.Transaction.DailyPayment.DividingInvoices.Invoice
{
    public class IssuanceInvoiceController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public IssuanceInvoiceController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        // print
        [HttpGet]
        [Route("getissuanceinvoicesbytransaction/{areaId}/{transaction}")]
        public async Task<ApiResponse<List<IssuanceModel>>> GetIssuanceInvoiceByTransactionAsync([Required] string areaId, [Required] string transaction)
        {
            var credential = Common.DecodeJwt(HttpContext.User);
            try
            {
                var execute = await _unitOfWork.IssuanceInvoice.GetIssuanceInvoiceByTransactionAsync(credential.DbCode!, areaId, transaction);
                if (execute.Count > 0)
                {
                    return ApiResponse<List<IssuanceModel>>.Builder()
                        .WithMessage("Issuance Invoices fetched successfully")
                        .Success()
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithResult(execute)
                        .Build();
                }
                else
                {
                    return ApiResponse<List<IssuanceModel>>.Builder()
                        .WithMessage("Issuance Invoices fetched unsuccessfully")
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithResult(new List<IssuanceModel>())
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<IssuanceModel>>(ex.Message);
            }
        }

        [HttpGet]
        [Route("getissuanceinvoices/{areaId}")]
        public async Task<ApiResponse<List<IssuanceModel>>> GetIssuanceInvoiceAsync([Required] string areaId)
        {
            var credential = Common.DecodeJwt(HttpContext.User);
            try
            {
                var execute = await _unitOfWork.IssuanceInvoice.GetIssuanceInvoiceAsync(credential.DbCode!, areaId);
                if (execute.Count > 0)
                {
                    return ApiResponse<List<IssuanceModel>>.Builder()
                        .WithMessage("Issuance Invoices fetched successfully")
                        .Success()
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithResult(execute)
                        .Build();
                }
                else
                {
                    return ApiResponse<List<IssuanceModel>>.Builder()
                        .WithMessage("Issuance Invoices fetched unsuccessfully")
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithResult(new List<IssuanceModel>())
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<IssuanceModel>>(ex.Message);
            }
        }
    }
}
