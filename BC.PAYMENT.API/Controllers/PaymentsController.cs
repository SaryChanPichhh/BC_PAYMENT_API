using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.Entities.Invoice;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Net;
using BC.PAYMENT.LOGGING;

namespace BC.PAYMENT.API.Controllers;

public class PaymentsController(IUnitOfWork unitOfWork, IOptions<AppSettings> appSettings) : BaseApiController
{
    [HttpGet("")]
    public async Task<ApiResponse<List<DividedInvoiceSummary>>> GetDeliveryPaidInvoice([FromQuery] DateTime date)
    {
        try
        {
            var claim = Common.DecodeJwt(HttpContext.User);

            var deliveries = await unitOfWork.DividedInvoices.GetDividedInvoiceSummary(claim.DbCode!, date.Date);
            if (!deliveries.Any())
                return ApiResponse<List<DividedInvoiceSummary>>.Builder()
                    .WithMessage("No summary")
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .WithResult(new List<DividedInvoiceSummary>())
                    .Build();

            return ApiResponse<List<DividedInvoiceSummary>>.Builder()
                .WithMessage("Summary fetched successfully.")
                .WithStatusCode((int)HttpStatusCode.OK)
                .WithResult(deliveries)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<DividedInvoiceSummary>>(ex.Message);
        }
    }
}