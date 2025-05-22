using System.ComponentModel.DataAnnotations;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.Entities.Transaction.Submitting.SubmittingInvoice;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BC.PAYMENT.API.Controllers.Transaction.Submitting.SubmittingInvoice
{

    public class RejectedInvoiceController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public RejectedInvoiceController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("getrejectedinvoicebyperiod/{year}/{month}")]
        public async Task<ApiResponse<List<RejectedInvoiceModel>>> GetRejectedInvoiceByPeriod([Required] int month, [Required] int year)
        {
            var credential = Common.DecodeJwt(User);
            var rejectedInvoice = new ApiResponse<List<RejectedInvoiceModel>>();
            try
            {
                var rejectedInvoices = await _unitOfWork.RejectedInvoice.GetAllRejectedInvoicesByPeriodAsync(credential.DbCode!, month, year);
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
            catch(Exception e)
            {
                rejectedInvoice.StatusCode = StatusCodes.Status400BadRequest;
                rejectedInvoice.Message = $"Error Exception : {e.Message}";
            }

            return rejectedInvoice;
        }
        
        [HttpGet]
        [Route("getrejectedinvoicebydate/{fromDate}/{toDate}")]
        public async Task<ApiResponse<List<RejectedInvoiceModel>>> GetRejectedInvoiceByDate([Required] string fromDate, [Required] string toDate)
        {
            var credential = Common.DecodeJwt(User);
            var rejectedInvoice = new ApiResponse<List<RejectedInvoiceModel>>();
            try
            {
                var rejectedInvoices = await _unitOfWork.RejectedInvoice.GetAllRejectedInvoicesByDateAsync(credential.DbCode!, fromDate, toDate);
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
            catch(Exception e)
            {
                rejectedInvoice.StatusCode = StatusCodes.Status400BadRequest;
                rejectedInvoice.Message = $"Error Exception : {e.Message}";
            }
            return rejectedInvoice;
        } 
        
        [HttpPut]
        [Route("updatestocancel")]
        public async Task<ApiResponse<int>> UpdateSubmittedInvoiceFromRejectToCancel([Required] string transactionCode, [Required] string submittedId)
        {
            var credential = Common.DecodeJwt(User);
            var rejectedInvoice = new ApiResponse<int>();
            try
            {
                var affectedRow = await _unitOfWork.RejectedInvoice.UpdateSubmittedInvoiceFromRejectToCancel(credential.DbCode!, transactionCode, submittedId);
                if (affectedRow > 0 )
                {
                    rejectedInvoice.Result = affectedRow;
                    rejectedInvoice.StatusCode = StatusCodes.Status200OK;
                    rejectedInvoice.Message = "Rejected Invoices updated successfully";
                    rejectedInvoice.Success = true;
                }
                else
                {
                    rejectedInvoice.StatusCode = StatusCodes.Status400BadRequest;
                    rejectedInvoice.Message = "Rejected Invoices updated unsuccessfully";
                    rejectedInvoice.Success = false;
                }
            }
            catch (SqlException e)
            {
                rejectedInvoice.StatusCode = StatusCodes.Status400BadRequest;
                rejectedInvoice.Message = $"Sql Exception : {e.Message}";
            }
            catch(Exception e)
            {
                rejectedInvoice.StatusCode = StatusCodes.Status400BadRequest;
                rejectedInvoice.Message = $"Error Exception : {e.Message}";
            }
            return rejectedInvoice;
        } 
    }
}
