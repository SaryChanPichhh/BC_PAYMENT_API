using System.Net;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.Invoice;
using BC.PAYMENT.CORE.Entities.Invoice;
using BC.PAYMENT.CORE.Entities.Transaction.Audit.VerifyInvoice;
using BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.AccountReceivable;
using BC.PAYMENT.LOGGING;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BC.PAYMENT.API.Controllers.Transaction.Audit.VerifyInvoice
{
    public class VerifyInvoicesController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public VerifyInvoicesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("getverifyinvoices")]
        public async Task<ApiResponse<List<VerifyInvoiceModel>>> GetVerifyInvoicesAsync()
        {
            var credential = Common.DecodeJwt(User);
            var verifyInvoices = new ApiResponse<List<VerifyInvoiceModel>>();
            try
            {
                var verifyInvoiceList = await _unitOfWork.VerifyInvoice.GetVerifyInvoicesAsync(credential.DbCode!);
                if (verifyInvoiceList.Any())
                {
                    verifyInvoices.StatusCode = (int)HttpStatusCode.OK;
                    verifyInvoices.Success = true;
                    verifyInvoices.Message = "Verify Invoices fetched successfully";
                    verifyInvoices.Result = verifyInvoiceList;
                }
                else
                {
                    verifyInvoices.StatusCode = (int)HttpStatusCode.BadRequest;
                    verifyInvoices.Success = false;
                    verifyInvoices.Message = "Verify Invoices fetched unsuccessfully";
                }
            }
            catch(SqlException ex)
            {
                verifyInvoices.StatusCode = (int)HttpStatusCode.InternalServerError;
                verifyInvoices.Success = false;
                verifyInvoices.Message = $@"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql SqlException",ex);
            }
            catch (Exception ex)
            {
                verifyInvoices.StatusCode = (int)HttpStatusCode.InternalServerError;
                verifyInvoices.Success = false;
                verifyInvoices.Message = $@"Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception",ex);
            }
            return verifyInvoices;
        }

        //[HttpPost]
        //[Route("getoldinvoices")]
        //public async Task<ApiResponse<List<ConfirmAccountReceivableModel.ConfirmBalanceDetails>>> GetOldInvoicsAsync(OldInvoiceRequestPostDto model)
        //{
        //    var credential = Common.DecodeJwt(HttpContext.User);
        //    var confirmAccountReceivable = new ApiResponse<List<ConfirmAccountReceivableModel.ConfirmBalanceDetails>>();
        //    try
        //    {
        //        var oldInvoiceParameter = new OldInvoiceRequestDto
        //        {
        //            DbCode = credential.DbCode,
        //            Date = model.Date,
        //            FromAccount = model.FromAccount,
        //            ToAccount = model.ToAccount,
        //            FromAnal = model.FromAnal,
        //            ToAnal = model.ToAnal,
        //            T0 = model.T0,
        //            T1 = model.T1,
        //            T2 = model.T2,
        //            T3 = model.T3,
        //            T4 = model.T4,
        //            T5 = model.T5,
        //            T6 = model.T6,
        //            T7 = model.T7,
        //            T8 = model.T8,
        //            T9 = model.T9,

        //        };
        //        var oldInvoiceModels = await _unitOfWork.Invoices.GetAllOldInvoices(oldInvoiceParameter);
        //        var oldInvoices = oldInvoiceModels.Select(oldInvoiceModel => new ConfirmAccountReceivableModel.ConfirmBalanceDetails
        //        {
        //            CustomerCode = oldInvoiceModel.CustomerCode,
        //            CustomerName = oldInvoiceModel.CustomerName,
        //            InvoicedCode = oldInvoiceModel.TransRef,
        //            InvoicedAmount = oldInvoiceModel.InvoiceValue,
        //            DbCode = credential.DbCode,
        //            CreatedBy = credential.Username,
        //        })
        //            .ToList();
        //        var affectedRow = await _unitOfWork.ConfirmAccountReceivable.AddConfirmAccountReceivableDetails(model.AccountReceivableId, oldInvoices);
        //        if (affectedRow > 0)
        //        {
        //            confirmAccountReceivable.Message = "Confirm Account Receivables Detail added successfully";
        //            confirmAccountReceivable.Success = true;
        //            confirmAccountReceivable.StatusCode = (int)HttpStatusCode.OK;
        //            confirmAccountReceivable.Result = oldInvoices;
        //        }
        //        else
        //        {
        //            confirmAccountReceivable.Message = "Confirm Account Receivables Detail added unsuccessfully";
        //            confirmAccountReceivable.StatusCode = (int)HttpStatusCode.BadRequest;
        //        }
        //    }
        //    catch (SqlException ex)
        //    {
        //        confirmAccountReceivable.Message = ex.Message;
        //        confirmAccountReceivable.StatusCode = (int)HttpStatusCode.InternalServerError;
        //    }
        //    catch (Exception ex)
        //    {
        //        confirmAccountReceivable.Message = ex.Message;
        //        confirmAccountReceivable.StatusCode = (int)HttpStatusCode.InternalServerError;
        //    }
        //    return confirmAccountReceivable;
        //}

        [HttpPost]
        [Route("addnewverifyinvoice")]
        public async Task<ApiResponse<List<VerifyInvoiceModel>>> AddNewVerifyInvoice(OldInvoiceRequestPostDto model)
        {
            var credential = Common.DecodeJwt(HttpContext.User);
            var confirmAccountReceivable = new ApiResponse<List<VerifyInvoiceModel>>();
            try
            {
                var oldInvoiceParameter = new OldInvoiceRequestDto
                {
                    DbCode = credential.DbCode,
                    Date = model.Date,
                    FromAccount = model.FromAccount,
                    ToAccount = model.ToAccount,
                    FromAnal = model.FromAnal,
                    ToAnal = model.ToAnal,
                    T0 = model.T0,
                    T1 = model.T1,
                    T2 = model.T2,
                    T3 = model.T3,
                    T4 = model.T4,
                    T5 = model.T5,
                    T6 = model.T6,
                    T7 = model.T7,
                    T8 = model.T8,
                    T9 = model.T9,

                };
                var oldInvoiceModels = await _unitOfWork.Invoices.GetAllOldInvoices(oldInvoiceParameter);
                var verifyInvoiceModels = oldInvoiceModels.Select(oldInvoiceModel => new VerifyInvoiceModel
                {
                    Transaction = oldInvoiceModel.TransactionCode,
                    CustomerCode = oldInvoiceModel.CustomerCode,
                    CustomerName = oldInvoiceModel.CustomerName,
                    Employee = oldInvoiceModel.Employee,
                    Status = true,
                    Total = oldInvoiceModel.InvoiceValue
                }).ToList();
                var affectedRow = await _unitOfWork.VerifyInvoice.InsertVerifyInvoicesAsync(oldInvoiceModels);
                if (affectedRow > 0)
                {
                    confirmAccountReceivable.Message = "Verify invoices added successfully";
                    confirmAccountReceivable.Success = true;
                    confirmAccountReceivable.StatusCode = (int)HttpStatusCode.OK;
                    confirmAccountReceivable.Result = verifyInvoiceModels;
                }
                else
                {
                    confirmAccountReceivable.Message = "Verify invoices added unsuccessfully";
                    confirmAccountReceivable.StatusCode = (int)HttpStatusCode.BadRequest;
                }
            }
            catch (SqlException ex)
            {
                confirmAccountReceivable.Message = ex.Message;
                confirmAccountReceivable.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            catch (Exception ex)
            {
                confirmAccountReceivable.Message = ex.Message;
                confirmAccountReceivable.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            return confirmAccountReceivable;
        }
    }
}
