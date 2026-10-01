using System.Net;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Mapper;
using BC.PAYMENT.API.MapperHelper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.Prepare.Account;
using BC.PAYMENT.CORE.Entities.Prepare.Account;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace BC.PAYMENT.API.Controllers.Setting.Account;

public class AccountReceivablePresetController(
    IUnitOfWork unitOfWork,
    IValidator<AccountReceivableCreateRequest> validator) : BaseApiController
{
    [HttpPost]
    [Route("")]
    public async Task<ApiResponse<AccountReceivableCreateRequest>> AddNewAccountReceivablePresetAsync(
        [FromBody] AccountReceivableCreateRequest model)
    {
        var claim = Common.DecodeJwt(HttpContext.User);
        try
        {
            var validatorPreset = await validator.ValidateAsync(model);
            if (!validatorPreset.IsValid)
                return GlobalExceptionHandler.ValidateSchemaError<AccountReceivableCreateRequest>(
                    validatorPreset.Errors.toProperties(), "Some fields is required!");

            var result = await unitOfWork.AccountReceivablePresets.AddNewAsync(model.FromCreateDtoToModel(claim));
            return ApiResponse<AccountReceivableCreateRequest>.Builder()
                .WithSuccess(result > 0)
                .WithMessage(result > 0
                    ? "Account Receivable Preset added successfully"
                    : "Account Receivable Preset added unsuccessfully").WithStatusCode(result > 0
                    ? (int)HttpStatusCode.Created
                    : (int)HttpStatusCode.BadRequest).WithResult(model).Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<AccountReceivableCreateRequest>(ex.Message);
        }
    }

    [HttpGet]
    [Route("")]
    public async Task<ApiResponse<List<AccountReceivablePresetModel>>> GetAccountReceivablePresetAsync()
    {
        var claim = Common.DecodeJwt(HttpContext.User);
        try
        {
            var execute = await unitOfWork.AccountReceivablePresets.GetAsync(claim.DbCode!);
            return ApiResponse<List<AccountReceivablePresetModel>>.Builder()
                .WithSuccess(execute.Count != 0)
                .WithMessage(execute.Count != 0
                    ? "Account Receivable Preset fetched successfully"
                    : "Account Receivable Preset fetched unsuccessfully")
                .WithStatusCode(StatusCodes.Status200OK).WithResult(execute).Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<AccountReceivablePresetModel>>(ex.Message);
        }
    }

    [HttpDelete]
    [Route("")]
    public async Task<ApiResponse<AccountReceivableDeleteRequest>> DeleteAccountReceivablePresetAsync(
        [FromBody] AccountReceivableDeleteRequest model)
    {
        var claim = Common.DecodeJwt(HttpContext.User);
        try
        {
            var accountReceivableModel = new AccountReceivablePresetModel
            {
                DbCode = claim.DbCode,
                CreditDebitType = model.CreditDebitType,
                AccountCode = model.AccountCode
            };
            var affectedRow =
                await unitOfWork.AccountReceivablePresets.DeleteAccountReceivableAsync(accountReceivableModel);
            return ApiResponse<AccountReceivableDeleteRequest>.Builder()
                .WithSuccess(affectedRow > 0)
                .WithMessage(affectedRow > 0
                    ? "Account Receivable Preset deleted successfully"
                    : "Account Receivable Preset deleted unsuccessfully").WithStatusCode(affectedRow > 0
                    ? (int)HttpStatusCode.Created
                    : (int)HttpStatusCode.BadRequest).WithResult(model).Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<AccountReceivableDeleteRequest>(ex.Message);
        }
    }
}