using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Mapper;
using BC.PAYMENT.API.MapperHelper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.ClosingInventoryAndInvoice.ClosingInventory;
using BC.PAYMENT.CORE.DTO.ClosingInventoryAndInvoice.ClosingInventory;
using BC.PAYMENT.CORE.Entities.ClosingInventoryAndInvoice.ClosingInventory;
using BC.PAYMENT.CORE.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using static BC.PAYMENT.CORE.DTO.ClosingInventoryAndInvoice.ClosingInventory.ClosingInventoryDto;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BC.PAYMENT.API.Controllers.ClosingInventoryAndInvoice.ClosingInventory;

public class ClosingInventoryController(IClosingInventoryRepository closingInventoryRepository) : BaseApiController
{
    #region Closing Inventory Daily

    [HttpGet]
    [Route("getclosingentryday/{dbCode}/{date}")]
    public async Task<ApiResponse<List<ClosingInventoryResponseDto>>> GetClosingEntryDayAsync(string dbCode,
        string date)
    {
        try
        {
            var execute = await closingInventoryRepository.GetClosingEntryDay(dbCode,
                Convert.ToDateTime(date).ClosingInventoryFormatDateTime());
            var data = execute.Select(x => new ClosingInventoryResponseDto
            {
                ItemCode = x.ItemCode,
                Location = x.Location,
                TransactionDate = Convert.ToDateTime(date),
                OpeningBalance = x.OpeningBalance,
                PurchaseOrder = x.PurchaseOrder ?? 0,
                Order = x.Order ?? 0,
                Sale = x.Sale ?? 0,
                Transfer = x.Transfer ?? 0,
                CreditNote = x.CreditNote ?? 0,
                InventoryAdjustment = x.InventoryAdjustment ?? 0,
                Print = x.Print ?? 0,
                DbCode = dbCode
            }).ToList();

            return ApiResponse<List<ClosingInventoryResponseDto>>.Builder()
                .WithMessage(execute.Any()
                    ? "Closing entries fetched successfully"
                    : "No closing entries found for the specified date")
                .WithStatusCode(execute.Any() ? StatusCodes.Status200OK : StatusCodes.Status404NotFound)
                .WithResult(execute.Any() ? data : null)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<ClosingInventoryResponseDto>>(ex.Message);
        }
    }


    [HttpPost]
    [Route("addnewitem")]
    public async Task<ApiResponse<int>> InsertNewItemAsync(NewItemDto dto)
    {
        try
        {
            var model = MapperDto.ToNewItemModel(dto, User);
            var isExists = await closingInventoryRepository.ExistItem(model);
            if (!isExists)
            {
                var isOutOfStock =
                    await closingInventoryRepository.CheckStockQuantityAsync(dto.DbCode, dto.Location, dto.ItemCode);
                if (!isOutOfStock)
                {
                    var affectedRow = await closingInventoryRepository.InsertNewItem(model);

                    return ApiResponse<int>.Builder()
                        .WithMessage(affectedRow > 0 ? "Item added successfully" : "Item added unsuccessfully")
                        .WithStatusCode(affectedRow > 0 ? StatusCodes.Status200OK : StatusCodes.Status404NotFound)
                        .WithResult(affectedRow > 0 ? affectedRow : 0)
                        .Build();
                }
            }

            return ApiResponse<int>.Builder()
                .WithMessage("Item added unsuccessfully")
                .WithStatusCode(StatusCodes.Status404NotFound)
                .WithResult(0)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }

    [HttpPost]
    [Route("dailyclosinginventory")]
    public async Task<ApiResponse<int>> InsertDailyClosingEntryAsync(List<ClosingInventoryPostDto> dto)
    {
        try
        {
            var model = MapperDto.ToClosingInventoryModel(dto, User);
            var affectedRows =
                await closingInventoryRepository.InsertDailyClosingEntryAsync(model, dto.FirstOrDefault().Status);

            return ApiResponse<int>.Builder()
                .WithMessage(affectedRows > 0 ? "Inventory closed successfully" : "Inventory closed unsuccessfully")
                .WithStatusCode(affectedRows > 0 ? StatusCodes.Status200OK : StatusCodes.Status404NotFound)
                .WithResult(affectedRows > 0 ? affectedRows : 0)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }

    #endregion

    #region Monthly Closing Inventory

    [HttpGet]
    [Route("getdailyinventory/{dbCode}/{date}")]
    public async Task<ApiResponse<List<ClosingInventoryResponseDto>>> GetAllStockBalanceByBranchCode(string dbCode,
        string date)
    {
        try
        {
            var execute = await closingInventoryRepository.GetAllStockBalanceByBranchCode(dbCode,
                Convert.ToDateTime(date).ClosingInventoryFormatDateTime());
            var data = execute.Select(x => new ClosingInventoryResponseDto
            {
                ItemCode = x.ItemCode,
                Location = x.Location,
                TransactionDate = x.ClosingDate,
                OpeningBalance = x.OpeningBalance,
                PurchaseOrder = x.PurchaseOrder ?? 0,
                Order = x.Order ?? 0,
                Sale = x.Sale ?? 0,
                Transfer = x.Transfer ?? 0,
                CreditNote = x.CreditNote ?? 0,
                InventoryAdjustment = x.InventoryAdjustment ?? 0,
                Print = x.Print ?? 0,
                DbCode = dbCode
            }).ToList();

            return ApiResponse<List<ClosingInventoryResponseDto>>.Builder()
                .WithMessage(execute.Any()
                    ? "Closing entries fetched successfully"
                    : "No closing entries found for the specified date")
                .WithStatusCode(execute.Any() ? StatusCodes.Status200OK : StatusCodes.Status404NotFound)
                .WithResult(execute.Any() ? data : null)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<ClosingInventoryResponseDto>>(ex.Message);
        }
    }

    [HttpGet]
    [Route("getdailyinventorybywarehouse")]
    public async Task<ApiResponse<List<ClosingInventoryResponseDto>>> GetAllStockBalanceByBranchCodeAndWarehouseAsync(
        ClosingInventoryPaginatedDto dto)
    {
        try
        {
            var execute = await closingInventoryRepository.GetAllStockBalanceByBranchCode(dto.DbCode,
                Convert.ToDateTime(dto.ClosingDate).ClosingInventoryFormatDateTime());
            var data = execute.Select(x => new ClosingInventoryResponseDto
            {
                ItemCode = x.ItemCode,
                Location = x.Location,
                TransactionDate = x.ClosingDate,
                OpeningBalance = x.OpeningBalance,
                PurchaseOrder = x.PurchaseOrder ?? 0,
                Order = x.Order ?? 0,
                Sale = x.Sale ?? 0,
                Transfer = x.Transfer ?? 0,
                CreditNote = x.CreditNote ?? 0,
                InventoryAdjustment = x.InventoryAdjustment ?? 0,
                Print = x.Print ?? 0,
                DbCode = dto.DbCode
            }).Where(x => x.Location.Equals(dto.Location)).ToList();

            return ApiResponse<List<ClosingInventoryResponseDto>>.Builder()
                .WithMessage(execute.Any()
                    ? "Closing entries fetched successfully"
                    : "Closing entries fetched unsuccessfully")
                .WithStatusCode(execute.Any() ? StatusCodes.Status200OK : StatusCodes.Status404NotFound)
                .WithResult(execute.Any() ? data : null)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<ClosingInventoryResponseDto>>(ex.Message);
        }
    }

    [HttpPost]
    [Route("monthlyclosinginventory")]
    public async Task<ApiResponse<int>> InsertClosingMonthlyAndYearlyAsync(List<MonthlyClosingPostDto> dto)
    {
        try
        {
            var model = MapperDto.ToMonthlyClosingInventoryModel(dto, User);
            var affectedRows =
                await closingInventoryRepository.InsertClosingMonthlyAndYearlyAsync(model, ClosingEntryType.Monthly);

            return ApiResponse<int>.Builder()
                .WithMessage(affectedRows > 0 ? "Inventory closed successfully" : "Inventory closed unsuccessfully")
                .WithStatusCode(affectedRows > 0 ? StatusCodes.Status200OK : StatusCodes.Status404NotFound)
                .WithResult(affectedRows > 0 ? affectedRows : 0)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }

    #endregion

    #region Yearly Closing Inventory

    [HttpGet]
    [Route("getmonthlyinventory/{dbCode}/{date}")]
    public async Task<ApiResponse<List<ClosingInventoryResponseDto>>> GetAllStockBalanceMonthlyByBranchCode(
        string dbCode, string date)
    {
        try
        {
            var execute = await closingInventoryRepository.GetAllStockBalanceMonthlyByBranchCode(dbCode,
                Convert.ToDateTime(date).ClosingInventoryFormatDateTime());
            var data = execute.Select(x => new ClosingInventoryResponseDto
            {
                ItemCode = x.ItemCode,
                Location = x.Location,
                TransactionDate = x.ClosingDate,
                OpeningBalance = x.OpeningBalance,
                PurchaseOrder = x.PurchaseOrder ?? 0,
                Order = x.Order ?? 0,
                Sale = x.Sale ?? 0,
                Transfer = x.Transfer ?? 0,
                CreditNote = x.CreditNote ?? 0,
                InventoryAdjustment = x.InventoryAdjustment ?? 0,
                Print = x.Print ?? 0,
                DbCode = dbCode
            }).OrderBy(x => x.TransactionDate).ToList();

            return ApiResponse<List<ClosingInventoryResponseDto>>.Builder()
                .WithMessage(execute.Any()
                    ? "Closing entries fetched successfully"
                    : "No closing entries found for the specified date")
                .WithStatusCode(execute.Any() ? StatusCodes.Status200OK : StatusCodes.Status404NotFound)
                .WithResult(execute.Any() ? data : null)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<ClosingInventoryResponseDto>>(ex.Message);
        }
    }

    [HttpPost]
    [Route("yearlyclosinginventory")]
    public async Task<ApiResponse<int>> InsertClosingYearlyAsync(List<MonthlyClosingPostDto> dto)
    {
        try
        {
            var model = MapperDto.ToMonthlyClosingInventoryModel(dto, User);
            var affectedRows =
                await closingInventoryRepository.InsertClosingMonthlyAndYearlyAsync(model, ClosingEntryType.Yearly);

            return ApiResponse<int>.Builder()
                .WithMessage(affectedRows > 0 ? "Inventory closed successfully" : "Inventory closed unsuccessfully")
                .WithStatusCode(affectedRows > 0 ? StatusCodes.Status200OK : StatusCodes.Status404NotFound)
                .WithResult(affectedRows > 0 ? affectedRows : 0)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }

    #endregion
}