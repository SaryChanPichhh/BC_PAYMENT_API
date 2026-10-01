using Azure.Core;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.Inventory;
using BC.PAYMENT.CORE.Entities.Inventory;
using BC.PAYMENT.INFRASTRUCTURE.Repository.General;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;

namespace BC.PAYMENT.API.Controllers.Inventory;

public class WarehousePresetController(IUnitOfWork unitOfWork) : BaseApiController
{
    [HttpGet]
    [Route("getwarehousepresets")]
    public async Task<ApiResponse<List<WarehousePresetModel>>> GetWarehousePresetsAsync()
    {
        try
        {
            var presets = await unitOfWork.WarehousePreset.GetWarehousePresetsAsync();
            if (presets.Any())
                return ApiResponse<List<WarehousePresetModel>>.Builder()
                    .WithResult(presets)
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithMessage("Warehouse presets fetched successfully")
                    .Build();
            else
                return ApiResponse<List<WarehousePresetModel>>.Builder()
                    .WithStatusCode(StatusCodes.Status404NotFound)
                    .WithMessage("Warehouse presets fetched unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<WarehousePresetModel>>(ex.Message);
        }
    }

    [HttpPost]
    [Route("createwarehousepreset")]
    public async Task<ApiResponse<int>> CreateWarehouseAsync(WarehousePresetDto dto)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var model = new WarehousePresetModel
            {
                Name = dto.Name,
                Note = dto.Note,
                Status = dto.Status,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = credential.Username
            };
            var affectedRow = await unitOfWork.WarehousePreset.CreateWarehouseAsync(model);
            if (affectedRow > 0)
                return ApiResponse<int>.Builder()
                    .WithResult(affectedRow)
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithMessage("Warehouse presets added successfully")
                    .Build();
            else
                return ApiResponse<int>.Builder()
                    .WithStatusCode(StatusCodes.Status404NotFound)
                    .WithMessage("Warehouse presets added unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }


    [HttpDelete]
    [Route("deletewarehousepreset/{id}")]
    public async Task<ApiResponse<int>> DeleteWarehouseAsync(int id)
    {
        try
        {
            var affectedRow = await unitOfWork.WarehousePreset.DeleteWarehouseAsync(id);
            if (affectedRow > 0)
                return ApiResponse<int>.Builder()
                    .WithResult(affectedRow)
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithMessage("Warehouse presets deleted successfully")
                    .Build();
            else
                return ApiResponse<int>.Builder()
                    .WithStatusCode(StatusCodes.Status404NotFound)
                    .WithMessage("Warehouse presets deleted unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }

    [HttpPut]
    [Route("deletewarehousepreset")]
    public async Task<ApiResponse<int>> UpdateWarehouseAsync(WarehousePresetPutDto dto)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var model = new WarehousePresetModel
            {
                Id = dto.Id,
                Name = dto.Name,
                Note = dto.Note,
                Status = dto.Status,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = credential.Username
            };
            var affectedRow = await unitOfWork.WarehousePreset.UpdateWarehouseAsync(model);
            if (affectedRow > 0)
                return ApiResponse<int>.Builder()
                    .WithResult(affectedRow)
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithMessage("Warehouse presets updated successfully")
                    .Build();
            else
                return ApiResponse<int>.Builder()
                    .WithStatusCode(StatusCodes.Status404NotFound)
                    .WithMessage("Warehouse presets updated unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }
}