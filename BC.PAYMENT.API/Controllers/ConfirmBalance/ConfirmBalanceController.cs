using BC.PAYMENT.API.MapperHelper;
using BC.PAYMENT.CORE.Contracts.Request.ConfirmBalance;
using BC.PAYMENT.CORE.Contracts.Response.ConfirmBalance;
using BC.PAYMENT.CORE.Entities;
using FluentValidation;

namespace BC.PAYMENT.API.Controllers.ConfirmBalance;

public class ConfirmBalanceController(
    IUnitOfWork unitOfWork,
    IValidator<CreateConfirmBalanceRequest>? createValidator = null,
    IValidator<UpdateConfirmBalanceRequest>? updateValidator = null) : BaseApiController
{
    [HttpGet]
    [Route("")]
    public async Task<ApiResponse<List<ConfirmBalanceResponse>>> GetConfirmAccountReceivableAsync()
    {
        var credential = Common.DecodeJwt(HttpContext.User);
        try
        {
            var execute = await unitOfWork.ConfirmBalance.GetConfirmAccountReceivable(credential.DbCode!);
            if (execute.Count > 0)
                return ApiResponse<List<ConfirmBalanceResponse>>.Builder()
                    .WithMessage("Confirm account receivables fetched successfully")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithSuccess(true)
                    .WithResult(execute)
                    .Build();
            return ApiResponse<List<ConfirmBalanceResponse>>.Builder()
                .WithMessage("Confirm account receivables fetched unsuccessfully")
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .WithResult(new List<ConfirmBalanceResponse>())
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<ConfirmBalanceResponse>>(ex.Message);
        }
    }

    [HttpPost]
    [Route("")]
    public async Task<ApiResponse<int>> AddConfirmAccountReceivableAsync([FromBody] CreateConfirmBalanceRequest request)
    {
        if (createValidator != null)
        {
            var validationResult = await createValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
                return GlobalExceptionHandler.ValidateSchemaError<int>(
                    validationResult.Errors.toProperties(), "Validation failed.");
        }

        var credential = Common.DecodeJwt(HttpContext.User);
        try
        {
            var model = new DtConfirmAccountReceivable
            {
                DbCode = credential?.DbCode,
                ConfirmBalanceOwner = request.ConfirmBalanceOwner,
                Participants = request.Participants,
                Description = request.Description,
                CreatedDate = DateTime.Now,
                CreatedBy = credential?.Username
            };

            var affectedRow = await unitOfWork.ConfirmBalance.AddConfirmAccountReceivable(model);
            if (affectedRow > 0)
                return ApiResponse<int>.Builder()
                    .WithMessage("Confirm account receivable added successfully")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithSuccess(true)
                    .WithResult(affectedRow)
                    .Build();

            return ApiResponse<int>.Builder()
                .WithMessage("Confirm account receivable added unsuccessfully")
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .WithResult(0)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }

    [HttpPut]
    [Route("")]
    public async Task<ApiResponse<int>> UpdateConfirmAccountReceivableAsync(
        [FromBody] UpdateConfirmBalanceRequest request)
    {
        if (updateValidator != null)
        {
            var validationResult = await updateValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
                return GlobalExceptionHandler.ValidateSchemaError<int>(
                    validationResult.Errors.toProperties(), "Validation failed.");
        }

        try
        {
            var model = new DtConfirmAccountReceivable
            {
                Id = request.Id,
                ConfirmBalanceOwner = request.ConfirmBalanceOwner,
                Participants = request.Participants,
                Description = request.Description
            };

            var affectedRow = await unitOfWork.ConfirmBalance.UpdateConfirmAccountReceivable(model);
            if (affectedRow > 0)
                return ApiResponse<int>.Builder()
                    .WithMessage("Confirm account receivable updated successfully")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithSuccess(true)
                    .WithResult(affectedRow)
                    .Build();
            return ApiResponse<int>.Builder()
                .WithMessage("Confirm account receivable updated unsuccessfully")
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .WithResult(0)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }

    [HttpDelete]
    [Route("{id:int}")]
    public async Task<ApiResponse<int>> DeleteConfirmAccountReceivableAsync(int id)
    {
        try
        {
            var affectedRow = await unitOfWork.ConfirmBalance.DeleteConfirmAccountReceivable(id);
            if (affectedRow > 0)
                return ApiResponse<int>.Builder()
                    .WithMessage("Confirm account receivable deleted successfully")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithSuccess(true)
                    .WithResult(affectedRow)
                    .Build();

            return ApiResponse<int>.Builder()
                .WithMessage("Confirm account receivable deleted unsuccessfully")
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .WithResult(0)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }

    [HttpGet]
    [Route("details/{confirmBalanceId:int}")]
    public async Task<ApiResponse<List<DtConfirmAccountReceivableDetailResponse>>> GetConfirmBalanceDetailsAsync(
        int confirmBalanceId)
    {
        var credential = Common.DecodeJwt(HttpContext.User);
        try
        {
            var execute =
                await unitOfWork.ConfirmBalance.GetConfirmBalanceDetailsAsync(credential.DbCode!, confirmBalanceId);
            if (execute.Count > 0)
                return ApiResponse<List<DtConfirmAccountReceivableDetailResponse>>.Builder()
                    .WithMessage("Confirm balance details fetched successfully")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithSuccess(true)
                    .WithResult(execute)
                    .Build();

            return ApiResponse<List<DtConfirmAccountReceivableDetailResponse>>.Builder()
                .WithMessage("Confirm balance details fetched unsuccessfully")
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .WithResult(new List<DtConfirmAccountReceivableDetailResponse>())
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<DtConfirmAccountReceivableDetailResponse>>(ex.Message);
        }
    }

    [HttpPost]
    [Route("details")]
    public async Task<ApiResponse<int>> AddConfirmBalanceDetailsAsync(
        [FromBody] List<CreateConfirmBalanceDetailRequest> requests)
    {
        var credential = Common.DecodeJwt(HttpContext.User);
        try
        {
            var details = requests.Select(r => new DtConfirmAccountReceivableDetail
            {
                HeaderId = r.HeaderId,
                DbCode = credential?.DbCode ?? string.Empty,
                CustomerCode = r.CustomerCode,
                CustomerName = r.CustomerName,
                InvoiceCode = r.InvoiceCode,
                InvoiceAmount = r.InvoiceAmount,
                Status = !string.IsNullOrWhiteSpace(r.Status) ? r.Status : "Pending",
                CreatedBy = credential?.Username ?? string.Empty,
                CreatedDate = DateTime.Now
            }).ToList();

            var affectedRow = await unitOfWork.ConfirmBalance.AddConfirmBalanceDetailsAsync(details);
            if (affectedRow > 0)
                return ApiResponse<int>.Builder()
                    .WithMessage("Confirm balance details added successfully")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithSuccess(true)
                    .WithResult(affectedRow)
                    .Build();

            return ApiResponse<int>.Builder()
                .WithMessage("Confirm balance details added unsuccessfully")
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .WithResult(0)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }

    [HttpPut]
    [Route("details")]
    public async Task<ApiResponse<int>> UpdateConfirmBalanceDetailsAsync(
        [FromBody] UpdateConfirmBalanceDetailRequest request)
    {
        var credential = Common.DecodeJwt(HttpContext.User);
        try
        {
            var detail = new DtConfirmAccountReceivableDetail
            {
                HeaderId = request.ConfirmBalanceId,
                InvoiceCode = request.InvoiceCode,
                Balance = request.Balance,
                Description = request.Description,
                IsAgree = request.IsCustomerAgreed,
                CustomerStatus = request.IsMet,
                UpdatedBy = credential?.Username ?? string.Empty,
                UpdatedDate = DateTime.Now
            };

            var affectedRow = await unitOfWork.ConfirmBalance.UpdateConfirmBalanceDetailsAsync(detail);
            if (affectedRow > 0)
                return ApiResponse<int>.Builder()
                    .WithMessage("Confirm balance detail updated successfully")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithSuccess(true)
                    .WithResult(affectedRow)
                    .Build();

            return ApiResponse<int>.Builder()
                .WithMessage("Confirm balance detail updated unsuccessfully")
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .WithResult(0)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }
}