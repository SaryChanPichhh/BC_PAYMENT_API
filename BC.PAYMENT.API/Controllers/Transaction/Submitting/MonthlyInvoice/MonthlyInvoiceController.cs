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
            var invoiceVerify = new ApiResponse<PaginatedResponse<MonthlyInvoiceModel>>();

            try
            {
                var execute = await _unitOfWork.InvoiceVerify.GetInvoiceVerifyByPeriodAsync(credential.DbCode!, model.Month, model.Year);
                var newResponds = execute.Skip(model.Page - 1 * model.PageSize).Take(model.PageSize).ToList();
                if(execute.Any())
                {
                    invoiceVerify.Result = new PaginatedResponse<MonthlyInvoiceModel>(newResponds, execute.Count,model.Page,model.PageSize);
                    invoiceVerify.StatusCode = StatusCodes.Status200OK;
                    invoiceVerify.Message = "Invoice Verify fetched successfully";
                    invoiceVerify.Success = true;
                }
                else
                {
                    invoiceVerify.StatusCode = StatusCodes.Status400BadRequest;
                    invoiceVerify.Message = "Invoice Verify fetched unsuccessfully";
                    invoiceVerify.Success = false;
                }
            }
            catch (SqlException ex)
            {
                invoiceVerify.StatusCode = StatusCodes.Status500InternalServerError;
                invoiceVerify.Message = $"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql Exception", ex);
            }
            catch(Exception ex)
            {
                invoiceVerify.StatusCode = StatusCodes.Status500InternalServerError;
                invoiceVerify.Message = $"Error Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }

            return invoiceVerify;
        }
        
        [HttpPost]
        [Route("getinvoiceverifybydate")]
        public async Task<ApiResponse<PaginatedResponse<MonthlyInvoiceModel>>> GetInvoiceVerifyByDate([FromBody] ByDateDto model )
        {
            var credential = Common.DecodeJwt(User);
            var invoiceVerify = new ApiResponse<PaginatedResponse<MonthlyInvoiceModel>>();
            try
            {
                var execute = await _unitOfWork.InvoiceVerify.GetInvoiceVerifyByDateAsync(credential.DbCode!, model.FromDate, model.ToDate);
                var newResponds = execute.Skip(model.Page - 1 * model.PageSize).Take(model.PageSize).ToList();
                if(execute.Any())
                {
                    invoiceVerify.Result = new PaginatedResponse<MonthlyInvoiceModel>(newResponds, execute.Count,model.Page,model.PageSize);
                    invoiceVerify.StatusCode = StatusCodes.Status200OK;
                    invoiceVerify.Message = "Invoice Verify fetched successfully";
                    invoiceVerify.Success = true;
                }
                else
                {
                    invoiceVerify.StatusCode = StatusCodes.Status400BadRequest;
                    invoiceVerify.Message = "Invoice Verify fetched unsuccessfully";
                    invoiceVerify.Success = false;
                }
            }
            catch (SqlException ex)
            {
                invoiceVerify.StatusCode = StatusCodes.Status500InternalServerError;
                invoiceVerify.Message = $"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql Exception", ex);
            }
            catch(Exception ex)
            {
                invoiceVerify.StatusCode = StatusCodes.Status500InternalServerError;
                invoiceVerify.Message = $"Error Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }
            return invoiceVerify;
        }
    }
}
