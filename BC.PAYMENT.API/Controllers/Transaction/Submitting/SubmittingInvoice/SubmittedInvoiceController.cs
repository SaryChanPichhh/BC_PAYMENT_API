using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.Entities.Transaction.Submitting.SubmittingInvoice;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BC.PAYMENT.API.Controllers.Transaction.Submitting.SubmittingInvoice
{

    public class SubmittedInvoiceController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubmittedInvoiceController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("getsubmittedinvoice/{fromDate}/{toDate}")]
        public async Task<ApiResponse<List<SubmittedInvoiceModel>>> GetSubmittedInvoicesByDate([Required] string fromDate,string toDate)
        {
            var credential = Common.DecodeJwt(User);
            var submittedInvoice = new ApiResponse<List<SubmittedInvoiceModel>>();
            try
            {
                var execute = await _unitOfWork.SubmittedInvoice.GetAllNotSubmitPaidInvoice(credential.DbCode!,fromDate,fromDate);
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
            catch(SqlException ex)
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
        
        [HttpPut]
        [Route("updatesubmiitedinvoicefrompendingtoreject/{submittedId}")]
        public async Task<ApiResponse<string>> UpdateSubmittedInvoiceFromPendingToRejectAsync([Required]string submittedId)
        {
            var credential = Common.DecodeJwt(User);
            var submittedInvoice = new ApiResponse<string>();
            try
            {
                var affectedRow = await _unitOfWork.SubmittedInvoice.UpdateInvoiceFromPendingToCancelAsync(credential.DbCode!,credential.Username!,submittedId);
                if (affectedRow > 0)
                {
                    submittedInvoice.Result = submittedId;
                    submittedInvoice.StatusCode = (int)HttpStatusCode.OK;
                    submittedInvoice.Message = "Submitted Invoices updated successfully";
                    submittedInvoice.Success = true;
                }
                else
                {
                    submittedInvoice.StatusCode =  (int)HttpStatusCode.BadRequest;
                    submittedInvoice.Message = "Submitted Invoices updated unsuccessfully";
                }
            }
            catch(SqlException ex)
            {
                submittedInvoice.StatusCode =  (int)HttpStatusCode.InternalServerError;
                Debug.WriteLine(ex.StackTrace);
                submittedInvoice.Message = $@"Sql Exception : {ex.Message}";
            }
            catch (Exception ex)
            {
                submittedInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                Debug.WriteLine(ex.StackTrace);
                submittedInvoice.Message = $@"Error Exception : {ex.Message}";
            }
            return submittedInvoice;
        }
    }
}
