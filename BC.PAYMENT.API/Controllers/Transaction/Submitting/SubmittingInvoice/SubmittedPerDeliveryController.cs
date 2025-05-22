using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.Entities.Transaction.Submitting.SubmittingInvoice;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DailyPayment;

namespace BC.PAYMENT.API.Controllers.Transaction.Submitting.SubmittingInvoice
{

    public class SubmittedPerDeliveryController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubmittedPerDeliveryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("getsubmittedperdeliverybydate/{fromDate}/{toDate}")]
        public async Task<ApiResponse<List<ExpenseDetailModel>>> GetSubmittedPerDeliveryByDateAsync([Required] string fromDate, string toDate)
        {
            var credential = Common.DecodeJwt(User);
            var submittedInvoice = new ApiResponse<List<ExpenseDetailModel>>();
            try
            {
                var execute = await _unitOfWork.SubmittedPerDelivery.GetSubmittedInvoicePerDeliveryAsync(credential.DbCode!, fromDate, fromDate);
                if (execute.Any())
                {
                    submittedInvoice.Result = execute;
                    submittedInvoice.StatusCode = StatusCodes.Status200OK;
                    submittedInvoice.Message = "Submitted Invoices fetched successfully";
                    submittedInvoice.Success = true;
                }
                else
                {
                    submittedInvoice.StatusCode = StatusCodes.Status400BadRequest;
                    submittedInvoice.Message = "Submitted Invoices fetched unsuccessfully";
                }
            }
            catch (SqlException ex)
            {
                submittedInvoice.StatusCode = StatusCodes.Status500InternalServerError;
                submittedInvoice.Message = $@"Sql Exception : {ex.Message}";
            }
            catch (Exception ex)
            {
                submittedInvoice.StatusCode = StatusCodes.Status500InternalServerError;
                submittedInvoice.Message = $@"Error Exception : {ex.Message}";
            }

            return submittedInvoice;
        }
        [HttpGet]
        [Route("getsubmittedperdeliverybyperiod/{year}/{month}")]
        public async Task<ApiResponse<List<ExpenseDetailModel>>> GetSubmittedPerDeliveryByPeriodAsync([Required] int month, int year)
        {
            var credential = Common.DecodeJwt(User);
            var submittedInvoice = new ApiResponse<List<ExpenseDetailModel>>();
            try
            {
                var execute = await _unitOfWork.SubmittedPerDelivery.GetSubmittedInvoicePerDeliveryByPeriodAsync(credential.DbCode!, month, year);
                if (execute.Any())
                {
                    submittedInvoice.Result = execute;
                    submittedInvoice.StatusCode = StatusCodes.Status200OK;
                    submittedInvoice.Message = "Submitted Invoices fetched successfully";
                    submittedInvoice.Success = true;
                }
                else
                {
                    submittedInvoice.StatusCode = StatusCodes.Status400BadRequest;
                    submittedInvoice.Message = "Submitted Invoices fetched unsuccessfully";
                }
            }
            catch (SqlException ex)
            {
                submittedInvoice.StatusCode = StatusCodes.Status500InternalServerError;
                submittedInvoice.Message = $@"Sql Exception : {ex.Message}";
            }
            catch (Exception ex)
            {
                submittedInvoice.StatusCode = StatusCodes.Status500InternalServerError;
                submittedInvoice.Message = $@"Error Exception : {ex.Message}";
            }

            return submittedInvoice;
        } 
        
        [HttpDelete]
        [Route("updatesubmittedperdelivery/{submittedInvoiceId}")]
        public async Task<ApiResponse<string>> DeleteSubmittedInvoicePerDeliveryAsync([Required] string submittedInvoiceId)
        {
            var credential = Common.DecodeJwt(User);
            var submittedInvoice = new ApiResponse<String>();
            try
            {
                var affectedRow = await _unitOfWork.SubmittedPerDelivery.DeleteSubmittedInvoiceAsync(submittedInvoiceId);
                if (affectedRow > 0 )
                {
                    submittedInvoice.Result = submittedInvoiceId;
                    submittedInvoice.StatusCode = StatusCodes.Status200OK;
                    submittedInvoice.Message = "Submitted Invoices deleted successfully";
                    submittedInvoice.Success = true;
                }
                else
                {
                    submittedInvoice.StatusCode = StatusCodes.Status400BadRequest;
                    submittedInvoice.Message = "Submitted Invoices deleted unsuccessfully";
                }
            }
            catch (SqlException ex)
            {
                submittedInvoice.StatusCode = StatusCodes.Status500InternalServerError;
                submittedInvoice.Message = $@"Sql Exception : {ex.Message}";
            }
            catch (Exception ex)
            {
                submittedInvoice.StatusCode = StatusCodes.Status500InternalServerError;
                submittedInvoice.Message = $@"Error Exception : {ex.Message}";
            }

            return submittedInvoice;
        }

    }
}
