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
    public class VerifyInvoicesController(IUnitOfWork unitOfWork) : BaseApiController
    {
        [HttpGet]
        [Route("getverifyinvoices")]
        public async Task<ApiResponse<List<VerifyInvoiceModel>>> GetVerifyInvoicesAsync()
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var verifyInvoiceList = await unitOfWork.VerifyInvoice.GetVerifyInvoicesAsync(credential.DbCode!);
                if (verifyInvoiceList.Any())
                {
                    return ApiResponse<List<VerifyInvoiceModel>>.Builder()
                        .WithResult(verifyInvoiceList)
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithMessage("Verify Invoices fetched successfully")
                        .Build();
                }
                else
                {
                    return ApiResponse<List<VerifyInvoiceModel>>.Builder()
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithMessage("Verify Invoices fetched unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<VerifyInvoiceModel>>(ex.Message);
            }
        }

        //[HttpPost]
        //[Route("getoldinvoices")]
        //public async Task<ApiResponse<List<ConfirmAccountReceivableModel.ConfirmBalanceDetails>>> GetOldInvoicsAsync(OldInvoiceRequestPostDto model)
        //{
        //    var credential = Common.DecodeJwt(HttpContext.User);
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
        //
        //        };
        //        var oldInvoiceModels = await unitOfWork.Invoices.GetAllOldInvoices(oldInvoiceParameter);
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
        //        var affectedRow = await unitOfWork.ConfirmAccountReceivable.AddConfirmAccountReceivableDetails(model.AccountReceivableId, oldInvoices);
        //        if (affectedRow > 0)
        //        {
        //            return ApiResponse<List<ConfirmAccountReceivableModel.ConfirmBalanceDetails>>.Builder()
        //                .WithResult(oldInvoices)
        //                .WithStatusCode((int)HttpStatusCode.OK)
        //                .WithMessage("Confirm Account Receivables Detail added successfully")
        //                .Build();
        //        }
        //        else
        //        {
        //            return ApiResponse<List<ConfirmAccountReceivableModel.ConfirmBalanceDetails>>.Builder()
        //                .WithStatusCode((int)HttpStatusCode.BadRequest)
        //                .WithMessage("Confirm Account Receivables Detail added unsuccessfully")
        //                .Build();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return GlobalExceptionHandler.ExceptionError<List<ConfirmAccountReceivableModel.ConfirmBalanceDetails>>(ex.Message);
        //    }
        //}

        [HttpPost]
        [Route("addnewverifyinvoice")]
        public async Task<ApiResponse<List<VerifyInvoiceModel>>> AddNewVerifyInvoice(OldInvoiceRequestPostDto model)
        {
            var credential = Common.DecodeJwt(HttpContext.User);
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
                var oldInvoiceModels = await unitOfWork.Invoices.GetAllOldInvoices(oldInvoiceParameter);
                var verifyInvoiceModels = oldInvoiceModels.Select(oldInvoiceModel => new VerifyInvoiceModel
                {
                    Transaction = oldInvoiceModel.TransactionCode,
                    CustomerCode = oldInvoiceModel.CustomerCode,
                    CustomerName = oldInvoiceModel.CustomerName,
                    Employee = oldInvoiceModel.Employee,
                    Status = true,
                    Total = oldInvoiceModel.InvoiceValue
                }).ToList();
                var affectedRow = await unitOfWork.VerifyInvoice.InsertVerifyInvoicesAsync(oldInvoiceModels);
                if (affectedRow > 0)
                {
                    return ApiResponse<List<VerifyInvoiceModel>>.Builder()
                        .WithResult(verifyInvoiceModels)
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithMessage("Verify invoices added successfully")
                        .Build();
                }
                else
                {
                    return ApiResponse<List<VerifyInvoiceModel>>.Builder()
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithMessage("Verify invoices added unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<VerifyInvoiceModel>>(ex.Message);
            }
        }
    }
}
