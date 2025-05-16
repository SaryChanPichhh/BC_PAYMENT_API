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
            var newInvoice = new ApiResponse<List<NewInvoiceModel>>();
            try
            {
                var execute = await _unitOfWork.NewInvoice.GetInvoices(InvoiceTypes.NewInvoice, credential.DbCode!, DateTime.Today);
                if (execute.Count > 0)
                {
                    newInvoice.Message = "New Invoices fetched successfully";
                    newInvoice.Success = true;
                    newInvoice.StatusCode = (int)HttpStatusCode.OK;
                    newInvoice.Result = execute;
                }
                else
                {
                    newInvoice.Message = "New Invoices fetched unsuccessfully";
                    newInvoice.StatusCode = (int)HttpStatusCode.BadRequest;
                    newInvoice.Result = new List<NewInvoiceModel>();
                }
            }
            catch (SqlException ex)
            {
                newInvoice.Message = ex.Message;
                newInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                newInvoice.Result = new List<NewInvoiceModel>();
            }
            catch (Exception ex)
            {
                newInvoice.Message = ex.Message;
                newInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                newInvoice.Result = new List<NewInvoiceModel>();
            }
            return newInvoice;
        }
        [HttpGet]
        [Route("getsaletype")]
        public async Task<ApiResponse<List<string>>> GetSaleTypeAsync()
        {
            var credential = Common.DecodeJwt(HttpContext.User);
            var newInvoice = new ApiResponse<List<string>>();
            try
            {
                var execute = await _unitOfWork.NewInvoice.GetSaleTypes(credential.DbCode!);
                if (execute.Count > 0)
                {
                    newInvoice.Message = "Sale type fetched successfully";
                    newInvoice.Success = true;
                    newInvoice.StatusCode = (int)HttpStatusCode.OK;
                    newInvoice.Result = execute;
                }
                else
                {
                    newInvoice.Message = "Sale type fetched unsuccessfully";
                    newInvoice.StatusCode = (int)HttpStatusCode.BadRequest;
                }
            }
            catch (SqlException ex)
            {
                newInvoice.Message = ex.Message;
                newInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            catch (Exception ex)
            {
                newInvoice.Message = ex.Message;
                newInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            return newInvoice;
        }

        [HttpPost]
        [Route("downloadinvoices")]
        public async Task<ApiResponse<List<NewInvoiceModel>>> GetInvoicesByInvoiceCodeAsync(NewInvoiceFilterDto model)
        {
            var credential = Common.DecodeJwt(HttpContext.User);
            var newInvoice = new ApiResponse<List<NewInvoiceModel>>();
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
                    newInvoice.Message = "New Invoices added successfully";
                    newInvoice.Success = true;
                    newInvoice.StatusCode = (int)HttpStatusCode.OK;
                    newInvoice.Result = execute;
                }
                else
                {
                    newInvoice.Message = "New Invoices added unsuccessfully";
                    newInvoice.StatusCode = (int)HttpStatusCode.BadRequest;
                    newInvoice.Result = new List<NewInvoiceModel>();
                }
            }
            catch (SqlException ex)
            {
                newInvoice.Message = ex.Message;
                newInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                newInvoice.Result = new List<NewInvoiceModel>();
            }
            catch (Exception ex)
            {
                newInvoice.Message = ex.Message;
                newInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                newInvoice.Result = new List<NewInvoiceModel>();
            }
            return newInvoice;
        }
    }
}
