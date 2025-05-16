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

    public class ReturnInvoiceController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReturnInvoiceController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        [Route("")]
        public async Task<ApiResponse<List<ReturnInvoiceModel>>> GetReturnInvoiceAsync()
        {
            var credential = Common.DecodeJwt(HttpContext.User);
            var returnInvoice = new ApiResponse<List<ReturnInvoiceModel>>();
            try
            {
                var execute = await _unitOfWork.ReturnInvoice.GetReturnInvoiceAsync(credential.DbCode!);
                if (execute.Count > 0)
                {
                    returnInvoice.Message = "Return Invoices fetched successfully";
                    returnInvoice.Success = true;
                    returnInvoice.StatusCode = (int)HttpStatusCode.OK;
                    returnInvoice.Result = execute;
                }
                else
                {
                    returnInvoice.Message = "Return Invoices fetched unsuccessfully";
                    returnInvoice.StatusCode = (int)HttpStatusCode.BadRequest;
                    returnInvoice.Result = new List<ReturnInvoiceModel>();
                }
            }
            catch (SqlException ex)
            {
                returnInvoice.Message = ex.Message;
                returnInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                returnInvoice.Result = new List<ReturnInvoiceModel>();
            }
            catch (Exception ex)
            {
                returnInvoice.Message = ex.Message;
                returnInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                returnInvoice.Result = new List<ReturnInvoiceModel>();
            }
            return returnInvoice;
        }

        [HttpGet]
        [Route("getreturninvoicesbydate/{fromDate}/{toDate}")]
        public async Task<ApiResponse<List<ReturnInvoiceModel>>> GetReturnInvoiceByDateAsync([Required] DateTime fromDate, [Required] DateTime toDate)
        {
            var credential = Common.DecodeJwt(HttpContext.User);
            var returnInvoice = new ApiResponse<List<ReturnInvoiceModel>>();
            try
            {
                var execute = await _unitOfWork.ReturnInvoice.GetReturnInvoiceByDateAsync(credential.DbCode!, fromDate, toDate);
                if (execute.Count > 0)
                {
                    returnInvoice.Message = "Return Invoices fetched successfully";
                    returnInvoice.Success = true;
                    returnInvoice.StatusCode = (int)HttpStatusCode.OK;
                    returnInvoice.Result = execute;
                }
                else
                {
                    returnInvoice.Message = "Return Invoices fetched unsuccessfully";
                    returnInvoice.StatusCode = (int)HttpStatusCode.BadRequest;
                    returnInvoice.Result = new List<ReturnInvoiceModel>();
                }
            }
            catch (SqlException ex)
            {
                returnInvoice.Message = ex.Message;
                returnInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                returnInvoice.Result = new List<ReturnInvoiceModel>();
            }
            catch (Exception ex)
            {
                returnInvoice.Message = ex.Message;
                returnInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                returnInvoice.Result = new List<ReturnInvoiceModel>();
            }
            return returnInvoice;
        }
    }
}
