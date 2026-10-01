using System.ComponentModel.DataAnnotations;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.Entities.Transaction.Submitting.SubmittingInvoice;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BC.PAYMENT.API.Controllers.Transaction.Submitting.SubmittingInvoice;

public class RejectedInvoiceController(IUnitOfWork unitOfWork) : BaseApiController
{
    [HttpGet]
    [Route("getrejectedinvoicebyperiod/{year}/{month}")]
    public async Task<ApiResponse<List<RejectedInvoiceModel>>> GetRejectedInvoiceByPeriod([Required] int month,
        [Required] int year)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var rejectedInvoices =
                await unitOfWork.RejectedInvoice.GetAllRejectedInvoicesByPeriodAsync(credential.DbCode!, month, year);
            if (rejectedInvoices.Any())
                return ApiResponse<List<RejectedInvoiceModel>>.Builder()
                    .WithMessage("Rejected Invoices fetched successfully")
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithResult(rejectedInvoices)
                    .Build();
            else
                return ApiResponse<List<RejectedInvoiceModel>>.Builder()
                    .WithMessage("Rejected Invoices fetched unsuccessfully")
                    .WithStatusCode(StatusCodes.Status400BadRequest)
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<RejectedInvoiceModel>>(ex.Message);
        }
    }

    [HttpGet]
    [Route("getrejectedinvoicebydate/{fromDate}/{toDate}")]
    public async Task<ApiResponse<List<RejectedInvoiceModel>>> GetRejectedInvoiceByDate([Required] string fromDate,
        [Required] string toDate)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var rejectedInvoices =
                await unitOfWork.RejectedInvoice.GetAllRejectedInvoicesByDateAsync(credential.DbCode!, fromDate,
                    toDate);
            if (rejectedInvoices.Any())
                return ApiResponse<List<RejectedInvoiceModel>>.Builder()
                    .WithMessage("Rejected Invoices fetched successfully")
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithResult(rejectedInvoices)
                    .Build();
            else
                return ApiResponse<List<RejectedInvoiceModel>>.Builder()
                    .WithMessage("Rejected Invoices fetched unsuccessfully")
                    .WithStatusCode(StatusCodes.Status400BadRequest)
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<RejectedInvoiceModel>>(ex.Message);
        }
    }

    [HttpPut]
    [Route("updatestocancel")]
    public async Task<ApiResponse<int>> UpdateSubmittedInvoiceFromRejectToCancel([Required] string transactionCode,
        [Required] string submittedId)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var affectedRow =
                await unitOfWork.RejectedInvoice.UpdateSubmittedInvoiceFromRejectToCancel(credential.DbCode!,
                    transactionCode, submittedId);
            if (affectedRow > 0)
                return ApiResponse<int>.Builder()
                    .WithMessage("Rejected Invoices updated successfully")
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithResult(affectedRow)
                    .Build();
            else
                return ApiResponse<int>.Builder()
                    .WithMessage("Rejected Invoices updated unsuccessfully")
                    .WithStatusCode(StatusCodes.Status400BadRequest)
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }
}