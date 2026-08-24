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

namespace BC.PAYMENT.API.Controllers.Transaction.DailyPayment.AccountReceivable
{
    public class ConfirmAccountReceivableController(IUnitOfWork unitOfWork) : BaseApiController
    {
        [HttpGet]
        [HttpGet]
        [Route("getaccountreceivable")]
        public async Task<ApiResponse<List<ConfirmAccountReceivableModel.ConfirmBalance>>> GetConfirmAccountReceivableAsync()
        {
            try
            {
                var credential = Common.DecodeJwt(HttpContext.User);
                var execute = await unitOfWork.ConfirmAccountReceivable.GetAsync(credential.DbCode!);
                
                return ApiResponse<List<ConfirmAccountReceivableModel.ConfirmBalance>>.Builder()
                    .WithMessage(execute.Count > 0 ? "Confirm Account Receivables fetched successfully" : "Confirm Account Receivables fetched unsuccessfully")
                    .WithStatusCode(execute.Count > 0 ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest)
                    .WithResult(execute.Count > 0 ? execute : new List<ConfirmAccountReceivableModel.ConfirmBalance>())
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<ConfirmAccountReceivableModel.ConfirmBalance>>(ex.Message);
            }
        }
        [HttpPost("addnewaccountreceivable")]
        public async Task<ApiResponse<ConfirmAccountReceivableDto>> AddConfirmAccountReceivableAsync(ConfirmAccountReceivableDto model)
        {
            try
            {
                var credential = Common.DecodeJwt(HttpContext.User);
                var confirmAccountReceivableModel = new ConfirmAccountReceivableModel.ConfirmBalance
                {
                    DbCode = credential.DbCode,
                    ConfirmBalanceOwner = model.ConfirmBalanceOwner,
                    Participants = model.Participants,
                    Description = model.Description,
                    CreatedDate = DateTime.Now,
                    CreatedBy = credential.Username
                };
                var affectedRow = await unitOfWork.ConfirmAccountReceivable.AddNewAsync(confirmAccountReceivableModel);
                
                return ApiResponse<ConfirmAccountReceivableDto>.Builder()
                    .WithMessage(affectedRow > 0 ? "Confirm Account Receivables added successfully" : "Confirm Account Receivables added unsuccessfully")
                    .WithStatusCode(affectedRow > 0 ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest)
                    .WithResult(affectedRow > 0 ? model : null)
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<ConfirmAccountReceivableDto>(ex.Message);
            }
        }
        [HttpDelete]
        [Route("deleteaccountreceivable/{id}")]
        public async Task<ApiResponse<int>> DeleteConfirmAccountReceivableAsync([Required] string id)
        {
            try
            {
                var credential = Common.DecodeJwt(HttpContext.User);
                var affectedRow = await unitOfWork.ConfirmAccountReceivable.DeleteAsync(id);
                
                return ApiResponse<int>.Builder()
                    .WithMessage(affectedRow > 0 ? "Confirm Account Receivables deleted successfully" : "Confirm Account Receivables deleted unsuccessfully")
                    .WithStatusCode(affectedRow > 0 ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest)
                    .WithResult(affectedRow > 0 ? int.Parse(id) : 0)
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
            }
        }
        [HttpPut("updateaccountreceivable")]
        public async Task<ApiResponse<ConfirmAccountReceivableUpdateDto>> UpdateConfirmAccountReceivableAsync(ConfirmAccountReceivableUpdateDto model)
        {
            try
            {
                var credential = Common.DecodeJwt(HttpContext.User);
                var confirmAccountReceivableModel = new ConfirmAccountReceivableModel.ConfirmBalance
                {
                    Id = model.Id,
                    DbCode = credential.DbCode,
                    ConfirmBalanceOwner = model.ConfirmBalanceOwner,
                    Participants = model.Participants,
                    Description = model.Description
                };
                var affectedRow = await unitOfWork.ConfirmAccountReceivable.UpdateAsync(confirmAccountReceivableModel);
                
                return ApiResponse<ConfirmAccountReceivableUpdateDto>.Builder()
                    .WithMessage(affectedRow > 0 ? "Confirm Account Receivables updated successfully" : "Confirm Account Receivables updated unsuccessfully")
                    .WithStatusCode(affectedRow > 0 ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest)
                    .WithResult(affectedRow > 0 ? model : null)
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<ConfirmAccountReceivableUpdateDto>(ex.Message);
            }
        }

        [HttpGet]
        [Route("getconfirmaccountreceivabledetail/{page}/{pageSize}/{accountReceivableId}")]
        public async Task<ApiResponse<PaginatedResponse<ConfirmAccountReceivableModel.ConfirmBalanceDetails>>> GetConfirmAccountReceivableDetailsAsync([Required] int accountReceivableId, [Required] int page, [Required] int pageSize)
        {
            try
            {
                var credential = Common.DecodeJwt(HttpContext.User);
                var execute = await unitOfWork.ConfirmAccountReceivable.GetConfirmBalanceAccountReceivableDetails(credential.DbCode!, accountReceivableId);
                var newResponds = execute.Skip((page-1)*pageSize).Take(pageSize).ToList();
                
                return ApiResponse<PaginatedResponse<ConfirmAccountReceivableModel.ConfirmBalanceDetails>>.Builder()
                    .WithMessage(execute.Count > 0 ? "Confirm Account Receivables fetched successfully" : "Confirm Account Receivables fetched unsuccessfully")
                    .WithStatusCode(execute.Count > 0 ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest)
                    .WithResult(execute.Count > 0 ? new PaginatedResponse<ConfirmAccountReceivableModel.ConfirmBalanceDetails>(newResponds, newResponds.Count,page,pageSize) : null)
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<PaginatedResponse<ConfirmAccountReceivableModel.ConfirmBalanceDetails>>(ex.Message);
            }
        }
        [HttpPut]
        [Route("updateconfirmaccountreceivabledetail")]
        public async Task<ApiResponse<ConfirmAccountReceivableDetailPutDto>> UpdateConfirmAccountReceivableDetailAsync([FromBody] ConfirmAccountReceivableDetailPutDto model)
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
                    IsCustomerAgreed = model.IsCustomerAgreed,
                };
                var affectedRow = await unitOfWork.ConfirmAccountReceivable.UpdateConfirmBalanceAccountReceivableDetails(credential.DbCode!, confirmAccountReceivableDetailModel);
                
                return ApiResponse<ConfirmAccountReceivableDetailPutDto>.Builder()
                    .WithMessage(affectedRow > 0 ? "Confirm Account Receivables Detail updated successfully" : "Confirm Account Receivables Detail updated unsuccessfully")
                    .WithStatusCode(affectedRow > 0 ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest)
                    .WithResult(affectedRow > 0 ? model : null)
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<ConfirmAccountReceivableDetailPutDto>(ex.Message);
            }
        }

        [HttpPost]
        [Route("addnewconfirmaccountreceivabledetail")]
        public async Task<ApiResponse<List<ConfirmAccountReceivableModel.ConfirmBalanceDetails>>> AddNewConfirmAccountReceivableDetailAsync(OldInvoiceRequestPostDto model)
        {
            try
            {
                var credential = Common.DecodeJwt(HttpContext.User);
                var oldInvoiceParameter = new OldInvoiceRequestDto
                {
                    DbCode = credential.DbCode,
                    Date = model.Date,
                    FromAccount = model.FromAccount,
                    ToAccount = model.ToAccount,
                    FromAnal = model.FromAnal,
                    ToAnal = model.ToAnal,
                    T0 = model.T0,
                    T1 = model.T1,
                    T2 = model.T2,
                    T3 = model.T3,
                    T4 = model.T4,
                    T5 = model.T5,
                    T6 = model.T6,
                    T7 = model.T7,
                    T8 = model.T8,
                    T9 = model.T9,

                };
                var oldInvoiceModels = await unitOfWork.Invoices.GetAllOldInvoices(oldInvoiceParameter);
                var oldInvoices = oldInvoiceModels.Select(oldInvoiceModel => new ConfirmAccountReceivableModel.ConfirmBalanceDetails
                {
                    CustomerCode = oldInvoiceModel.CustomerCode,
                    CustomerName = oldInvoiceModel.CustomerName,
                    InvoicedCode = oldInvoiceModel.TransRef,
                    InvoicedAmount = oldInvoiceModel.InvoiceValue,
                    DbCode = credential.DbCode,
                    CreatedBy = credential.Username,
                })
                    .ToList();
                var affectedRow = await unitOfWork.ConfirmAccountReceivable.AddConfirmAccountReceivableDetails(model.AccountReceivableId, oldInvoices);
                
                return ApiResponse<List<ConfirmAccountReceivableModel.ConfirmBalanceDetails>>.Builder()
                    .WithMessage(affectedRow > 0 ? "Confirm Account Receivables Detail added successfully" : "Confirm Account Receivables Detail added unsuccessfully")
                    .WithStatusCode(affectedRow > 0 ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest)
                    .WithResult(affectedRow > 0 ? oldInvoices : null)
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<ConfirmAccountReceivableModel.ConfirmBalanceDetails>>(ex.Message);
            }
        }



    }
}
