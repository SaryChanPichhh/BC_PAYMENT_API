using System.Net;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.Filter;
using BC.PAYMENT.CORE.DTO.Transaction.DailyPayment.DailyPayment;
using BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DailyPayment;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BC.PAYMENT.API.Controllers.Transaction.Submitting.SubmittingInvoice;

public class SubmittingPerDeliveryController(IUnitOfWork unitOfWork) : BaseApiController
{
    [HttpPost]
    [Route("getallsubmittingperdelivery")]
    public async Task<ApiResponse<PaginatedResponse<ExpenseDetailModel>>> GetAllSubmittedInvoices(
        [FromBody] ByDateDto model)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var submittedInvoices =
                await unitOfWork.SubmittingPerDelivery.GetSubmittedInvoicesAsync(credential.DbCode!, model.FromDate,
                    model.ToDate);
            var newResponds = submittedInvoices.Skip(model.Page - 1 * model.PageSize).Take(model.PageSize).ToList();
            if (submittedInvoices.Any())
            {
                var result = new PaginatedResponse<ExpenseDetailModel>(newResponds, submittedInvoices.Count, model.Page,
                    model.PageSize);
                return ApiResponse<PaginatedResponse<ExpenseDetailModel>>.Builder()
                    .WithMessage("Submitted Invoices fetched successfully")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(result)
                    .Build();
            }
            else
            {
                return ApiResponse<PaginatedResponse<ExpenseDetailModel>>.Builder()
                    .WithMessage("Submitted Invoices fetched unsuccessfully")
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .Build();
            }
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<PaginatedResponse<ExpenseDetailModel>>(ex.Message);
        }
    }

    [HttpPost]
    [Route("addnewsubmittedinvoice")]
    public async Task<ApiResponse<ExpenseDetailPostDto>> AddNewSubmittedInvoiceAynsc(
        [FromBody] ExpenseDetailPostDto model)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var submittedInvoicesModel = new ExpenseDetailModel
            {
                Id = model.Id,
                Dollar = model.Dollar,
                Riel = model.Riel,
                ExchangeRate = model.Exchange,
                Total = model.Total,
                ExpenseRiel = model.ExpenseRiel,
                Misaligned = model.Misaligned,
                ExpenseDollar = model.ExpenseDollar,
                CreateBy = credential.Username!,
                CreateDate = DateTime.Now,
                DbCode = credential.DbCode!
            };
            var affectedRow =
                await unitOfWork.SubmittingPerDelivery.AddNewSubmittedInvoicesAsync(submittedInvoicesModel);
            if (affectedRow > 0)
                return ApiResponse<ExpenseDetailPostDto>.Builder()
                    .WithMessage("Submitted Invoices added successfully")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(model)
                    .Build();
            else
                return ApiResponse<ExpenseDetailPostDto>.Builder()
                    .WithMessage("Submitted Invoices added unsuccessfully")
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<ExpenseDetailPostDto>(ex.Message);
        }
    }
}