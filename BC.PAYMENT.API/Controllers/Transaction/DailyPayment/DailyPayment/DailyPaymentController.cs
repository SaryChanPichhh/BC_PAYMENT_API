using System.ComponentModel.DataAnnotations;
using System.Net;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.Filter;
using BC.PAYMENT.CORE.DTO.Transaction.DailyPayment.DailyPayment;
using BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DailyPayment;
using BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DeliveryPaid;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BC.PAYMENT.API.Controllers.Transaction.DailyPayment.DailyPayment
{
    public class DailyPaymentController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public DailyPaymentController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        [Route("getpaidinvoicebyperiod")]
        public async Task<ApiResponse<PaginatedResponse<PaidInvoiceModel>>> GetPaidInvoiceByPeriod([FromBody] ByPeriodDto model)
        {
            var credential = Common.DecodeJwt(User);
            var paidInvoice = new ApiResponse<PaginatedResponse<PaidInvoiceModel>>();
            try
            {
                var paidInvoices =
                    await _unitOfWork.DailyPayment.GetPaidInvoiceByPeriodAsync(credential.DbCode!, model.Month, model.Year);
                var newPaidInvoicesResponds = paidInvoices.Skip((model.Page - 1) * model.PageSize).Take(model.PageSize).ToList();
                if (newPaidInvoicesResponds.Any())
                {
                    paidInvoice.StatusCode = (int)HttpStatusCode.OK;
                    paidInvoice.Success = true;
                    paidInvoice.Message = "Paid invoices fetch successfully";
                    paidInvoice.Result = new PaginatedResponse<PaidInvoiceModel>(newPaidInvoicesResponds, paidInvoices.Count,model.Page,model.PageSize) ;
                }
                else
                {
                    paidInvoice.StatusCode = (int)HttpStatusCode.BadRequest;
                    paidInvoice.Success = false;
                    paidInvoice.Message = "Paid invoices fetch unsuccessfully";
                }
            }
            catch (SqlException ex)
            {
                paidInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                paidInvoice.Success = false;
                paidInvoice.Message = $@"Sql Exception : {ex.Message}";
            }
            catch (Exception ex)
            {
                paidInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                paidInvoice.Success = false;
                paidInvoice.Message = $@"Error Exception : {ex.Message}";
            }
            return paidInvoice;
        }
        [HttpPost]
        [Route("getpaidinvoicebydate")]
        public async Task<ApiResponse<PaginatedResponse<PaidInvoiceModel>>> GetPaidInvoiceByDateAsync([FromBody] ByDateDto model)
        {
            var credential = Common.DecodeJwt(User);
            var paidInvoice = new ApiResponse<PaginatedResponse<PaidInvoiceModel>>();
            try
            {
                var paidInvoices =
                    await _unitOfWork.DailyPayment.GetPaidInvoiceByDateAsync(credential.DbCode!, model.FromDate, model.ToDate);
                var newPaidInvoiceResponds =
                    paidInvoices.Skip(model.Page - 1 * model.PageSize).Take(model.PageSize).ToList();
                if (paidInvoices.Any())
                {
                    paidInvoice.StatusCode = (int)HttpStatusCode.OK;
                    paidInvoice.Success = true;
                    paidInvoice.Message = "Paid invoices fetch successfully";
                    paidInvoice.Result = new PaginatedResponse<PaidInvoiceModel>(newPaidInvoiceResponds, newPaidInvoiceResponds.Count,model.Page,model.PageSize);
                }
                else
                {
                    paidInvoice.StatusCode = (int)HttpStatusCode.BadRequest;
                    paidInvoice.Success = false;
                    paidInvoice.Message = "Paid invoices fetch unsuccessfully";
                }
            }
            catch (SqlException ex)
            {
                paidInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                paidInvoice.Success = false;
                paidInvoice.Message = $@"Sql Exception : {ex.Message}";
            }
            catch (Exception ex)
            {
                paidInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                paidInvoice.Success = false;
                paidInvoice.Message = $@"Error Exception : {ex.Message}";
            }

            return paidInvoice;
        }
        
        [HttpDelete]
        [Route("deletepaidinvoice/{paymentId}/{dividedId}")]
        public async Task<ApiResponse<int>> DeletePaidInvoiceAsync([Required] int paymentId, [Required] int dividedId)
        {
            var credential = Common.DecodeJwt(User);
            var paidInvoice = new ApiResponse<int>();
            try
            {
                var affectedRow =
                    await _unitOfWork.DailyPayment.DeletePaidInvoiceAsync(paymentId,dividedId);
                if (affectedRow > 0 )
                {
                    paidInvoice.StatusCode = (int)HttpStatusCode.OK;
                    paidInvoice.Success = true;
                    paidInvoice.Message = "Paid invoices deleted successfully";
                    paidInvoice.Result = affectedRow;
                }
                else
                {
                    paidInvoice.StatusCode = (int)HttpStatusCode.BadRequest;
                    paidInvoice.Success = false;
                    paidInvoice.Message = "Paid invoices deleted unsuccessfully";
                }
            }
            catch (SqlException ex)
            {
                paidInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                paidInvoice.Success = false;
                paidInvoice.Message = $@"Sql Exception : {ex.Message}";
            }
            catch (Exception ex)
            {
                paidInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                paidInvoice.Success = false;
                paidInvoice.Message = $@"Error Exception : {ex.Message}";
            }

            return paidInvoice;
        } 
        
        [HttpPut]
        [Route("updatepaidinvoice")]
        public async Task<ApiResponse<int>> UpdatePaidInvoiceAsync([FromBody] DailyPaymentUpdateDto model)
        {
            var credential = Common.DecodeJwt(User);
            var paidInvoice = new ApiResponse<int>();
            try
            {
                var paidInvoiceModel = new DeliveryInvoicePaidUpdateModel()
                {
                    DbCode = credential.DbCode,
                    DividedInvoiceId = model.DividedInvoiceId,
                    CreateBy = credential.Username,
                    OldAmount = model.OldAmount,
                    NewAmount = model.NewAmount,
                    Description = model.Description,
                    PaymentId = model.PaymentId,
                    OldPaidAmount = model.OldPaidAmount,
                    NewPaidAmount = model.NewPaidAmount,

                };
                var affectedRow =
                    await _unitOfWork.DailyPayment.UpdatePaidInvoiceAsync(paidInvoiceModel);
                if (affectedRow > 0 )
                {
                    paidInvoice.StatusCode = (int)HttpStatusCode.OK;
                    paidInvoice.Success = true;
                    paidInvoice.Message = "Paid invoices updated successfully";
                    paidInvoice.Result = affectedRow;
                }
                else
                {
                    paidInvoice.StatusCode = (int)HttpStatusCode.BadRequest;
                    paidInvoice.Success = false;
                    paidInvoice.Message = "Paid invoices updated unsuccessfully";
                }
            }
            catch (SqlException ex)
            {
                paidInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                paidInvoice.Success = false;
                paidInvoice.Message = $@"Sql Exception : {ex.Message}";
            }
            catch (Exception ex)
            {
                paidInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                paidInvoice.Success = false;
                paidInvoice.Message = $@"Error Exception : {ex.Message}";
            }
            return paidInvoice;
        }
    }
}
