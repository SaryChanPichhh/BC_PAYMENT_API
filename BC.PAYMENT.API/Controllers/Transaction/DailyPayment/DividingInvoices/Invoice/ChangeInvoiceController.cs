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
            var changeInvoice = new ApiResponse<List<NewInvoiceModel>>();
            try
            {
                var execute = await _unitOfWork.NewInvoice.GetInvoices(InvoiceTypes.NewInvoice, credential.DbCode!, DateTime.Today);
                if (execute.Count > 0)
                {
                    changeInvoice.Message = "New Invoices fetched successfully";
                    changeInvoice.Success = true;
                    changeInvoice.StatusCode = (int)HttpStatusCode.OK;
                    changeInvoice.Result = execute;
                }
                else
                {
                    changeInvoice.Message = "New Invoices fetched unsuccessfully";
                    changeInvoice.StatusCode = (int)HttpStatusCode.BadRequest;
                    changeInvoice.Result = new List<NewInvoiceModel>();
                }
            }
            catch (SqlException ex)
            {
                changeInvoice.Message = ex.Message;
                changeInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                changeInvoice.Result = new List<NewInvoiceModel>();
            }
            catch (Exception ex)
            {
                changeInvoice.Message = ex.Message;
                changeInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                changeInvoice.Result = new List<NewInvoiceModel>();
            }
            return changeInvoice;
        }

        [HttpGet]
        [Route("getlocalinvoice/{fromDate}/{toDate}")]
        public async Task<ApiResponse<List<ChangeInvoiceModel>>> GetLocalInvoiceAsync([Required] DateTime fromDate, DateTime toDate)
        {
            var credential = Common.DecodeJwt(HttpContext.User);
            var changeInvoice = new ApiResponse<List<ChangeInvoiceModel>>();
            try
            {
                var execute = await _unitOfWork.ChangeInvoice.GetLocalInvoiceAsync(credential.DbCode!, fromDate, toDate);
                if (execute.Count > 0)
                {
                    changeInvoice.Message = "Invoices fetched successfully";
                    changeInvoice.Success = true;
                    changeInvoice.StatusCode = (int)HttpStatusCode.OK;
                    changeInvoice.Result = execute;
                }
                else
                {
                    changeInvoice.Message = "Invoices fetched unsuccessfully";
                    changeInvoice.StatusCode = (int)HttpStatusCode.BadRequest;
                    changeInvoice.Result = new List<ChangeInvoiceModel>();
                }
            }
            catch (SqlException ex)
            {
                changeInvoice.Message = ex.Message;
                changeInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                changeInvoice.Result = new List<ChangeInvoiceModel>();
            }
            catch (Exception ex)
            {
                changeInvoice.Message = ex.Message;
                changeInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                changeInvoice.Result = new List<ChangeInvoiceModel>();
            }
            return changeInvoice;
        }

        [HttpGet]
        [Route("getotherbranchesinvoice/{fromDate}/{toDate}")]
        public async Task<ApiResponse<List<ChangeInvoiceModel>>> GetOtherBranchesInvoiceAsync([Required] DateTime fromDate, DateTime toDate)
        {
            var credential = Common.DecodeJwt(HttpContext.User);
            var changeInvoice = new ApiResponse<List<ChangeInvoiceModel>>();
            try
            {
                var execute = await _unitOfWork.ChangeInvoice.GetOtherBranchInvoiceAsync(credential.DbCode!, fromDate, toDate);
                if (execute.Count > 0)
                {
                    changeInvoice.Message = "Invoices fetched successfully";
                    changeInvoice.Success = true;
                    changeInvoice.StatusCode = (int)HttpStatusCode.OK;
                    changeInvoice.Result = execute;
                }
                else
                {
                    changeInvoice.Message = "Invoices fetched unsuccessfully";
                    changeInvoice.StatusCode = (int)HttpStatusCode.BadRequest;
                    changeInvoice.Result = new List<ChangeInvoiceModel>();
                }
            }
            catch (SqlException ex)
            {
                changeInvoice.Message = ex.Message;
                changeInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                changeInvoice.Result = new List<ChangeInvoiceModel>();
            }
            catch (Exception ex)
            {
                changeInvoice.Message = ex.Message;
                changeInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                changeInvoice.Result = new List<ChangeInvoiceModel>();
            }
            return changeInvoice;
        }

        [HttpPost]
        [Route("addifexistsinvoice")]
        public async Task<ApiResponse<ChangeInvoiceDto>> AddIfExistsInvoiceAsync(ChangeInvoiceDto model)
        {
            var credential = Common.DecodeJwt(HttpContext.User);
            var changeInvoice = new ApiResponse<ChangeInvoiceDto>();
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
                    changeInvoice.Message = "Invoices added successfully";
                    changeInvoice.Success = true;
                    changeInvoice.StatusCode = (int)HttpStatusCode.OK;
                    changeInvoice.Result = model;
                }
                else
                {
                    changeInvoice.Message = "Invoices added unsuccessfully";
                    changeInvoice.StatusCode = (int)HttpStatusCode.BadRequest;
                    changeInvoice.Result = new ChangeInvoiceDto();
                }
            }
            catch (SqlException ex)
            {
                changeInvoice.Message = ex.Message;
                changeInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                changeInvoice.Result = new ChangeInvoiceDto();
            }
            catch (Exception ex)
            {
                changeInvoice.Message = ex.Message;
                changeInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                changeInvoice.Result = new ChangeInvoiceDto();
            }
            return changeInvoice;
        }
        [HttpPost]
        [Route("addifnotexistsinvoice")]
        public async Task<ApiResponse<ChangeInvoiceDto>> AddIfNotExistsInvoiceAsync(ChangeInvoiceDto model)
        {
            var credential = Common.DecodeJwt(HttpContext.User);
            var changeInvoice = new ApiResponse<ChangeInvoiceDto>();
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
                    changeInvoice.Message = "Invoices added successfully";
                    changeInvoice.Success = true;
                    changeInvoice.StatusCode = (int)HttpStatusCode.OK;
                    changeInvoice.Result = model;
                }
                else
                {
                    changeInvoice.Message = "Invoices added unsuccessfully";
                    changeInvoice.StatusCode = (int)HttpStatusCode.BadRequest;
                    changeInvoice.Result = new ChangeInvoiceDto();
                }
            }
            catch (SqlException ex)
            {
                changeInvoice.Message = ex.Message;
                changeInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                changeInvoice.Result = new ChangeInvoiceDto();
            }
            catch (Exception ex)
            {
                changeInvoice.Message = ex.Message;
                changeInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                changeInvoice.Result = new ChangeInvoiceDto();
            }
            return changeInvoice;
        }
    }
}
