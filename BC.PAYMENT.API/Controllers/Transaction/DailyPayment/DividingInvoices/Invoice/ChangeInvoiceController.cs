using System.ComponentModel.DataAnnotations;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Net;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DividingInvoices.Invoice;
using BC.PAYMENT.CORE.DTO.Transaction.DailyPayment.DividingInvoices.Invoice;

namespace BC.PAYMENT.API.Controllers.Transaction.DailyPayment.DividingInvoices.Invoice
{
    public class ChangeInvoiceController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChangeInvoiceController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("getinvoices")]
        public async Task<ApiResponse<List<NewInvoiceModel>>> GetInvoicesAsync()
        {
            var credential = Common.DecodeJwt(HttpContext.User);
            try
            {
                var execute = await _unitOfWork.NewInvoice.GetInvoices(InvoiceTypes.NewInvoice, credential.DbCode!, DateTime.Today);
                if (execute.Count > 0)
                {
                    return ApiResponse<List<NewInvoiceModel>>.Builder()
                        .WithMessage("New Invoices fetched successfully")
                        .WithSuccess(true)
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithResult(execute)
                        .Build();
                }
                else
                {
                    return ApiResponse<List<NewInvoiceModel>>.Builder()
                        .WithMessage("New Invoices fetched unsuccessfully")
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithResult(new List<NewInvoiceModel>())
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<NewInvoiceModel>>(ex.Message);
            }
        }

        [HttpGet]
        [Route("getlocalinvoice/{fromDate}/{toDate}")]
        public async Task<ApiResponse<List<ChangeInvoiceModel>>> GetLocalInvoiceAsync([Required] DateTime fromDate, DateTime toDate)
        {
            var credential = Common.DecodeJwt(HttpContext.User);
            try
            {
                var execute = await _unitOfWork.ChangeInvoice.GetLocalInvoiceAsync(credential.DbCode!, fromDate, toDate);
                if (execute.Count > 0)
                {
                    return ApiResponse<List<ChangeInvoiceModel>>.Builder()
                        .WithMessage("Invoices fetched successfully")
                        .WithSuccess(true)
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithResult(execute)
                        .Build();
                }
                else
                {
                    return ApiResponse<List<ChangeInvoiceModel>>.Builder()
                        .WithMessage("Invoices fetched unsuccessfully")
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithResult(new List<ChangeInvoiceModel>())
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<ChangeInvoiceModel>>(ex.Message);
            }
        }

        [HttpGet]
        [Route("getotherbranchesinvoice/{fromDate}/{toDate}")]
        public async Task<ApiResponse<List<ChangeInvoiceModel>>> GetOtherBranchesInvoiceAsync([Required] DateTime fromDate, DateTime toDate)
        {
            var credential = Common.DecodeJwt(HttpContext.User);
            try
            {
                var execute = await _unitOfWork.ChangeInvoice.GetOtherBranchInvoiceAsync(credential.DbCode!, fromDate, toDate);
                if (execute.Count > 0)
                {
                    return ApiResponse<List<ChangeInvoiceModel>>.Builder()
                        .WithMessage("Invoices fetched successfully")
                        .WithSuccess(true)
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithResult(execute)
                        .Build();
                }
                else
                {
                    return ApiResponse<List<ChangeInvoiceModel>>.Builder()
                        .WithMessage("Invoices fetched unsuccessfully")
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithResult(new List<ChangeInvoiceModel>())
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<ChangeInvoiceModel>>(ex.Message);
            }
        }

        [HttpPost]
        [Route("addifexistsinvoice")]
        public async Task<ApiResponse<ChangeInvoiceDto>> AddIfExistsInvoiceAsync(ChangeInvoiceDto model)
        {
            var credential = Common.DecodeJwt(HttpContext.User);
            try
            {
                var changeInvoiceModel = new ChangeInvoiceModel
                {
                    DbCode = credential.DbCode,
                    UserName = credential.Username,
                    Transaction = model.TransactionCode,
                    CustomerCode = model.CustomerCode,
                    CustomerName = model.CustomerName,
                    InvoiceValue = model.InvoiceValue,
                    EntriesCode = model.EntriesCode,

                };
                var affectedRow = await _unitOfWork.ChangeInvoice.InsertIfExistsInvoiceAsync(changeInvoiceModel);
                if (affectedRow > 0)
                {
                    return ApiResponse<ChangeInvoiceDto>.Builder()
                        .WithMessage("Invoices added successfully")
                        .WithSuccess(true)
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithResult(model)
                        .Build();
                }
                else
                {
                    return ApiResponse<ChangeInvoiceDto>.Builder()
                        .WithMessage("Invoices added unsuccessfully")
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithResult(new ChangeInvoiceDto())
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<ChangeInvoiceDto>(ex.Message);
            }
        }
        [HttpPost]
        [Route("addifnotexistsinvoice")]
        public async Task<ApiResponse<ChangeInvoiceDto>> AddIfNotExistsInvoiceAsync(ChangeInvoiceDto model)
        {
            var credential = Common.DecodeJwt(HttpContext.User);
            try
            {
                var changeInvoiceModel = new ChangeInvoiceModel
                {
                    DbCode = credential.DbCode,
                    UserName = credential.Username,
                    Transaction = model.TransactionCode,
                    CustomerCode = model.CustomerCode,
                    CustomerName = model.CustomerName,
                    InvoiceValue = model.InvoiceValue,
                    EntriesCode = model.EntriesCode,
                };
                var affectedRow = await _unitOfWork.ChangeInvoice.InsertIfNotExistsInvoiceAsync(changeInvoiceModel);
                if (affectedRow > 0)
                {
                    return ApiResponse<ChangeInvoiceDto>.Builder()
                        .WithMessage("Invoices added successfully")
                        .WithSuccess(true)
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithResult(model)
                        .Build();
                }
                else
                {
                    return ApiResponse<ChangeInvoiceDto>.Builder()
                        .WithMessage("Invoices added unsuccessfully")
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithResult(new ChangeInvoiceDto())
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<ChangeInvoiceDto>(ex.Message);
            }
        }
    }
}
