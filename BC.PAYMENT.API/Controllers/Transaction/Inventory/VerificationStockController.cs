using System.ComponentModel.DataAnnotations;
using System.Net;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.Filter;
using BC.PAYMENT.CORE.DTO.Transaction.Inventory.VerificationStock;
using BC.PAYMENT.CORE.Entities.General;
using BC.PAYMENT.CORE.Entities.Transaction.Inventory.VerificationStock;
using BC.PAYMENT.LOGGING;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BC.PAYMENT.API.Controllers.Transaction.Inventory;

public class VerificationStockController(IUnitOfWork unitOfWork) : BaseApiController
{
    [HttpGet]
    [Route("getmaxsequence")]
    public async Task<ApiResponse<int>> GetMaxSequence()
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var maxSeq = await unitOfWork.VerificationStock.GetMaxSequence(credential.DbCode!);
            if (maxSeq > 0)
                return ApiResponse<int>.Builder()
                    .WithResult(maxSeq)
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithMessage("Max Sequence fetched successfully")
                    .Build();
            else
                return ApiResponse<int>.Builder()
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .WithMessage("Max Sequence fetched unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }

    [HttpGet]
    [Route("getrectype/{movType}")]
    public async Task<ApiResponse<BcModels>> GetRecTypesAsync([Required] string movType)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var execute = await unitOfWork.VerificationStock.GetRecTypes(credential.DbCode!, movType);
            if (execute != null)
                return ApiResponse<BcModels>.Builder()
                    .WithResult(execute)
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithMessage("Rectype fetched successfully")
                    .Build();
            else
                return ApiResponse<BcModels>.Builder()
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .WithMessage("Rectype fetched unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<BcModels>(ex.Message);
        }
    }

    [HttpGet]
    [Route("getitemcostbyitemcode/{itemCode}")]
    public async Task<ApiResponse<double>> GetItemCostByItemCodeAsync([Required] string itemCode)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var execute = await unitOfWork.VerificationStock.GetItemCostAsync(credential.DbCode!, itemCode);
            if (execute != null)
                return ApiResponse<double>.Builder()
                    .WithResult(execute)
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithMessage("Item cost fetched successfully")
                    .Build();
            else
                return ApiResponse<double>.Builder()
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .WithMessage("Item cost fetched unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<double>(ex.Message);
        }
    }

    [HttpPost]
    [Route("getitemstockbylocationandcountingdate")]
    public async Task<ApiResponse<List<VerificationStockModel>>> GetAllStockItemByLocationAndCountingDateAsync(
        [FromBody] PaginatedVerificationStockByDateDto model)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var execute = await unitOfWork.VerificationStock.GetAllStockItemByLocationAndCountingDateAsync(
                credential.DbCode!, model.Location, Convert.ToDateTime(model.FromDate),
                Convert.ToDateTime(model.ToDate), model.Page, model.PageSize);
            if (execute.Any())
                return ApiResponse<List<VerificationStockModel>>.Builder()
                    .WithResult(execute)
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithMessage("Items fetched successfully")
                    .Build();
            else
                return ApiResponse<List<VerificationStockModel>>.Builder()
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .WithMessage("Items fetched unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<VerificationStockModel>>(ex.Message);
        }
    }

    [HttpPost]
    [Route("getitemstockbylocationandcountingperiod")]
    public async Task<ApiResponse<List<VerificationStockModel>>> GetAllStockItemByLocationAndCountingDateAsync(
        [FromBody] PaginatedVerificationStockByPeriodDto model)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var execute = await unitOfWork.VerificationStock.GetAllStockItemByLocationAndCountingPeriodAsync(
                credential.DbCode!, model.Location, model.Month, model.Year, model.Page, model.PageSize);
            if (execute.Any())
                return ApiResponse<List<VerificationStockModel>>.Builder()
                    .WithResult(execute)
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithMessage("Items fetched successfully")
                    .Build();
            else
                return ApiResponse<List<VerificationStockModel>>.Builder()
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .WithMessage("Items fetched unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<VerificationStockModel>>(ex.Message);
        }
    }

    [HttpPost]
    [Route("saveitemafterverification")]
    public async Task<ApiResponse<List<VerificationStockDto>>> SaveRecordStockItemAfterVerify(
        [FromBody] List<VerificationStockDto> model)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var itemModel = model.Select(x => new VerificationStockModel
            {
                ItemCode = x.ItemCode,
                ItemDescription = x.ItemDescription,
                Location = x.Location,
                UnitStock = x.UnitStock,
                Physical = x.Physical,
                OnOrder = x.OnOrder,
                SubTotal = x.SubTotal,
                Quantity = x.Quantity,
                Total = x.Total,
                CreateBy = credential.Username!,
                DbCode = credential.DbCode!
            }).ToList();
            var execute = await unitOfWork.VerificationStock.SaveRecordStockItemAfterVerify(itemModel);
            if (execute > 0)
                return ApiResponse<List<VerificationStockDto>>.Builder()
                    .WithResult(model)
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithMessage("Items saved successfully")
                    .Build();
            else
                return ApiResponse<List<VerificationStockDto>>.Builder()
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .WithMessage("Items saved unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<VerificationStockDto>>(ex.Message);
        }
    }

    [HttpPost]
    [Route("adjustmentinventory")]
    public async Task<ApiResponse<List<AdjustmentInventoryDto>>> AdjustmentInventoryAsync(
        [FromBody] List<AdjustmentInventoryDto> model)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var affectedRow = 0;
            var i = 1;
            foreach (var item in model)
            {
                var sequence = await unitOfWork.VerificationStock.GetMaxSequence(credential.DbCode!);
                var movTypes = await unitOfWork.VerificationStock.GetRecTypes(credential.DbCode!, item.StatusType);
                var movRef =
                    await unitOfWork.Generators.GenerateAdjRefCode(credential.DbCode!, movTypes.MovType,
                        movTypes.RecType);
                var itemCost = await unitOfWork.VerificationStock.GetItemCostAsync(credential.DbCode!, item.ItemCode);
                var inventory = new InventoryAdjustmentModel
                {
                    Sequence = sequence,
                    RecType = movTypes.RecType,
                    MovPrd = Convert.ToInt32(credential.Period),
                    MovRef = movRef.Trim(),
                    MovLine = i.ToString("D4"),
                    Location = item.Location,
                    ItemCode = item.ItemCode,
                    MovDate = DateTime.Today.ToShortDateString(),
                    StatusInv = "80",
                    IRStat = movTypes.MovType,
                    BatchNo = sequence,
                    BatchLine = i.ToString("D5"),
                    LineRef = "",
                    Quantity = item.AdjustQty,
                    Cost = itemCost,
                    MovUnits = "1",
                    MovType = item.StatusType,
                    UpdatePhysical = DateTime.Today.ToString("yyyyMMdd"),
                    UpdateOrder = "0",
                    AllocRef = "",
                    AccountCode = "",
                    AssetCode = "",
                    AnalM0 = "",
                    AnalM1 = "",
                    AnalM2 = "",
                    AnalM3 = "",
                    AnalM4 = "",
                    AnalM5 = "",
                    AnalM6 = "",
                    AnalM7 = "",
                    AnalM8 = "",
                    AnalM9 = "",
                    OrigLineNo = "",
                    IdEntered = credential.Username!,
                    IdAlloc = ""
                };
                affectedRow += await unitOfWork.VerificationStock.AdjustInventory(inventory);
            }

            if (affectedRow > 0)
                return ApiResponse<List<AdjustmentInventoryDto>>.Builder()
                    .WithResult(model)
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithMessage("Adjustment Inventory saved successfully")
                    .Build();
            else
                return ApiResponse<List<AdjustmentInventoryDto>>.Builder()
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .WithMessage("Adjustment Inventory saved unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<AdjustmentInventoryDto>>(ex.Message);
        }
    }
}