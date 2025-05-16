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
    public class ConfirmAccountReceivableController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public ConfirmAccountReceivableController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        [Route("getaccountreceivable")]
        public async Task<ApiResponse<List<ConfirmAccountReceivableModel.ConfirmBalance>>> GetConfirmAccountReceivableAsync()
        {
            var credential = Common.DecodeJwt(HttpContext.User);
            var confirmAccountReceivable = new ApiResponse<List<ConfirmAccountReceivableModel.ConfirmBalance>>();
            try
            {
                var execute = await _unitOfWork.ConfirmAccountReceivable.GetAsync(credential.DbCode!);
                if (execute.Count > 0)
                {
                    confirmAccountReceivable.Message = "Confirm Account Receivables fetched successfully";
                    confirmAccountReceivable.Success = true;
                    confirmAccountReceivable.StatusCode = (int)HttpStatusCode.OK;
                    confirmAccountReceivable.Result = execute;
                }
                else
                {
                    confirmAccountReceivable.Message = "Confirm Account Receivables fetched unsuccessfully";
                    confirmAccountReceivable.StatusCode = (int)HttpStatusCode.BadRequest;
                    confirmAccountReceivable.Result = new List<ConfirmAccountReceivableModel.ConfirmBalance>();
                }
            }
            catch (SqlException ex)
            {
                confirmAccountReceivable.Message = ex.Message;
                confirmAccountReceivable.StatusCode = (int)HttpStatusCode.InternalServerError;
                confirmAccountReceivable.Result = new List<ConfirmAccountReceivableModel.ConfirmBalance>();
            }
            catch (Exception ex)
            {
                confirmAccountReceivable.Message = ex.Message;
                confirmAccountReceivable.StatusCode = (int)HttpStatusCode.InternalServerError;
                confirmAccountReceivable.Result = new List<ConfirmAccountReceivableModel.ConfirmBalance>();
            }
            return confirmAccountReceivable;
        }
        [HttpPost("addnewaccountreceivable")]
        public async Task<ApiResponse<ConfirmAccountReceivableDto>> AddConfirmAccountReceivableAsync(ConfirmAccountReceivableDto model)
        {
            var credential = Common.DecodeJwt(HttpContext.User);
            var confirmAccountReceivable = new ApiResponse<ConfirmAccountReceivableDto>();
            try
            {
                var confirmAccountReceivableModel = new ConfirmAccountReceivableModel.ConfirmBalance
                {
                    DbCode = credential.DbCode,
                    ConfirmBalanceOwner = model.ConfirmBalanceOwner,
                    Participants = model.Participants,
                    Description = model.Description,
                    CreatedDate = DateTime.Now,
                    CreatedBy = credential.Username
                };
                var affectedRow = await _unitOfWork.ConfirmAccountReceivable.AddNewAsync(confirmAccountReceivableModel);
                if (affectedRow > 0)
                {
                    confirmAccountReceivable.Message = "Confirm Account Receivables added successfully";
                    confirmAccountReceivable.Success = true;
                    confirmAccountReceivable.StatusCode = (int)HttpStatusCode.OK;
                    confirmAccountReceivable.Result = model;
                }
                else
                {
                    confirmAccountReceivable.Message = "Confirm Account Receivables added unsuccessfully";
                    confirmAccountReceivable.StatusCode = (int)HttpStatusCode.BadRequest;
                }
            }
            catch (SqlException ex)
            {
                confirmAccountReceivable.Message = ex.Message;
                confirmAccountReceivable.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            catch (Exception ex)
            {
                confirmAccountReceivable.Message = ex.Message;
                confirmAccountReceivable.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            return confirmAccountReceivable;
        }
        [HttpDelete]
        [Route("deleteaccountreceivable/{id}")]
        public async Task<ApiResponse<int>> DeleteConfirmAccountReceivableAsync([Required] string id)
        {
            var credential = Common.DecodeJwt(HttpContext.User);
            var confirmAccountReceivable = new ApiResponse<int>();
            try
            {
                var affectedRow = await _unitOfWork.ConfirmAccountReceivable.DeleteAsync(id);
                if (affectedRow > 0)
                {
                    confirmAccountReceivable.Message = "Confirm Account Receivables deleted successfully";
                    confirmAccountReceivable.Success = true;
                    confirmAccountReceivable.StatusCode = (int)HttpStatusCode.OK;
                    confirmAccountReceivable.Result = int.Parse(id);
                }
                else
                {
                    confirmAccountReceivable.Message = "Confirm Account Receivables deleted unsuccessfully";
                    confirmAccountReceivable.StatusCode = (int)HttpStatusCode.BadRequest;
                }
            }
            catch (SqlException ex)
            {
                confirmAccountReceivable.Message = ex.Message;
                confirmAccountReceivable.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            catch (Exception ex)
            {
                confirmAccountReceivable.Message = ex.Message;
                confirmAccountReceivable.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            return confirmAccountReceivable;
        }
        [HttpPut("updateaccountreceivable")]
        public async Task<ApiResponse<ConfirmAccountReceivableUpdateDto>> UpdateConfirmAccountReceivableAsync(ConfirmAccountReceivableUpdateDto model)
        {
            var credential = Common.DecodeJwt(HttpContext.User);
            var confirmAccountReceivable = new ApiResponse<ConfirmAccountReceivableUpdateDto>();
            try
            {
                var confirmAccountReceivableModel = new ConfirmAccountReceivableModel.ConfirmBalance
                {
                    Id = model.Id,
                    DbCode = credential.DbCode,
                    ConfirmBalanceOwner = model.ConfirmBalanceOwner,
                    Participants = model.Participants,
                    Description = model.Description
                };
                var affectedRow = await _unitOfWork.ConfirmAccountReceivable.UpdateAsync(confirmAccountReceivableModel);
                if (affectedRow > 0)
                {
                    confirmAccountReceivable.Message = "Confirm Account Receivables updated successfully";
                    confirmAccountReceivable.Success = true;
                    confirmAccountReceivable.StatusCode = (int)HttpStatusCode.OK;
                    confirmAccountReceivable.Result = model;
                }
                else
                {
                    confirmAccountReceivable.Message = "Confirm Account Receivables updated unsuccessfully";
                    confirmAccountReceivable.StatusCode = (int)HttpStatusCode.BadRequest;
                }
            }
            catch (SqlException ex)
            {
                confirmAccountReceivable.Message = ex.Message;
                confirmAccountReceivable.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            catch (Exception ex)
            {
                confirmAccountReceivable.Message = ex.Message;
                confirmAccountReceivable.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            return confirmAccountReceivable;
        }

        [HttpGet]
        [Route("getconfirmaccountreceivabledetail/{page}/{pageSize}/{accountReceivableId}")]
        public async Task<ApiResponse<PaginatedResponse<ConfirmAccountReceivableModel.ConfirmBalanceDetails>>> GetConfirmAccountReceivableDetailsAsync([Required] int accountReceivableId, [Required] int page, [Required] int pageSize)
        {
            var credential = Common.DecodeJwt(HttpContext.User);
            var confirmAccountReceivable = new ApiResponse<PaginatedResponse<ConfirmAccountReceivableModel.ConfirmBalanceDetails>>();
            try
            {
                var execute = await _unitOfWork.ConfirmAccountReceivable.GetConfirmBalanceAccountReceivableDetails(credential.DbCode!, accountReceivableId);
                var newResponds = execute.Skip((page-1)*pageSize).Take(pageSize).ToList();
                if (execute.Count > 0)
                {
                    confirmAccountReceivable.Message = "Confirm Account Receivables fetched successfully";
                    confirmAccountReceivable.Success = true;
                    confirmAccountReceivable.StatusCode = (int)HttpStatusCode.OK;
                    confirmAccountReceivable.Result = new PaginatedResponse<ConfirmAccountReceivableModel.ConfirmBalanceDetails>(newResponds, newResponds.Count,page,pageSize);
                }
                else
                {
                    confirmAccountReceivable.Message = "Confirm Account Receivables fetched unsuccessfully";
                    confirmAccountReceivable.StatusCode = (int)HttpStatusCode.BadRequest;
                }
            }
            catch (SqlException ex)
            {
                confirmAccountReceivable.Message = ex.Message;
                confirmAccountReceivable.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            catch (Exception ex)
            {
                confirmAccountReceivable.Message = ex.Message;
                confirmAccountReceivable.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            return confirmAccountReceivable;
        }
        [HttpPut]
        [Route("updateconfirmaccountreceivabledetail")]
        public async Task<ApiResponse<ConfirmAccountReceivableDetailPutDto>> UpdateConfirmAccountReceivableDetailAsync([FromBody] ConfirmAccountReceivableDetailPutDto model)
        {
            var credential = Common.DecodeJwt(HttpContext.User);
            var confirmAccountReceivable = new ApiResponse<ConfirmAccountReceivableDetailPutDto>();
            try
            {
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
                var affectedRow = await _unitOfWork.ConfirmAccountReceivable.UpdateConfirmBalanceAccountReceivableDetails(credential.DbCode!, confirmAccountReceivableDetailModel);
                if (affectedRow > 0)
                {
                    confirmAccountReceivable.Message = "Confirm Account Receivables Detail updated successfully";
                    confirmAccountReceivable.Success = true;
                    confirmAccountReceivable.StatusCode = (int)HttpStatusCode.OK;
                    confirmAccountReceivable.Result = model;
                }
                else
                {
                    confirmAccountReceivable.Message = "Confirm Account Receivables Detail updated unsuccessfully";
                    confirmAccountReceivable.StatusCode = (int)HttpStatusCode.BadRequest;
                }
            }
            catch (SqlException ex)
            {
                confirmAccountReceivable.Message = ex.Message;
                confirmAccountReceivable.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            catch (Exception ex)
            {
                confirmAccountReceivable.Message = ex.Message;
                confirmAccountReceivable.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            return confirmAccountReceivable;
        }

        [HttpPost]
        [Route("addnewconfirmaccountreceivabledetail")]
        public async Task<ApiResponse<List<ConfirmAccountReceivableModel.ConfirmBalanceDetails>>> AddNewConfirmAccountReceivableDetailAsync(OldInvoiceRequestPostDto model)
        {
            var credential = Common.DecodeJwt(HttpContext.User);
            var confirmAccountReceivable = new ApiResponse<List<ConfirmAccountReceivableModel.ConfirmBalanceDetails>>();
            try
            {
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
                var oldInvoiceModels = await _unitOfWork.Invoices.GetAllOldInvoices(oldInvoiceParameter);
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
                var affectedRow = await _unitOfWork.ConfirmAccountReceivable.AddConfirmAccountReceivableDetails(model.AccountReceivableId, oldInvoices);
                if (affectedRow > 0)
                {
                    confirmAccountReceivable.Message = "Confirm Account Receivables Detail added successfully";
                    confirmAccountReceivable.Success = true;
                    confirmAccountReceivable.StatusCode = (int)HttpStatusCode.OK;
                    confirmAccountReceivable.Result = oldInvoices;
                }
                else
                {
                    confirmAccountReceivable.Message = "Confirm Account Receivables Detail added unsuccessfully";
                    confirmAccountReceivable.StatusCode = (int)HttpStatusCode.BadRequest;
                }
            }
            catch (SqlException ex)
            {
                confirmAccountReceivable.Message = ex.Message;
                confirmAccountReceivable.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            catch (Exception ex)
            {
                confirmAccountReceivable.Message = ex.Message;
                confirmAccountReceivable.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            return confirmAccountReceivable;
        }



    }
}
