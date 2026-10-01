using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DailyPayment;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.CORE.Entities.Report.DailyPaymentReport.ExpenseInvoice;

namespace BC.PAYMENT.API.Controllers.Report.DailyPaymentReport;

public class ExpenseReportController(IUnitOfWork unitOfWork) : BaseApiController
{
    [HttpGet]
    [Route("getexpenseinvoicereportbydate/{fromDate}/{toDate}")]
    public async Task<ApiResponse<List<ExpenseInvoiceReportModel>>> GetCreditInvoiceReportByDateAsync(
        [Required] string fromDate, [Required] string toDate)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var execute = await unitOfWork.ExpenseInvoice.GetExpenseInvoiceReportsByDateAsync(credential.DbCode,
                Convert.ToDateTime(fromDate), Convert.ToDateTime(toDate));
            if (execute.Any())
                return ApiResponse<List<ExpenseInvoiceReportModel>>.Builder()
                    .WithResult(execute)
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithMessage("Expense invoices report fetched successfully")
                    .Build();
            else
                return ApiResponse<List<ExpenseInvoiceReportModel>>.Builder()
                    .WithStatusCode(StatusCodes.Status400BadRequest)
                    .WithMessage("Expense invoices report fetched unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<ExpenseInvoiceReportModel>>(ex.Message);
        }
    }
}