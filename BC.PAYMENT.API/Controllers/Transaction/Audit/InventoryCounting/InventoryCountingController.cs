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

namespace BC.PAYMENT.API.Controllers.Transaction.Audit.InventoryCounting;

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
        try
        {
            var inventoryCountings =
                await _unitOfWork.StockInventoryCounting.GetInventoryCountingAsync(credential.DbCode!);
            if (inventoryCountings.Any())
                return ApiResponse<List<StockInventoryCountingModel>>.Builder()
                    .WithResult(inventoryCountings)
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithMessage("Inventory Countings fetched successfully")
                    .Build();
            else
                return ApiResponse<List<StockInventoryCountingModel>>.Builder()
                    .WithStatusCode(StatusCodes.Status400BadRequest)
                    .WithMessage("Inventory Countings fetched unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<StockInventoryCountingModel>>(ex.Message);
        }
    }

    [HttpPost]
    [Route("addnewstockcounting")]
    public async Task<ApiResponse<InventoryCountingDto>> AddNewInventoryCountingAsync(
        [FromBody] InventoryCountingDto model)
    {
        var credential = Common.DecodeJwt(User);
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
            if (affectedRow > 0)
                return ApiResponse<InventoryCountingDto>.Builder()
                    .WithResult(model)
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithMessage("Inventory Countings added successfully")
                    .Build();
            else
                return ApiResponse<InventoryCountingDto>.Builder()
                    .WithStatusCode(StatusCodes.Status400BadRequest)
                    .WithMessage("Inventory Countings added unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<InventoryCountingDto>(ex.Message);
        }
    }

    [HttpPut]
    [Route("updatestockcounting")]
    public async Task<ApiResponse<InventoryCountingDto>> UpdateInventoryCountingAsync(
        [FromBody] InventoryCountingUpdateDto model)
    {
        var credential = Common.DecodeJwt(User);
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
            if (affectedRow > 0)
                return ApiResponse<InventoryCountingDto>.Builder()
                    .WithResult(model)
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithMessage("Inventory Countings updated successfully")
                    .Build();
            else
                return ApiResponse<InventoryCountingDto>.Builder()
                    .WithStatusCode(StatusCodes.Status400BadRequest)
                    .WithMessage("Inventory Countings updated unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<InventoryCountingDto>(ex.Message);
        }
    }

    [HttpDelete]
    [Route("deletestockcounting/{stockId}")]
    public async Task<ApiResponse<string>> DeleteInventoryCountingAsync([Required] string stockId)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var affectedRow =
                await _unitOfWork.StockInventoryCounting.DeleteInventoryCountingAsync(stockId);
            if (affectedRow > 0)
                return ApiResponse<string>.Builder()
                    .WithResult(stockId)
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithMessage("Inventory Countings deleted successfully")
                    .Build();
            else
                return ApiResponse<string>.Builder()
                    .WithStatusCode(StatusCodes.Status400BadRequest)
                    .WithMessage("Inventory Countings deleted unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<string>(ex.Message);
        }
    }
}