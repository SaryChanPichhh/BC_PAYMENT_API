using System.ComponentModel.DataAnnotations;
using BC.PAYMENT.CORE.Contracts.Request.Inventory.StockCounting;
using BC.PAYMENT.CORE.Contracts.Response.Inventory.StockCounting;
using BC.PAYMENT.CORE.Entities;

namespace BC.PAYMENT.API.Controllers.Inventory;

public class CountingStockController(IUnitOfWork unitOfWork) : BaseApiController
{
    [HttpGet]
    [Route("")]
    public async Task<ApiResponse<List<ScStockResponse>>> GetCountingStockAsync()
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var result = await unitOfWork.StockCounting.GetCountingStockAsync(credential.DbCode);
            if (result.Count != 0)
                return ApiResponse<List<ScStockResponse>>.Builder()
                    .WithResult(result)
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithMessage("Counting stock fetched successfully")
                    .Build();

            return ApiResponse<List<ScStockResponse>>.Builder()
                .WithStatusCode(StatusCodes.Status400BadRequest)
                .WithMessage("Counting stock fetched unsuccessfully")
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<ScStockResponse>>(ex.Message);
        }
    }

    [HttpGet]
    [Route("counting-stock-details")]
    public async Task<ApiResponse<List<ScCountItemResponse>>> GetCountingStockItemDetailAsync([FromQuery] int headerId)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var result = await unitOfWork.StockCounting.GetCountingStockItemDetailAsync(credential?.DbCode, headerId);
            if (result.Count != 0)
                return ApiResponse<List<ScCountItemResponse>>.Builder()
                    .WithResult(result)
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithMessage("Counting stock fetched successfully")
                    .Build();
            return ApiResponse<List<ScCountItemResponse>>.Builder()
                .WithStatusCode(StatusCodes.Status400BadRequest)
                .WithMessage("Counting stock fetched unsuccessfully")
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<ScCountItemResponse>>(ex.Message);
        }
    }

    [HttpPost]
    [Route("counting-stock-details/{headerId:int}")]
    public async Task<ApiResponse<int>> SaveCountingItemDetailAsync([Required] int headerId,
        [FromBody] List<CreateScCountItemRequest> ls)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var stockModel = new ScStock
            {
                StockId = headerId,
                DbCode = credential.DbCode,
                Period = credential.Period,
                UserName = credential.Username
            };

            var stockItemDetail = ls.Select(x => new ScCountItem
            {
                DbCode = credential.DbCode,
                StockId = headerId,
                Period = credential.Period,
                ItemCode = x.ItemCode,
                ItemDesc = x.ItemDesc,
                ItemDescKh = x.ItemDescKh,
                Quantity = x.Quantity,
                CreatedBy = credential.Username,
                CreatedDate = credential.CurrectDate,
                Status = true
            }).ToList();
            var result = await unitOfWork.StockCounting.SaveCountingItemDetailAsync(stockModel, stockItemDetail);
            if (result > 0)
                return ApiResponse<int>.Builder()
                    .WithResult(result)
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithMessage("Counting stock added successfully")
                    .Build();

            return ApiResponse<int>.Builder()
                .WithStatusCode(StatusCodes.Status400BadRequest)
                .WithMessage("Counting stock added unsuccessfully")
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }

    [HttpPost]
    [Route("counting-stock-details-object/{headerId:int}")]
    public async Task<ApiResponse<int>> SaveCountingItemDetailAsync([Required] int headerId,
        [FromBody] CreateScCountItemRequest req)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var stockItemDetail = new ScCountItem
            {
                DbCode = credential.DbCode,
                StockId = headerId,
                Period = credential.Period,
                ItemCode = req.ItemCode,
                ItemDesc = req.ItemDesc,
                ItemDescKh = req.ItemDescKh,
                Quantity = req.Quantity,
                CreatedBy = credential.Username,
                CreatedDate = credential.CurrectDate,
                Status = true
            };
            var result = await unitOfWork.StockCounting.SaveCountingItemDetailAsync(stockItemDetail);
            if (result > 0)
                return ApiResponse<int>.Builder()
                    .WithResult(result)
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithMessage("Counting stock added successfully")
                    .Build();

            return ApiResponse<int>.Builder()
                .WithStatusCode(StatusCodes.Status400BadRequest)
                .WithMessage("Counting stock added unsuccessfully")
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }

    [HttpPut]
    [Route("counting-stock-details")]
    public async Task<ApiResponse<int>> UpdateCountingStockItemDetailAsync(UpdateScCountingItemRequest req)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var stockItemDetail = new ScCountItem
            {
                DbCode = credential.DbCode,
                Period = credential.Period,
                ItemCode = req.ItemCode,
                ItemDesc = req.ItemDesc,
                ItemDescKh = req.ItemDescKh,
                Quantity = req.Quantity,
                UpdatedBy = credential.Username,
                UpdatedDate = credential.CurrectDate,
                Status = true,
                Id = req.Id
            };
            var result = await unitOfWork.StockCounting.UpdateCountingStockItemDetailAsync(stockItemDetail);
            if (result > 0)
                return ApiResponse<int>.Builder()
                    .WithResult(result)
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithMessage("Counting stock updated successfully")
                    .Build();

            return ApiResponse<int>.Builder()
                .WithStatusCode(StatusCodes.Status400BadRequest)
                .WithMessage("Counting stock updated unsuccessfully")
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }

    [HttpDelete]
    [Route("counting-stock-details/{id:int}")]
    public async Task<ApiResponse<int>> DeleteCountingStockItemDetailAsync([Required] int id)
    {
        try
        {
            var affectedRow = await unitOfWork.StockCounting.DeleteCountingStockItemDetailAsync(id);
            if (affectedRow > 0)
                return ApiResponse<int>.Builder()
                    .WithResult(affectedRow)
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithMessage("Counting stock deleted successfully")
                    .Build();
            return ApiResponse<int>.Builder()
                .WithStatusCode(StatusCodes.Status400BadRequest)
                .WithMessage("Counting stock deleted unsuccessfully")
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }
}