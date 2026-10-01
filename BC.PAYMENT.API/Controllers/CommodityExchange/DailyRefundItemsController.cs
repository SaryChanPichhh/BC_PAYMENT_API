using System.ComponentModel.DataAnnotations;
using System.Net;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.CommondityExchange.DailyRefundItems;
using BC.PAYMENT.CORE.Entities.CommondityExchange.DailyRefundItems;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BC.PAYMENT.API.Controllers.CommodityExchange;

public class DailyRefundItemsController(IUnitOfWork unitOfWork) : BaseApiController
{
    [HttpGet]
    [Route("getdailyrefunditemsallbranches")]
    public async Task<ApiResponse<List<RefundItemDetailDto>>> GetItemsefundByAllBranchAsync()
    {
        try
        {
            var result = await unitOfWork.DailyRefundItem.GetItemsRefundByAllBranchAsync();
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
            newRespond.ForEach(x => { x.Image = $"api/DailyRefundItems/getitemimage/{x.Id}"; });
            if (result.Any())
                return ApiResponse<List<RefundItemDetailDto>>.Builder()
                    .WithResult(newRespond)
                    .WithMessage("Items refunded by all branches fetched successfully")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .Build();
            else
                return ApiResponse<List<RefundItemDetailDto>>.Builder()
                    .WithResult(new List<RefundItemDetailDto>())
                    .WithMessage("Items refunded by all branches fetched successfully")
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<RefundItemDetailDto>>(ex.Message);
        }
    }

    [HttpGet]
    [Route("getitemimage/{detailId}")]
    public async Task<IActionResult> GetImageByDetailId([Required] int detailId)
    {
        var response = await unitOfWork.DailyRefundItem.GetImageByDetailIdAsync(detailId);
        try
        {
            if (response != null) return File(response, "image/jpeg");
        }
        catch (Exception ex)
        {
            return Ok(ex.Message);
        }

        return NotFound();
    }

    [HttpGet]
    [Route("getdailyrefunditemsbybranches/{dbCode}")]
    public async Task<ApiResponse<List<RefundItemDetailDto>>> GetItemsRefundByByBranchAsync([Required] string dbCode)
    {
        try
        {
            var result = await unitOfWork.DailyRefundItem.GetItemsRefundByByBranchAsync(dbCode);
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
            newRespond.ForEach(x => { x.Image = $"api/DailyRefundItems/getitemimage/{x.Id}"; });
            if (result.Any())
                return ApiResponse<List<RefundItemDetailDto>>.Builder()
                    .WithResult(newRespond)
                    .WithMessage("Items refunded by all branches fetched successfully")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .Build();
            else
                return ApiResponse<List<RefundItemDetailDto>>.Builder()
                    .WithResult(new List<RefundItemDetailDto>())
                    .WithMessage("Items refunded by all branches fetched successfully")
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<RefundItemDetailDto>>(ex.Message);
        }
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
        try
        {
            var affectedRow = await unitOfWork.DailyRefundItem.DeleteRequestItem(masterId, detailId);
            if (affectedRow > 0)
                return ApiResponse<int>.Builder()
                    .WithResult(affectedRow)
                    .WithMessage("Items refunded deleted successfully")
                    .WithStatusCode((int)HttpStatusCode.NoContent)
                    .Build();
            else
                return ApiResponse<int>.Builder()
                    .WithResult(0)
                    .WithMessage("Items refunded deleted fetched successfully")
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }

    [HttpPost]
    [Route("receiveditem/{masterId}/{detailId}")]
    public async Task<ApiResponse<int>> InsertReceivedItem([Required] int masterId, [Required] int detailId)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var affectedRow = await unitOfWork.DailyRefundItem.InsertReceivedItem(credential.Username, detailId);
            if (affectedRow > 0)
                return ApiResponse<int>.Builder()
                    .WithResult(affectedRow)
                    .WithMessage("Items refunded added successfully")
                    .WithStatusCode((int)HttpStatusCode.NoContent)
                    .Build();
            else
                return ApiResponse<int>.Builder()
                    .WithResult(0)
                    .WithMessage("Items refunded added fetched successfully")
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }
}