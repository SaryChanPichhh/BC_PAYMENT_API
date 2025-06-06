using System.ComponentModel.DataAnnotations;
using Azure.Core;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.CommondityExchange.Invoices;
using BC.PAYMENT.CORE.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BC.PAYMENT.API.Controllers.CommodityExchange.Invoices
{
    public class CheckingInvoicesController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public CheckingInvoicesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpPost]
        [Route("getoldinvoice")]
        public async Task<ApiResponse<List<CheckingInvoiceDto>>> GetOldExchangeInvoiceByTransactionAsync([Required] int page, [Required] int pageSize,[Required] InvoiceType request)
        {
            var credential = Common.DecodeJwt(User);
            var respond = new ApiResponse<List<CheckingInvoiceDto>>();
            try
            {
                var execute = await _unitOfWork.CheckingInvoice.LoadOldInvoicesAsync(credential.DbCode, request,page,pageSize);
                if (execute.Any())
                {
                    respond.StatusCode = StatusCodes.Status200OK;
                    respond.Message = $@"Old invoices fetched successfully";
                    respond.Result = execute;
                    respond.Success = true;
                }
                else
                {
                    respond.StatusCode = StatusCodes.Status400BadRequest;
                    respond.Message = $@"Old invoices fetched unsuccessfully";
                    respond.Result = new List<CheckingInvoiceDto>();
                }
            }
            catch (SqlException ex)
            {
                respond.Message = $@"Sql Exception : {ex.Message}";
                respond.StatusCode = StatusCodes.Status500InternalServerError;
            }
            catch (Exception ex)
            {
                respond.Message = $@"Error Exception : {ex.Message}";
                respond.StatusCode = StatusCodes.Status500InternalServerError;
            }

            return respond;
        }
        [HttpPost]
        [Route("getnewinvoice")]
        public async Task<ApiResponse<List<CheckingInvoiceDto>>> GetNewExchangeInvoiceByTransactionAsync([Required] int page, [Required] int pageSize, InvoiceType request)
        {
            var credential = Common.DecodeJwt(User);
            var respond = new ApiResponse<List<CheckingInvoiceDto>>();
            try
            {
                var execute = await _unitOfWork.CheckingInvoice.LoadNewInvoicesAsync(credential.DbCode, page,pageSize, request);
                if (execute.Any())
                {
                    respond.StatusCode = StatusCodes.Status200OK;
                    respond.Message = $@"New invoices fetched successfully";
                    respond.Result = execute;
                    respond.Success = true;
                }
                else
                {
                    respond.StatusCode = StatusCodes.Status400BadRequest;
                    respond.Message = $@"New invoices fetched unsuccessfully";
                    respond.Result = new List<CheckingInvoiceDto>();
                }
            }
            catch (SqlException ex)
            {
                respond.Message = $@"Sql Exception : {ex.Message}";
                respond.StatusCode = StatusCodes.Status500InternalServerError;
            }
            catch (Exception ex)
            {
                respond.Message = $@"Error Exception : {ex.Message}";
                respond.StatusCode = StatusCodes.Status500InternalServerError;
            }

            return respond;
        }
        
        [HttpPost]
        [Route("getexchangenewinvoicebytransactioncode")]
        public async Task<ApiResponse<ExchangeInvoiceDetailRespondDto>> GetExchangeNewInvoiceDetailByTransactionCode([Required] string transactionCode)
        {
            var credential = Common.DecodeJwt(User);
            var respond = new ApiResponse<ExchangeInvoiceDetailRespondDto>();
            try
            {
                var execute = await _unitOfWork.CheckingInvoice.GetExchangeNewInvoiceDetailByTransactionCode(credential.DbCode, transactionCode);
                if (execute != null)
                {
                    respond.StatusCode = StatusCodes.Status200OK;
                    respond.Message = $@"Invoices detail fetched successfully";
                    respond.Result = execute;
                    respond.Success = true;
                }
                else
                {
                    respond.StatusCode = StatusCodes.Status400BadRequest;
                    respond.Message = $@"Invoices detail fetched unsuccessfully";
                }
            }
            catch (SqlException ex)
            {
                respond.Message = $@"Sql Exception : {ex.Message}";
                respond.StatusCode = StatusCodes.Status500InternalServerError;
            }
            catch (Exception ex)
            {
                respond.Message = $@"Error Exception : {ex.Message}";
                respond.StatusCode = StatusCodes.Status500InternalServerError;
            }

            return respond;
        }
        
        [HttpPost]
        [Route("getexchangeoldinvoicebytransactioncode")]
        public async Task<ApiResponse<ExchangeInvoiceDetailRespondDto>> GetExchangeOldInvoiceDetailByTransactionCode([Required] string transactionCode)
        {
            var credential = Common.DecodeJwt(User);
            var respond = new ApiResponse<ExchangeInvoiceDetailRespondDto>();
            try
            {
                var execute = await _unitOfWork.CheckingInvoice.GetExchangeOldInvoiceDetailByTransactionCode(credential.DbCode, transactionCode);
                if (execute != null)
                {
                    respond.StatusCode = StatusCodes.Status200OK;
                    respond.Message = $@"Invoices detail fetched successfully";
                    respond.Result = execute;
                    respond.Success = true;
                }
                else
                {
                    respond.StatusCode = StatusCodes.Status400BadRequest;
                    respond.Message = $@"Invoices detail fetched unsuccessfully";
                }
            }
            catch (SqlException ex)
            {
                respond.Message = $@"Sql Exception : {ex.Message}";
                respond.StatusCode = StatusCodes.Status500InternalServerError;
            }
            catch (Exception ex)
            {
                respond.Message = $@"Error Exception : {ex.Message}";
                respond.StatusCode = StatusCodes.Status500InternalServerError;
            }

            return respond;
        }
        
        [HttpPost]
        [Route("getrepairenewinvoicebytransactioncode")]
        public async Task<ApiResponse<RepairInvoiceDetailRespondDto>> GetRepairNewInvoiceDetailByTransactionCode([Required] string transactionCode)
        {
            var credential = Common.DecodeJwt(User);
            var respond = new ApiResponse<RepairInvoiceDetailRespondDto>();
            try
            {
                var execute = await _unitOfWork.CheckingInvoice.GetRepairNewInvoiceDetailByTransactionCode(credential.DbCode, transactionCode);
                if (execute != null)
                {
                    respond.StatusCode = StatusCodes.Status200OK;
                    respond.Message = $@"Invoices detail fetched successfully";
                    respond.Result = execute;
                    respond.Success = true;
                }
                else
                {
                    respond.StatusCode = StatusCodes.Status400BadRequest;
                    respond.Message = $@"Invoices detail fetched unsuccessfully";
                }
            }
            catch (SqlException ex)
            {
                respond.Message = $@"Sql Exception : {ex.Message}";
                respond.StatusCode = StatusCodes.Status500InternalServerError;
            }
            catch (Exception ex)
            {
                respond.Message = $@"Error Exception : {ex.Message}";
                respond.StatusCode = StatusCodes.Status500InternalServerError;
            }

            return respond;
        }

        [HttpPost]
        [Route("getrepairoldinvoicebytransactioncode")]
        public async Task<ApiResponse<RepairInvoiceDetailRespondDto>> GetRepairOldInvoiceDetailByTransactionCode([Required] string transactionCode)
        {
            var credential = Common.DecodeJwt(User);
            var respond = new ApiResponse<RepairInvoiceDetailRespondDto>();
            try
            {
                var execute = await _unitOfWork.CheckingInvoice.GetRepairOldInvoiceDetailByTransactionCode(credential.DbCode, transactionCode);
                if (execute != null)
                {
                    respond.StatusCode = StatusCodes.Status200OK;
                    respond.Message = $@"Invoices detail fetched successfully";
                    respond.Result = execute;
                    respond.Success = true;
                }
                else
                {
                    respond.StatusCode = StatusCodes.Status400BadRequest;
                    respond.Message = $@"Invoices detail fetched unsuccessfully";
                }
            }
            catch (SqlException ex)
            {
                respond.Message = $@"Sql Exception : {ex.Message}";
                respond.StatusCode = StatusCodes.Status500InternalServerError;
            }
            catch (Exception ex)
            {
                respond.Message = $@"Error Exception : {ex.Message}";
                respond.StatusCode = StatusCodes.Status500InternalServerError;
            }
            return respond;
        }




    }
}
