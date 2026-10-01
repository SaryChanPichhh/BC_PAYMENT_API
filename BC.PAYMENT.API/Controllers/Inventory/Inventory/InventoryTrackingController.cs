using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.Inventory;
using BC.PAYMENT.CORE.Entities.Inventory.Inventory;
using BC.PAYMENT.CORE.Entities.Inventory.InventoryExpired;
using BC.PAYMENT.CORE.Entities.Inventory.InventoryReport;
using BC.PAYMENT.CORE.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BC.PAYMENT.API.Controllers.Inventory.Inventory;

public class InventoryTrackingController : BaseApiController
{
    private readonly IUnitOfWork _unitOfWork;

    public InventoryTrackingController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    #region Inventory Tracking

    [HttpGet]
    [Route("getinventorytracking/{dbCode}")]
    public async Task<ApiResponse<List<InventoryTrackingWarehouseModel>>>
        GetInventoryTrackingWarehouseListByDbCodeAsync(string dbCode)
    {
        try
        {
            var execute = await _unitOfWork.InventoryTracking.GetInventoryTrackingWarehouseListByDbCodeAsync(dbCode);
            if (execute.Any())
                return ApiResponse<List<InventoryTrackingWarehouseModel>>.Builder()
                    .WithResult(execute)
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithMessage("Inventory Tracking fetched successfully")
                    .Build();
            else
                return ApiResponse<List<InventoryTrackingWarehouseModel>>.Builder()
                    .WithResult(new List<InventoryTrackingWarehouseModel>())
                    .WithStatusCode(StatusCodes.Status404NotFound)
                    .WithMessage("Inventory Tracking fetched unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<InventoryTrackingWarehouseModel>>(ex.Message);
        }
    }

    [HttpDelete]
    [Route("deleteinventorytracking/{Id}")]
    public async Task<ApiResponse<int>> DeleteInventoryTrackingAsync(string Id)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var affectedRow = await _unitOfWork.InventoryTracking.DeleteInventoryTrackingAsync(credential.DbCode, Id);
            if (affectedRow > 0)
                return ApiResponse<int>.Builder()
                    .WithResult(affectedRow)
                    .WithStatusCode(StatusCodes.Status201Created)
                    .WithMessage("Inventory Tracking deleted successfully")
                    .Build();
            else
                return ApiResponse<int>.Builder()
                    .WithStatusCode(StatusCodes.Status404NotFound)
                    .WithMessage("Inventory Tracking deleted unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }

    [HttpPut]
    [Route("updateinventorytracking")]
    public async Task<ApiResponse<int>> UpdateInventoryTrackingAsync([FromBody] InventoryTrackingPutDto model)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var modelToUpdate = new InventoryTrackingWarehouseModel
            {
                Id = Convert.ToInt32(model.Id),
                DbCode = credential.DbCode,
                Warehouse = model.Warehouse,
                Description = model.Description,
                InventoryTrackingTypes = model.InventoryTrackingTypes,
                SelectedTotal = model.SelectedTotal,
                UpdatedBy = credential.Username,
                UpdatedDate = DateTime.Today
            };
            var affectedRow = await _unitOfWork.InventoryTracking.UpdateInventoryTrackingAsync(modelToUpdate);
            if (affectedRow > 0)
                return ApiResponse<int>.Builder()
                    .WithResult(affectedRow)
                    .WithStatusCode(StatusCodes.Status201Created)
                    .WithMessage("Inventory Tracking updated successfully")
                    .Build();
            else
                return ApiResponse<int>.Builder()
                    .WithStatusCode(StatusCodes.Status404NotFound)
                    .WithMessage("Inventory Tracking updated unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }

    [HttpPost]
    [Route("addinventorytracking")]
    public async Task<ApiResponse<int>> AddInventoryTrackingAsync(InventoryTrackingDto model)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var modelToPost = new InventoryTrackingWarehouseModel
            {
                DbCode = credential.DbCode,
                Warehouse = model.Warehouse,
                Description = model.Description,
                InventoryTrackingTypes = model.InventoryTrackingTypes,
                SelectedTotal = model.SelectedTotal,
                CreatedBy = credential.Username,
                CreatedDate = DateTime.Today
            };
            var affectedRow = await _unitOfWork.InventoryTracking.AddInventoryTrackingAsync(modelToPost);
            if (affectedRow > 0)
                return ApiResponse<int>.Builder()
                    .WithResult(affectedRow)
                    .WithStatusCode(StatusCodes.Status201Created)
                    .WithMessage("Inventory Tracking added successfully")
                    .Build();
            else
                return ApiResponse<int>.Builder()
                    .WithStatusCode(StatusCodes.Status404NotFound)
                    .WithMessage("Inventory Tracking added unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }

    #endregion

    #region InBound Good Management

    [HttpPost]
    [Route("addwarehouesdata")]
    public async Task<ApiResponse<int>> AddWarehouseDataAsync(InBoundGoodsDto model)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var modelToPost = new InventoryTrackingWarehouseDataModel
            {
                DbCode = credential.DbCode,
                Warehouse = model.Warehouse,
                ItemDescription = model.ItemDescription,
                InventoryTrackingTypes = model.InventoryTrackingTypes,
                TransactionDate = model.TransactionDate,
                CreatedBy = credential.Username,
                CreatedDate = DateTime.Now,
                ItemCode = model.ItemCode,
                Quantity = model.Quantity
            };
            var affectedRow = await _unitOfWork.InventoryTracking.AddWarehouseData(modelToPost);
            if (affectedRow > 0)
                return ApiResponse<int>.Builder()
                    .WithResult(affectedRow)
                    .WithStatusCode(StatusCodes.Status201Created)
                    .WithMessage("Warehouse data added successfully")
                    .Build();
            else
                return ApiResponse<int>.Builder()
                    .WithStatusCode(StatusCodes.Status404NotFound)
                    .WithMessage("Warehouse data added unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }

    [HttpDelete]
    [Route("deletewarehouesdata/{id}")]
    public async Task<ApiResponse<int>> DeleteWarehouseData(string id)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var affectedRow = await _unitOfWork.InventoryTracking.DeleteWarehouseData(credential.DbCode, id);
            if (affectedRow > 0)
                return ApiResponse<int>.Builder()
                    .WithResult(affectedRow)
                    .WithStatusCode(StatusCodes.Status201Created)
                    .WithMessage("Warehouse data deleted successfully")
                    .Build();
            else
                return ApiResponse<int>.Builder()
                    .WithStatusCode(StatusCodes.Status404NotFound)
                    .WithMessage("Warehouse data deleted unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }

    [HttpPut]
    [Route("updatewarehouesdata")]
    public async Task<ApiResponse<int>> UpdateWarehouseData(InBoundGoodsPutDto dto)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var model = new InventoryTrackingWarehouseDataModel
            {
                Id = dto.Id,
                DbCode = credential.DbCode,
                ItemCode = dto.ItemCode,
                Warehouse = dto.Warehouse,
                Quantity = dto.Quantity,
                ItemDescription = dto.ItemDescription,
                TransactionDate = dto.TransactionDate,
                InventoryTrackingTypes = dto.InventoryTrackingTypes,
                CreatedBy = credential.Username,
                CreatedDate = DateTime.Today
            };

            var affectedRow = await _unitOfWork.InventoryTracking.UpdateWarehouseData(model);
            if (affectedRow > 0)
                return ApiResponse<int>.Builder()
                    .WithResult(affectedRow)
                    .WithStatusCode(StatusCodes.Status201Created)
                    .WithMessage("Warehouse data updated successfully")
                    .Build();
            else
                return ApiResponse<int>.Builder()
                    .WithStatusCode(StatusCodes.Status404NotFound)
                    .WithMessage("Warehouse data updated unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }

    [HttpGet]
    [Route("getinventorywarehousedata")]
    public async Task<ApiResponse<List<InventoryTrackingWarehouseDataModel>>>
        GetInventoryTrackingWarehouseDataListByDbCodeAndWarehouseAsync(SummaryTrackingDto dto)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            InventoryTrackingTypes TrackingType(string inventoryTrackingType)
            {
                return inventoryTrackingType switch
                {
                    "Plus" => InventoryTrackingTypes.Plus,
                    "Subtract" => InventoryTrackingTypes.Subtract,
                    "Transfer" => InventoryTrackingTypes.Subtract,
                    _ => throw new ArgumentException("Invalid inventory tracking type")
                };
            }

            var execute =
                await _unitOfWork.InventoryTracking.GetInventoryTrackingWarehouseDataListByDbCodeAndWarehouseAsync(
                    credential.DbCode, dto.Location, TrackingType(dto.InventoryTrackingType));
            if (execute.Any())
                return ApiResponse<List<InventoryTrackingWarehouseDataModel>>.Builder()
                    .WithResult(execute)
                    .WithStatusCode(StatusCodes.Status201Created)
                    .WithMessage("Warehouse data fetched successfully")
                    .Build();
            else
                return ApiResponse<List<InventoryTrackingWarehouseDataModel>>.Builder()
                    .WithStatusCode(StatusCodes.Status404NotFound)
                    .WithMessage("Warehouse data fetched unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<InventoryTrackingWarehouseDataModel>>(ex.Message);
        }
    }

    #endregion

    #region Summary Inventory Tracking

    [HttpGet]
    [Route("getsummaryinventorytracking")]
    public async Task<ApiResponse<List<InventoryTrackingWarehouseDataModel>>> GetSummaryInventoryTrackingByDbCodeAsync()
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var execute =
                await _unitOfWork.InventoryTracking.GetSummaryInventoryTrackingByDbCodeAsync(credential.DbCode);
            if (execute.Any())
                return ApiResponse<List<InventoryTrackingWarehouseDataModel>>.Builder()
                    .WithResult(execute)
                    .WithStatusCode(StatusCodes.Status201Created)
                    .WithMessage("Warehouse data fetched successfully")
                    .Build();
            else
                return ApiResponse<List<InventoryTrackingWarehouseDataModel>>.Builder()
                    .WithStatusCode(StatusCodes.Status404NotFound)
                    .WithMessage("Warehouse data fetched unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<InventoryTrackingWarehouseDataModel>>(ex.Message);
        }
    }

    [HttpGet]
    [Route("getsummaryinventorytrackingbydate")]
    public async Task<ApiResponse<List<InventoryTrackingWarehouseDataModel>>>
        GetSummaryInventoryTrackingByDbCodeAsync(string date)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var execute =
                await _unitOfWork.InventoryTracking.GetSummaryInventoryTrackingByDbCodeAndDateAsync(credential.DbCode,
                    Convert.ToDateTime(date));
            if (execute.Any())
                return ApiResponse<List<InventoryTrackingWarehouseDataModel>>.Builder()
                    .WithResult(execute)
                    .WithStatusCode(StatusCodes.Status201Created)
                    .WithMessage("Warehouse data fetched successfully")
                    .Build();
            else
                return ApiResponse<List<InventoryTrackingWarehouseDataModel>>.Builder()
                    .WithStatusCode(StatusCodes.Status404NotFound)
                    .WithMessage("Warehouse data fetched unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<InventoryTrackingWarehouseDataModel>>(ex.Message);
        }
    }

    [HttpPost]
    [Route("closingtrackinginventory")]
    public async Task<ApiResponse<int>> ClosingTrackingInventoryAsync(List<ClosingTrackingInventoryDto> model)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var closingModel = model.Select(x => new InventoryTrackingWarehouseDataModel
                {
                    DbCode = credential.DbCode,
                    ItemCode = x.ItemCode,
                    Warehouse = x.Warehouse,
                    Quantity = x.Quantity,
                    CreatedDate = DateTime.Now,
                    CreatedBy = credential.Username
                }).ToList()
                ;
            var execute = await _unitOfWork.InventoryTracking.ClosingTrackingInventoryAsync(closingModel);
            if (string.IsNullOrEmpty(execute))
                return ApiResponse<int>.Builder()
                    .WithResult(1)
                    .WithStatusCode(StatusCodes.Status201Created)
                    .WithMessage("Tracking inventory closed successfully")
                    .Build();
            else
                return ApiResponse<int>.Builder()
                    .WithResult(0)
                    .WithStatusCode(StatusCodes.Status404NotFound)
                    .WithMessage("Tracking inventory closed unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }

    #endregion

    #region Balance Daily

    #endregion
}