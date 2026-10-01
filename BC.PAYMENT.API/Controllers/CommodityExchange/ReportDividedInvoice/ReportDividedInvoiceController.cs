using System.ComponentModel.DataAnnotations;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.CommondityExchange.RepairItem;
using BC.PAYMENT.CORE.DTO.CommondityExchange.ReportDividedInvoice;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BC.PAYMENT.API.Controllers.CommodityExchange.ReportDividedInvoice;

public class ReportDividedInvoiceController(IUnitOfWork unitOfWork) : BaseApiController
{
    [HttpGet]
    [Route("getreportdividedivoicebydate/{fromdate}/{todate}")]
    public async Task<ApiResponse<List<ReportDividedInvoiceDto>>> GetReportDividedInvoiceAsync(
        [Required] string fromdate, string todate)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var execute = await unitOfWork.ReportDividedInvoice.GetReportDividedInvoiceAsync(credential.DbCode,
                Convert.ToDateTime(fromdate), Convert.ToDateTime(todate));
            if (execute != null)
                return ApiResponse<List<ReportDividedInvoiceDto>>.Builder()
                    .WithMessage("Reports fetched successfully")
                    .WithStatusCode(StatusCodes.Status204NoContent)
                    .WithResult(execute)
                    .Build();
            else
                return ApiResponse<List<ReportDividedInvoiceDto>>.Builder()
                    .WithMessage("Reports fetched unsuccessfully")
                    .WithStatusCode(StatusCodes.Status400BadRequest)
                    .WithResult(new List<ReportDividedInvoiceDto>())
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<ReportDividedInvoiceDto>>(ex.Message);
        }
    }
}