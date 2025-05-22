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
using Microsoft.Data.SqlClient;
using static BC.PAYMENT.CORE.Entities.Accounting.AccountReceivableModel;

namespace BC.PAYMENT.API.Controllers.Transaction.Submitting.SubmittingInvoice
{

    public class SubmittingController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubmittingController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


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
            var submittedInvoice = new ApiResponse<SubmittingInvoiceDto>();
            try
            {
                var submittedInvoices = new List<SubmittedInvoiceModel>();

                foreach (var invoice in model.AccountReceivables)
                {
                    if(invoice.InvoiceType == "N")
                        await _unitOfWork.Invoices.PostPrintInvoiceAsync(invoice.TransactionCode, RequestType.Invoice,credential.DbCode!,credential.Period,credential.Username!);
                    var analysisByCustomerCodeAsync =
                        await _unitOfWork.Generators.GetSaleAnalysisByCustomerCodeAsync(invoice.CustomerCode!, credential.DbCode!);
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
                            await _unitOfWork.AccountReceivable.InsertAccountReceivable(accountReceivableParameter, true);
                        }
                        else
                        {
                            var total = invoice.Paid-invoice.HalfPaid;
                            // split account receivable to two account receivable
                            var results = await _unitOfWork.AccountReceivable.SplitAccountReceivable(invoice.CustomerCode,
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
                                await _unitOfWork.AccountReceivable.InsertAccountReceivable(accountReceivableParameter);
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

                var affectedRow = await _unitOfWork.SubmittingInvoice.AddSubmittedInvoices(submittedInvoices);
                if (affectedRow > 0)
                {
                    submittedInvoice.StatusCode = (int)HttpStatusCode.OK;
                    submittedInvoice.Success = true;
                    submittedInvoice.Message = "Submitted invoices added successfully";
                    submittedInvoice.Result = model;
                }
                else
                {
                    submittedInvoice.StatusCode = (int)HttpStatusCode.BadRequest;
                    submittedInvoice.Success = false;
                    submittedInvoice.Message = "Submitted invoices added unsuccessfully";
                }
            }
            catch (Exception ex)
            {
                submittedInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                submittedInvoice.Success = false;
                submittedInvoice.Message = $@"Exception : {ex.Message}";
            }
            return submittedInvoice;
        }


        [HttpGet]
        [Route("getallnotsubmitpaidinvoice/{fromDate}/{toDate}")]
        public async Task<ApiResponse<List<SubmittedInvoiceModel>>> GetAllNotSubmitPaidInvoiceAsync([Required] string fromDate, [Required] string toDate)
        {
            var credential = Common.DecodeJwt(User);
            var submittedInvoice = new ApiResponse<List<SubmittedInvoiceModel>>();
            try
            {
                var execute = await _unitOfWork.SubmittingInvoice.GetAllNotSubmitPaidInvoice(credential.DbCode!, fromDate, toDate);
                if (execute.Count > 0)
                {
                    submittedInvoice.StatusCode = (int)HttpStatusCode.OK;
                    submittedInvoice.Success = true;
                    submittedInvoice.Message = "Unsubmitted invoices fetched successfully";
                    submittedInvoice.Result = execute;
                }
                else
                {
                    submittedInvoice.StatusCode = (int)HttpStatusCode.BadRequest;
                    submittedInvoice.Success = false;
                    submittedInvoice.Message = "Unsubmitted invoices fetched unsuccessfully";
                }
            }
            catch (SqlException ex)
            {
                submittedInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                submittedInvoice.Success = false;
                submittedInvoice.Message = $@"Sql Exception : {ex.Message}";
            }
            catch (Exception ex)
            {
                submittedInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                submittedInvoice.Success = false;
                submittedInvoice.Message = $@"Exception : {ex.Message}";
            }
            return submittedInvoice;
        }
    }
}
