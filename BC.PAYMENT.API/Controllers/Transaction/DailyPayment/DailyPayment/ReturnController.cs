using System.ComponentModel.DataAnnotations;
using System.Net;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DailyPayment;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BC.PAYMENT.API.Controllers.Transaction.DailyPayment.DailyPayment
{
    public class ReturnController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReturnController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        //[HttpPost]
        //[Route("getreturninvoicesbydate/{fromDate}/{toDate}")]
        //public async Task<ApiResponse<List<ReturnInvoiceModel>>> GetReturnInvoicesByDateAsync([Required] string fromDate, [Required] string toDate)
        //{
        //    var credential = Common.DecodeJwt(HttpContext.User);
        //    var returnInvoice = new ApiResponse<List<ReturnInvoiceModel>>();
        //    try
        //    {
        //        var execute = await _unitOfWork.DailyPayment.GetReturnInvoiceByDateAsync(credential.DbCode!,Convert.ToDateTime(fromDate),Convert.ToDateTime(toDate));
        //        if (execute.Count > 0)
        //        {
        //            returnInvoice.Message = "Return Invoices fetched successfully";
        //            returnInvoice.Success = true;
        //            returnInvoice.StatusCode = (int)HttpStatusCode.OK;
        //            returnInvoice.Result = execute;
        //        }
        //        else
        //        {
        //            returnInvoice.Message = "Return Invoices fetched unsuccessfully";
        //            returnInvoice.StatusCode = (int)HttpStatusCode.BadRequest;
        //            returnInvoice.Result = new List<ReturnInvoiceModel>();
        //        }
        //    }
        //    catch (SqlException ex)
        //    {
        //        returnInvoice.Message = ex.Message;
        //        returnInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
        //        returnInvoice.Result = new List<ReturnInvoiceModel>();
        //    }
        //    catch (Exception ex)
        //    {
        //        returnInvoice.Message = ex.Message;
        //        returnInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
        //        returnInvoice.Result = new List<ReturnInvoiceModel>();
        //    }
        //    return returnInvoice;
        //}
        //[HttpPost]
        //[Route("getreturninvoicesbyperiod/{year}/{month}")]
        //public async Task<ApiResponse<List<ReturnInvoiceModel>>> GetReturnInvoicesByPeriodAsync([Required] int year, [Required] int month)
        //{
        //    var credential = Common.DecodeJwt(HttpContext.User);
        //    var returnInvoice = new ApiResponse<List<ReturnInvoiceModel>>();
        //    try
        //    {
        //        var execute = await _unitOfWork.DailyPayment.GetReturnInvoiceByPeriodAsync(credential.DbCode!,month.ToString(),year.ToString());
        //        if (execute.Count > 0)
        //        {
        //            returnInvoice.Message = "Return Invoices fetched successfully";
        //            returnInvoice.Success = true;
        //            returnInvoice.StatusCode = (int)HttpStatusCode.OK;
        //            returnInvoice.Result = execute;
        //        }
        //        else
        //        {
        //            returnInvoice.Message = "Return Invoices fetched unsuccessfully";
        //            returnInvoice.StatusCode = (int)HttpStatusCode.BadRequest;
        //            returnInvoice.Result = new List<ReturnInvoiceModel>();
        //        }
        //    }
        //    catch (SqlException ex)
        //    {
        //        returnInvoice.Message = ex.Message;
        //        returnInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
        //        returnInvoice.Result = new List<ReturnInvoiceModel>();
        //    }
        //    catch (Exception ex)
        //    {
        //        returnInvoice.Message = ex.Message;
        //        returnInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
        //        returnInvoice.Result = new List<ReturnInvoiceModel>();
        //    }
        //    return returnInvoice;
        //}
        
        [HttpDelete]
        [Route("{dividedId}")]
        public async Task<ApiResponse<int>> UpdateReturnInvoiceAsync([Required] int dividedId)
        {
            var credential = Common.DecodeJwt(HttpContext.User);
            var returnInvoice = new ApiResponse<int>();
            try
            {
                var affectedRow = await _unitOfWork.DailyPayment.UpdateReturnInvoiceAsync(dividedId.ToString());
                if (affectedRow > 0)
                {
                    returnInvoice.Message = "Return Invoices fetched successfully";
                    returnInvoice.Success = true;
                    returnInvoice.StatusCode = (int)HttpStatusCode.OK;
                    returnInvoice.Result = affectedRow;
                }
                else
                {
                    returnInvoice.Message = "Return Invoices fetched unsuccessfully";
                    returnInvoice.StatusCode = (int)HttpStatusCode.BadRequest;
                }
            }
            catch (SqlException ex)
            {
                returnInvoice.Message = ex.Message;
                returnInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            catch (Exception ex)
            {
                returnInvoice.Message = ex.Message;
                returnInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            return returnInvoice;
        }
    }
}
