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
    public class DistrictController(IUnitOfWork unitOfWork) : BaseApiController
    {
        [HttpGet]
        [Route("")]
        public async Task<ApiResponse<List<DistrictModel>>> GetDistrict()
        {
            var claim = Common.DecodeJwt(HttpContext.User);
            try
            {
                var execute = await unitOfWork.Districts.GetAsync(claim.DbCode!);
                if (execute.Any())
                {
                    return ApiResponse<List<DistrictModel>>.Builder()
                        .WithMessage("District fetched successfully")
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithResult(execute)
                        .Build();
                }
                else
                {
                    return ApiResponse<List<DistrictModel>>.Builder()
                        .WithMessage("District fetched unsuccessfully")
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithResult(new List<DistrictModel>())
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<DistrictModel>>(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetDistrictByDistrict/{districtName}")]
        public async Task<ApiResponse<List<DistrictModel>>> GetDistrictByDistrictAsync([Required]string districtName)
        {
            var claim = Common.DecodeJwt(HttpContext.User);
            try
            {
                var execute = await unitOfWork.Districts.GetDistrictsByDistrictAsync(districtName);
                if (execute.Any())
                {
                    return ApiResponse<List<DistrictModel>>.Builder()
                        .WithMessage("District fetched successfully")
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithResult(execute)
                        .Build();
                }
                else
                {
                    return ApiResponse<List<DistrictModel>>.Builder()
                        .WithMessage("District fetched unsuccessfully")
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithResult(new List<DistrictModel>())
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<DistrictModel>>(ex.Message);
            }
        }
        
        [HttpGet]
        [Route("GetDistrictByProvince/{provinceName}")]
        public async Task<ApiResponse<List<DistrictModel>>> GetDistrictByProvinceAsync([Required]string provinceName)
        {
            var claim = Common.DecodeJwt(HttpContext.User);
            try
            {
                var execute = await unitOfWork.Districts.GetDistrictsByProvinceAsync(provinceName);
                if (execute.Any())
                {
                    return ApiResponse<List<DistrictModel>>.Builder()
                        .WithMessage("District fetched successfully")
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithResult(execute)
                        .Build();
                }
                else
                {
                    return ApiResponse<List<DistrictModel>>.Builder()
                        .WithMessage("District fetched unsuccessfully")
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithResult(new List<DistrictModel>())
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<DistrictModel>>(ex.Message);
            }
        }
        
        [HttpDelete]
        [Route("{districtCode}")]
        public async Task<ApiResponse<int>> DeleteDistrictAsync([Required]string districtCode)
        {
            var claim = Common.DecodeJwt(HttpContext.User);
            try
            {
                var affectedRow = await unitOfWork.Districts.DeleteAsync(districtCode);
                if (affectedRow > 0 )  
                {
                    return ApiResponse<int>.Builder()
                        .WithMessage("District deleted successfully")
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithResult(affectedRow)
                        .Build();
                }
                else
                {
                    return ApiResponse<int>.Builder()
                        .WithMessage("District deleted unsuccessfully")
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
        public async Task<ApiResponse<int>> UpdateDistrictAsync([Required] DistrictUpdateDto model)
        {
            var claim = Common.DecodeJwt(HttpContext.User);
            try
            {
                var districtModel = new DistrictModel()
                {
                    District = model.District,
                    DistrictId = model.DistrictId,
                    ProvinceId = model.ProvinceId,
                };
                var affectedRow = await unitOfWork.Districts.UpdateAsync(districtModel);
                if (affectedRow > 0 )  
                {
                    return ApiResponse<int>.Builder()
                        .WithMessage("District updated successfully")
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithResult(affectedRow)
                        .Build();
                }
                else
                {
                    return ApiResponse<int>.Builder()
                        .WithMessage("District updated unsuccessfully")
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
        public async Task<ApiResponse<int>> AddDistrictAsync([Required] DistrictDto model)
        {
            var claim = Common.DecodeJwt(HttpContext.User);
            try
            {
                var districtModel = new DistrictModel()
                {
                    District = model.District,
                    ProvinceId = model.ProvinceId,
                };
                var affectedRow = await unitOfWork.Districts.AddNewAsync(districtModel);
                if (affectedRow > 0 )  
                {
                    return ApiResponse<int>.Builder()
                        .WithMessage("District added successfully")
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithResult(affectedRow)
                        .Build();
                }
                else
                {
                    return ApiResponse<int>.Builder()
                        .WithMessage("District added unsuccessfully")
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
