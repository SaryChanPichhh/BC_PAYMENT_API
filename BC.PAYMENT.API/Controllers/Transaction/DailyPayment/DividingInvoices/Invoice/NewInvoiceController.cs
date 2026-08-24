using System.Net;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.Transaction.DailyPayment.DividingInvoices.Invoice;
using BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DividingInvoices.Invoice;
using BC.PAYMENT.CORE.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BC.PAYMENT.API.Controllers.Transaction.DailyPayment.DividingInvoices.Invoice
{
    public class NewInvoiceController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public NewInvoiceController(IUnitOfWork unitOfWork)
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
        [Route("getsaletype")]
        public async Task<ApiResponse<List<string>>> GetSaleTypeAsync()
        {
            var credential = Common.DecodeJwt(HttpContext.User);
            try
            {
                var execute = await _unitOfWork.NewInvoice.GetSaleTypes(credential.DbCode!);
                if (execute.Count > 0)
                {
                    return ApiResponse<List<string>>.Builder()
                        .WithMessage("Sale type fetched successfully")
                        .WithSuccess(true)
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithResult(execute)
                        .Build();
                }
                else
                {
                    return ApiResponse<List<string>>.Builder()
                        .WithMessage("Sale type fetched unsuccessfully")
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<string>>(ex.Message);
            }
        }

        [HttpPost]
        [Route("downloadinvoices")]
        public async Task<ApiResponse<List<NewInvoiceModel>>> GetInvoicesByInvoiceCodeAsync(NewInvoiceFilterDto model)
        {
            var credential = Common.DecodeJwt(HttpContext.User);
            try
            {
                var execute = await _unitOfWork.NewInvoice.GetInvoicesByInvoiceCode(model.StartInvoiceType!, model.EndInvoiceType!, model.FromDate!, model.ToDate!, credential.DbCode!);
                execute.ForEach(invoice =>
                {
                    invoice.DbCode = credential.DbCode;
                    invoice.CreatedBy = credential.Username;
                    invoice.InvoiceTypes = InvoiceTypes.NewInvoice;
                    invoice.EntriesCode = model.EntriesCode;
                });
                var affectedRow = await _unitOfWork.NewInvoice.SaveInvoices(execute);
                if (affectedRow > 0)
                {
                    return ApiResponse<List<NewInvoiceModel>>.Builder()
                        .WithMessage("New Invoices added successfully")
                        .WithSuccess(true)
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithResult(execute)
                        .Build();
                }
                else
                {
                    return ApiResponse<List<NewInvoiceModel>>.Builder()
                        .WithMessage("New Invoices added unsuccessfully")
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
    }
}
