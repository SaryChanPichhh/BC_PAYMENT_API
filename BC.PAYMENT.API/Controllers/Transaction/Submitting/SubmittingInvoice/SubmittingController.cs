using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mime;
using System.Transactions;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.Transaction.Submitting.SubmittingInvoice;
using BC.PAYMENT.CORE.Entities.Accounting;
using BC.PAYMENT.CORE.Entities.Transaction.Submitting.SubmittingInvoice;
using BC.PAYMENT.CORE.Enums;
using Microsoft.AspNetCore.Mvc;
using static BC.PAYMENT.CORE.Entities.Accounting.AccountReceivableModel;

namespace BC.PAYMENT.API.Controllers.Transaction.Submitting.SubmittingInvoice
{

    public class SubmittingController(IUnitOfWork unitOfWork) : BaseApiController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addnewsubmittedinvoices")]
        public async Task<ApiResponse<SubmittingInvoiceDto>> PostSubmittedInvoiceAsync(SubmittingInvoiceDto model)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var submittedInvoices = new List<SubmittedInvoiceModel>();

                foreach (var invoice in model.AccountReceivables)
                {
                    if(invoice.InvoiceType == "N")
                        await unitOfWork.Invoices.PostPrintInvoiceAsync(invoice.TransactionCode, RequestType.Invoice,credential.DbCode!,credential.Period,credential.Username!);
                    var analysisByCustomerCodeAsync =
                        await unitOfWork.Generators.GetSaleAnalysisByCustomerCodeAsync(invoice.CustomerCode!, credential.DbCode!);
                    if (model.IsAutoAccountsReceivable)
                    {
                        if (invoice.Status)
                        {
                            var accountReceivableParameter = new AccountReceivableModel.AccountReceivableParameter(
                                credential.DbCode!,
                                invoice.CustomerCode!,
                                    invoice.Period,
                                invoice.Paid,
                                invoice.TransactionCode!,
                                invoice.Period,
                                invoice.Period.ToString(),
                                credential.Username!,
                                analysisByCustomerCodeAsync.AnalysisC0,
                                analysisByCustomerCodeAsync.AnalysisC1,
                                analysisByCustomerCodeAsync.AnalysisC2,
                                analysisByCustomerCodeAsync.AnalysisC3,
                                analysisByCustomerCodeAsync.AnalysisC4,
                                analysisByCustomerCodeAsync.AnalysisC5,
                                analysisByCustomerCodeAsync.AnalysisC6,
                                analysisByCustomerCodeAsync.AnalysisC7,
                                analysisByCustomerCodeAsync.AnalysisC8,
                                analysisByCustomerCodeAsync.AnalysisC9,
                                0,
                                credential.Username!,
                                credential.Username!,
                                credential.Username!,
                                DateTime.Today
                            );
                            await unitOfWork.AccountReceivable.InsertAccountReceivable(accountReceivableParameter, true);
                        }
                        else
                        {
                            var total = invoice.Paid-invoice.HalfPaid;
                            // split account receivable to two account receivable
                            var results = await unitOfWork.AccountReceivable.SplitAccountReceivable(invoice.CustomerCode,
                                invoice.TransactionCode, credential.DbCode, total, invoice.HalfPaid);
                            if (results)
                            {
                                var accountReceivableParameter = new AccountReceivableParameter(
                                    credential.DbCode,
                                    invoice.CustomerCode,
                                invoice.Period,
                                    invoice.HalfPaid,
                                    invoice.TransactionCode,
                                    invoice.Period,
                                    invoice.Period.ToString(),
                                    credential.Username,
                                    analysisByCustomerCodeAsync.AnalysisC0,
                                    analysisByCustomerCodeAsync.AnalysisC1,
                                    analysisByCustomerCodeAsync.AnalysisC2,
                                    analysisByCustomerCodeAsync.AnalysisC3,
                                    analysisByCustomerCodeAsync.AnalysisC4,
                                    analysisByCustomerCodeAsync.AnalysisC5,
                                    analysisByCustomerCodeAsync.AnalysisC6,
                                    analysisByCustomerCodeAsync.AnalysisC7,
                                    analysisByCustomerCodeAsync.AnalysisC8,
                                    analysisByCustomerCodeAsync.AnalysisC9,
                                    0,
                                    credential.Username,
                                    credential.Username,
                                    credential.Username,
                                    DateTime.Today
                                );
                                await unitOfWork.AccountReceivable.InsertAccountReceivable(accountReceivableParameter);
                            }
                        }
                    }
                }
                foreach (var invoice in model.SubmittedInvoices)
                {
                    var submittedInvoiceModel = new SubmittedInvoiceModel
                    {
                        DbCode = credential.DbCode,
                        InvoiceId = invoice.InvoiceId,
                        CustomerCode = invoice.CustomerCode,
                        TransactionCode = invoice.TransactionCode,
                        InvoiceAmount = invoice.InvoiceAmount,
                        HalfPaid = invoice.HalfPaid,
                        Paid = invoice.PaidAmount
                    };
                    submittedInvoices.Add(submittedInvoiceModel);
                }

                var affectedRow = await unitOfWork.SubmittingInvoice.AddSubmittedInvoices(submittedInvoices);
                if (affectedRow > 0)
                {
                    return ApiResponse<SubmittingInvoiceDto>.Builder()
                        .WithMessage("Submitted invoices added successfully")
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithResult(model)
                        .Build();
                }
                else
                {
                    return ApiResponse<SubmittingInvoiceDto>.Builder()
                        .WithMessage("Submitted invoices added unsuccessfully")
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<SubmittingInvoiceDto>(ex.Message);
            }
        }


        [HttpGet]
        [Route("getallnotsubmitpaidinvoice/{fromDate}/{toDate}")]
        public async Task<ApiResponse<List<SubmittedInvoiceModel>>> GetAllNotSubmitPaidInvoiceAsync([Required] string fromDate, [Required] string toDate)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var execute = await unitOfWork.SubmittingInvoice.GetAllNotSubmitPaidInvoice(credential.DbCode!, fromDate, toDate);
                if (execute.Count > 0)
                {
                    return ApiResponse<List<SubmittedInvoiceModel>>.Builder()
                        .WithMessage("Unsubmitted invoices fetched successfully")
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithResult(execute)
                        .Build();
                }
                else
                {
                    return ApiResponse<List<SubmittedInvoiceModel>>.Builder()
                        .WithMessage("Unsubmitted invoices fetched unsuccessfully")
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<SubmittedInvoiceModel>>(ex.Message);
            }
        }
    }
}
