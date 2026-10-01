using System.ComponentModel.DataAnnotations;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.Entities.Transaction.Submitting.SubmittingInvoice;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;


namespace BC.PAYMENT.API.Controllers.Transaction.Submitting.SubmittingInvoice;

public class RejectedInvoicePerDeliveryController(IUnitOfWork unitOfWork) : BaseApiController
{
    [HttpGet]
    [Route("getrejectedinvoiceperdelivery/{fromDate}/{toDate}")]
    public async Task<ApiResponse<List<RejectedInvoicePerDelivery>>> GetRejectedInvoicePerDeliveryByDateAsync(
        string fromDate, string toDate)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var rejectedInvoices =
                await unitOfWork.RejectedInvoicePerDelivery.GetAllRejectedInvoicePerDeliveryByDateAsync(
                    credential.DbCode!, fromDate, toDate);
            if (rejectedInvoices.Any())
                return ApiResponse<List<RejectedInvoicePerDelivery>>.Builder()
                    .WithMessage("Rejected Invoices fetched successfully")
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithResult(rejectedInvoices)
                    .Build();
            else
                return ApiResponse<List<RejectedInvoicePerDelivery>>.Builder()
                    .WithMessage("Rejected Invoices fetched unsuccessfully")
                    .WithStatusCode(StatusCodes.Status400BadRequest)
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<RejectedInvoicePerDelivery>>(ex.Message);
        }
    }

    [HttpGet]
    [Route("getrejectedinvoiceperdelivery/{year}/{month}")]
    public async Task<ApiResponse<List<RejectedInvoicePerDelivery>>> GetRejectedInvoicePerDeliveryByPeriodAsync(
        [Required] int year, [Required] int month)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var rejectedInvoices =
                await unitOfWork.RejectedInvoicePerDelivery.GetAllRejectedInvoicePerDeliveryByPeriodAsync(
                    credential.DbCode!, month, year);
            if (rejectedInvoices.Any())
                return ApiResponse<List<RejectedInvoicePerDelivery>>.Builder()
                    .WithMessage("Rejected Invoices fetched successfully")
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithResult(rejectedInvoices)
                    .Build();
            else
                return ApiResponse<List<RejectedInvoicePerDelivery>>.Builder()
                    .WithMessage("Rejected Invoices fetched unsuccessfully")
                    .WithStatusCode(StatusCodes.Status400BadRequest)
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<RejectedInvoicePerDelivery>>(ex.Message);
        }
    }
}