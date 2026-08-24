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
    public class ReturnChangeInvoiceController(IUnitOfWork unitOfWork) : BaseApiController 
    {
        [HttpPost]
        [Route("getreturnchangeinvoicebydate")]
        public async Task<ApiResponse<PaginatedResponse<ReturnChangeInvoiceModel>>> GetReturnChangeInvoiceByDateAsync([FromBody] ByDateDto model)
        {
            var credential = Common.DecodeJwt(HttpContext.User);
            try
            {
                var execute = await unitOfWork.DailyPayment.GetReturnChangeInvoiceByDateAsync(credential.DbCode!, Convert.ToDateTime(model.FromDate), Convert.ToDateTime(model.ToDate));
                var newReturnChangeInvoice = execute.Skip(model.Page - 1 * model.PageSize).Take(model.PageSize).ToList();
                if (newReturnChangeInvoice.Count > 0)
                {
                    return ApiResponse<PaginatedResponse<ReturnChangeInvoiceModel>>.Builder()
                        .WithMessage("Return Change Invoices fetched successfully")
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithResult(new PaginatedResponse<ReturnChangeInvoiceModel>(newReturnChangeInvoice, execute.Count,model.Page,model.PageSize))
                        .Build();
                }
                else
                {
                    return ApiResponse<PaginatedResponse<ReturnChangeInvoiceModel>>.Builder()
                        .WithMessage("Return Change Invoices fetched unsuccessfully")
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithResult(new PaginatedResponse<ReturnChangeInvoiceModel>(null,0,0,0))
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<PaginatedResponse<ReturnChangeInvoiceModel>>(ex.Message);
            }
        }
        [HttpPost]
        [Route("getreturnchangeinvoicebyperiod")]
        public async Task<ApiResponse<PaginatedResponse<ReturnChangeInvoiceModel>>> GetReturnChangeInvoiceByPeriodAsync([FromBody] ByPeriodDto model)
        {
            var credential = Common.DecodeJwt(HttpContext.User);
            try
            {
                var execute = await unitOfWork.DailyPayment.GetReturnChangeInvoiceByPeriodAsync(credential.DbCode!, model.Month, model.Year);
                var newReturnChangeInvoice = execute.Skip(model.Page - 1 * model.PageSize).Take(model.PageSize).ToList();
                if (newReturnChangeInvoice.Count > 0)
                {
                    return ApiResponse<PaginatedResponse<ReturnChangeInvoiceModel>>.Builder()
                        .WithMessage("Return Change Invoices fetched successfully")
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithResult(new PaginatedResponse<ReturnChangeInvoiceModel>(newReturnChangeInvoice, execute.Count,model.Page,model.PageSize))
                        .Build();
                }
                else
                {
                    return ApiResponse<PaginatedResponse<ReturnChangeInvoiceModel>>.Builder()
                        .WithMessage("Return Change Invoices fetched unsuccessfully")
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithResult(new PaginatedResponse<ReturnChangeInvoiceModel>(null,0,0,0))
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<PaginatedResponse<ReturnChangeInvoiceModel>>(ex.Message);
            }
        }
        [HttpPost]
        [Route("addnewreturnchangeinvoice")]
        public async Task<ApiResponse<ReturnChangeInvoiceDto>> AddNewReturnChangeInvoiceByPeriodAsync([FromBody] ReturnChangeInvoiceDto model)
        {
            var credential = Common.DecodeJwt(HttpContext.User);
            try
            {
                var returnChangeInvoiceModel = new ReturnChangeInvoiceModel
                {
                    ReturnId = model.ReturnId,
                    CreateBy = credential.Username,
                };
                var affectedRow = await unitOfWork.DailyPayment.InsertGetReturnInvoice(returnChangeInvoiceModel);
                if (affectedRow > 0)
                {
                    return ApiResponse<ReturnChangeInvoiceDto>.Builder()
                        .WithMessage("Return Change Invoices fetched successfully")
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .Build();
                }
                else
                {
                    return ApiResponse<ReturnChangeInvoiceDto>.Builder()
                        .WithMessage("Return Change Invoices fetched unsuccessfully")
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<ReturnChangeInvoiceDto>(ex.Message);
            }
        }
    }
}
