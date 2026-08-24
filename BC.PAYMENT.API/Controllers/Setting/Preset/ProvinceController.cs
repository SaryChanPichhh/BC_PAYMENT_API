using System.ComponentModel.DataAnnotations;
using System.Net;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.Prepare.Preset;
using BC.PAYMENT.CORE.Entities.Prepare.Preset;
using Microsoft.AspNetCore.Mvc;

namespace BC.PAYMENT.API.Controllers.Setting.Preset
{
    public class ProvinceController(IUnitOfWork unitOfWork) : BaseApiController
    {
        [HttpGet]
        [Route("")]
        public async Task<ApiResponse<List<ProvinceModel>>> GetProvinceAsync([Required] string provinceName)
        {
            var claim = Common.DecodeJwt(HttpContext.User);
            try
            {
                var execute = await unitOfWork.Provinces.GetAsync(provinceName);
                if (execute.Any())
                {
                    return ApiResponse<List<ProvinceModel>>.Builder()
                        .WithMessage("Provinces fetched successfully")
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithResult(execute)
                        .Build();
                }
                else
                {
                    return ApiResponse<List<ProvinceModel>>.Builder()
                        .WithMessage("Provinces fetched unsuccessfully")
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithResult(new List<ProvinceModel>())
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<ProvinceModel>>(ex.Message);
            }
        }

        [HttpDelete]
        [Route("{provinceCode}")]
        public async Task<ApiResponse<int>> DeleteProvinceAsync([Required] string provinceCode)
        {
            var claim = Common.DecodeJwt(HttpContext.User);
            try
            {
                var affectedRow = await unitOfWork.Districts.DeleteAsync(provinceCode);
                if (affectedRow > 0)
                {
                    return ApiResponse<int>.Builder()
                        .WithMessage("Province deleted successfully")
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithResult(affectedRow)
                        .Build();
                }
                else
                {
                    return ApiResponse<int>.Builder()
                        .WithMessage("Province deleted unsuccessfully")
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
            }
        }

        [HttpPatch]
        [Route("")]
        public async Task<ApiResponse<int>> UpdateProvinceAsync([Required] ProvinceUpdateDto model)
        {
            var claim = Common.DecodeJwt(HttpContext.User);
            try
            {
                var provinceModel = new ProvinceModel()
                {
                    ProvinceId = model.ProvinceId,
                    Province = model.Province,
                };
                var affectedRow = await unitOfWork.Provinces.UpdateAsync(provinceModel);
                if (affectedRow > 0)
                {
                    return ApiResponse<int>.Builder()
                        .WithMessage("Province updated successfully")
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithResult(affectedRow)
                        .Build();
                }
                else
                {
                    return ApiResponse<int>.Builder()
                        .WithMessage("Province updated unsuccessfully")
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
            }
        }

        [HttpPost]
        [Route("")]
        public async Task<ApiResponse<int>> AddDistrictAsync([Required] ProvinceDto model)
        {
            var claim = Common.DecodeJwt(HttpContext.User);
            try
            {
                var provinceModel = new ProvinceModel()
                { 
                    Province = model.Province,
                };
                var affectedRow = await unitOfWork.Provinces.AddNewAsync(provinceModel);
                if (affectedRow > 0)
                {
                    return ApiResponse<int>.Builder()
                        .WithMessage("Province added successfully")
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithResult(affectedRow)
                        .Build();
                }
                else
                {
                    return ApiResponse<int>.Builder()
                        .WithMessage("Province added unsuccessfully")
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
            }
        }
    }
}
