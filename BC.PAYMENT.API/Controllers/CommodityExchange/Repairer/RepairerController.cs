using System.ComponentModel.DataAnnotations;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.CommondityExchange.RepairItem;
using BC.PAYMENT.CORE.Entities.CommondityExchange.RepairItem;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BC.PAYMENT.API.Controllers.CommodityExchange.Repairer
{
    public class RepairerController(IUnitOfWork unitOfWork) : BaseApiController
    {
        #region RepairingGoods
        [HttpGet]
        [Route("getitemrepairedforrepairation")]
        public async Task<ApiResponse<List<ItemRepairItemReceivedRespondDto>>> LoadItemRepairedToReparationAsync()
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var execute = await unitOfWork.Repairer.LoadItemRepairedToReparation(credential.DbCode);
                var newRespond = execute.Select(x => new ItemRepairItemReceivedRespondDto
                {
                    Id = x.Id,
                    TransactionCode = x.TransactionCode,
                    CustomerCode = x.CustomerCode,
                    CustomerName = x.CustomerName,
                    Area = x.Area,
                    Store = x.Store,
                    Market = x.Market,
                    ItemCode = x.ItemCode,
                    Quantity = x.Quantity,
                    Description = x.Description,
                    Status = x.Status,
                    CreatedDate = x.CreatedDate,
                }).ToList();
                if (execute.Any())
                {
                    return ApiResponse<List<ItemRepairItemReceivedRespondDto>>.Builder()
                        .WithMessage("Repaired's items fetched successfully")
                        .WithStatusCode(StatusCodes.Status200OK)
                        .WithResult(newRespond)
                        .Build();
                }
                else
                {
                    return ApiResponse<List<ItemRepairItemReceivedRespondDto>>.Builder()
                        .WithMessage("Repaired's items fetched unsuccessfully")
                        .WithStatusCode(StatusCodes.Status400BadRequest)
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<ItemRepairItemReceivedRespondDto>>(ex.Message);
            }
        }
        [HttpPut]
        [Route("updateafterrepiarerreceived/{transactionCode}")]
        public async Task<ApiResponse<int>> UpdateAfterRepairerReceivedItemsAsync([Required] string transactionCode)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var affectedRow = await unitOfWork.Repairer.UpdateAfterRepairerReceivedItemsAsync(credential.DbCode, credential.Username, transactionCode);
                if (affectedRow > 0)
                {
                    return ApiResponse<int>.Builder()
                        .WithMessage("Updated successfully")
                        .WithStatusCode(StatusCodes.Status204NoContent)
                        .WithResult(affectedRow)
                        .Build();
                }
                else
                {
                    return ApiResponse<int>.Builder()
                        .WithMessage("Updated unsuccessfully")
                        .WithStatusCode(StatusCodes.Status400BadRequest)
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
            }
        }
        #endregion

        #region Finished Repairing Goods


        [HttpGet]
        [Route("getallitemafterrepairerreceived")]
        public async Task<ApiResponse<List<CompletedRepairItemDto>>> GetAllItemAfterRepairerReceivedAsync()
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var execute = await unitOfWork.CompletedRepair.GetAllCompletedRepairItemsAsync(credential.DbCode);
                if (execute.Any())
                {
                    return ApiResponse<List<CompletedRepairItemDto>>.Builder()
                        .WithMessage("Repaired's items fetched successfully")
                        .WithStatusCode(StatusCodes.Status200OK)
                        .WithResult(execute)
                        .Build();
                }
                else
                {
                    return ApiResponse<List<CompletedRepairItemDto>>.Builder()
                        .WithMessage("Repaired's items fetched unsuccessfully")
                        .WithStatusCode(StatusCodes.Status400BadRequest)
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<CompletedRepairItemDto>>(ex.Message);
            }
        }
        
        [HttpPost]
        [Route("updateitemwhencompleted")]
        public async Task<ApiResponse<CompletedRepairItemPostDto>> UpdatedAfterRepairerReceivedAsyc(CompletedRepairItemPostDto model)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var repairCompletedModel = new ItemRepairReceivedModel
                {
                    CustomerCode = model.CustomerCode,
                    TransactionCode = model.TransactionCode,
                    DbCode = credential.DbCode,
                    CreateBy = credential.Username,
                    Quantity = model.Quantity,
                    RepairStatus = model.RepairStatus,
                    ItemCode = model.ItemCode,
                    Reason = model.Reason,
                    Description = model.Description,
                    RepairToolCode = model.RepairToolCode,
                    ItemStatus = model.ItemStatus,
                };
                var affectedRow = await unitOfWork.CompletedRepair.UpdatedAfterRepairerReceivedAsyc(repairCompletedModel);
                if (affectedRow > 0 )
                {
                    return ApiResponse<CompletedRepairItemPostDto>.Builder()
                        .WithMessage("Repaired's items fetched successfully")
                        .WithStatusCode(StatusCodes.Status204NoContent)
                        .WithResult(model)
                        .Build();
                }
                else
                {
                    return ApiResponse<CompletedRepairItemPostDto>.Builder()
                        .WithMessage("Repaired's items fetched unsuccessfully")
                        .WithStatusCode(StatusCodes.Status400BadRequest)
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<CompletedRepairItemPostDto>(ex.Message);
            }
        }
        #endregion

        [HttpGet]
        [Route("loaditembybarcode/{barcode}")]
        public async Task<ApiResponse<ItemBeingRepaired>> LoadItemByBarCode(string barcode)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var execute = await unitOfWork.CompletedRepair.GetItemByTransactionCode(credential.DbCode, barcode);
                if (execute != null)
                {
                    return ApiResponse<ItemBeingRepaired>.Builder()
                        .WithMessage("Items fetched successfully")
                        .WithStatusCode(StatusCodes.Status204NoContent)
                        .WithResult(execute)
                        .Build();
                }
                else
                {
                    return ApiResponse<ItemBeingRepaired>.Builder()
                        .WithMessage("Items fetched unsuccessfully")
                        .WithStatusCode(StatusCodes.Status400BadRequest)
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<ItemBeingRepaired>(ex.Message);
            }
        }
        
        [HttpPost]
        [Route("insertcompletedrepairitem")]
        public async Task<ApiResponse<CompletedRepairItemPostDto>> InsertCompletedRepairItem(CompletedRepairItemPostDto model)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var repairedItemModel = new ItemRepairReceivedModel
                {
                    CustomerCode = model.CustomerCode,
                    TransactionCode = model.TransactionCode,
                    Quantity = model.Quantity,
                    RepairStatus = model.RepairStatus,
                    Description = model.Description,
                    CreateBy = credential.Username,
                    ItemCode = model.ItemCode,
                    RepairToolCode = model.RepairToolCode,
                    ItemStatus = model.ItemStatus,
                    ItemDescription = model.ItemDescription
                };
                var execute = await unitOfWork.CompletedRepair.InsertCompletedRepairItemAsync(repairedItemModel);
                if (execute != null)
                {
                    return ApiResponse<CompletedRepairItemPostDto>.Builder()
                        .WithMessage("Items added successfully")
                        .WithStatusCode(StatusCodes.Status204NoContent)
                        .WithResult(model)
                        .Build();
                }
                else
                {
                    return ApiResponse<CompletedRepairItemPostDto>.Builder()
                        .WithMessage("Items added unsuccessfully")
                        .WithStatusCode(StatusCodes.Status400BadRequest)
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<CompletedRepairItemPostDto>(ex.Message);
            }
        }

    }
}
