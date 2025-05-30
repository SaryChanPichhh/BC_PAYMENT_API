
using System.ComponentModel.DataAnnotations;
using System.Net;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.CommondityExchange.DailyRefundItems;
using BC.PAYMENT.CORE.Entities.CommondityExchange.DailyRefundItems;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BC.PAYMENT.API.Controllers.CommodityExchange
{
    public class DailyRefundItemsController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public DailyRefundItemsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        [Route("getdailyrefunditemsallbranches")]
        public async Task<ApiResponse<List<RefundItemDetailDto>>> GetItemsefundByAllBranchAsync()
        {
            var response = new ApiResponse<List<RefundItemDetailDto>>();
            try
            {
                var result = await _unitOfWork.DailyRefundItem.GetItemsRefundByAllBranchAsync();
                var newRespond = result.Select(x => new RefundItemDetailDto
                {
                    Id = x.Id,
                    MasterId = x.MasterId,
                    ItemCode = x.ItemCode,
                    ItemName = x.ItemName,
                    Quantity = x.Quantity,
                    CustomerCode = x.CustomerCode,
                    CustomerName = x.CustomerName,
                    ChangeType = x.ChangeType,
                    Market = x.Market,
                    Area = x.Area,
                    Status = x.Status,
                    Type = x.Type,
                    Store = x.Store,
                    RequestDate = x.RequestDate,
                    InvoiceNumber = x.InvoiceNumber

                }).ToList();
                newRespond.ForEach(x =>
                {
                    x.Image = $"api/DailyRefundItems/getitemimage/{x.Id}";
                });
                if (result.Any())
                {
                    response.Result = newRespond;
                    response.Message = "Items refunded by all branches fetched successfully";
                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.Success = true;
                }
                else
                {
                    response.Result = new List<RefundItemDetailDto>();
                    response.Message = "Items refunded by all branches fetched successfully";
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                }
            }catch(SqlException ex)
            {
                response.Message = $"Sql Exception : {ex.Message}";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            catch (Exception ex)
            {
                response.Message = $"Error Exception : {ex.Message}";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            return response;
        }

        [HttpGet]
        [Route("getitemimage/{detailId}")]
        public async Task<IActionResult> GetImageByDetailId([Required] int detailId)
        {
            var response = await _unitOfWork.DailyRefundItem.GetImageByDetailIdAsync(detailId);
            try
            {
                if (response != null)
                {
                    return File(response, "image/jpeg");
                }
            }
            catch(Exception ex)
            {
                return BadRequest($"Error fetching image: {ex.Message}");
            }
            return NotFound();
        }
        [HttpGet]
        [Route("getdailyrefunditemsbybranches/{dbCode}")]
        public async Task<ApiResponse<List<RefundItemDetailDto>>> GetItemsRefundByByBranchAsync([Required] string dbCode)
        {
            var response = new ApiResponse<List<RefundItemDetailDto>>();
            try
            {
                var result = await _unitOfWork.DailyRefundItem.GetItemsRefundByByBranchAsync(dbCode);
                var newRespond = result.Select(x => new RefundItemDetailDto
                {
                    Id = x.Id,
                    MasterId = x.MasterId,
                    ItemCode = x.ItemCode,
                    ItemName = x.ItemName,
                    Quantity = x.Quantity,
                    CustomerCode = x.CustomerCode,
                    CustomerName = x.CustomerName,
                    ChangeType = x.ChangeType,
                    Market = x.Market,
                    Area = x.Area,
                    Status = x.Status,
                    Type = x.Type,
                    Store = x.Store,
                    RequestDate = x.RequestDate,
                    InvoiceNumber = x.InvoiceNumber

                }).ToList();
                newRespond.ForEach(x =>
                {
                    x.Image = $"api/DailyRefundItems/getitemimage/{x.Id}";
                });
                if (result.Any())
                {
                    response.Result = newRespond;
                    response.Message = "Items refunded by all branches fetched successfully";
                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.Success = true;
                }
                else
                {
                    response.Result = new List<RefundItemDetailDto>();
                    response.Message = "Items refunded by all branches fetched successfully";
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                }
            }catch(SqlException ex)
            {
                response.Message = $"Sql Exception : {ex.Message}";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            catch (Exception ex)
            {
                response.Message = $"Error Exception : {ex.Message}";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            return response;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="masterId">TB_BC_CHANGEINVOICE's Id</param>
        /// <param name="detailId">TB_BC_CHANGEINVOICE_DETAIL's Id</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("deleterefunditems/{masterId}/{detailId}")]
        public async Task<ApiResponse<int>> GetItemsefundByAllBranchAsync([Required] int masterId, [Required] int detailId)
        {
            var response = new ApiResponse<int>();
            try
            {
                var affectedRow = await _unitOfWork.DailyRefundItem.DeleteRequestItem(masterId,detailId);
                if (affectedRow > 0)
                {
                    response.Result = affectedRow;
                    response.Message = "Items refunded deleted successfully";
                    response.StatusCode = (int)HttpStatusCode.NoContent;
                    response.Success = true;
                }
                else
                {
                    response.Result = 0;
                    response.Message = "Items refunded deleted fetched successfully";
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                }
            }catch(SqlException ex)
            {
                response.Message = $"Sql Exception : {ex.Message}";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            catch (Exception ex)
            {
                response.Message = $"Error Exception : {ex.Message}";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            return response;
        }
        
        [HttpPost]
        [Route("receiveditem/{masterId}/{detailId}")]
        public async Task<ApiResponse<int>> InsertReceivedItem([Required] int masterId, [Required] int detailId)
        {
            var credential = Common.DecodeJwt(User);
            var response = new ApiResponse<int>();
            try
            {
                var affectedRow = await _unitOfWork.DailyRefundItem.InsertReceivedItem(credential.Username, detailId);
                if (affectedRow > 0)
                {
                    response.Result = affectedRow;
                    response.Message = "Items refunded added successfully";
                    response.StatusCode = (int)HttpStatusCode.NoContent;
                    response.Success = true;
                }
                else
                {
                    response.Result = 0;
                    response.Message = "Items refunded added fetched successfully";
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                }
            }catch(SqlException ex)
            {
                response.Message = $"Sql Exception : {ex.Message}";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            catch (Exception ex)
            {
                response.Message = $"Error Exception : {ex.Message}";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            return response;
        }
    }
}
