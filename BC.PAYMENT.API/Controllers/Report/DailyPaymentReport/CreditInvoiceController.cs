using System.ComponentModel.DataAnnotations;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.Entities.Preset.DailySaleAnalysis;
using BC.PAYMENT.CORE.Entities.Report.DailyPaymentReport.CreditInvoice;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BC.PAYMENT.API.Controllers.Report.DailyPaymentReport;

public class CreditInvoiceController : BaseApiController
{
    private readonly IUnitOfWork _unitOfWork;

    public CreditInvoiceController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    [Route("getcreditinvoicebydate/{fromDate}/{toDate}")]
    public async Task<ApiResponse<List<CreditInvoiceReportModel>>> GetCreditInvoiceReportByDateAsync(
        [Required] string fromDate, [Required] string toDate)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var execute = await _unitOfWork.CreditInvoice.GetCreditInvoiceAsync(credential.DbCode,
                Convert.ToDateTime(fromDate), Convert.ToDateTime(toDate));
            if (execute.Any())
                return ApiResponse<List<CreditInvoiceReportModel>>.Builder()
                    .WithResult(execute)
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithMessage("Credit Invoice fetched successfully")
                    .Build();
            else
                return ApiResponse<List<CreditInvoiceReportModel>>.Builder()
                    .WithStatusCode(StatusCodes.Status400BadRequest)
                    .WithMessage("Credit Invoice fetched unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<CreditInvoiceReportModel>>(ex.Message);
        }
    }
}