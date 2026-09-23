using BC.PAYMENT.CORE.Contracts.Response.Inventory;
using Microsoft.AspNetCore.Mvc;

namespace BC.PAYMENT.API.Controllers.Inventory;

public class InventoryController(IUnitOfWork unitOfWork) : BaseApiController
{
    [HttpPost]
    [Route("by-dbCode-and-location")]
    public async Task<ApiResponse<List<InventoryResponse>>> GetCountingStockAsync([FromBody] List<string> locations)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var inventoryList = new List<InventoryResponse>();
            foreach (var item in locations)
            {
                inventoryList.AddRange(await unitOfWork.Inventory.GetInventoryByLocationAsync(credential.DbCode, item));
            }
            if (inventoryList.Count != 0)
            {
                return ApiResponse<List<InventoryResponse>>.Builder()
                    .WithResult(inventoryList)
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithMessage("Inventory fetched successfully")
                    .Build();
            }

            return ApiResponse<List<InventoryResponse>>.Builder()
                .WithStatusCode(StatusCodes.Status400BadRequest)
                .WithMessage("Inventory fetched unsuccessfully")
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<InventoryResponse>>(ex.Message);
        }
    }
}