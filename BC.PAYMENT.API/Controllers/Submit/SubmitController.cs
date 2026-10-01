using System.ComponentModel.DataAnnotations;
using BC.PAYMENT.CORE.Contracts.Request.Submit;
using BC.PAYMENT.CORE.Contracts.Response.SubmitInvoice;
using BC.PAYMENT.CORE.Contracts.Transaction.Submitting.SubmittingInvoice;
using BC.PAYMENT.CORE.Entities;
using BC.PAYMENT.CORE.Entities.Transaction.Submitting.SubmittingInvoice;
using BC.PAYMENT.CORE.Enums;

namespace BC.PAYMENT.API.Controllers.Submit;

public class SubmitController(IUnitOfWork unitOfWork) : BaseApiController
{
    [HttpPost]
    [Route("")]
    public async Task<ApiResponse<int>> PostSubmittedInvoiceAsync([FromBody] List<BcInvoiceSubmittedRequest> req)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var submittedInvoices = new List<BcInvoiceSubmitted>();
            foreach (var invoice in req)
            {
                var submittedInvoiceModel = new BcInvoiceSubmitted
                {
                    DbCode = credential.DbCode,
                    InvoiceId = invoice.InvoiceId,
                    CustomerCode = invoice.CustomerCode,
                    TransactionCode = invoice.TransactionCode,
                    Money = invoice.Money,
                    Paid = invoice.Paid,
                    SubmittedBy = credential.Username,
                    SubmittedDate = credential.CurrectDate
                };
                submittedInvoices.Add(submittedInvoiceModel);
            }

            var affectedRow = await unitOfWork.SubmittingInvoice.AddSubmittedInvoices(submittedInvoices);
            if (affectedRow > 0)
                return ApiResponse<int>.Builder()
                    .WithMessage("Submitted invoices added successfully")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(affectedRow)
                    .Build();
            return ApiResponse<int>.Builder()
                .WithMessage("Submitted invoices added unsuccessfully")
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }

    [HttpGet]
    [Route("by-date")]
    public async Task<ApiResponse<List<SubmitInvoiceResponse>>> GetSubmittedInvoiceByDateAsync(
        [FromQuery] string fromDate, [FromQuery] string toDate)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var execute =
                await unitOfWork.SubmittingInvoice.GetSubmittedInvoiceByDateAsync(credential.DbCode!, fromDate, toDate);
            if (execute.Count != 0)
                return ApiResponse<List<SubmitInvoiceResponse>>.Builder()
                    .WithMessage("Submitted Invoices fetched successfully")
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithResult(execute)
                    .Build();
            return ApiResponse<List<SubmitInvoiceResponse>>.Builder()
                .WithMessage("Submitted Invoices fetched unsuccessfully")
                .WithStatusCode(StatusCodes.Status400BadRequest)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<SubmitInvoiceResponse>>(ex.Message);
        }
    }

    [HttpGet]
    [Route("pending-invoice-by-date")]
    public async Task<ApiResponse<List<SubmitInvoiceResponse>>> GetSubmittedPendingInvoiceByDateAsync(
        [FromQuery] string fromDate, [FromQuery] string toDate)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var execute =
                await unitOfWork.SubmittingInvoice.GetSubmittedPendingInvoiceByDateAsync(credential.DbCode!, fromDate,
                    toDate);
            if (execute.Count != 0)
                return ApiResponse<List<SubmitInvoiceResponse>>.Builder()
                    .WithMessage("Submitted Pending Invoices fetched successfully")
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithResult(execute)
                    .Build();
            return ApiResponse<List<SubmitInvoiceResponse>>.Builder()
                .WithMessage("Submitted Pending Invoices fetched unsuccessfully")
                .WithStatusCode(StatusCodes.Status400BadRequest)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<SubmitInvoiceResponse>>(ex.Message);
        }
    }

    [HttpGet]
    [Route("approved-invoice-by-date")]
    public async Task<ApiResponse<List<ApproveSubmitInvoiceResponse>>> GetApprovedSubmittedInvoiceByDateAsync(
        [FromQuery] string fromDate, [FromQuery] string toDate)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var execute =
                await unitOfWork.SubmittingInvoice.GetApprovedSubmittedInvoiceByDateAsync(credential.DbCode!, fromDate,
                    toDate);
            if (execute.Any())
                return ApiResponse<List<ApproveSubmitInvoiceResponse>>.Builder()
                    .WithMessage("Approved Submitted Invoices fetched successfully")
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithResult(execute)
                    .Build();
            return ApiResponse<List<ApproveSubmitInvoiceResponse>>.Builder()
                .WithMessage("Approved Submitted Invoices is empty.")
                .WithSuccess(true)
                .WithStatusCode(StatusCodes.Status200OK)
                .WithResult([])
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<ApproveSubmitInvoiceResponse>>(ex.Message);
        }
    }

    [HttpGet]
    [Route("approved-invoice-by-period")]
    public async Task<ApiResponse<List<ApproveSubmitInvoiceResponse>>> GetApprovedSubmittedInvoicePeriodAsync(
        [FromQuery] int year, [FromQuery] int month)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var execute =
                await unitOfWork.SubmittingInvoice.GetApprovedSubmittedInvoicePeriodAsync(credential.DbCode!, year,
                    month);
            if (execute.Count != 0)
                return ApiResponse<List<ApproveSubmitInvoiceResponse>>.Builder()
                    .WithMessage("Approved Submitted Invoices fetched successfully")
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithResult(execute)
                    .Build();
            return ApiResponse<List<ApproveSubmitInvoiceResponse>>.Builder()
                .WithMessage("Approved Submitted Invoices fetched unsuccessfully")
                .WithStatusCode(StatusCodes.Status400BadRequest)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<ApproveSubmitInvoiceResponse>>(ex.Message);
        }
    }

    [HttpPut]
    [Route("cancel-invoice/{submittedId}")]
    public async Task<ApiResponse<string>> UpdateSubmittedInvoiceFromPendingToRejectAsync([Required] string submittedId)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var affectedRow =
                await unitOfWork.SubmittedInvoice.UpdateInvoiceFromPendingToCancelAsync(credential.DbCode!,
                    credential.Username!, submittedId);
            if (affectedRow > 0)
                return ApiResponse<string>.Builder()
                    .WithMessage("Submitted Invoices updated successfully")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(submittedId)
                    .Build();
            return ApiResponse<string>.Builder()
                .WithMessage("Submitted Invoices updated unsuccessfully")
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<string>(ex.Message);
        }
    }

    [HttpPut]
    [Route("update-status/{submittedId}/{status}")]
    public async Task<ApiResponse<bool>> UpdateApprovedStatusBcInvoiceSubmittedAsync([Required] string submittedId,
        [Required] string status)
    {
        try
        {
            var affectedRow =
                await unitOfWork.SubmittingInvoice.UpdateStatusBcInvoiceSubmittedAsync(submittedId, status);
            if (affectedRow)
                return ApiResponse<bool>.Builder()
                    .WithMessage("Submitted Invoices updated successfully")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(affectedRow)
                    .Build();
            return ApiResponse<bool>.Builder()
                .WithMessage("Submitted Invoices updated unsuccessfully")
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<bool>(ex.Message);
        }
    }

    [HttpPost]
    [Route("save-approval-invoice")]
    public async Task<ApiResponse<int>> AddNewApprovalInvoice([FromBody] BcApprovalInvoiceRequest request)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var model = new BcApprovalInvoice
            {
                ApprovalStatus = request.ApprovalStatus,
                SubmittedId = request.SubmittedId,
                ApprovalBy = credential.Username,
                ApprovalDate = DateTime.Today,
                DbCode = credential.DbCode,
                Description = request.Description
            };
            var affectedRow = await unitOfWork.SubmittingInvoice.AddNewApprovalInvoice(model);
            if (affectedRow > 0)
                return ApiResponse<int>.Builder()
                    .WithMessage("Submitted Invoices added successfully")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(affectedRow)
                    .Build();
            return ApiResponse<int>.Builder()
                .WithMessage("Submitted Invoices added unsuccessfully")
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }
}