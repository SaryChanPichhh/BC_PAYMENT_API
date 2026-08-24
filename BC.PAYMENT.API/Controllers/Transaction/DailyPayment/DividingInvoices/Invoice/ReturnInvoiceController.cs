using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.Net;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DividingInvoices.Invoice;

namespace BC.PAYMENT.API.Controllers.Transaction.DailyPayment.DividingInvoices.Invoice
{

    public class ReturnInvoiceController(IUnitOfWork unitOfWork) : BaseApiController
    {
        [HttpGet]
        [Route("")]
        public async Task<ApiResponse<List<ReturnInvoiceModel>>> GetReturnInvoiceAsync()
        {
            var credential = Common.DecodeJwt(HttpContext.User);
            try
            {
                var execute = await unitOfWork.ReturnInvoice.GetReturnInvoiceAsync(credential.DbCode!);
                if (execute.Count > 0)
                {
                    return ApiResponse<List<ReturnInvoiceModel>>.Builder()
                        .WithMessage("Return Invoices fetched successfully")
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithResult(execute)
                        .Build();
                }
                else
                {
                    return ApiResponse<List<ReturnInvoiceModel>>.Builder()
                        .WithMessage("Return Invoices fetched unsuccessfully")
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithResult(new List<ReturnInvoiceModel>())
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<ReturnInvoiceModel>>(ex.Message);
            }
        }

        [HttpGet]
        [Route("getreturninvoicesbydate/{fromDate}/{toDate}")]
        public async Task<ApiResponse<List<ReturnInvoiceModel>>> GetReturnInvoiceByDateAsync([Required] DateTime fromDate, [Required] DateTime toDate)
        {
            var credential = Common.DecodeJwt(HttpContext.User);
            try
            {
                var execute = await unitOfWork.ReturnInvoice.GetReturnInvoiceByDateAsync(credential.DbCode!, fromDate, toDate);
                if (execute.Count > 0)
                {
                    return ApiResponse<List<ReturnInvoiceModel>>.Builder()
                        .WithMessage("Return Invoices fetched successfully")
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithResult(execute)
                        .Build();
                }
                else
                {
                    return ApiResponse<List<ReturnInvoiceModel>>.Builder()
                        .WithMessage("Return Invoices fetched unsuccessfully")
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithResult(new List<ReturnInvoiceModel>())
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<ReturnInvoiceModel>>(ex.Message);
            }
        }
    }
}
