using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Runtime.InteropServices;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.Entities.Preset.ExchangeItemAnalysis;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BC.PAYMENT.API.Controllers.Preset.ExchangeItemAnalysis;

public class ExchangeItemAnalysisController(IUnitOfWork unitOfWork) : BaseApiController
{
    [HttpPost]
    [Route("getexchangeitemanalysisbydate")]
    public async Task<ApiResponse<List<ExchangeItemAnalysisModel>>> GetAllByDateAsync([Required] string dbCode,
        [Required] string type, [Optional] string isReceived, [Required] string fromDate, [Required] string toDate)
    {
        try
        {
            var execute = await unitOfWork.ExchangeItemAnalysis.GetAllByDateAsync(type, isReceived, dbCode,
                Convert.ToDateTime(fromDate), Convert.ToDateTime(toDate));
            if (execute.Any())
                return ApiResponse<List<ExchangeItemAnalysisModel>>.Builder()
                    .WithResult(execute)
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithMessage("Data fetched successfully")
                    .Build();
            else
                return ApiResponse<List<ExchangeItemAnalysisModel>>.Builder()
                    .WithStatusCode(StatusCodes.Status400BadRequest)
                    .WithMessage("Data fetched unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<ExchangeItemAnalysisModel>>(ex.Message);
        }
    }

    [HttpPost]
    [Route("getexchangeitemanalysisbyperiod")]
    public async Task<ApiResponse<List<ExchangeItemAnalysisModel>>> GetAllByPeriodAsync([Required] string dbCode,
        [Required] string type, [Optional] string isReceived, [Required] int fromPeriod, [Required] int toPeriod)
    {
        try
        {
            var execute =
                await unitOfWork.ExchangeItemAnalysis.GetAllByPeriodAsync(type, isReceived, dbCode, fromPeriod,
                    toPeriod);
            if (execute.Any())
                return ApiResponse<List<ExchangeItemAnalysisModel>>.Builder()
                    .WithResult(execute)
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithMessage("Data fetched successfully")
                    .Build();
            else
                return ApiResponse<List<ExchangeItemAnalysisModel>>.Builder()
                    .WithStatusCode(StatusCodes.Status400BadRequest)
                    .WithMessage("Data fetched unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<ExchangeItemAnalysisModel>>(ex.Message);
        }
    }
}