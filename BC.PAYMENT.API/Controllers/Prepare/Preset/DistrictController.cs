using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.APPLICATION.Interfaces.Prepare.Preset;
using BC.PAYMENT.CORE.DTO.Prepare.Preset;
using BC.PAYMENT.CORE.Entities.Prepare.Preset;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BC.PAYMENT.API.Controllers.Prepare.Preset
{
    public class DistrictController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public DistrictController( IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("")]
        public async Task<ApiResponse<List<DistrictModel>>> GetDistrict()
        {
            var district = new ApiResponse<List<DistrictModel>>();
            var claim = Common.DecodeJwt(HttpContext.User);
            try
            {
                var execute = await _unitOfWork.Districts.GetAsync(claim.DbCode!);
                if (execute.Any())
                {
                    district.Message = "District fetched successfully";
                    district.Success = true;
                    district.StatusCode = (int)HttpStatusCode.OK;
                    district.Result = execute;
                }
                else
                {
                    district.Message = "District fetched unsuccessfully";
                    district.Success = false;
                    district.StatusCode = (int)HttpStatusCode.BadRequest;
                    district.Result = new List<DistrictModel>();
                }
            }
            catch (SqlException ex)
            {
                district.Message =$@"Sql Exception : {ex.Message}";
                Debug.WriteLine(ex.StackTrace);
                district.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            catch (Exception ex)
            {
                district.Message =$@"Error Exception : {ex.Message}";
                Debug.WriteLine(ex.StackTrace);
                district.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            return district;
        }
        [HttpGet]
        [Route("GetDistrictByDistrict/{districtName}")]
        public async Task<ApiResponse<List<DistrictModel>>> GetDistrictByDistrictAsync([Required]string districtName)
        {
            var district = new ApiResponse<List<DistrictModel>>();
            var claim = Common.DecodeJwt(HttpContext.User);
            try
            {
                var execute = await _unitOfWork.Districts.GetDistrictsByDistrictAsync(districtName);
                if (execute.Any())
                {
                    district.Message = "District fetched successfully";
                    district.Success = true;
                    district.StatusCode = (int)HttpStatusCode.OK;
                    district.Result = execute;
                }
                else
                {
                    district.Message = "District fetched unsuccessfully";
                    district.Success = false;
                    district.StatusCode = (int)HttpStatusCode.BadRequest;
                    district.Result = new List<DistrictModel>();
                }
            }
            catch (SqlException ex)
            {
                district.Message =$@"Sql Exception : {ex.Message}";
                Debug.WriteLine(ex.StackTrace);
                district.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            catch (Exception ex)
            {
                district.Message =$@"Error Exception : {ex.Message}";
                Debug.WriteLine(ex.StackTrace);
                district.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            return district;
        }
        
        [HttpGet]
        [Route("GetDistrictByProvince/{provinceName}")]
        public async Task<ApiResponse<List<DistrictModel>>> GetDistrictByProvinceAsync([Required]string provinceName)
        {
            var district = new ApiResponse<List<DistrictModel>>();
            var claim = Common.DecodeJwt(HttpContext.User);
            try
            {
                var execute = await _unitOfWork.Districts.GetDistrictsByProvinceAsync(provinceName);
                if (execute.Any())
                {
                    district.Message = "District fetched successfully";
                    district.Success = true;
                    district.StatusCode = (int)HttpStatusCode.OK;
                    district.Result = execute;
                }
                else
                {
                    district.Message = "District fetched unsuccessfully";
                    district.Success = false;
                    district.StatusCode = (int)HttpStatusCode.BadRequest;
                    district.Result = new List<DistrictModel>();
                }
            }
            catch (SqlException ex)
            {
                district.Message =$@"Sql Exception : {ex.Message}";
                Debug.WriteLine(ex.StackTrace);
                district.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            catch (Exception ex)
            {
                district.Message =$@"Error Exception : {ex.Message}";
                Debug.WriteLine(ex.StackTrace);
                district.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            return district;
        }
        
        [HttpDelete]
        [Route("{districtCode}")]
        public async Task<ApiResponse<int>> DeleteDistrictAsync([Required]string districtCode)
        {
            var district = new ApiResponse<int>();
            var claim = Common.DecodeJwt(HttpContext.User);
            try
            {
                var affectedRow = await _unitOfWork.Districts.DeleteAsync(districtCode);
                if (affectedRow > 0 )  
                {
                    district.Message = "District deleted successfully";
                    district.Success = true;
                    district.StatusCode = (int)HttpStatusCode.OK;
                    district.Result = affectedRow;
                }
                else
                {
                    district.Message = "District deleted unsuccessfully";
                    district.Success = false;
                    district.StatusCode = (int)HttpStatusCode.BadRequest;
                }
            }
            catch (SqlException ex)
            {
                district.Message =$@"Sql Exception : {ex.Message}";
                Debug.WriteLine(ex.StackTrace);
                district.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            catch (Exception ex)
            {
                district.Message =$@"Error Exception : {ex.Message}";
                Debug.WriteLine(ex.StackTrace);
                district.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            return district;
        }
        
        [HttpPatch]
        [Route("")]
        public async Task<ApiResponse<int>> UpdateDistrictAsync([Required] DistrictUpdateDto model)
        {
            var district = new ApiResponse<int>();
            var claim = Common.DecodeJwt(HttpContext.User);
            try
            {
                var districtModel = new DistrictModel()
                {
                    District = model.District,
                    DistrictId = model.DistrictId,
                    ProvinceId = model.ProvinceId,
                };
                var affectedRow = await _unitOfWork.Districts.UpdateAsync(districtModel);
                if (affectedRow > 0 )  
                {
                    district.Message = "District updated successfully";
                    district.Success = true;
                    district.StatusCode = (int)HttpStatusCode.OK;
                    district.Result = affectedRow;
                }
                else
                {
                    district.Message = "District updated unsuccessfully";
                    district.Success = false;
                    district.StatusCode = (int)HttpStatusCode.BadRequest;
                }
            }
            catch (SqlException ex)
            {
                district.Message =$@"Sql Exception : {ex.Message}";
                Debug.WriteLine(ex.StackTrace);
                district.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            catch (Exception ex)
            {
                district.Message =$@"Error Exception : {ex.Message}";
                Debug.WriteLine(ex.StackTrace);
                district.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            return district;
        }
        
        [HttpPost]
        [Route("")]
        public async Task<ApiResponse<int>> AddDistrictAsync([Required] DistrictDto model)
        {
            var district = new ApiResponse<int>();
            var claim = Common.DecodeJwt(HttpContext.User);
            try
            {
                var districtModel = new DistrictModel()
                {
                    District = model.District,
                    ProvinceId = model.ProvinceId,
                };
                var affectedRow = await _unitOfWork.Districts.AddNewAsync(districtModel);
                if (affectedRow > 0 )  
                {
                    district.Message = "District added successfully";
                    district.Success = true;
                    district.StatusCode = (int)HttpStatusCode.OK;
                    district.Result = affectedRow;
                }
                else
                {
                    district.Message = "District added unsuccessfully";
                    district.Success = false;
                    district.StatusCode = (int)HttpStatusCode.BadRequest;
                }
            }
            catch (SqlException ex)
            {
                district.Message =$@"Sql Exception : {ex.Message}";
                Debug.WriteLine(ex.StackTrace);
                district.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            catch (Exception ex)
            {
                district.Message =$@"Error Exception : {ex.Message}";
                Debug.WriteLine(ex.StackTrace);
                district.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            return district;
        }
    }
}
