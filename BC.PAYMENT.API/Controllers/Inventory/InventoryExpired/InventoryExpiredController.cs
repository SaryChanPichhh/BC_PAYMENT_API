using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.Filter;
using BC.PAYMENT.CORE.DTO.Inventory;
using BC.PAYMENT.CORE.Entities.Inventory.InventoryExpired;
using BC.PAYMENT.CORE.Entities.Inventory.InventoryReport;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BC.PAYMENT.API.Controllers.Inventory.InventoryExpired;

public class InventoryExpiredController : BaseApiController
{
    private readonly IUnitOfWork _unitOfWork;
    private const string IMAGE_PATH = @"D:\BC Payment\Photos\BC PHOTOS";

    public InventoryExpiredController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    [Route("getinventoryexpired")]
    public async Task<ApiResponse<List<InventoryExpiredModel>>> GetInventoryReceiveAsync(InventoryExpiredDto dto)
    {
        try
        {
            var execute = await _unitOfWork.InventoryExpired.GetItemExpiredAsync(dto.DbCode, dto.Location);
            execute.ForEach(x =>
            {
                x.ImagePath = string.IsNullOrEmpty(x.ItemCode) ? null : $"{IMAGE_PATH}\\{x.ItemCode}.jpg";
            });
            if (execute.Any())
                return ApiResponse<List<InventoryExpiredModel>>.Builder()
                    .WithResult(execute)
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithMessage("Inventory Expired fetched successfully")
                    .Build();
            else
                return ApiResponse<List<InventoryExpiredModel>>.Builder()
                    .WithStatusCode(StatusCodes.Status404NotFound)
                    .WithMessage("Inventory Expired fetched unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<InventoryExpiredModel>>(ex.Message);
        }
    }

    [HttpGet]
    [Route("getinventorynearingexpired")]
    public async Task<ApiResponse<List<InventoryExpiredModel>>> GetItemExpiredSoonAsync(InventoryExpiredDto dto)
    {
        try
        {
            var execute = await _unitOfWork.InventoryExpired.GetItemExpiredSoonAsync(dto.DbCode, dto.Location);
            execute.ForEach(x =>
            {
                x.ImagePath = string.IsNullOrEmpty(x.ItemCode) ? null : $"{IMAGE_PATH}\\{x.ItemCode}.jpg";
            });
            if (execute.Any())
                return ApiResponse<List<InventoryExpiredModel>>.Builder()
                    .WithResult(execute)
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithMessage("Inventory Nearing Expiration fetched successfully")
                    .Build();
            else
                return ApiResponse<List<InventoryExpiredModel>>.Builder()
                    .WithResult(new List<InventoryExpiredModel>())
                    .WithStatusCode(StatusCodes.Status404NotFound)
                    .WithMessage("Inventory Nearing Expiration fetched unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<InventoryExpiredModel>>(ex.Message);
        }
    }
}