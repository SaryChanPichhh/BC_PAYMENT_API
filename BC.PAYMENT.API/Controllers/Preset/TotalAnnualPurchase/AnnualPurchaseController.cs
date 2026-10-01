using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.INFRASTRUCTURE.Repository.General;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.CORE.Contracts.General;
using BC.PAYMENT.CORE.DTO.Preset.ExchangeItemAnalysis;
using BC.PAYMENT.CORE.Entities.General;
using BC.PAYMENT.CORE.Entities.Preset.AnnualPurchase;
using BC.PAYMENT.CORE.DTO.General;

namespace BC.PAYMENT.API.Controllers.Preset.TotalAnnualPurchase;

public class AnnualPurchaseController : BaseApiController
{
    private readonly IUnitOfWork _unitOfWork;

    public AnnualPurchaseController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    #region Sale Of Year

    [HttpPost]
    [Route("getreportsaleofyear")]
    public async Task<ApiResponse<List<AnnualPurchaseModel>>> GetSalesReportListAsync([Required] List<int> years,
        [Required] int page, [Required] int pageSize)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var execute =
                await _unitOfWork.AnnualPurchase.GetSalesReportListAsync(credential.DbCode, years, page, pageSize);
            if (execute.Any())
                return ApiResponse<List<AnnualPurchaseModel>>.Builder()
                    .WithResult(execute)
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithMessage("Items fetched successfully")
                    .Build();
            else
                return ApiResponse<List<AnnualPurchaseModel>>.Builder()
                    .WithStatusCode(StatusCodes.Status400BadRequest)
                    .WithMessage("Items fetched unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<AnnualPurchaseModel>>(ex.Message);
        }
    }

    [HttpPost]
    [Route("getreportsaleofyearbysaletype")]
    public async Task<ApiResponse<List<AnnualPurchaseModel>>> GetAllReportSalesPerYearsAsync(ReportSaleBySaleType model)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var execute = await _unitOfWork.AnnualPurchase.GetAllReportSalesPerYearsAsync(credential.DbCode,
                model.Markets, model.CustomerCode, model.SaleTypes);
            if (execute.Any())
                return ApiResponse<List<AnnualPurchaseModel>>.Builder()
                    .WithResult(execute)
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithMessage("Items fetched successfully")
                    .Build();
            else
                return ApiResponse<List<AnnualPurchaseModel>>.Builder()
                    .WithStatusCode(StatusCodes.Status400BadRequest)
                    .WithMessage("Items fetched unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<AnnualPurchaseModel>>(ex.Message);
        }
    }

    #endregion

    [HttpPost]
    [Route("getcustomerbymarketcode")]
    public async Task<ApiResponse<List<Customer>>> GetCustomerByMarketCodeAsync([FromForm] List<string> markets,
        [FromForm] List<string> saleTypes)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var execute =
                await _unitOfWork.Customers.GetCustomerByMarketCodeAsync(credential.DbCode, markets, saleTypes);
            if (execute.Any())
                return ApiResponse<List<Customer>>.Builder()
                    .WithResult(execute)
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithMessage("Customer fetched successfully")
                    .Build();
            else
                return ApiResponse<List<Customer>>.Builder()
                    .WithStatusCode(StatusCodes.Status400BadRequest)
                    .WithMessage("Customer fetched unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<Customer>>(ex.Message);
        }
    }

    [HttpPost]
    [Route("getmarketbysaletypes")]
    public async Task<ApiResponse<List<MarketDto>>> GetCustomerByMarketCodeAsync([FromBody] MarketFilterDto model)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var execute = await _unitOfWork.Markets.LoadMarketBySaleTypesAsync(credential.DbCode, model.SaleTypes,
                model.FromMov, model.ToMov);
            if (execute.Any())
                return ApiResponse<List<MarketDto>>.Builder()
                    .WithResult(execute.Select(x => new MarketDto
                    {
                        MarketName = x.MarketName,
                        MarketNameKhmer = x.MarketNameKhmer
                    }).ToList())
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithMessage("Market fetched successfully")
                    .Build();
            else
                return ApiResponse<List<MarketDto>>.Builder()
                    .WithStatusCode(StatusCodes.Status400BadRequest)
                    .WithMessage("Market fetched unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<MarketDto>>(ex.Message);
        }
    }

    [HttpGet]
    [Route("getsaletype")]
    public async Task<ApiResponse<List<string>>> GetSalesReportListAsync()
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var execute = await _unitOfWork.AnnualPurchase.SaleCodesAsync(credential.DbCode);
            if (execute.Any())
                return ApiResponse<List<string>>.Builder()
                    .WithResult(execute)
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithMessage("Items fetched successfully")
                    .Build();
            else
                return ApiResponse<List<string>>.Builder()
                    .WithStatusCode(StatusCodes.Status400BadRequest)
                    .WithMessage("Items fetched unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<string>>(ex.Message);
        }
    }

    #region Sale Of The Day

    [HttpPost]
    [Route("getdailysalereportvalue")]
    public async Task<ApiResponse<List<DailySaleReportValueModel>>> GetDailySaleReportValueAsync(
        [FromBody] DailySaleReportValueDto model)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var execute = await _unitOfWork.AnnualPurchase.GetDailySaleReportValueAsync(model.Date, model.Branches);
            if (execute.Any())
                return ApiResponse<List<DailySaleReportValueModel>>.Builder()
                    .WithResult(execute)
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithMessage("Daily sale report fetched successfully")
                    .Build();
            else
                return ApiResponse<List<DailySaleReportValueModel>>.Builder()
                    .WithStatusCode(StatusCodes.Status400BadRequest)
                    .WithMessage("Daily sale report fetched unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<DailySaleReportValueModel>>(ex.Message);
        }
    }

    [HttpGet]
    [Route("getdailysalereportdetailvalue/{date}/{branchCode}")]
    public async Task<ApiResponse<List<DailySaleReportValueDetailsModel>>> GetDailySaleReportDetailsValueAsync(
        [Required] string date, [Required] string branchCode)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var execute =
                await _unitOfWork.AnnualPurchase.GetDailySaleReportDetailsValueAsync(Convert.ToDateTime(date),
                    branchCode);
            if (execute.Any())
                return ApiResponse<List<DailySaleReportValueDetailsModel>>.Builder()
                    .WithResult(execute)
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithMessage("Daily sale report detail fetched successfully")
                    .Build();
            else
                return ApiResponse<List<DailySaleReportValueDetailsModel>>.Builder()
                    .WithStatusCode(StatusCodes.Status400BadRequest)
                    .WithMessage("Daily sale report detail fetched unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<DailySaleReportValueDetailsModel>>(ex.Message);
        }
    }

    #endregion
}