using System.ComponentModel.DataAnnotations;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.Transaction.Inventory.VerificationStock;
using BC.PAYMENT.CORE.Entities.Transaction.Inventory.VerificationStock;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Net;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.LOGGING;

namespace BC.PAYMENT.API.Controllers.Transaction.Inventory
{
    public class VerificationRFIDStockController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public VerificationRFIDStockController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
                    var movTypes = await _unitOfWork.VerificationStock.GetRecTypes(credential.DbCode!, item.StatusType);
                    var movRef = await _unitOfWork.Generators.GenerateAdjRefCode(credential.DbCode!,movTypes.MovType, movTypes.RecType);
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
                Logger.Instance.Error("Sql Exception", ex);
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
        [Route("getallstockitembylocationandsubmitcode")]
        public async Task<ApiResponse<List<VerificationStockModel>>> GetAllStockItemByLocationAndSubmitCodeAsync([FromBody] VerificationStockPostDto model)
        {
            var verificationStock = new ApiResponse<List<VerificationStockModel>>();
            try
            {
                var result = await _unitOfWork.VerificationRFID.GetAllStockItemByLocationAndSubmitCode(model.DbCode,model.SubmitCode,model.Location);
                if (result.Any())
                {
                    verificationStock.Result = result;
                    verificationStock.StatusCode = (int)HttpStatusCode.OK;
                    verificationStock.Message = "All stock fetched successfully";
                    verificationStock.Success = true;
                }
                else
                {
                    verificationStock.Result = new List<VerificationStockModel>();
                    verificationStock.StatusCode = (int)HttpStatusCode.BadRequest;
                    verificationStock.Message = "All stock fetched unsuccessfully";
                    verificationStock.Success = false;
                }
            }
            catch (SqlException ex)
            {
                verificationStock.StatusCode = (int)HttpStatusCode.InternalServerError;
                verificationStock.Message = $@"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql Exception", ex);
            }
            catch (Exception ex)
            {
                verificationStock.StatusCode = (int)HttpStatusCode.InternalServerError;
                verificationStock.Message = $@"Error Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }
            return verificationStock;
        }
        
        [HttpPost]
        [Route("savestockverification")]
        public async Task<ApiResponse<VerificationRFIDDto>> SaveRecordStockItemAfterVerify([FromBody] VerificationRFIDDto model)
        {
            var credential = Common.DecodeJwt(User);
            var verificationStock = new ApiResponse<VerificationRFIDDto>();
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
                var affectedRow = await _unitOfWork.VerificationStock.SaveRecordStockItemAfterVerify(itemModel);
                if (affectedRow > 0 )
                {
                    var result = await _unitOfWork.VerificationRFID.UpdateSubmittedStatus(model.SubmittedCodeAndLocation.DbCode, model.SubmittedCodeAndLocation.SubmitCode);
                    if (result > 0)
                    {
                        verificationStock.Result = model;
                        verificationStock.StatusCode = (int)HttpStatusCode.OK;
                        verificationStock.Message = "All stock fetched successfully";
                        verificationStock.Success = true;
                    }
                    else
                    {
                        verificationStock.Result = new VerificationRFIDDto();
                        verificationStock.StatusCode = (int)HttpStatusCode.BadRequest;
                        verificationStock.Message = "All stock fetched unsuccessfully";
                        verificationStock.Success = false;
                    }
                }
                else
                {
                    verificationStock.Result = new VerificationRFIDDto();
                    verificationStock.StatusCode = (int)HttpStatusCode.BadRequest;
                    verificationStock.Message = "All stock fetched unsuccessfully";
                    verificationStock.Success = false;
                }
            }
            catch (SqlException ex)
            {
                verificationStock.StatusCode = (int)HttpStatusCode.InternalServerError;
                verificationStock.Message = $@"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql Exception", ex);
            }
            catch (Exception ex)
            {
                verificationStock.StatusCode = (int)HttpStatusCode.InternalServerError;
                verificationStock.Message = $@"Error Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }
            return verificationStock;
        }
        [HttpGet]
        [Route("getallwarehouse/{dbCode}")]
        public async Task<ApiResponse<List<string>>> GetAllWarehouseAsync([Required] string dbCode)
        {
            var verificationStock = new ApiResponse<List<string>>();
            try
            {
                var result = await _unitOfWork.VerificationRFID.GetAllWarehouse(dbCode);
                if (result.Any())
                {
                    verificationStock.Result = result;
                    verificationStock.StatusCode = (int)HttpStatusCode.OK;
                    verificationStock.Message = "Warehouse fetched successfully";
                    verificationStock.Success = true;
                }
                else
                {
                    verificationStock.Result = new List<string>();
                    verificationStock.StatusCode = (int)HttpStatusCode.BadRequest;
                    verificationStock.Message = "Warehouse fetched unsuccessfully";
                    verificationStock.Success = false;
                }
            }
            catch (SqlException ex)
            {
                verificationStock.StatusCode = (int)HttpStatusCode.InternalServerError;
                verificationStock.Message = $@"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql Exception", ex);
            }
            catch (Exception ex)
            {
                verificationStock.StatusCode = (int)HttpStatusCode.InternalServerError;
                verificationStock.Message = $@"Error Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }
            return verificationStock;
        }
        [HttpGet]
        [Route("getrfidsubmitteddetail/{dbCode}")]
        public async Task<ApiResponse<List<VerificationStockModel>>> GetRFIDSubmittedItemDetailAsync([FromBody] VerificationStockPostDto model)
        {
            var verificationStock = new ApiResponse<List<VerificationStockModel>>();
            try
            {
                var result = await _unitOfWork.VerificationRFID.GetRFIDSubmittedItemDetail(model.DbCode,model.SubmitCode,model.Location);
                if (result.Any())
                {
                    verificationStock.Result = result;
                    verificationStock.StatusCode = (int)HttpStatusCode.OK;
                    verificationStock.Message = "RFID detail fetched successfully";
                    verificationStock.Success = true;
                }
                else
                {
                    verificationStock.Result = new List<VerificationStockModel>();
                    verificationStock.StatusCode = (int)HttpStatusCode.BadRequest;
                    verificationStock.Message = "RFID detail fetched unsuccessfully";
                    verificationStock.Success = false;
                }
            }
            catch (SqlException ex)
            {
                verificationStock.StatusCode = (int)HttpStatusCode.InternalServerError;
                verificationStock.Message = $@"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql Exception", ex);
            }
            catch (Exception ex)
            {
                verificationStock.StatusCode = (int)HttpStatusCode.InternalServerError;
                verificationStock.Message = $@"Error Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }
            return verificationStock;
        }
        [HttpGet]
        [Route("getsubmittedcode/{dbCode}/{location}")]
        public async Task<ApiResponse<List<VerificationStockModel>>> GetAllSubmitCodeEntriesAsync([Required] string dbCode, [Required] string location)
        {
            var verificationStock = new ApiResponse<List<VerificationStockModel>>();
            try
            {
                var result = await _unitOfWork.VerificationRFID.GetAllSubmitCodeEntries(dbCode, location);
                if (result.Any())
                {
                    verificationStock.Result = result;
                    verificationStock.StatusCode = (int)HttpStatusCode.OK;
                    verificationStock.Message = "All stock fetched successfully";
                    verificationStock.Success = true;
                }
                else
                {
                    verificationStock.Result = new List<VerificationStockModel>();
                    verificationStock.StatusCode = (int)HttpStatusCode.BadRequest;
                    verificationStock.Message = "All stock fetched unsuccessfully";
                    verificationStock.Success = false;
                }
            }
            catch (SqlException ex)
            {
                verificationStock.StatusCode = (int)HttpStatusCode.InternalServerError;
                verificationStock.Message = $@"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql Exception", ex);
            }
            catch (Exception ex)
            {
                verificationStock.StatusCode = (int)HttpStatusCode.InternalServerError;
                verificationStock.Message = $@"Error Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }
            return verificationStock;
        }
    }
}
