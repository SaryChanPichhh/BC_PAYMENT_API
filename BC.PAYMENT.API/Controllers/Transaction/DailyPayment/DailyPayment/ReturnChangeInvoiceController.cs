using System.ComponentModel.DataAnnotations;
using System.Net;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.Filter;
using BC.PAYMENT.CORE.DTO.Transaction.DailyPayment.DailyPayment;
using BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DailyPayment;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BC.PAYMENT.API.Controllers.Transaction.DailyPayment.DailyPayment
{
    public class ReturnChangeInvoiceController : BaseApiController 
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReturnChangeInvoiceController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpPost]
        [Route("getreturnchangeinvoicebydate")]
        public async Task<ApiResponse<PaginatedResponse<ReturnChangeInvoiceModel>>> GetReturnChangeInvoiceByDateAsync([FromBody] ByDateDto model)
        {
            var credential = Common.DecodeJwt(HttpContext.User);
            var returnChangeInvoice = new ApiResponse<PaginatedResponse<ReturnChangeInvoiceModel>>();
            try
            {
                var execute = await _unitOfWork.DailyPayment.GetReturnChangeInvoiceByDateAsync(credential.DbCode!, Convert.ToDateTime(model.FromDate), Convert.ToDateTime(model.ToDate));
                var newReturnChangeInvoice = execute.Skip(model.Page - 1 * model.PageSize).Take(model.PageSize).ToList();
                if (newReturnChangeInvoice.Count > 0)
                {
                    returnChangeInvoice.Message = "Return Change Invoices fetched successfully";
                    returnChangeInvoice.Success = true;
                    returnChangeInvoice.StatusCode = (int)HttpStatusCode.OK;
                    returnChangeInvoice.Result = new PaginatedResponse<ReturnChangeInvoiceModel>(newReturnChangeInvoice, execute.Count,model.Page,model.PageSize);
                }
                else
                {
                    returnChangeInvoice.Message = "Return Change Invoices fetched unsuccessfully";
                    returnChangeInvoice.StatusCode = (int)HttpStatusCode.BadRequest;
                    returnChangeInvoice.Result = new PaginatedResponse<ReturnChangeInvoiceModel>(null,0,0,0);
                }
            }
            catch (SqlException ex)
            {
                returnChangeInvoice.Message = ex.Message;
                returnChangeInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                returnChangeInvoice.Result = new PaginatedResponse<ReturnChangeInvoiceModel>(null, 0, 0, 0);
            }
            catch (Exception ex)
            {
                returnChangeInvoice.Message = ex.Message;
                returnChangeInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                returnChangeInvoice.Result = new PaginatedResponse<ReturnChangeInvoiceModel>(null, 0, 0, 0);
            }
            return returnChangeInvoice;
        }
        [HttpPost]
        [Route("getreturnchangeinvoicebyperiod")]
        public async Task<ApiResponse<PaginatedResponse<ReturnChangeInvoiceModel>>> GetReturnChangeInvoiceByPeriodAsync([FromBody] ByPeriodDto model)
        {
            var credential = Common.DecodeJwt(HttpContext.User);
            var returnChangeInvoice = new ApiResponse<PaginatedResponse<ReturnChangeInvoiceModel>>();
            try
            {
                var execute = await _unitOfWork.DailyPayment.GetReturnChangeInvoiceByPeriodAsync(credential.DbCode!, model.Month, model.Year);
                var newReturnChangeInvoice = execute.Skip(model.Page - 1 * model.PageSize).Take(model.PageSize).ToList();
                if (newReturnChangeInvoice.Count > 0)
                {
                    returnChangeInvoice.Message = "Return Change Invoices fetched successfully";
                    returnChangeInvoice.Success = true;
                    returnChangeInvoice.StatusCode = (int)HttpStatusCode.OK;
                    returnChangeInvoice.Result = new PaginatedResponse<ReturnChangeInvoiceModel>(newReturnChangeInvoice, execute.Count,model.Page,model.PageSize);
                }
                else
                {
                    returnChangeInvoice.Message = "Return Change Invoices fetched unsuccessfully";
                    returnChangeInvoice.StatusCode = (int)HttpStatusCode.BadRequest;
                    returnChangeInvoice.Result = new PaginatedResponse<ReturnChangeInvoiceModel>(null,0,0,0);
                }
            }
            catch (SqlException ex)
            {
                returnChangeInvoice.Message = ex.Message;
                returnChangeInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                returnChangeInvoice.Result = new PaginatedResponse<ReturnChangeInvoiceModel>(null, 0, 0, 0);
            }
            catch (Exception ex)
            {
                returnChangeInvoice.Message = ex.Message;
                returnChangeInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                returnChangeInvoice.Result = new PaginatedResponse<ReturnChangeInvoiceModel>(null, 0, 0, 0);
            }
            return returnChangeInvoice;
        }
        [HttpPost]
        [Route("addnewreturnchangeinvoice")]
        public async Task<ApiResponse<ReturnChangeInvoiceDto>> AddNewReturnChangeInvoiceByPeriodAsync([FromBody] ReturnChangeInvoiceDto model)
        {
            var credential = Common.DecodeJwt(HttpContext.User);
            var returnChangeInvoice = new ApiResponse<ReturnChangeInvoiceDto>();
            try
            {
                var returnChangeInvoiceModel = new ReturnChangeInvoiceModel
                {
                    ReturnId = model.ReturnId,
                    CreateBy = credential.Username,
                };
                var affectedRow = await _unitOfWork.DailyPayment.InsertGetReturnInvoice(returnChangeInvoiceModel);
                if (affectedRow > 0)
                {
                    returnChangeInvoice.Message = "Return Change Invoices fetched successfully";
                    returnChangeInvoice.Success = true;
                    returnChangeInvoice.StatusCode = (int)HttpStatusCode.OK;
                }
                else
                {
                    returnChangeInvoice.Message = "Return Change Invoices fetched unsuccessfully";
                    returnChangeInvoice.StatusCode = (int)HttpStatusCode.BadRequest;
                }
            }
            catch (SqlException ex)
            {
                returnChangeInvoice.Message = ex.Message;
                returnChangeInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            catch (Exception ex)
            {
                returnChangeInvoice.Message = ex.Message;
                returnChangeInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            return returnChangeInvoice;
        }
    }
}
