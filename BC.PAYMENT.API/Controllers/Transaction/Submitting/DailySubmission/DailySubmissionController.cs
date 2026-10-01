using System.ComponentModel.DataAnnotations;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.Filter;
using BC.PAYMENT.CORE.DTO.Transaction.Submitting.DailySubmission;
using BC.PAYMENT.CORE.Entities.Transaction.Submitting.DailySubmission;
using BC.PAYMENT.LOGGING;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BC.PAYMENT.API.Controllers.Transaction.Submitting.DailySubmission;

public class DailySubmissionController(IUnitOfWork unitOfWork) : BaseApiController
{
    private static IEnumerable<DateTime> EachDay(DateTime startDate, DateTime endDate)
    {
        for (var date = startDate; date <= endDate; date = date.AddDays(1)) yield return date;
    }

    [HttpGet]
    [Route("getallapprovaldate")]
    public async Task<ApiResponse<ApprovalHistoryResponeDto>> GetAllApprovalDateAsync()
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var approvalHistoryModel = new ApprovalHistoryResponeDto();
            approvalHistoryModel.ApprovedInvoices =
                await unitOfWork.Approve.GetApprovedInvoiceAsync(credential.DbCode!);
            approvalHistoryModel.ReSubmittedInvoices =
                await unitOfWork.Approve.GetReSubmitInvoiceAsync(credential.DbCode!);
            approvalHistoryModel.RejectedInvoices =
                await unitOfWork.Approve.GetRejectedInvoiceAsync(credential.DbCode!);
            var publicHoliday = await unitOfWork.PublicHoliday.GetListHoliday();
            publicHoliday.ForEach(x =>
            {
                foreach (var date in EachDay(x.StartDate, x.EndDate))
                    approvalHistoryModel.PublicHolidays[date] = x.Remark!;
            });

            if (approvalHistoryModel != null)
                return ApiResponse<ApprovalHistoryResponeDto>.Builder()
                    .WithResult(approvalHistoryModel)
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithMessage("Approval History fetched successfully")
                    .Build();
            else
                return ApiResponse<ApprovalHistoryResponeDto>.Builder()
                    .WithStatusCode(StatusCodes.Status400BadRequest)
                    .WithMessage("Approval History fetched unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<ApprovalHistoryResponeDto>(ex.Message);
        }
    }

    [HttpPost]
    [Route("getsubmittedinvoicebydate")]
    public async Task<ApiResponse<List<DailySubmissionModel>>> GetSubmittedInvoiceByDateAsync(
        [FromBody] ByDateDto model)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var execute = await unitOfWork.Approve.GetSubmittedInvoiceByDateAsync(credential.DbCode!, model.FromDate,
                model.ToDate, model.Page, model.PageSize);
            if (execute.Any())
                return ApiResponse<List<DailySubmissionModel>>.Builder()
                    .WithResult(execute)
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithMessage("Submitted Invoice fetched successfully")
                    .Build();
            else
                return ApiResponse<List<DailySubmissionModel>>.Builder()
                    .WithStatusCode(StatusCodes.Status400BadRequest)
                    .WithMessage("Submitted Invoice fetched unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<DailySubmissionModel>>(ex.Message);
        }
    }

    [HttpPost]
    [Route("getsubmittedinvoicebyperiod")]
    public async Task<ApiResponse<List<DailySubmissionModel>>> GetSubmittedInvoiceByPeriodAsync(
        [FromBody] ByPeriodDto model)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var execute = await unitOfWork.Approve.GetSubmittedInvoiceByPeriodAsync(credential.DbCode!, model.Month,
                model.Year, model.Page, model.PageSize);
            if (execute.Any())
                return ApiResponse<List<DailySubmissionModel>>.Builder()
                    .WithResult(execute)
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithMessage("Submitted Invoices fetched successfully")
                    .Build();
            else
                return ApiResponse<List<DailySubmissionModel>>.Builder()
                    .WithStatusCode(StatusCodes.Status400BadRequest)
                    .WithMessage("Submitted Invoices fetched unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<DailySubmissionModel>>(ex.Message);
        }
    }

    [HttpGet]
    [Route("getapprovalinvoicebydate/{fromDate}/{toDate}")]
    public async Task<ApiResponse<List<ApprovalInvoiceModel>>> GetApprovalInvoiceByDateAsync([Required] string fromDate,
        [Required] string toDate)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var execute = await unitOfWork.Approve.GetApprovalListByDateAsync(credential.DbCode!, fromDate, toDate);
            if (execute.Any())
                return ApiResponse<List<ApprovalInvoiceModel>>.Builder()
                    .WithResult(execute)
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithMessage("Approved Invoices fetched successfully")
                    .Build();
            else
                return ApiResponse<List<ApprovalInvoiceModel>>.Builder()
                    .WithStatusCode(StatusCodes.Status400BadRequest)
                    .WithMessage("Approved Invoices fetched unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<ApprovalInvoiceModel>>(ex.Message);
        }
    }

    [HttpGet]
    [Route("getsubmittedinvoicebyperiod/{year}/{month}")]
    public async Task<ApiResponse<List<ApprovalInvoiceModel>>> GetSubmittedInvoiceByPeriodAsync([Required] int year,
        [Required] int month)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var execute = await unitOfWork.Approve.GetApprovalListByPeriodAsync(credential.DbCode!, month, year);
            if (execute.Any())
                return ApiResponse<List<ApprovalInvoiceModel>>.Builder()
                    .WithResult(execute)
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithMessage("Approved Invoices fetched successfully")
                    .Build();
            else
                return ApiResponse<List<ApprovalInvoiceModel>>.Builder()
                    .WithStatusCode(StatusCodes.Status400BadRequest)
                    .WithMessage("Approved Invoices fetched unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<ApprovalInvoiceModel>>(ex.Message);
        }
    }

    [HttpPut]
    [Route("updatesubmittedinvoicestatus/{submittedInvoiceId}")]
    public async Task<ApiResponse<int>> UpdateSubmittedInvoiceStatusAsync([Required] int submittedInvoiceId)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var affectedRow =
                await unitOfWork.Approve.UpdateStatusSubmittedInvoiceBySubmittedInvoiceId(submittedInvoiceId,
                    credential.Username!);
            if (affectedRow > 0)
                return ApiResponse<int>.Builder()
                    .WithResult(affectedRow)
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithMessage("Submitted Invoices updated to processing successfully")
                    .Build();
            else
                return ApiResponse<int>.Builder()
                    .WithStatusCode(StatusCodes.Status400BadRequest)
                    .WithMessage("Submitted Invoices updated to processing unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }

    [HttpPost]
    [Route("findsubmitedinvoicebytransactioncode/{transactionCode}")]
    public async Task<ApiResponse<int>> UpdateSubmittedInvoiceStatusAsync([Required] string transactionCode)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var affectedRow =
                await unitOfWork.Approve.FindSubmittedInvoiceByTransactionCodeAsync(credential.DbCode!,
                    transactionCode);
            if (affectedRow > 0)
                return ApiResponse<int>.Builder()
                    .WithResult(affectedRow)
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithMessage("Submitted Invoices Id fetched successfully")
                    .Build();
            else
                return ApiResponse<int>.Builder()
                    .WithStatusCode(StatusCodes.Status400BadRequest)
                    .WithMessage("Submitted Invoices Id fetched unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }

    [HttpGet]
    [Route("findhistorypaymentbytransactioncode/{transactionCode}")]
    public async Task<ApiResponse<List<HistoryPaymentModel>>> FindHistoryPaymentByTransactionCodeAsync(
        [Required] string transactionCode)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var execute =
                await unitOfWork.Approve.FindHistoryPaymentByTransactionCode(credential.DbCode!, transactionCode);
            if (execute.Any())
                return ApiResponse<List<HistoryPaymentModel>>.Builder()
                    .WithResult(execute)
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithMessage("Submitted Invoices fetched successfully")
                    .Build();
            else
                return ApiResponse<List<HistoryPaymentModel>>.Builder()
                    .WithStatusCode(StatusCodes.Status400BadRequest)
                    .WithMessage("Submitted Invoices fetched unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<HistoryPaymentModel>>(ex.Message);
        }
    }

    [HttpPost]
    [Route("scansubmittedinvoice/{transactionCode}")]
    public async Task<ApiResponse<List<HistoryPaymentModel>>> ScanBarCodeAsync([Required] string transactionCode)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var submittedId =
                await unitOfWork.Approve.FindSubmittedInvoiceByTransactionCodeAsync(credential.DbCode!,
                    transactionCode);
            if (submittedId > 0)
            {
                await unitOfWork.Approve.UpdateStatusSubmittedInvoiceBySubmittedInvoiceId(submittedId,
                    credential.Username!);

                var execute =
                    await unitOfWork.Approve.FindHistoryPaymentByTransactionCode(credential.DbCode!,
                        transactionCode);
                if (execute.Any())
                    return ApiResponse<List<HistoryPaymentModel>>.Builder()
                        .WithResult(execute)
                        .WithStatusCode(StatusCodes.Status200OK)
                        .WithMessage("Submitted Invoices Id fetched successfully")
                        .Build();
                else
                    return ApiResponse<List<HistoryPaymentModel>>.Builder()
                        .WithStatusCode(StatusCodes.Status400BadRequest)
                        .WithMessage("Submitted Invoices Id fetched unsuccessfully")
                        .Build();
            }

            return ApiResponse<List<HistoryPaymentModel>>.Builder()
                .WithStatusCode(StatusCodes.Status400BadRequest)
                .WithMessage("Submitted Invoices Id fetched unsuccessfully")
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<HistoryPaymentModel>>(ex.Message);
        }
    }
}