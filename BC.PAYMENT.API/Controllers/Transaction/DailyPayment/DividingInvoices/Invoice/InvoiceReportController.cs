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

    public class InvoiceReportController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public InvoiceReportController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("{date}")]
        public async Task<ApiResponse<List<InvoiceReportModel>>> GetInvoiceReportAsync([Required] DateTime date)
        {
            var invoice = new ApiResponse<List<InvoiceReportModel>>();
            var credential = Common.DecodeJwt(User);
            try
            {
                var execute =
                    await _unitOfWork.InvoiceReport.GetInvoiceReportAsync(credential.DbCode!, date);
                if (execute.Any())
                {
                    invoice.StatusCode = (int)HttpStatusCode.OK;
                    invoice.Message = $@"Invoice fetched successfully";
                    invoice.Success = true;
                    invoice.Result = execute;
                }
                else
                {
                    invoice.StatusCode = (int)HttpStatusCode.BadRequest;
                    invoice.Message = $@"Invoice fetched unsuccessfully";
                    invoice.Result = new();
                }
            }
            catch (SqlException ex)
            {
                invoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                invoice.Message = $@"Sql Exception : {ex.Message}";
            }
            catch (Exception ex)
            {
                invoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                invoice.Message = $@"Error Exception : {ex.Message}";
            }
            return invoice;
        }
    }
}
