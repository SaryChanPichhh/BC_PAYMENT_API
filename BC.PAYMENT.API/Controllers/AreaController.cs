

using BC.PAYMENT.CORE.Contracts.Response.Area;
using BC.PAYMENT.CORE.Contracts.Request.Area;
using BC.PAYMENT.CORE.Entities.General;
namespace BC.PAYMENT.API.Controllers;

public class AreaController(IUnitOfWork unitOfWork) : BaseApiController
{
    [HttpGet("")]
    public async Task<ApiResponse<List<AreaResponse>>> GetArea()
    {
        try
        {
            var claim = Common.DecodeJwt(HttpContext.User);
            var data = await unitOfWork.Areas.GetArea(claim?.DbCode);
            return ApiResponse<List<AreaResponse>>.Builder()
                .WithMessage(data.Count != 0 ? "Areas fetched successfully." : "No areas found.")
                .WithStatusCode(data.Count != 0 ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest)
                .WithResult(data.Count != 0 ? data : new List<AreaResponse>())
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<AreaResponse>>(ex.Message);
        }   
    }

    [HttpPost("")]
    public async Task<ApiResponse<bool>> CreateArea([FromBody] CreateAreaRequest request)
    {
        try
        {
            var claim = Common.DecodeJwt(HttpContext.User);
            var area = new Area
            {
                AreaId = request.Id,
                DbCode = claim?.DbCode,
                AreaName = request.AreaName,
                AreaNameKhmer = request.AreaNameKhmer,
                Other = request.Other,
                Status = request.Status ?? true,
                CreatedBy = request.CreatedBy ?? claim?.Username,
                CreatedAt = request.CreatedAt ?? DateTime.Now
            };

            var success = await unitOfWork.Areas.CreateAreaAsync(area);

            return ApiResponse<bool>.Builder()
                .WithMessage(success ? "Area created successfully." : "Failed to create area.")
                .WithStatusCode(success ? (int)HttpStatusCode.Created : (int)HttpStatusCode.BadRequest)
                .WithResult(success)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<bool>(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<ApiResponse<bool>> UpdateArea(string id, [FromBody] UpdateAreaRequest request)
    {
        try
        {
            var claim = Common.DecodeJwt(HttpContext.User);
            var area = new Area
            {
                AreaId = id,
                DbCode = claim?.DbCode,
                AreaName = request.AreaName,
                AreaNameKhmer = request.AreaNameKhmer,
                Other = request.Other,
                Status = request.Status,
                UpdatedBy = request.UpdatedBy ?? claim?.Username,
                UpdatedAt = request.UpdatedAt ?? DateTime.Now
            };

            var success = await unitOfWork.Areas.UpdateAreaAsync(area);

            return ApiResponse<bool>.Builder()
                .WithMessage(success ? "Area updated successfully." : "Failed to update area.")
                .WithStatusCode(success ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest)
                .WithResult(success)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<bool>(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> DeleteArea(string id)
    {
        try
        {
            var claim = Common.DecodeJwt(HttpContext.User);
            var success = await unitOfWork.Areas.DeleteAreaAsync(id, claim?.DbCode);

            return ApiResponse<bool>.Builder()
                .WithMessage(success ? "Area deleted successfully." : "Failed to delete area.")
                .WithStatusCode(success ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest)
                .WithResult(success)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<bool>(ex.Message);
        }
    }

    [HttpGet("generate-id")]
    public async Task<ApiResponse<string>> GenerateAreaId()
    {
        try
        {
            var areaId = await unitOfWork.Areas.GenerateAreaIdAsync();
            return ApiResponse<string>.Builder()
                .WithMessage("Area ID generated successfully.")
                .WithStatusCode((int)HttpStatusCode.OK)
                .WithResult(areaId)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<string>(ex.Message);
        }
    }
}
