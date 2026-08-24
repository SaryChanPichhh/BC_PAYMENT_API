using System.ComponentModel.DataAnnotations;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.Filter;
using BC.PAYMENT.CORE.Entities.Transaction.Submitting.InvoiceVerify;
using BC.PAYMENT.LOGGING;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BC.PAYMENT.API.Controllers.Transaction.Submitting.InvoiceVerify
{
    public class MonthlyInvoiceController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public MonthlyInvoiceController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        [Route("getinvoiceverifybyperiod")]
        public async Task<ApiResponse<PaginatedResponse<MonthlyInvoiceModel>>> GetInvoiceVerifyByPeriod([FromBody] ByPeriodDto model)
        {
            var credential = Common.DecodeJwt(User);

            try
            {
                var execute = await _unitOfWork.InvoiceVerify.GetInvoiceVerifyByPeriodAsync(credential.DbCode!, model.Month, model.Year);
                var newResponds = execute.Skip(model.Page - 1 * model.PageSize).Take(model.PageSize).ToList();
                if(execute.Any())
                {
                    return ApiResponse<PaginatedResponse<MonthlyInvoiceModel>>.Builder()
                        .WithResult(new PaginatedResponse<MonthlyInvoiceModel>(newResponds, execute.Count,model.Page,model.PageSize))
                        .WithStatusCode(StatusCodes.Status200OK)
                        .WithMessage("Invoice Verify fetched successfully")
                        .WithSuccess(true)
                        .Build();
                }
                else
                {
                    return ApiResponse<PaginatedResponse<MonthlyInvoiceModel>>.Builder()
                        .WithStatusCode(StatusCodes.Status400BadRequest)
                        .WithMessage("Invoice Verify fetched unsuccessfully")
                        .WithSuccess(false)
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<PaginatedResponse<MonthlyInvoiceModel>>(ex.Message);
            }
        }
        
        [HttpPost]
        [Route("getinvoiceverifybydate")]
        public async Task<ApiResponse<PaginatedResponse<MonthlyInvoiceModel>>> GetInvoiceVerifyByDate([FromBody] ByDateDto model )
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var execute = await _unitOfWork.InvoiceVerify.GetInvoiceVerifyByDateAsync(credential.DbCode!, model.FromDate, model.ToDate);
                var newResponds = execute.Skip(model.Page - 1 * model.PageSize).Take(model.PageSize).ToList();
                if(execute.Any())
                {
                    return ApiResponse<PaginatedResponse<MonthlyInvoiceModel>>.Builder()
                        .WithResult(new PaginatedResponse<MonthlyInvoiceModel>(newResponds, execute.Count,model.Page,model.PageSize))
                        .WithStatusCode(StatusCodes.Status200OK)
                        .WithMessage("Invoice Verify fetched successfully")
                        .WithSuccess(true)
                        .Build();
                }
                else
                {
                    return ApiResponse<PaginatedResponse<MonthlyInvoiceModel>>.Builder()
                        .WithStatusCode(StatusCodes.Status400BadRequest)
                        .WithMessage("Invoice Verify fetched unsuccessfully")
                        .WithSuccess(false)
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<PaginatedResponse<MonthlyInvoiceModel>>(ex.Message);
            }
        }
    }
}
