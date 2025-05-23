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
using Microsoft.Data.SqlClient;

namespace BC.PAYMENT.API.Controllers.Transaction.Inventory
{
    public class VerificationStockController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public VerificationStockController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("getmaxsequence")]
        public async Task<ApiResponse<int>> GetMaxSequence()
        {
            var credential = Common.DecodeJwt(User);
            var maxSequence = new ApiResponse<int>();
            try
            {
                var maxSeq = await _unitOfWork.VerificationStock.GetMaxSequence(credential.DbCode!);
                if (maxSeq > 0)
                {
                    maxSequence.Result = maxSeq;
                    maxSequence.StatusCode = (int)HttpStatusCode.OK;
                    maxSequence.Message = "Max Sequence fetched successfully";
                    maxSequence.Success = true;
                }
                else
                {
                    maxSequence.StatusCode = (int)HttpStatusCode.BadRequest;
                    maxSequence.Message = "Max Sequence fetched unsuccessfully";
                }
            }catch (SqlException ex)
            {
                maxSequence.StatusCode = (int)HttpStatusCode.InternalServerError;
                maxSequence.Message = $@"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql Exception",ex);
            }
            catch (Exception ex)
            {
                maxSequence.StatusCode = (int)HttpStatusCode.InternalServerError;
                maxSequence.Message = $@"Error Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }
            return maxSequence;
        }
        
        
        [HttpGet]
        [Route("getrectype/{movType}")]
        public async Task<ApiResponse<BcModels>> GetRecTypesAsync([Required] string movType)
        {
            var credential = Common.DecodeJwt(User);
            var recType = new ApiResponse<BcModels>();
            try
            {
                var execute = await _unitOfWork.VerificationStock.GetRecTypes(credential.DbCode!,movType);
                if (execute != null)
                {
                    recType.Result = execute;
                    recType.StatusCode = (int)HttpStatusCode.OK;
                    recType.Message = "Rectype fetched successfully";
                    recType.Success = true;
                }
                else
                {
                    recType.StatusCode = (int)HttpStatusCode.BadRequest;
                    recType.Message = "Rectype fetched unsuccessfully";
                }
            }catch (SqlException ex)
            {
                recType.StatusCode = (int)HttpStatusCode.InternalServerError;
                recType.Message = $@"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql Exception",ex);
            }
            catch (Exception ex)
            {
                recType.StatusCode = (int)HttpStatusCode.InternalServerError;
                recType.Message = $@"Error Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }
            return recType;
        }
        
        [HttpGet]
        [Route("getitemcostbyitemcode/{itemCode}")]
        public async Task<ApiResponse<double>> GetItemCostByItemCodeAsync([Required] string itemCode)
        {
            var credential = Common.DecodeJwt(User);
            var itemCost = new ApiResponse<double>();
            try
            {
                var execute = await _unitOfWork.VerificationStock.GetItemCostAsync(credential.DbCode!, itemCode);
                if (execute != null)
                {
                    itemCost.Result = execute;
                    itemCost.StatusCode = (int)HttpStatusCode.OK;
                    itemCost.Message = "Item cost fetched successfully";
                    itemCost.Success = true;
                }
                else
                {
                    itemCost.StatusCode = (int)HttpStatusCode.BadRequest;
                    itemCost.Message = "Item cost fetched unsuccessfully";
                }
            }catch (SqlException ex)
            {
                itemCost.StatusCode = (int)HttpStatusCode.InternalServerError;
                itemCost.Message = $@"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql Exception",ex);
            }
            catch (Exception ex)
            {
                itemCost.StatusCode = (int)HttpStatusCode.InternalServerError;
                itemCost.Message = $@"Error Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }
            return itemCost;
        }
        [HttpPost]
        [Route("getitemstockbylocationandcountingdate")]
        public async Task<ApiResponse<List<VerificationStockModel>>> GetAllStockItemByLocationAndCountingDateAsync([FromBody] PaginatedVerificationStockByDateDto model)
        {
            var credential = Common.DecodeJwt(User);
            var itemCost = new ApiResponse<List<VerificationStockModel>>();
            try
            {
                var execute = await _unitOfWork.VerificationStock.GetAllStockItemByLocationAndCountingDateAsync(credential.DbCode!, model.Location,Convert.ToDateTime(model.FromDate),Convert.ToDateTime( model.ToDate),model.Page,model.PageSize);
                if (execute.Any())
                {
                    itemCost.Result = execute;
                    itemCost.StatusCode = (int)HttpStatusCode.OK;
                    itemCost.Message = "Items fetched successfully";
                    itemCost.Success = true;
                }
                else
                {
                    itemCost.StatusCode = (int)HttpStatusCode.BadRequest;
                    itemCost.Message = "Items fetched unsuccessfully";
                }
            }catch (SqlException ex)
            {
                itemCost.StatusCode = (int)HttpStatusCode.InternalServerError;
                itemCost.Message = $@"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql Exception",ex);
            }
            catch (Exception ex)
            {
                itemCost.StatusCode = (int)HttpStatusCode.InternalServerError;
                itemCost.Message = $@"Error Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }
            return itemCost;
        }
        
        [HttpPost]
        [Route("getitemstockbylocationandcountingperiod")]
        public async Task<ApiResponse<List<VerificationStockModel>>> GetAllStockItemByLocationAndCountingDateAsync([FromBody] PaginatedVerificationStockByPeriodDto model)
        {
            var credential = Common.DecodeJwt(User);
            var itemCost = new ApiResponse<List<VerificationStockModel>>();
            try
            {
                var execute = await _unitOfWork.VerificationStock.GetAllStockItemByLocationAndCountingPeriodAsync(credential.DbCode!, model.Location,model.Month,model.Year,model.Page,model.PageSize);
                if (execute.Any())
                {
                    itemCost.Result = execute;
                    itemCost.StatusCode = (int)HttpStatusCode.OK;
                    itemCost.Message = "Items fetched successfully";
                    itemCost.Success = true;
                }
                else
                {
                    itemCost.StatusCode = (int)HttpStatusCode.BadRequest;
                    itemCost.Message = "Items fetched unsuccessfully";
                }
            }catch (SqlException ex)
            {
                itemCost.StatusCode = (int)HttpStatusCode.InternalServerError;
                itemCost.Message = $@"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql Exception",ex);
            }
            catch (Exception ex)
            {
                itemCost.StatusCode = (int)HttpStatusCode.InternalServerError;
                itemCost.Message = $@"Error Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }
            return itemCost;
        }
        [HttpPost]
        [Route("saveitemafterverification")]
        public async Task<ApiResponse<List<VerificationStockDto>>> SaveRecordStockItemAfterVerify([FromBody] List<VerificationStockDto> model)
        {
            var credential = Common.DecodeJwt(User);
            var saveRecord = new ApiResponse<List<VerificationStockDto>>();
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
                    DbCode = credential.DbCode!,
                }).ToList();
                var execute = await _unitOfWork.VerificationStock.SaveRecordStockItemAfterVerify(itemModel);
                if (execute > 0)
                {
                    saveRecord.Result = model;
                    saveRecord.StatusCode = (int)HttpStatusCode.OK;
                    saveRecord.Message = "Items saved successfully";
                    saveRecord.Success = true;
                }
                else
                {
                    saveRecord.StatusCode = (int)HttpStatusCode.BadRequest;
                    saveRecord.Message = "Items saved unsuccessfully";
                }
            }catch (SqlException ex)
            {
                saveRecord.StatusCode = (int)HttpStatusCode.InternalServerError;
                saveRecord.Message = $@"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql Exception",ex);
            }
            catch (Exception ex)
            {
                saveRecord.StatusCode = (int)HttpStatusCode.InternalServerError;
                saveRecord.Message = $@"Error Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }
            return saveRecord;
        }
        
        [HttpPost]
        [Route("adjustmentinventory")]
        public async Task<ApiResponse<List<AdjustmentInventoryDto>>> AdjustmentInventoryAsync([FromBody] List<AdjustmentInventoryDto> model)
        {
            var credential = Common.DecodeJwt(User);
            var saveRecord = new ApiResponse<List<AdjustmentInventoryDto>>();
            try
            {
                var affectedRow = 0;
                var i = 1;
                foreach (var item in model)
                {
                    var sequence = await _unitOfWork.VerificationStock.GetMaxSequence(credential.DbCode!);
                    var movTypes = await _unitOfWork.VerificationStock.GetRecTypes(credential.DbCode!,item.StatusType );
                    var movRef = await _unitOfWork.Generators.GenerateAdjRefCode(credential.DbCode!, movTypes.RecType);
                    var itemCost = await _unitOfWork.VerificationStock.GetItemCostAsync(credential.DbCode!, item.ItemCode);
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
                    affectedRow += await _unitOfWork.VerificationStock.AdjustInventory(inventory);
                }
                if (affectedRow > 0)
                {
                    saveRecord.Result = model;
                    saveRecord.StatusCode = (int)HttpStatusCode.OK;
                    saveRecord.Message = "Adjustment Inventory saved successfully";
                    saveRecord.Success = true;
                }
                else
                {
                    saveRecord.StatusCode = (int)HttpStatusCode.BadRequest;
                    saveRecord.Message = "Adjustment Inventory saved unsuccessfully";
                }
            }
            catch (SqlException ex)
            {
                saveRecord.StatusCode = (int)HttpStatusCode.InternalServerError;
                saveRecord.Message = $@"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql Exception",ex);
            }
            catch (Exception ex)
            {
                saveRecord.StatusCode = (int)HttpStatusCode.InternalServerError;
                saveRecord.Message = $@"Error Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }
            return saveRecord;
        }
    }
}
