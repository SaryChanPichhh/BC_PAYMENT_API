using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.Entities.Transaction.Submitting.SubmittingInvoice;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BC.PAYMENT.API.Controllers.Transaction.Submitting.SubmittingInvoice
{

    public class SubmittedInvoiceController(IUnitOfWork unitOfWork) : BaseApiController
    {
        [HttpGet]
        [Route("getsubmittedinvoice/{fromDate}/{toDate}")]
        public async Task<ApiResponse<List<SubmittedInvoiceModel>>> GetSubmittedInvoicesByDate([Required] string fromDate,string toDate)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var execute = await unitOfWork.SubmittedInvoice.GetAllNotSubmitPaidInvoice(credential.DbCode!,fromDate,fromDate);
                if (execute.Any())
                {
                    return ApiResponse<List<SubmittedInvoiceModel>>.Builder()
                        .WithMessage("Submitted Invoices fetched successfully")
                        .WithStatusCode(StatusCodes.Status200OK)
                        .WithResult(execute)
                        .Build();
                }
                else
                {
                    return ApiResponse<List<SubmittedInvoiceModel>>.Builder()
                        .WithMessage("Submitted Invoices fetched unsuccessfully")
                        .WithStatusCode(StatusCodes.Status400BadRequest)
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<SubmittedInvoiceModel>>(ex.Message);
            }
        }
        
        [HttpPut]
        [Route("updatesubmiitedinvoicefrompendingtoreject/{submittedId}")]
        public async Task<ApiResponse<string>> UpdateSubmittedInvoiceFromPendingToRejectAsync([Required]string submittedId)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var affectedRow = await unitOfWork.SubmittedInvoice.UpdateInvoiceFromPendingToCancelAsync(credential.DbCode!,credential.Username!,submittedId);
                if (affectedRow > 0)
                {
                    return ApiResponse<string>.Builder()
                        .WithMessage("Submitted Invoices updated successfully")
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithResult(submittedId)
                        .Build();
                }
                else
                {
                    return ApiResponse<string>.Builder()
                        .WithMessage("Submitted Invoices updated unsuccessfully")
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<string>(ex.Message);
            }
        }
    }
}
