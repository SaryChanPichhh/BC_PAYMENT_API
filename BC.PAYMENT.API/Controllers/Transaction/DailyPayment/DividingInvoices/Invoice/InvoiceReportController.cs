using System.ComponentModel.DataAnnotations;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.Entities.General;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DividingInvoices.Invoice;

namespace BC.PAYMENT.API.Controllers.Transaction.DailyPayment.DividingInvoices.Invoice
{

    public class InvoiceReportController(IUnitOfWork unitOfWork) : BaseApiController
    {
        [HttpGet]
        [Route("{date}")]
        public async Task<ApiResponse<List<InvoiceReportModel>>> GetInvoiceReportAsync([Required] DateTime date)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var execute =
                    await unitOfWork.InvoiceReport.GetInvoiceReportAsync(credential.DbCode!, date);
                if (execute.Any())
                {
                    return ApiResponse<List<InvoiceReportModel>>.Builder()
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithMessage($@"Invoice fetched successfully")
                        .WithSuccess(true)
                        .WithResult(execute)
                        .Build();
                }
                else
                {
                    return ApiResponse<List<InvoiceReportModel>>.Builder()
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithMessage($@"Invoice fetched unsuccessfully")
                        .WithResult(new())
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<InvoiceReportModel>>(ex.Message);
            }
        }
    }
}
