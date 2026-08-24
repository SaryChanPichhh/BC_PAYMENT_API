using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DividingInvoices.Invoice;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.Net;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DividingInvoices.DailyReport;

namespace BC.PAYMENT.API.Controllers.Transaction.DailyPayment.DividingInvoices.DailyReport
{
    public class CheckReturnInvoiceController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public CheckReturnInvoiceController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        [Route("{fromDate}/{toDate}")]
        public async Task<ApiResponse<List<PaymentInvoiceModel>>> GetInvoiceReportAsync([Required] DateTime fromDate, [Required] DateTime toDate)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var execute =
                    await _unitOfWork.CheckReturnInvoice.GetAllNewAndChangeDividedInvoiceByDate(credential.DbCode!, fromDate, toDate);
                if (execute.Any())
                {
                    return ApiResponse<List<PaymentInvoiceModel>>.Builder()
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithMessage($@"Invoice fetched successfully")
                        .WithSuccess(true)
                        .WithResult(execute)
                        .Build();
                }
                else
                {
                    return ApiResponse<List<PaymentInvoiceModel>>.Builder()
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithMessage($@"Invoice fetched unsuccessfully")
                        .WithResult(new())
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<PaymentInvoiceModel>>(ex.Message);
            }
        }
    }
}
