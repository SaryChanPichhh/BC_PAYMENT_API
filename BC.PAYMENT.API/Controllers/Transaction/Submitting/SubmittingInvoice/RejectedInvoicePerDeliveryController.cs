
using System.ComponentModel.DataAnnotations;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.Entities.Transaction.Submitting.SubmittingInvoice;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;


namespace BC.PAYMENT.API.Controllers.Transaction.Submitting.SubmittingInvoice
{

    public class RejectedInvoicePerDeliveryController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public RejectedInvoicePerDeliveryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("getrejectedinvoiceperdelivery/{fromDate}/{toDate}")]
        public async Task<ApiResponse<List<RejectedInvoicePerDelivery>>> GetRejectedInvoicePerDeliveryByDateAsync(string fromDate, string toDate)
        {
            var credential = Common.DecodeJwt(User);
            var rejectedInvoice = new ApiResponse<List<RejectedInvoicePerDelivery>>();
            try
            {
                var rejectedInvoices = await _unitOfWork.RejectedInvoicePerDelivery.GetAllRejectedInvoicePerDeliveryByDateAsync(credential.DbCode!, fromDate, toDate);
                if (rejectedInvoices.Any())
                {
                    rejectedInvoice.Result = rejectedInvoices;
                    rejectedInvoice.StatusCode = StatusCodes.Status200OK;
                    rejectedInvoice.Message = "Rejected Invoices fetched successfully";
                    rejectedInvoice.Success = true;
                }
                else
                {
                    rejectedInvoice.StatusCode = StatusCodes.Status400BadRequest;
                    rejectedInvoice.Message = "Rejected Invoices fetched unsuccessfully";
                    rejectedInvoice.Success = false;
                }

            }
            catch (SqlException e)
            {
                rejectedInvoice.StatusCode = StatusCodes.Status400BadRequest;
                rejectedInvoice.Message = $"Sql Exception : {e.Message}";
            }
            catch (Exception e)
            {
                rejectedInvoice.StatusCode = StatusCodes.Status400BadRequest;
                rejectedInvoice.Message = $"Error Exception : {e.Message}";
            }
            return rejectedInvoice;
        }
        [HttpGet]
        [Route("getrejectedinvoiceperdelivery/{year}/{month}")]
        public async Task<ApiResponse<List<RejectedInvoicePerDelivery>>> GetRejectedInvoicePerDeliveryByPeriodAsync([Required]int year, [Required] int month)
        {
            var credential = Common.DecodeJwt(User);
            var rejectedInvoice = new ApiResponse<List<RejectedInvoicePerDelivery>>();
            try
            {
                var rejectedInvoices = await _unitOfWork.RejectedInvoicePerDelivery.GetAllRejectedInvoicePerDeliveryByPeriodAsync(credential.DbCode!, month, year);
                if (rejectedInvoices.Any())
                {
                    rejectedInvoice.Result = rejectedInvoices;
                    rejectedInvoice.StatusCode = StatusCodes.Status200OK;
                    rejectedInvoice.Message = "Rejected Invoices fetched successfully";
                    rejectedInvoice.Success = true;
                }
                else
                {
                    rejectedInvoice.StatusCode = StatusCodes.Status400BadRequest;
                    rejectedInvoice.Message = "Rejected Invoices fetched unsuccessfully";
                    rejectedInvoice.Success = false;
                }

            }
            catch (SqlException e)
            {
                rejectedInvoice.StatusCode = StatusCodes.Status400BadRequest;
                rejectedInvoice.Message = $"Sql Exception : {e.Message}";
            }
            catch (Exception e)
            {
                rejectedInvoice.StatusCode = StatusCodes.Status400BadRequest;
                rejectedInvoice.Message = $"Error Exception : {e.Message}";
            }

            return rejectedInvoice;
        }

    }
}
