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

namespace BC.PAYMENT.API.Controllers.Preset.DailySaleAnalysis
{
    public class DailySaleAnalysisController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public DailySaleAnalysisController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpPost]
        [Route("getdailysaleanalysis")]
        public async Task<ApiResponse<List<DailySaleAnalysisModel>>> GetDailySaleAnalysisAsync([Required] int fromMov, [Required] int toMov)
        {
            var credential = Common.DecodeJwt(User);
            var dailySaleAnalysis = new ApiResponse<List<DailySaleAnalysisModel>>();
            try
            {
                var execute = await _unitOfWork.DailyAnalysis.GetDailySaleAnalysisAsync(fromMov.ToString(), toMov.ToString());
                if (execute.Any())
                {
                    dailySaleAnalysis.Result = execute;
                    dailySaleAnalysis.StatusCode = StatusCodes.Status200OK;
                    dailySaleAnalysis.Success = true;
                    dailySaleAnalysis.Message = "Daily sale fetched successfully";
                }
                else
                {
                    dailySaleAnalysis.StatusCode = StatusCodes.Status400BadRequest;
                    dailySaleAnalysis.Message = "Daily sale fetched unsuccessfully";
                }
            }
            catch (SqlException ex)
            {
                dailySaleAnalysis.StatusCode = StatusCodes.Status500InternalServerError;
                dailySaleAnalysis.Message = $"Sql Exception : ${ex.Message}";
            }
            catch (Exception ex)
            {
                dailySaleAnalysis.StatusCode = StatusCodes.Status500InternalServerError;
                dailySaleAnalysis.Message = $"Sql Exception : ${ex.Message}";
            }
            return dailySaleAnalysis;
        }
        [HttpPost]
        [Route("getdailysaleanalysisbyperiodanditemcode")]
        public async Task<ApiResponse<List<DailySaleAnalysisModel>>> GetDailySaleAnalysisByItemCodeAndPeriodAsync([FromBody] DailySaleAnalysisFilterByPeriodDto model)
        {
            var credential = Common.DecodeJwt(User);
            var dailySaleAnalysis = new ApiResponse<List<DailySaleAnalysisModel>>();
            try
            {
                var execute = await _unitOfWork.DailyAnalysis.GetDailySaleAnalysisByItemCodeAndPeriodAsync(model.FromPeriod.ToString(), model.ToPeriod.ToString(),model.ItemCodes);
                if (execute.Any())
                {
                    dailySaleAnalysis.Result = execute;
                    dailySaleAnalysis.StatusCode = StatusCodes.Status200OK;
                    dailySaleAnalysis.Success = true;
                    dailySaleAnalysis.Message = "Daily sale fetched successfully";
                }
                else
                {
                    dailySaleAnalysis.StatusCode = StatusCodes.Status400BadRequest;
                    dailySaleAnalysis.Message = "Daily sale fetched unsuccessfully";
                }
            }
            catch (SqlException ex)
            {
                dailySaleAnalysis.StatusCode = StatusCodes.Status500InternalServerError;
                dailySaleAnalysis.Message = $"Sql Exception : ${ex.Message}";
            }
            catch (Exception ex)
            {
                dailySaleAnalysis.StatusCode = StatusCodes.Status500InternalServerError;
                dailySaleAnalysis.Message = $"Sql Exception : ${ex.Message}";
            }
            return dailySaleAnalysis;
        }
        
        [HttpPost]
        [Route("getdailysaleanalysisbydateanditemcode")]
        public async Task<ApiResponse<List<DailySaleAnalysisModel>>> GetDailySaleAnalysisByItemCodeAndDateAsync([FromBody] DailySaleAnalysisFilterByDateDto model)
        {
            var credential = Common.DecodeJwt(User);
            var dailySaleAnalysis = new ApiResponse<List<DailySaleAnalysisModel>>();
            try
            {
                var execute = await _unitOfWork.DailyAnalysis.GetDailySaleAnalysisByItemCodeAndDateAsync(Convert.ToDateTime(model.FromDate), Convert.ToDateTime(model.ToDate),model.ItemCodes);
                if (execute.Any())
                {
                    dailySaleAnalysis.Result = execute;
                    dailySaleAnalysis.StatusCode = StatusCodes.Status200OK;
                    dailySaleAnalysis.Success = true;
                    dailySaleAnalysis.Message = "Daily sale fetched successfully";
                }
                else
                {
                    dailySaleAnalysis.StatusCode = StatusCodes.Status400BadRequest;
                    dailySaleAnalysis.Message = "Daily sale fetched unsuccessfully";
                }
            }
            catch (SqlException ex)
            {
                dailySaleAnalysis.StatusCode = StatusCodes.Status500InternalServerError;
                dailySaleAnalysis.Message = $"Sql Exception : ${ex.Message}";
            }
            catch (Exception ex)
            {
                dailySaleAnalysis.StatusCode = StatusCodes.Status500InternalServerError;
                dailySaleAnalysis.Message = $"Sql Exception : ${ex.Message}";
            }
            return dailySaleAnalysis;
        }
    }
}
