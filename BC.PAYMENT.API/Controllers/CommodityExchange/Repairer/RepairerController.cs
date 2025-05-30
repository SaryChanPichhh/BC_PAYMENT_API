using System.ComponentModel.DataAnnotations;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.CommondityExchange.RepairItem;
using BC.PAYMENT.CORE.Entities.CommondityExchange.RepairItem;
using DevExpress.CodeParser;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BC.PAYMENT.API.Controllers.CommodityExchange.Repairer
{
    public class RepairerController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public RepairerController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region RepairingGoods
        [HttpGet]
        [Route("getitemrepairedforrepairation")]
        public async Task<ApiResponse<List<ItemRepairItemReceivedRespondDto>>> LoadItemRepairedToReparationAsync()
        {
            var credential = Common.DecodeJwt(User);
            var repiredItem = new ApiResponse<List<ItemRepairItemReceivedRespondDto>>();
            try
            {
                var execute = await _unitOfWork.Repairer.LoadItemRepairedToReparation(credential.DbCode);
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
                    repiredItem.Result = newRespond;
                    repiredItem.StatusCode = StatusCodes.Status200OK;
                    repiredItem.Success = true;
                    repiredItem.Message = "Repaired's items fetched successfully";
                }
                else
                {
                    repiredItem.StatusCode = StatusCodes.Status400BadRequest;
                    repiredItem.Message = "Repaired's items fetched unsuccessfully";
                }

            }
            catch (SqlException ex)
            {
                repiredItem.StatusCode = StatusCodes.Status500InternalServerError;
                repiredItem.Message = $"Sql Exception : ${ex.Message}";
            }
            catch (Exception ex)
            {
                repiredItem.StatusCode = StatusCodes.Status500InternalServerError;
                repiredItem.Message = $"Sql Exception : ${ex.Message}";
            }
            return repiredItem;
        }
        [HttpPut]
        [Route("updateafterrepiarerreceived/{transactionCode}")]
        public async Task<ApiResponse<int>> UpdateAfterRepairerReceivedItemsAsync([Required] string transactionCode)
        {
            var credential = Common.DecodeJwt(User);
            var repiredItem = new ApiResponse<int>();
            try
            {
                var affectedRow = await _unitOfWork.Repairer.UpdateAfterRepairerReceivedItemsAsync(credential.DbCode, credential.Username, transactionCode);
                if (affectedRow > 0)
                {
                    repiredItem.Result = affectedRow;
                    repiredItem.StatusCode = StatusCodes.Status204NoContent;
                    repiredItem.Success = true;
                    repiredItem.Message = "Updated successfully";
                }
                else
                {
                    repiredItem.StatusCode = StatusCodes.Status400BadRequest;
                    repiredItem.Message = "Updated unsuccessfully";
                }

            }
            catch (SqlException ex)
            {
                repiredItem.StatusCode = StatusCodes.Status500InternalServerError;
                repiredItem.Message = $"Sql Exception : ${ex.Message}";
            }
            catch (Exception ex)
            {
                repiredItem.StatusCode = StatusCodes.Status500InternalServerError;
                repiredItem.Message = $"Sql Exception : ${ex.Message}";
            }
            return repiredItem;
        }
        #endregion

        #region Finished Repairing Goods


        [HttpGet]
        [Route("getallitemafterrepairerreceived")]
        public async Task<ApiResponse<List<CompletedRepairItemDto>>> GetAllItemAfterRepairerReceivedAsync()
        {
            var credential = Common.DecodeJwt(User);
            var repiredItem = new ApiResponse<List<CompletedRepairItemDto>>();
            try
            {
                var execute = await _unitOfWork.CompletedRepair.GetAllCompletedRepairItemsAsync(credential.DbCode);
                if (execute.Any())
                {
                    repiredItem.Result = execute;
                    repiredItem.StatusCode = StatusCodes.Status200OK;
                    repiredItem.Success = true;
                    repiredItem.Message = "Repaired's items fetched successfully";
                }
                else
                {
                    repiredItem.StatusCode = StatusCodes.Status400BadRequest;
                    repiredItem.Message = "Repaired's items fetched unsuccessfully";
                }

            }
            catch (SqlException ex)
            {
                repiredItem.StatusCode = StatusCodes.Status500InternalServerError;
                repiredItem.Message = $"Sql Exception : ${ex.Message}";
            }
            catch (Exception ex)
            {
                repiredItem.StatusCode = StatusCodes.Status500InternalServerError;
                repiredItem.Message = $"Sql Exception : ${ex.Message}";
            }
            return repiredItem;
        }
        
        [HttpPost]
        [Route("updateitemwhencompleted")]
        public async Task<ApiResponse<CompletedRepairItemPostDto>> UpdatedAfterRepairerReceivedAsyc(CompletedRepairItemPostDto model)
        {
            var credential = Common.DecodeJwt(User);
            var repiredItem = new ApiResponse<CompletedRepairItemPostDto>();
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
                var affectedRow = await _unitOfWork.CompletedRepair.UpdatedAfterRepairerReceivedAsyc(repairCompletedModel);
                if (affectedRow > 0 )
                {
                    repiredItem.Result = model;
                    repiredItem.StatusCode = StatusCodes.Status204NoContent;
                    repiredItem.Success = true;
                    repiredItem.Message = "Repaired's items fetched successfully";
                }
                else
                {
                    repiredItem.StatusCode = StatusCodes.Status400BadRequest;
                    repiredItem.Message = "Repaired's items fetched unsuccessfully";
                }

            }
            catch (SqlException ex)
            {
                repiredItem.StatusCode = StatusCodes.Status500InternalServerError;
                repiredItem.Message = $"Sql Exception : ${ex.Message}";
            }
            catch (Exception ex)
            {
                repiredItem.StatusCode = StatusCodes.Status500InternalServerError;
                repiredItem.Message = $"Sql Exception : ${ex.Message}";
            }
            return repiredItem;
        }
        #endregion
    }
}
