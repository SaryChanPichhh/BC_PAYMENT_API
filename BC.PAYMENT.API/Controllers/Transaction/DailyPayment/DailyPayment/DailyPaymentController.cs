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
    public class DailyPaymentController(IUnitOfWork unitOfWork) : BaseApiController
    {
        [HttpPost]
        [Route("getpaidinvoicebyperiod")]
        public async Task<ApiResponse<PaginatedResponse<PaidInvoiceModel>>> GetPaidInvoiceByPeriod([FromBody] ByPeriodDto model)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var paidInvoices = await unitOfWork.DailyPayment.GetPaidInvoiceByPeriodAsync(credential.DbCode!, model.Month, model.Year);
                var newPaidInvoicesResponds = paidInvoices.Skip((model.Page - 1) * model.PageSize).Take(model.PageSize).ToList();
                if (newPaidInvoicesResponds.Any())
                {
                    return ApiResponse<PaginatedResponse<PaidInvoiceModel>>.Builder()
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithMessage("Paid invoices fetch successfully")
                        .WithResult(new PaginatedResponse<PaidInvoiceModel>(newPaidInvoicesResponds, paidInvoices.Count, model.Page, model.PageSize))
                        .Build();
                }
                else
                {
                    return ApiResponse<PaginatedResponse<PaidInvoiceModel>>.Builder()
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithMessage("Paid invoices fetch unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<PaginatedResponse<PaidInvoiceModel>>(ex.Message);
            }
        }

        [HttpPost]
        [Route("getpaidinvoicebydate")]
        public async Task<ApiResponse<PaginatedResponse<PaidInvoiceModel>>> GetPaidInvoiceByDateAsync([FromBody] ByDateDto model)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var paidInvoices = await unitOfWork.DailyPayment.GetPaidInvoiceByDateAsync(credential.DbCode!, model.FromDate, model.ToDate);
                var newPaidInvoiceResponds = paidInvoices.Skip(model.Page - 1 * model.PageSize).Take(model.PageSize).ToList();
                if (paidInvoices.Any())
                {
                    return ApiResponse<PaginatedResponse<PaidInvoiceModel>>.Builder()
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithMessage("Paid invoices fetch successfully")
                        .WithResult(new PaginatedResponse<PaidInvoiceModel>(newPaidInvoiceResponds, newPaidInvoiceResponds.Count, model.Page, model.PageSize))
                        .Build();
                }
                else
                {
                    return ApiResponse<PaginatedResponse<PaidInvoiceModel>>.Builder()
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithMessage("Paid invoices fetch unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<PaginatedResponse<PaidInvoiceModel>>(ex.Message);
            }
        }
        
        [HttpDelete]
        [Route("deletepaidinvoice/{paymentId}/{dividedId}")]
        public async Task<ApiResponse<int>> DeletePaidInvoiceAsync([Required] int paymentId, [Required] int dividedId)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var affectedRow = await unitOfWork.DailyPayment.DeletePaidInvoiceAsync(paymentId, dividedId);
                if (affectedRow > 0)
                {
                    return ApiResponse<int>.Builder()
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithMessage("Paid invoices deleted successfully")
                        .WithResult(affectedRow)
                        .Build();
                }
                else
                {
                    return ApiResponse<int>.Builder()
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithMessage("Paid invoices deleted unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
            }
        } 
        
        [HttpPut]
        [Route("updatepaidinvoice")]
        public async Task<ApiResponse<int>> UpdatePaidInvoiceAsync([FromBody] DailyPaymentUpdateDto model)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var paidInvoiceModel = new DeliveryGeneralInvoicePaidUpdateModel()
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
                var affectedRow = await unitOfWork.DailyPayment.UpdatePaidInvoiceAsync(paidInvoiceModel);
                if (affectedRow > 0)
                {
                    return ApiResponse<int>.Builder()
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithMessage("Paid invoices updated successfully")
                        .WithResult(affectedRow)
                        .Build();
                }
                else
                {
                    return ApiResponse<int>.Builder()
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithMessage("Paid invoices updated unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
            }
        }
    }
}
