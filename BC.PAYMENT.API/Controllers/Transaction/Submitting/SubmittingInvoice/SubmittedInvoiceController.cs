using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.Contracts.Response.SubmitInvoice;
using BC.PAYMENT.CORE.Entities.Transaction.Submitting.SubmittingInvoice;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BC.PAYMENT.API.Controllers.Transaction.Submitting.SubmittingInvoice;

public class SubmittedInvoiceController(IUnitOfWork unitOfWork) : BaseApiController
{
    [HttpGet]
    [Route("getsubmittedinvoice/{fromDate}/{toDate}")]
    public async Task<ApiResponse<List<SubmittedInvoiceModel>>> GetSubmittedInvoicesByDate([Required] string fromDate,
        string toDate)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var execute =
                await unitOfWork.SubmittedInvoice.GetAllNotSubmitPaidInvoice(credential.DbCode!, fromDate, fromDate);
            if (execute.Any())
                return ApiResponse<List<SubmittedInvoiceModel>>.Builder()
                    .WithMessage("Submitted Invoices fetched successfully")
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithResult(execute)
                    .Build();
            else
                return ApiResponse<List<SubmittedInvoiceModel>>.Builder()
                    .WithMessage("Submitted Invoices fetched unsuccessfully")
                    .WithStatusCode(StatusCodes.Status400BadRequest)
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<SubmittedInvoiceModel>>(ex.Message);
        }
    }
}