using System.ComponentModel.DataAnnotations;
using System.Net;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.Transaction.DailyPayment.DividingInvoices.Invoice;
using BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DividingInvoices.Invoice;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BC.PAYMENT.API.Controllers.Transaction.DailyPayment.DividingInvoices.Invoice
{
    public class DividedInvoiceController(IUnitOfWork unitOfWork) : BaseApiController
    {
        [HttpGet]
        [Route("{deliverId}/{date}")]
        public async Task<ApiResponse<List<DividedInvoiceModel>>> GetDividedInvoiceByDeliverIdAndDate([Required] string deliverId, [Required] DateTime date)
        {
            var credential = Common.DecodeJwt(User);

            try
            {
                var execute =
                    await unitOfWork.DividedInvoice.GetDividedInvoicesByDeliveryIdAndDateAsync(credential.DbCode!,
                        deliverId, date);
                if (execute.Any())
                {
                    return ApiResponse<List<DividedInvoiceModel>>.Builder()
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithMessage($@"Divided Invoice fetched successfully")
                        .Success()
                        .WithResult(execute)
                        .Build();
                }
                else
                {
                    return ApiResponse<List<DividedInvoiceModel>>.Builder()
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithMessage($@"Divided Invoice fetched unsuccessfully")
                        .WithResult(new())
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<DividedInvoiceModel>>(ex.Message);
            }
        }


        [HttpDelete]
        [Route("")]
        public async Task<ApiResponse<DividedInvoiceDeleteDto>> DeleteDividedInvoiceByDeliverIdAndDate([FromBody] DividedInvoiceDeleteDto model)
        {
            try
            {
                var affectedRow =
                    await unitOfWork.DividedInvoice.DeleteDividedInvoiceAsync(model.InvoiceId, model.TransactionCode,
                        model.DeliveryId, model.Note);
                if (affectedRow > 0)
                {
                    return ApiResponse<DividedInvoiceDeleteDto>.Builder()
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithMessage($@"Divided Invoice deleted successfully")
                        .Success()
                        .WithResult(model)
                        .Build();
                }
                else
                {
                    return ApiResponse<DividedInvoiceDeleteDto>.Builder()
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithMessage($@"Divided Invoice deleted unsuccessfully")
                        .WithResult(new())
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<DividedInvoiceDeleteDto>(ex.Message);
            }
        }
    }
}
