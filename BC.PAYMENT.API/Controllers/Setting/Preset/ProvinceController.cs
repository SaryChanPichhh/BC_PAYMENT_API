using System.ComponentModel.DataAnnotations;
using BC.PAYMENT.CORE.Contracts.Request.Province;
using BC.PAYMENT.CORE.Contracts.Response.Province;
using BC.PAYMENT.CORE.Contracts.Setting.Preset;
using BC.PAYMENT.CORE.Entities.Setting.Preset;

namespace BC.PAYMENT.API.Controllers.Setting.Preset;

public class ProvinceController(IUnitOfWork unitOfWork) : BaseApiController
{
    [HttpGet]
    [Route("")]
    public async Task<ApiResponse<List<ProvinceResponse>>> GetProvinceAsync()
    {
        try
        {
            var execute = await unitOfWork.Provinces.GetAllProvinces();
            return ApiResponseFactory.SuccessResponse(execute,
                execute.Count != 0 ? "provinces fetched successfully" : "provinces fetched empty");
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<ProvinceResponse>>(ex.Message);
        }
    }

    [HttpDelete]
    [Route("{provinceCode}")]
    public async Task<ApiResponse<int>> DeleteProvinceAsync([Required] string provinceCode)
    {
        var claim = Common.DecodeJwt(HttpContext.User);
        try
        {
            var affectedRow = await unitOfWork.Provinces.DeleteAsync(provinceCode);
            if (affectedRow > 0)
                return ApiResponse<int>.Builder()
                    .WithMessage("Province deleted successfully")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(affectedRow)
                    .Build();
            else
                return ApiResponse<int>.Builder()
                    .WithMessage("Province deleted unsuccessfully")
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }

    [HttpPut]
    [Route("")]
    public async Task<ApiResponse<int>> UpdateProvinceAsync([Required] [FromBody] ProvinceUpdateRequest model)
    {
        var claim = Common.DecodeJwt(HttpContext.User);
        try
        {
            var provinceModel = new ProvinceModel
            {
                ProvinceId = model.ProvinceId,
                Province = model.Province
            };
            var affectedRow = await unitOfWork.Provinces.UpdateAsync(provinceModel);
            if (affectedRow > 0)
                return ApiResponse<int>.Builder()
                    .WithMessage("Province updated successfully")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(affectedRow)
                    .Build();
            else
                return ApiResponse<int>.Builder()
                    .WithMessage("Province updated unsuccessfully")
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }

    [HttpPost]
    [Route("")]
    public async Task<ApiResponse<int>> AddDistrictAsync([Required] [FromBody] ProvinceCreateRequest model)
    {
        var claim = Common.DecodeJwt(HttpContext.User);
        try
        {
            var provinceModel = new ProvinceModel
            {
                Province = model.Province
            };
            var affectedRow = await unitOfWork.Provinces.AddNewAsync(provinceModel);
            if (affectedRow > 0)
                return ApiResponse<int>.Builder()
                    .WithMessage("Province added successfully")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(affectedRow)
                    .Build();
            else
                return ApiResponse<int>.Builder()
                    .WithMessage("Province added unsuccessfully")
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }
}