using System.ComponentModel.DataAnnotations;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.CommondityExchange.RepairItem;
using BC.PAYMENT.CORE.Entities.Preset.ItemTransaction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BC.PAYMENT.API.Controllers.Preset.ItemTransaction
{
    public class ItemTransactionAnalysisController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public ItemTransactionAnalysisController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpPost]
        [Route("getitemtransaction")]
        public async Task<ApiResponse<List<ItemTransactionAnalysisModel>>> GetAllItemAfterRepairerReceivedAsync( [Required] string itemCode, [Required] int fromMov,[Required]int toMov)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var execute = await _unitOfWork.ItemTransactionAnalysis.GetItemTransactionAsync(fromMov,toMov, itemCode);
                if (execute.Any())
                {
                    return ApiResponse<List<ItemTransactionAnalysisModel>>.Builder()
                        .WithResult(execute)
                        .WithStatusCode(StatusCodes.Status200OK)
                        .WithMessage("Item transaction fetched successfully")
                        .Build();
                }
                else
                {
                    return ApiResponse<List<ItemTransactionAnalysisModel>>.Builder()
                        .WithStatusCode(StatusCodes.Status400BadRequest)
                        .WithMessage("Item transaction fetched unsuccessfully")
                        .Build();
                }

            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<ItemTransactionAnalysisModel>>(ex.Message);
            }
        }
        
        [HttpPost]
        [Route("getitembyperiodanditems")]
        public async Task<ApiResponse<List<string>>> GetItemByPeriodAndItemsAsync( [Required] List<string> itemCode, [Required] int fromMov,[Required]int toMov)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var execute = await _unitOfWork.ItemTransactionAnalysis.GetItemByPeriodAndItemsAsync(credential.DbCode,fromMov,toMov, itemCode);
                if (execute.Any())
                {
                    return ApiResponse<List<string>>.Builder()
                        .WithResult(execute)
                        .WithStatusCode(StatusCodes.Status200OK)
                        .WithMessage("Items fetched successfully")
                        .Build();
                }
                else
                {
                    return ApiResponse<List<string>>.Builder()
                        .WithStatusCode(StatusCodes.Status400BadRequest)
                        .WithMessage("Items fetched unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<string>>(ex.Message);
            }
        }
        
        [HttpPost]
        [Route("getallitembyperiod")]
        public async Task<ApiResponse<List<string>>> GetAllItemByPeriodAsync( [Required] int fromMov,[Required]int toMov)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var execute = await _unitOfWork.ItemTransactionAnalysis.GetAllItemByPeriodAsync(credential.DbCode,fromMov,toMov);
                if (execute.Any())
                {
                    return ApiResponse<List<string>>.Builder()
                        .WithResult(execute)
                        .WithStatusCode(StatusCodes.Status200OK)
                        .WithMessage("Items fetched successfully")
                        .Build();
                }
                else
                {
                    return ApiResponse<List<string>>.Builder()
                        .WithStatusCode(StatusCodes.Status400BadRequest)
                        .WithMessage("Items fetched unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<string>>(ex.Message);
            }
        }

    }
}
