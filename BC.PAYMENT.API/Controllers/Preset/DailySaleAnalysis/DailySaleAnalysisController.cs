using System.ComponentModel.DataAnnotations;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.Filter;
using BC.PAYMENT.CORE.DTO.Preset.DailySaleAnalysis;
using BC.PAYMENT.CORE.DTO.Preset.InventoryValue;
using BC.PAYMENT.CORE.Entities.Preset.DailySaleAnalysis;
using BC.PAYMENT.CORE.Entities.Preset.InventoryValue;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BC.PAYMENT.API.Controllers.Preset.DailySaleAnalysis;

public class DailySaleAnalysisController(IUnitOfWork unitOfWork) : BaseApiController
{
    [HttpPost]
    [Route("getdailysaleanalysis")]
    public async Task<ApiResponse<List<DailySaleAnalysisModel>>> GetDailySaleAnalysisAsync([Required] int fromMov,
        [Required] int toMov)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var execute =
                await unitOfWork.DailyAnalysis.GetDailySaleAnalysisAsync(fromMov.ToString(), toMov.ToString());
            if (execute.Any())
                return ApiResponse<List<DailySaleAnalysisModel>>.Builder()
                    .WithResult(execute)
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithSuccess(true)
                    .WithMessage("Daily sale fetched successfully")
                    .Build();
            else
                return ApiResponse<List<DailySaleAnalysisModel>>.Builder()
                    .WithStatusCode(StatusCodes.Status400BadRequest)
                    .WithMessage("Daily sale fetched unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<DailySaleAnalysisModel>>(ex.Message);
        }
    }

    [HttpPost]
    [Route("getdailysaleanalysisbyperiodanditemcode")]
    public async Task<ApiResponse<List<DailySaleAnalysisModel>>> GetDailySaleAnalysisByItemCodeAndPeriodAsync(
        [FromBody] DailySaleAnalysisFilterByPeriodDto model)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var execute =
                await unitOfWork.DailyAnalysis.GetDailySaleAnalysisByItemCodeAndPeriodAsync(model.FromPeriod.ToString(),
                    model.ToPeriod.ToString(), model.ItemCodes);
            if (execute.Any())
                return ApiResponse<List<DailySaleAnalysisModel>>.Builder()
                    .WithResult(execute)
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithSuccess(true)
                    .WithMessage("Daily sale fetched successfully")
                    .Build();
            else
                return ApiResponse<List<DailySaleAnalysisModel>>.Builder()
                    .WithStatusCode(StatusCodes.Status400BadRequest)
                    .WithMessage("Daily sale fetched unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<DailySaleAnalysisModel>>(ex.Message);
        }
    }

    [HttpPost]
    [Route("getdailysaleanalysisbydateanditemcode")]
    public async Task<ApiResponse<List<DailySaleAnalysisModel>>> GetDailySaleAnalysisByItemCodeAndDateAsync(
        [FromBody] DailySaleAnalysisFilterByDateDto model)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var execute = await unitOfWork.DailyAnalysis.GetDailySaleAnalysisByItemCodeAndDateAsync(
                Convert.ToDateTime(model.FromDate), Convert.ToDateTime(model.ToDate), model.ItemCodes);
            if (execute.Any())
                return ApiResponse<List<DailySaleAnalysisModel>>.Builder()
                    .WithResult(execute)
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithSuccess(true)
                    .WithMessage("Daily sale fetched successfully")
                    .Build();
            else
                return ApiResponse<List<DailySaleAnalysisModel>>.Builder()
                    .WithStatusCode(StatusCodes.Status400BadRequest)
                    .WithMessage("Daily sale fetched unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<DailySaleAnalysisModel>>(ex.Message);
        }
    }
}