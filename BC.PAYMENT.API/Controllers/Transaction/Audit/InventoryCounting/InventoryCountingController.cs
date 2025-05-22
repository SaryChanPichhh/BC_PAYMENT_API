using System.ComponentModel.DataAnnotations;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.Transaction.Audit.InventoryCounting;
using BC.PAYMENT.CORE.Entities.Transaction.Audit.InventoryCounting;
using BC.PAYMENT.LOGGING;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BC.PAYMENT.API.Controllers.Transaction.Audit.InventoryCounting
{
    public class InventoryCountingController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public InventoryCountingController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("getinventorycounting")]
        public async Task<ApiResponse<List<StockInventoryCountingModel>>> GetInventoryCountingAsync()
        {
            var credential = Common.DecodeJwt(User);
            var inventoryCounting = new ApiResponse<List<StockInventoryCountingModel>>();
            try
            {
                var inventoryCountings =
                    await _unitOfWork.StockInventoryCounting.GetInventoryCountingAsync(credential.DbCode!);
                if (inventoryCountings.Any())
                {
                    inventoryCounting.Result = inventoryCountings;
                    inventoryCounting.StatusCode = StatusCodes.Status200OK;
                    inventoryCounting.Message = "Inventory Countings fetched successfully";
                    inventoryCounting.Success = true;
                }
                else
                {
                    inventoryCounting.StatusCode = StatusCodes.Status400BadRequest;
                    inventoryCounting.Message = "Inventory Countings fetched unsuccessfully";
                    inventoryCounting.Success = false;
                }
            }
            catch (SqlException e)
            {
                inventoryCounting.StatusCode = StatusCodes.Status400BadRequest;
                inventoryCounting.Message = $"Sql Exception : {e.Message}";
                Logger.Instance.Error("Sql Exception",e);
            }
            catch (Exception e)
            {
                inventoryCounting.StatusCode = StatusCodes.Status400BadRequest;
                inventoryCounting.Message = $"Error Exception : {e.Message}";
                Logger.Instance.Error("Error Exception", e);
            }

            return inventoryCounting;
        }
        
        [HttpPost]
        [Route("addnewstockcounting")]
        public async Task<ApiResponse<InventoryCountingDto>> AddNewInventoryCountingAsync([FromBody] InventoryCountingDto model)
        {
            var credential = Common.DecodeJwt(User);
            var inventoryCounting = new ApiResponse<InventoryCountingDto>();
            try
            {
                var inventoryCountingModel = new StockInventoryCountingModel
                {
                    DbCode = credential.DbCode,
                    CreateBy = credential.Username,
                    Stock = model.WarehouseCode,
                    Warehouse = model.Warehouse,
                    StockController = model.StockController,
                    Participation = model.Participation,
                    Period = model.Period,
                    CountingDate = model.CountingDate
                };
                var affectedRow =
                    await _unitOfWork.StockInventoryCounting.InsertInventoryCountingAsync(inventoryCountingModel);
                if (affectedRow > 0 )
                {
                    inventoryCounting.Result = model;
                    inventoryCounting.StatusCode = StatusCodes.Status200OK;
                    inventoryCounting.Message = "Inventory Countings added successfully";
                    inventoryCounting.Success = true;
                }
                else
                {
                    inventoryCounting.StatusCode = StatusCodes.Status400BadRequest;
                    inventoryCounting.Message = "Inventory Countings added unsuccessfully";
                    inventoryCounting.Success = false;
                }
            }
            catch (SqlException e)
            {
                inventoryCounting.StatusCode = StatusCodes.Status400BadRequest;
                inventoryCounting.Message = $"Sql Exception : {e.Message}";
                Logger.Instance.Error("Sql Exception",e);
            }
            catch (Exception e)
            {
                inventoryCounting.StatusCode = StatusCodes.Status400BadRequest;
                inventoryCounting.Message = $"Error Exception : {e.Message}";
                Logger.Instance.Error("Error Exception", e);
            }

            return inventoryCounting;
        }
        
        [HttpPut]
        [Route("updatestockcounting")]
        public async Task<ApiResponse<InventoryCountingDto>> UpdateInventoryCountingAsync([FromBody] InventoryCountingUpdateDto model)
        {
            var credential = Common.DecodeJwt(User);
            var inventoryCounting = new ApiResponse<InventoryCountingDto>();
            try
            {
                var inventoryCountingModel = new StockInventoryCountingModel
                {
                    DbCode = credential.DbCode,
                    CreateBy = credential.Username,
                    Stock = model.WarehouseCode,
                    Warehouse = model.Warehouse,
                    StockController = model.StockController,
                    Participation = model.Participation,
                    Period = model.Period,
                    CountingDate = model.CountingDate,
                    StockId = model.StockId
                };
                var affectedRow =
                    await _unitOfWork.StockInventoryCounting.UpdateInventoryCountingAsync(inventoryCountingModel);
                if (affectedRow > 0 )
                {
                    inventoryCounting.Result = model;
                    inventoryCounting.StatusCode = StatusCodes.Status200OK;
                    inventoryCounting.Message = "Inventory Countings updated successfully";
                    inventoryCounting.Success = true;
                }
                else
                {
                    inventoryCounting.StatusCode = StatusCodes.Status400BadRequest;
                    inventoryCounting.Message = "Inventory Countings updated unsuccessfully";
                    inventoryCounting.Success = false;
                }
            }
            catch (SqlException e)
            {
                inventoryCounting.StatusCode = StatusCodes.Status400BadRequest;
                inventoryCounting.Message = $"Sql Exception : {e.Message}";
                Logger.Instance.Error("Sql Exception",e);
            }
            catch (Exception e)
            {
                inventoryCounting.StatusCode = StatusCodes.Status400BadRequest;
                inventoryCounting.Message = $"Error Exception : {e.Message}";
                Logger.Instance.Error("Error Exception", e);
            }

            return inventoryCounting;
        }
        
        [HttpDelete]
        [Route("deletestockcounting/{stockId}")]
        public async Task<ApiResponse<string>> DeleteInventoryCountingAsync([Required] string stockId)
        {
            var credential = Common.DecodeJwt(User);
            var inventoryCounting = new ApiResponse<string>();
            try
            {
                var affectedRow =
                    await _unitOfWork.StockInventoryCounting.DeleteInventoryCountingAsync(stockId);
                if (affectedRow > 0 )
                {
                    inventoryCounting.Result = stockId;
                    inventoryCounting.StatusCode = StatusCodes.Status200OK;
                    inventoryCounting.Message = "Inventory Countings deleted successfully";
                    inventoryCounting.Success = true;
                }
                else
                {
                    inventoryCounting.StatusCode = StatusCodes.Status400BadRequest;
                    inventoryCounting.Message = "Inventory Countings deleted unsuccessfully";
                    inventoryCounting.Success = false;
                }
            }
            catch (SqlException e)
            {
                inventoryCounting.StatusCode = StatusCodes.Status400BadRequest;
                inventoryCounting.Message = $"Sql Exception : {e.Message}";
                Logger.Instance.Error("Sql Exception",e);
            }
            catch (Exception e)
            {
                inventoryCounting.StatusCode = StatusCodes.Status400BadRequest;
                inventoryCounting.Message = $"Error Exception : {e.Message}";
                Logger.Instance.Error("Error Exception", e);
            }

            return inventoryCounting;
        }
    }
}
