using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.Entities.Transaction.Submitting.SubmittingInvoice;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DailyPayment;

namespace BC.PAYMENT.API.Controllers.Transaction.Submitting.SubmittingInvoice;

public class SubmittedPerDeliveryController(IUnitOfWork unitOfWork) : BaseApiController
{
    [HttpGet]
    [Route("getsubmittedperdeliverybydate/{fromDate}/{toDate}")]
    public async Task<ApiResponse<List<ExpenseDetailModel>>> GetSubmittedPerDeliveryByDateAsync(
        [Required] string fromDate, string toDate)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var execute =
                await unitOfWork.SubmittedPerDelivery.GetSubmittedInvoicePerDeliveryAsync(credential.DbCode!, fromDate,
                    fromDate);
            if (execute.Any())
                return ApiResponse<List<ExpenseDetailModel>>.Builder()
                    .WithMessage("Submitted Invoices fetched successfully")
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithResult(execute)
                    .Build();
            else
                return ApiResponse<List<ExpenseDetailModel>>.Builder()
                    .WithMessage("Submitted Invoices fetched unsuccessfully")
                    .WithStatusCode(StatusCodes.Status400BadRequest)
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<ExpenseDetailModel>>(ex.Message);
        }
    }

    [HttpGet]
    [Route("getsubmittedperdeliverybyperiod/{year}/{month}")]
    public async Task<ApiResponse<List<ExpenseDetailModel>>> GetSubmittedPerDeliveryByPeriodAsync([Required] int month,
        int year)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var execute =
                await unitOfWork.SubmittedPerDelivery.GetSubmittedInvoicePerDeliveryByPeriodAsync(credential.DbCode!,
                    month, year);
            if (execute.Any())
                return ApiResponse<List<ExpenseDetailModel>>.Builder()
                    .WithMessage("Submitted Invoices fetched successfully")
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithResult(execute)
                    .Build();
            else
                return ApiResponse<List<ExpenseDetailModel>>.Builder()
                    .WithMessage("Submitted Invoices fetched unsuccessfully")
                    .WithStatusCode(StatusCodes.Status400BadRequest)
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<ExpenseDetailModel>>(ex.Message);
        }
    }

    [HttpDelete]
    [Route("updatesubmittedperdelivery/{submittedInvoiceId}")]
    public async Task<ApiResponse<string>> DeleteSubmittedInvoicePerDeliveryAsync([Required] string submittedInvoiceId)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var affectedRow = await unitOfWork.SubmittedPerDelivery.DeleteSubmittedInvoiceAsync(submittedInvoiceId);
            if (affectedRow > 0)
                return ApiResponse<string>.Builder()
                    .WithMessage("Submitted Invoices deleted successfully")
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithResult(submittedInvoiceId)
                    .Build();
            else
                return ApiResponse<string>.Builder()
                    .WithMessage("Submitted Invoices deleted unsuccessfully")
                    .WithStatusCode(StatusCodes.Status400BadRequest)
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<string>(ex.Message);
        }
    }
}