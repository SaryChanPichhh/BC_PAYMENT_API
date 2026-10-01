using System.ComponentModel.DataAnnotations;
using System.Net;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.Invoice;
using BC.PAYMENT.CORE.DTO.Transaction.DailyPayment.AccountReceivable;
using BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.AccountReceivable;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BC.PAYMENT.API.Controllers.Transaction.DailyPayment.AccountReceivable;

public class ConfirmAccountReceivableController(IUnitOfWork unitOfWork) : BaseApiController
{
    [HttpGet]
    [Route("getconfirmaccountreceivabledetail/{page}/{pageSize}/{accountReceivableId}")]
    public async Task<ApiResponse<PaginatedResponse<ConfirmAccountReceivableModel.ConfirmBalanceDetails>>>
        GetConfirmAccountReceivableDetailsAsync([Required] int accountReceivableId, [Required] int page,
            [Required] int pageSize)
    {
        try
        {
            var credential = Common.DecodeJwt(HttpContext.User);
            var execute =
                await unitOfWork.ConfirmAccountReceivable.GetConfirmBalanceAccountReceivableDetails(credential.DbCode!,
                    accountReceivableId);
            var newResponds = execute.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return ApiResponse<PaginatedResponse<ConfirmAccountReceivableModel.ConfirmBalanceDetails>>.Builder()
                .WithMessage(execute.Count > 0
                    ? "Confirm Account Receivables fetched successfully"
                    : "Confirm Account Receivables fetched unsuccessfully")
                .WithStatusCode(execute.Count > 0 ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest)
                .WithResult(execute.Count > 0
                    ? new PaginatedResponse<ConfirmAccountReceivableModel.ConfirmBalanceDetails>(newResponds,
                        newResponds.Count, page, pageSize)
                    : null)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler
                .ExceptionError<PaginatedResponse<ConfirmAccountReceivableModel.ConfirmBalanceDetails>>(ex.Message);
        }
    }

    [HttpPut]
    [Route("updateconfirmaccountreceivabledetail")]
    public async Task<ApiResponse<ConfirmAccountReceivableDetailPutDto>> UpdateConfirmAccountReceivableDetailAsync(
        [FromBody] ConfirmAccountReceivableDetailPutDto model)
    {
        try
        {
            var credential = Common.DecodeJwt(HttpContext.User);
            var confirmAccountReceivableDetailModel = new ConfirmAccountReceivableModel.ConfirmBalanceDetails
            {
                ConfirmBalanceId = model.ConfirmBalanceId,
                InvoicedCode = model.InvoicedCode,
                UpdatedBy = credential.Username,
                Balance = model.Balance,
                IsMet = model.IsMet,
                Description = model.Description,
                IsCustomerAgreed = model.IsCustomerAgreed
            };
            var affectedRow =
                await unitOfWork.ConfirmAccountReceivable.UpdateConfirmBalanceAccountReceivableDetails(
                    credential.DbCode!, confirmAccountReceivableDetailModel);

            return ApiResponse<ConfirmAccountReceivableDetailPutDto>.Builder()
                .WithMessage(affectedRow > 0
                    ? "Confirm Account Receivables Detail updated successfully"
                    : "Confirm Account Receivables Detail updated unsuccessfully")
                .WithStatusCode(affectedRow > 0 ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest)
                .WithResult(affectedRow > 0 ? model : null)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<ConfirmAccountReceivableDetailPutDto>(ex.Message);
        }
    }
}