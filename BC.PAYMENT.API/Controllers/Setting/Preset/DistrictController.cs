using System.ComponentModel.DataAnnotations;
using System.Net;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.Contracts.Setting.Preset;
using BC.PAYMENT.CORE.Entities.Setting.Preset;
using Microsoft.AspNetCore.Mvc;

namespace BC.PAYMENT.API.Controllers.Setting.Preset
{
    public class DistrictController(IUnitOfWork unitOfWork) : BaseApiController
    {
        [HttpGet]
        [Route("")]
        public async Task<ApiResponse<List<DistrictResponseDTO>>> GetDistrict()
        {
            var claim = Common.DecodeJwt(HttpContext.User);
            try
            {
                var execute = await unitOfWork.Districts.GetAsync(claim.DbCode!);
                if (execute.Any())
                {
                    return ApiResponse<List<DistrictResponseDTO>>.Builder()
                        .WithMessage("District fetched successfully")
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithResult(execute)
                        .Build();
                }
                else
                {
                    return ApiResponse<List<DistrictResponseDTO>>.Builder()
                        .WithMessage("District fetched unsuccessfully")
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithResult(new List<DistrictResponseDTO>())
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<DistrictResponseDTO>>(ex.Message);
            }
        }

        [HttpGet]
        [Route("by-name/{districtName}")]
        public async Task<ApiResponse<List<DistrictResponseDTO>>> GetDistrictByDistrictAsync([Required]string districtName)
        {
            var claim = Common.DecodeJwt(HttpContext.User);
            try
            {
                var execute = await unitOfWork.Districts.GetDistrictsByDistrictAsync(districtName);
                if (execute.Any())
                {
                    return ApiResponse<List<DistrictResponseDTO>>.Builder()
                        .WithMessage("District fetched successfully")
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithResult(execute)
                        .Build();
                }
                else
                {
                    return ApiResponse<List<DistrictResponseDTO>>.Builder()
                        .WithMessage("District fetched unsuccessfully")
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithResult(new List<DistrictResponseDTO>())
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<DistrictResponseDTO>>(ex.Message);
            }
        }
        
        [HttpGet]
        [Route("by-province-id/{provinceId}")]
        public async Task<ApiResponse<List<DistrictResponseDTO>>> GetDistrictByProvinceAsync([Required]string provinceId)
        {
            try
            {
                var execute = await unitOfWork.Districts.GetDistrictsByProvinceAsync(provinceId);
                if (execute.Any())
                {
                    return ApiResponse<List<DistrictResponseDTO>>.Builder()
                        .WithMessage("District fetched successfully")
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithResult(execute)
                        .Build();
                }
                else
                {
                    return ApiResponse<List<DistrictResponseDTO>>.Builder()
                        .WithMessage("District fetched unsuccessfully")
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithResult(new List<DistrictResponseDTO>())
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<DistrictResponseDTO>>(ex.Message);
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
        
        [HttpPut]
        [Route("")]
        public async Task<ApiResponse<int>> UpdateDistrictAsync([Required] DistrictUpdateDto model)
        {
            var claim = Common.DecodeJwt(HttpContext.User);
            try
            {
                var districtModel = new DistrictModel
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
