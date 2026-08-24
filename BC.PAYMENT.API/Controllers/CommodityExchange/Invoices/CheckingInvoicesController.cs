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
    public class CheckingInvoicesController(IUnitOfWork unitOfWork) : BaseApiController
    {
        [HttpPost]
        [Route("getoldinvoice")]
        public async Task<ApiResponse<List<CheckingInvoiceDto>>> GetOldExchangeInvoiceByTransactionAsync([Required] int page, [Required] int pageSize,[Required] InvoiceType request)
        {
            try
            {
                var credential = Common.DecodeJwt(User);
                var execute = await unitOfWork.CheckingInvoice.LoadOldInvoicesAsync(credential.DbCode, request,page,pageSize);
                
                return ApiResponse<List<CheckingInvoiceDto>>.Builder()
                    .WithMessage(execute.Any() ? "Old invoices fetched successfully" : "Old invoices fetched unsuccessfully")
                    .WithStatusCode(execute.Any() ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest)
                    .WithResult(execute.Any() ? execute : new List<CheckingInvoiceDto>())
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<CheckingInvoiceDto>>(ex.Message);
            }
        }
        [HttpPost]
        [Route("getnewinvoice")]
        public async Task<ApiResponse<List<CheckingInvoiceDto>>> GetNewExchangeInvoiceByTransactionAsync([Required] int page, [Required] int pageSize, InvoiceType request)
        {
            try
            {
                var credential = Common.DecodeJwt(User);
                var execute = await unitOfWork.CheckingInvoice.LoadNewInvoicesAsync(credential.DbCode, page,pageSize, request);
                
                return ApiResponse<List<CheckingInvoiceDto>>.Builder()
                    .WithMessage(execute.Any() ? "New invoices fetched successfully" : "New invoices fetched unsuccessfully")
                    .WithStatusCode(execute.Any() ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest)
                    .WithResult(execute.Any() ? execute : new List<CheckingInvoiceDto>())
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<CheckingInvoiceDto>>(ex.Message);
            }
        }
        
        [HttpPost]
        [Route("getexchangenewinvoicebytransactioncode")]
        public async Task<ApiResponse<ExchangeInvoiceDetailRespondDto>> GetExchangeNewInvoiceDetailByTransactionCode([Required] string transactionCode)
        {
            try
            {
                var credential = Common.DecodeJwt(User);
                var execute = await unitOfWork.CheckingInvoice.GetExchangeNewInvoiceDetailByTransactionCode(credential.DbCode, transactionCode);
                
                return ApiResponse<ExchangeInvoiceDetailRespondDto>.Builder()
                    .WithMessage(execute != null ? "Invoices detail fetched successfully" : "Invoices detail fetched unsuccessfully")
                    .WithStatusCode(execute != null ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest)
                    .WithResult(execute)
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<ExchangeInvoiceDetailRespondDto>(ex.Message);
            }
        }
        
        [HttpPost]
        [Route("getexchangeoldinvoicebytransactioncode")]
        public async Task<ApiResponse<ExchangeInvoiceDetailRespondDto>> GetExchangeOldInvoiceDetailByTransactionCode([Required] string transactionCode)
        {
            try
            {
                var credential = Common.DecodeJwt(User);
                var execute = await unitOfWork.CheckingInvoice.GetExchangeOldInvoiceDetailByTransactionCode(credential.DbCode, transactionCode);
                
                return ApiResponse<ExchangeInvoiceDetailRespondDto>.Builder()
                    .WithMessage(execute != null ? "Invoices detail fetched successfully" : "Invoices detail fetched unsuccessfully")
                    .WithStatusCode(execute != null ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest)
                    .WithResult(execute)
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<ExchangeInvoiceDetailRespondDto>(ex.Message);
            }
        }
        
        [HttpPost]
        [Route("getrepairenewinvoicebytransactioncode")]
        public async Task<ApiResponse<RepairInvoiceDetailRespondDto>> GetRepairNewInvoiceDetailByTransactionCode([Required] string transactionCode)
        {
            try
            {
                var credential = Common.DecodeJwt(User);
                var execute = await unitOfWork.CheckingInvoice.GetRepairNewInvoiceDetailByTransactionCode(credential.DbCode, transactionCode);
                
                return ApiResponse<RepairInvoiceDetailRespondDto>.Builder()
                    .WithMessage(execute != null ? "Invoices detail fetched successfully" : "Invoices detail fetched unsuccessfully")
                    .WithStatusCode(execute != null ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest)
                    .WithResult(execute)
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<RepairInvoiceDetailRespondDto>(ex.Message);
            }
        }

        [HttpPost]
        [Route("getrepairoldinvoicebytransactioncode")]
        public async Task<ApiResponse<RepairInvoiceDetailRespondDto>> GetRepairOldInvoiceDetailByTransactionCode([Required] string transactionCode)
        {
            try
            {
                var credential = Common.DecodeJwt(User);
                var execute = await unitOfWork.CheckingInvoice.GetRepairOldInvoiceDetailByTransactionCode(credential.DbCode, transactionCode);
                
                return ApiResponse<RepairInvoiceDetailRespondDto>.Builder()
                    .WithMessage(execute != null ? "Invoices detail fetched successfully" : "Invoices detail fetched unsuccessfully")
                    .WithStatusCode(execute != null ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest)
                    .WithResult(execute)
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<RepairInvoiceDetailRespondDto>(ex.Message);
            }
        }




    }
}
