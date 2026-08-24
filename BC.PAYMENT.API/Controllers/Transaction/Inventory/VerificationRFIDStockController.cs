using System.ComponentModel.DataAnnotations;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.Transaction.Inventory.VerificationStock;
using BC.PAYMENT.CORE.Entities.Transaction.Inventory.VerificationStock;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.LOGGING;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BC.PAYMENT.API.Controllers.Transaction.Inventory
{
    public class VerificationRFIDStockController(IUnitOfWork unitOfWork) : BaseApiController
    {
        [HttpPost]
        [Route("adjustmentinventory")]
        public async Task<ApiResponse<List<AdjustmentInventoryDto>>> AdjustmentInventoryAsync([FromBody] List<AdjustmentInventoryDto> model)
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
                    var movRef = await unitOfWork.Generators.GenerateAdjRefCode(credential.DbCode!,movTypes.MovType, movTypes.RecType);
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
                        IdAlloc = "",
                    };
                    affectedRow += await unitOfWork.VerificationStock.AdjustInventory(inventory);
                }
                if (affectedRow > 0)
                {
                    return ApiResponse<List<AdjustmentInventoryDto>>.Builder()
                        .WithResult(model)
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithMessage("Adjustment Inventory saved successfully")
                        .Build();
                }
                else
                {
                    return ApiResponse<List<AdjustmentInventoryDto>>.Builder()
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithMessage("Adjustment Inventory saved unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<AdjustmentInventoryDto>>(ex.Message);
            }
        }

        [HttpPost]
        [Route("getallstockitembylocationandsubmitcode")]
        public async Task<ApiResponse<List<VerificationStockModel>>> GetAllStockItemByLocationAndSubmitCodeAsync([FromBody] VerificationStockPostDto model)
        {
            try
            {
                var result = await unitOfWork.VerificationRFID.GetAllStockItemByLocationAndSubmitCode(model.DbCode,model.SubmitCode,model.Location);
                if (result.Any())
                {
                    return ApiResponse<List<VerificationStockModel>>.Builder()
                        .WithResult(result)
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithMessage("All stock fetched successfully")
                        .Build();
                }
                else
                {
                    return ApiResponse<List<VerificationStockModel>>.Builder()
                        .WithResult(new List<VerificationStockModel>())
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithMessage("All stock fetched unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<VerificationStockModel>>(ex.Message);
            }
        }
        
        [HttpPost]
        [Route("savestockverification")]
        public async Task<ApiResponse<VerificationRFIDDto>> SaveRecordStockItemAfterVerify([FromBody] VerificationRFIDDto model)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var itemModel = model.StockForVerification.Select(x => new VerificationStockModel
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
                    DbCode = credential.DbCode!,
                }).ToList();
                var affectedRow = await unitOfWork.VerificationStock.SaveRecordStockItemAfterVerify(itemModel);
                if (affectedRow > 0 )
                {
                    var result = await unitOfWork.VerificationRFID.UpdateSubmittedStatus(model.SubmittedCodeAndLocation.DbCode, model.SubmittedCodeAndLocation.SubmitCode);
                    if (result > 0)
                    {
                        return ApiResponse<VerificationRFIDDto>.Builder()
                            .WithResult(model)
                            .WithStatusCode((int)HttpStatusCode.OK)
                            .WithMessage("All stock fetched successfully")
                            .Build();
                    }
                    else
                    {
                        return ApiResponse<VerificationRFIDDto>.Builder()
                            .WithResult(new VerificationRFIDDto())
                            .WithStatusCode((int)HttpStatusCode.BadRequest)
                            .WithMessage("All stock fetched unsuccessfully")
                            .Build();
                    }
                }
                else
                {
                    return ApiResponse<VerificationRFIDDto>.Builder()
                        .WithResult(new VerificationRFIDDto())
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithMessage("All stock fetched unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<VerificationRFIDDto>(ex.Message);
            }
        }

        [HttpGet]
        [Route("getallwarehouse/{dbCode}")]
        public async Task<ApiResponse<List<string>>> GetAllWarehouseAsync([Required] string dbCode)
        {
            try
            {
                var result = await unitOfWork.VerificationRFID.GetAllWarehouse(dbCode);
                if (result.Any())
                {
                    return ApiResponse<List<string>>.Builder()
                        .WithResult(result)
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithMessage("Warehouse fetched successfully")
                        .Build();
                }
                else
                {
                    return ApiResponse<List<string>>.Builder()
                        .WithResult(new List<string>())
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithMessage("Warehouse fetched unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<string>>(ex.Message);
            }
        }

        [HttpGet]
        [Route("getrfidsubmitteddetail/{dbCode}")]
        public async Task<ApiResponse<List<VerificationStockModel>>> GetRFIDSubmittedItemDetailAsync([FromBody] VerificationStockPostDto model)
        {
            try
            {
                var result = await unitOfWork.VerificationRFID.GetRFIDSubmittedItemDetail(model.DbCode,model.SubmitCode,model.Location);
                if (result.Any())
                {
                    return ApiResponse<List<VerificationStockModel>>.Builder()
                        .WithResult(result)
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithMessage("RFID detail fetched successfully")
                        .Build();
                }
                else
                {
                    return ApiResponse<List<VerificationStockModel>>.Builder()
                        .WithResult(new List<VerificationStockModel>())
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithMessage("RFID detail fetched unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<VerificationStockModel>>(ex.Message);
            }
        }

        [HttpGet]
        [Route("getsubmittedcode/{dbCode}/{location}")]
        public async Task<ApiResponse<List<VerificationStockModel>>> GetAllSubmitCodeEntriesAsync([Required] string dbCode, [Required] string location)
        {
            try
            {
                var result = await unitOfWork.VerificationRFID.GetAllSubmitCodeEntries(dbCode, location);
                if (result.Any())
                {
                    return ApiResponse<List<VerificationStockModel>>.Builder()
                        .WithResult(result)
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithMessage("All stock fetched successfully")
                        .Build();
                }
                else
                {
                    return ApiResponse<List<VerificationStockModel>>.Builder()
                        .WithResult(new List<VerificationStockModel>())
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithMessage("All stock fetched unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<VerificationStockModel>>(ex.Message);
            }
        }
    }
}
