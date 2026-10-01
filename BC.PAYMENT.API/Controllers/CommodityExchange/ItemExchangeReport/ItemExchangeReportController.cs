using System.ComponentModel.DataAnnotations;
using System.Net;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.CommondityExchange.ItemExchangeReport;
using BC.PAYMENT.CORE.DTO.CommondityExchange.ItemRepairReport;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BC.PAYMENT.API.Controllers.CommodityExchange.ItemExchangeReport;

public class ItemExchangeReportController(IUnitOfWork unitOfWork) : BaseApiController
{
    [HttpGet]
    [Route("getexchangeiteminvoicebydate/{fromDate}/{toDate}")]
    public async Task<ApiResponse<List<ExchangeItemReportDto>>> GetExchangeItemInvoiceAsync([Required] string fromDate,
        [Required] string toDate)
    {
        var credential = Common.DecodeJwt(User);
        var response = new ApiResponse<List<ExchangeItemReportDto>>();
        try
        {
            var result = await unitOfWork.ItemExchangeReport.GetExchangeItemInvoiceAsync(credential.DbCode,
                Convert.ToDateTime(fromDate), Convert.ToDateTime(toDate));
            if (result.Any())
                return ApiResponse<List<ExchangeItemReportDto>>.Builder()
                    .WithMessage("Exchange item invoices fetched successfully")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(result)
                    .Build();
            else
                return ApiResponse<List<ExchangeItemReportDto>>.Builder()
                    .WithMessage("Exchange item invoices fetched unsuccessfully")
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .WithResult(new List<ExchangeItemReportDto>())
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<ExchangeItemReportDto>>(ex.Message);
        }

        return response;
    }

    [HttpGet]
    [Route("getreceivedexchangeiteminvoicebydate/{fromDate}/{toDate}")]
    public async Task<ApiResponse<List<ItemReceivedDto>>> GetReceivedExchangeItemInvoiceAsync(
        [Required] string fromDate, [Required] string toDate)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var result = await unitOfWork.ItemExchangeReport.GetReceivedExchangeItemInvoiceAsync(credential.DbCode,
                Convert.ToDateTime(fromDate), Convert.ToDateTime(toDate));
            if (result.Any())
                return ApiResponse<List<ItemReceivedDto>>.Builder()
                    .WithResult(result)
                    .WithMessage("Received exchange item invoices fetched successfully")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .Build();
            else
                return ApiResponse<List<ItemReceivedDto>>.Builder()
                    .WithResult(new List<ItemReceivedDto>())
                    .WithMessage("Received exchange item invoices fetched unsuccessfully")
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<ItemReceivedDto>>(ex.Message);
        }
    }

    [HttpGet]
    [Route("getpendingexchangeiteminvoicebydate/{dbCode}")]
    public async Task<ApiResponse<List<ReportExchangePendingItemDto>>> GetPendingExchangeItemInvoiceAsync(
        [Required] string dbCode)
    {
        try
        {
            var result = await unitOfWork.ItemExchangeReport.GetPendingExchangeItemInvoiceAsync(dbCode);
            if (result.Any())
                return ApiResponse<List<ReportExchangePendingItemDto>>.Builder()
                    .WithResult(result)
                    .WithMessage("Pending exchange item invoices fetched successfully")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .Build();
            else
                return ApiResponse<List<ReportExchangePendingItemDto>>.Builder()
                    .WithResult(new List<ReportExchangePendingItemDto>())
                    .WithMessage("Pending exchange item invoices fetched unsuccessfully")
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<ReportExchangePendingItemDto>>(ex.Message);
        }
    }

    [HttpGet]
    [Route("getexchangeinvoicenotyetsendtocustomerreportbydate/{fromdate}/{todate}")]
    public async Task<ApiResponse<List<ReportExchangeDto>>> GetExchangeInvoiceNotYetSendToCustomerReportAsync(
        [Required] string fromdate, [Required] string todate)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var result =
                await unitOfWork.ItemExchangeReport.GetExchangeInvoiceNotYetSendToCustomerReportAsync(credential.DbCode,
                    Convert.ToDateTime(fromdate), Convert.ToDateTime(todate));
            if (result.Any())
                return ApiResponse<List<ReportExchangeDto>>.Builder()
                    .WithResult(result)
                    .WithMessage("Exchange invoice not yet send to customer fetched successfully")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .Build();
            else
                return ApiResponse<List<ReportExchangeDto>>.Builder()
                    .WithResult(new List<ReportExchangeDto>())
                    .WithMessage("Exchange invoice not yet send to customer fetched unsuccessfully")
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<ReportExchangeDto>>(ex.Message);
        }
    }

    [HttpGet]
    [Route("getexchangeinvoicealreadysendtocustomerreportbydate/{fromdate}/{todate}")]
    public async Task<ApiResponse<List<ReportExchangeDto>>> GetExchangeInvoiceAlreadySendToCustomerReportAsync(
        [Required] string fromdate, [Required] string todate)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var result =
                await unitOfWork.ItemExchangeReport.GetExchangeInvoiceAlreadySendToCustomerReportAsync(
                    credential.DbCode, Convert.ToDateTime(fromdate), Convert.ToDateTime(todate));
            if (result.Any())
                return ApiResponse<List<ReportExchangeDto>>.Builder()
                    .WithResult(result)
                    .WithMessage("Exchange invoice already to customer fetched successfully")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .Build();
            else
                return ApiResponse<List<ReportExchangeDto>>.Builder()
                    .WithResult(new List<ReportExchangeDto>())
                    .WithMessage("Exchange invoice already to customer fetched unsuccessfully")
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<ReportExchangeDto>>(ex.Message);
        }
    }
}