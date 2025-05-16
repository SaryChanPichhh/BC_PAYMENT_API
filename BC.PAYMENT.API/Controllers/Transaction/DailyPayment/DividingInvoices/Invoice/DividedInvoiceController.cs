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
    public class DividedInvoiceController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public DividedInvoiceController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }

        [HttpGet]
        [Route("{deliverId}/{date}")]
        public async Task<ApiResponse<List<DividedInvoiceModel>>> GetDividedInvoiceByDeliverIdAndDate([Required] string deliverId, [Required] DateTime date)
        {
            var dividedInvoice = new ApiResponse<List<DividedInvoiceModel>>();
            var credential = Common.DecodeJwt(User);

            try
            {
                var execute =
                    await _unitOfWork.DividedInvoice.GetDividedInvoicesByDeliveryIdAndDateAsync(credential.DbCode!,
                        deliverId, date);
                if (execute.Any())
                {
                    dividedInvoice.StatusCode = (int)HttpStatusCode.OK;
                    dividedInvoice.Message = $@"Divided Invoice fetched successfully";
                    dividedInvoice.Success = true;
                    dividedInvoice.Result = execute;
                }
                else
                {
                    dividedInvoice.StatusCode = (int)HttpStatusCode.BadRequest;
                    dividedInvoice.Message = $@"Divided Invoice fetched unsuccessfully";
                    dividedInvoice.Result = new();
                }
            }
            catch (SqlException ex)
            {
                dividedInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                dividedInvoice.Message = $@"Sql Exception : {ex.Message}";
            }
            catch (Exception ex)
            {
                dividedInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                dividedInvoice.Message = $@"Error Exception : {ex.Message}";
            }
            return dividedInvoice;
        }


        [HttpDelete]
        [Route("")]
        public async Task<ApiResponse<DividedInvoiceDeleteDto>> DeleteDividedInvoiceByDeliverIdAndDate([FromBody] DividedInvoiceDeleteDto model)
        {
            var dividedInvoice = new ApiResponse<DividedInvoiceDeleteDto>();
            try
            {
                var affectedRow =
                    await _unitOfWork.DividedInvoice.DeleteDividedInvoiceAsync(model.InvoiceId, model.TransactionCode,
                        model.DeliveryId, model.Note);
                if (affectedRow > 0)
                {
                    dividedInvoice.StatusCode = (int)HttpStatusCode.OK;
                    dividedInvoice.Message = $@"Divided Invoice deleted successfully";
                    dividedInvoice.Success = true;
                    dividedInvoice.Result = model;
                }
                else
                {
                    dividedInvoice.StatusCode = (int)HttpStatusCode.BadRequest;
                    dividedInvoice.Message = $@"Divided Invoice deleted unsuccessfully";
                    dividedInvoice.Result = new();
                }
            }
            catch (SqlException ex)
            {
                dividedInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                dividedInvoice.Message = $@"Sql Exception : {ex.Message}";
            }
            catch (Exception ex)
            {
                dividedInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                dividedInvoice.Message = $@"Error Exception : {ex.Message}";
            }
            return dividedInvoice;
        }
    }
}
