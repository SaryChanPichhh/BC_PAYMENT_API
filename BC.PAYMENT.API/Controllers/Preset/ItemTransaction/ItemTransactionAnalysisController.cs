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
            var itemTransaction = new ApiResponse<List<ItemTransactionAnalysisModel>>();
            try
            {
                var execute = await _unitOfWork.ItemTransactionAnalysis.GetItemTransactionAsync(fromMov,toMov, itemCode);
                if (execute.Any())
                {
                    itemTransaction.Result = execute;
                    itemTransaction.StatusCode = StatusCodes.Status200OK;
                    itemTransaction.Success = true;
                    itemTransaction.Message = "Item transaction fetched successfully";
                }
                else
                {
                    itemTransaction.StatusCode = StatusCodes.Status400BadRequest;
                    itemTransaction.Message = "Item transaction fetched unsuccessfully";
                }

            }
            catch (SqlException ex)
            {
                itemTransaction.StatusCode = StatusCodes.Status500InternalServerError;
                itemTransaction.Message = $"Sql Exception : ${ex.Message}";
            }
            catch (Exception ex)
            {
                itemTransaction.StatusCode = StatusCodes.Status500InternalServerError;
                itemTransaction.Message = $"Sql Exception : ${ex.Message}";
            }
            return itemTransaction;
        }
        
        [HttpPost]
        [Route("getitembyperiodanditems")]
        public async Task<ApiResponse<List<string>>> GetItemByPeriodAndItemsAsync( [Required] List<string> itemCode, [Required] int fromMov,[Required]int toMov)
        {
            var credential = Common.DecodeJwt(User);
            var itemTransaction = new ApiResponse<List<string>>();
            try
            {
                var execute = await _unitOfWork.ItemTransactionAnalysis.GetItemByPeriodAndItemsAsync(credential.DbCode,fromMov,toMov, itemCode);
                if (execute.Any())
                {
                    itemTransaction.Result = execute;
                    itemTransaction.StatusCode = StatusCodes.Status200OK;
                    itemTransaction.Success = true;
                    itemTransaction.Message = "Items fetched successfully";
                }
                else
                {
                    itemTransaction.StatusCode = StatusCodes.Status400BadRequest;
                    itemTransaction.Message = "Items fetched unsuccessfully";
                }
            }
            catch (SqlException ex)
            {
                itemTransaction.StatusCode = StatusCodes.Status500InternalServerError;
                itemTransaction.Message = $"Sql Exception : ${ex.Message}";
            }
            catch (Exception ex)
            {
                itemTransaction.StatusCode = StatusCodes.Status500InternalServerError;
                itemTransaction.Message = $"Sql Exception : ${ex.Message}";
            }
            return itemTransaction;
        }
        
        [HttpPost]
        [Route("getallitembyperiod")]
        public async Task<ApiResponse<List<string>>> GetAllItemByPeriodAsync( [Required] int fromMov,[Required]int toMov)
        {
            var credential = Common.DecodeJwt(User);
            var itemTransaction = new ApiResponse<List<string>>();
            try
            {
                var execute = await _unitOfWork.ItemTransactionAnalysis.GetAllItemByPeriodAsync(credential.DbCode,fromMov,toMov);
                if (execute.Any())
                {
                    itemTransaction.Result = execute;
                    itemTransaction.StatusCode = StatusCodes.Status200OK;
                    itemTransaction.Success = true;
                    itemTransaction.Message = "Items fetched successfully";
                }
                else
                {
                    itemTransaction.StatusCode = StatusCodes.Status400BadRequest;
                    itemTransaction.Message = "Items fetched unsuccessfully";
                }
            }
            catch (SqlException ex)
            {
                itemTransaction.StatusCode = StatusCodes.Status500InternalServerError;
                itemTransaction.Message = $"Sql Exception : ${ex.Message}";
            }
            catch (Exception ex)
            {
                itemTransaction.StatusCode = StatusCodes.Status500InternalServerError;
                itemTransaction.Message = $"Sql Exception : ${ex.Message}";
            }
            return itemTransaction;
        }

    }
}
