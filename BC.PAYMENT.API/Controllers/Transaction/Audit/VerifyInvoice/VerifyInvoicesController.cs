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

namespace BC.PAYMENT.API.Controllers.Transaction.Audit.VerifyInvoice;

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
                return ApiResponse<List<VerifyInvoiceModel>>.Builder()
                    .WithResult(verifyInvoiceList)
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithMessage("Verify Invoices fetched successfully")
                    .Build();
            else
                return ApiResponse<List<VerifyInvoiceModel>>.Builder()
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .WithMessage("Verify Invoices fetched unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<VerifyInvoiceModel>>(ex.Message);
        }
    }
}