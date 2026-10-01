using System.Net;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.Transaction.Inventory.VerificationStock;
using BC.PAYMENT.CORE.Entities.Transaction.Inventory.VerificationStock;
using BC.PAYMENT.LOGGING;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BC.PAYMENT.API.Controllers.Transaction.Inventory;

public class VerificationStockReportController(IUnitOfWork unitOfWork) : BaseApiController
{
    [HttpGet]
    [Route("getverificationinventorystockbydate/{fromDate}/{toDate}")]
    public async Task<ApiResponse<List<VerificationStockReportDto>>> GetVerificationInventoryStockByDate(
        string fromDate, string toDate)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var verificationStocks = await unitOfWork.VerificationStock.GetVerificationStockReport(credential.DbCode,
                Convert.ToDateTime(fromDate), Convert.ToDateTime(toDate
                ));
            if (verificationStocks.Any())
                return ApiResponse<List<VerificationStockReportDto>>.Builder()
                    .WithResult(verificationStocks)
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithMessage("Verification Stock report fetched successfully")
                    .Build();
            else
                return ApiResponse<List<VerificationStockReportDto>>.Builder()
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .WithMessage("Verification Stock report fetched unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<VerificationStockReportDto>>(ex.Message);
        }
    }

    [HttpGet]
    [Route("getverificationinventorystockbyperiod/{year}/{month}")]
    public async Task<ApiResponse<List<VerificationStockReportDto>>> GetVerificationStockReportByPeriod(int year,
        int month)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var verificationStocks =
                await unitOfWork.VerificationStock.GetVerificationStockReportByPeriod(credential.DbCode, month, year);
            if (verificationStocks.Any())
                return ApiResponse<List<VerificationStockReportDto>>.Builder()
                    .WithResult(verificationStocks)
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithMessage("Verification Stock report fetched successfully")
                    .Build();
            else
                return ApiResponse<List<VerificationStockReportDto>>.Builder()
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .WithMessage("Verification Stock report fetched unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<VerificationStockReportDto>>(ex.Message);
        }
    }
}