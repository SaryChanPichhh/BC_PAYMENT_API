using BC.PAYMENT.CORE.Contracts.Response.Item;

namespace BC.PAYMENT.API.Controllers.Inventory;

public class ItemController(IUnitOfWork unitOfWork) : BaseApiController
{
    [HttpGet]
    [Route("")]
    public async Task<ApiResponse<List<ItemResponse>>> GetItemAsync()
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var result = await unitOfWork.Items.GetItemListAsync(credential?.DbCode);
            if (result.Count != 0)
            {
                return ApiResponse<List<ItemResponse>>.Builder()
                    .WithResult(result)
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithMessage("Items fetched successfully")
                    .Build();
            }
            return ApiResponse<List<ItemResponse>>.Builder()
                .WithStatusCode(StatusCodes.Status400BadRequest)
                .WithMessage("Items fetched unsuccessfully")
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<ItemResponse>>(ex.Message);
        }
    }
}   