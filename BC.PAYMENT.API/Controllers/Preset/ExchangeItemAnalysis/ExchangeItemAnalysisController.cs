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

namespace BC.PAYMENT.API.Controllers.Preset.ExchangeItemAnalysis
{
    public class ExchangeItemAnalysisController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public ExchangeItemAnalysisController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        [Route("getexchangeitemanalysisbydate")]
        public async Task<ApiResponse<List<ExchangeItemAnalysisModel>>> GetAllByDateAsync([Required] string dbCode, [Required] string type, [Optional] string isReceived, [Required] string fromDate, [Required] string toDate)
        {
            var exchangeItem = new ApiResponse<List<ExchangeItemAnalysisModel>>();
            try
            {
                var execute = await _unitOfWork.ExchangeItemAnalysis.GetAllByDateAsync(type,isReceived, dbCode, Convert.ToDateTime(fromDate),Convert.ToDateTime(toDate));
                if (execute.Any())
                {
                    exchangeItem.Result = execute;
                    exchangeItem.StatusCode = StatusCodes.Status200OK;
                    exchangeItem.Success = true;
                    exchangeItem.Message = "Data fetched successfully";
                }
                else
                {
                    exchangeItem.StatusCode = StatusCodes.Status400BadRequest;
                    exchangeItem.Message = "Data fetched unsuccessfully";
                }
            }
            catch (SqlException ex)
            {
                exchangeItem.StatusCode = StatusCodes.Status500InternalServerError;
                exchangeItem.Message = $"Sql Exception : ${ex.Message}";
            }
            catch (Exception ex)
            {
                exchangeItem.StatusCode = StatusCodes.Status500InternalServerError;
                exchangeItem.Message = $"Sql Exception : ${ex.Message}";
            }
            return exchangeItem;
        }
        [HttpPost]
        [Route("getexchangeitemanalysisbyperiod")]
        public async Task<ApiResponse<List<ExchangeItemAnalysisModel>>> GetAllByPeriodAsync([Required] string dbCode, [Required] string type, [Optional] string isReceived, [Required] int fromPeriod, [Required] int toPeriod)
        {
            var exchangeItem = new ApiResponse<List<ExchangeItemAnalysisModel>>();
            try
            {
                var execute = await _unitOfWork.ExchangeItemAnalysis.GetAllByPeriodAsync(type,isReceived, dbCode, fromPeriod, toPeriod);
                if (execute.Any())
                {
                    exchangeItem.Result = execute;
                    exchangeItem.StatusCode = StatusCodes.Status200OK;
                    exchangeItem.Success = true;
                    exchangeItem.Message = "Data fetched successfully";
                }
                else
                {
                    exchangeItem.StatusCode = StatusCodes.Status400BadRequest;
                    exchangeItem.Message = "Data fetched unsuccessfully";
                }
            }
            catch (SqlException ex)
            {
                exchangeItem.StatusCode = StatusCodes.Status500InternalServerError;
                exchangeItem.Message = $"Sql Exception : ${ex.Message}";
            }
            catch (Exception ex)
            {
                exchangeItem.StatusCode = StatusCodes.Status500InternalServerError;
                exchangeItem.Message = $"Sql Exception : ${ex.Message}";
            }
            return exchangeItem;
        }
    }
}
