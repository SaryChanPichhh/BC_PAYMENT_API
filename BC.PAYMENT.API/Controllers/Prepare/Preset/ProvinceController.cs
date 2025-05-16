using BC.PAYMENT.API.Models;
using BC.PAYMENT.CORE.DTO.Prepare.Preset;
using BC.PAYMENT.CORE.Entities.Prepare.Preset;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.APPLICATION.Interfaces.General;

namespace BC.PAYMENT.API.Controllers.Prepare.Preset
{
    public class ProvinceController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProvinceController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("")]
        public async Task<ApiResponse<List<ProvinceModel>>> GetProvinceAsync([Required] string provinceName)
        {
            var province = new ApiResponse<List<ProvinceModel>>();
            var claim = Common.DecodeJwt(HttpContext.User);
            try
            {
                var execute = await _unitOfWork.Provinces.GetAsync(provinceName);
                if (execute.Any())
                {
                    province.Message = "Provinces fetched successfully";
                    province.Success = true;
                    province.StatusCode = (int)HttpStatusCode.OK;
                    province.Result = execute;
                }
                else
                {
                    province.Message = "Provinces fetched unsuccessfully";
                    province.Success = false;
                    province.StatusCode = (int)HttpStatusCode.BadRequest;
                    province.Result = new List<ProvinceModel>();
                }
            }
            catch (SqlException ex)
            {
                province.Message =$@"Sql Exception : {ex.Message}";
                Debug.WriteLine(ex.StackTrace);
                province.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            catch (Exception ex)
            {
                province.Message =$@"Error Exception : {ex.Message}";
                Debug.WriteLine(ex.StackTrace);
                province.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            return province;
        }

        [HttpDelete]
        [Route("{provinceCode}")]
        public async Task<ApiResponse<int>> DeleteProvinceAsync([Required] string provinceCode)
        {
            var province = new ApiResponse<int>();
            var claim = Common.DecodeJwt(HttpContext.User);
            try
            {
                var affectedRow = await _unitOfWork.Districts.DeleteAsync(provinceCode);
                if (affectedRow > 0)
                {
                    province.Message = "Province deleted successfully";
                    province.Success = true;
                    province.StatusCode = (int)HttpStatusCode.OK;
                    province.Result = affectedRow;
                }
                else
                {
                    province.Message = "Province deleted unsuccessfully";
                    province.Success = false;
                    province.StatusCode = (int)HttpStatusCode.BadRequest;
                }
            }
            catch (SqlException ex)
            {
                province.Message =$@"Sql Exception : {ex.Message}";
                Debug.WriteLine(ex.StackTrace);
                province.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            catch (Exception ex)
            {
                province.Message =$@"Error Exception : {ex.Message}";
                Debug.WriteLine(ex.StackTrace);
                province.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            return province;
        }

        [HttpPatch]
        [Route("")]
        public async Task<ApiResponse<int>> UpdateProvinceAsync([Required] ProvinceUpdateDto model)
        {
            var province = new ApiResponse<int>();
            var claim = Common.DecodeJwt(HttpContext.User);
            try
            {
                var provinceModel = new ProvinceModel()
                {
                    ProvinceId = model.ProvinceId,
                    Province = model.Province,
                };
                var affectedRow = await _unitOfWork.Provinces.UpdateAsync(provinceModel);
                if (affectedRow > 0)
                {
                    province.Message = "Province updated successfully";
                    province.Success = true;
                    province.StatusCode = (int)HttpStatusCode.OK;
                    province.Result = affectedRow;
                }
                else
                {
                    province.Message = "Province updated unsuccessfully";
                    province.Success = false;
                    province.StatusCode = (int)HttpStatusCode.BadRequest;
                }
            }
            catch (SqlException ex)
            {
                province.Message =$@"Sql Exception : {ex.Message}";
                Debug.WriteLine(ex.StackTrace);
                province.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            catch (Exception ex)
            {
                province.Message =$@"Error Exception : {ex.Message}";
                Debug.WriteLine(ex.StackTrace);
                province.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            return province;
        }

        [HttpPost]
        [Route("")]
        public async Task<ApiResponse<int>> AddDistrictAsync([Required] ProvinceDto model)
        {
            var province = new ApiResponse<int>();
            var claim = Common.DecodeJwt(HttpContext.User);
            try
            {
                var provinceModel = new ProvinceModel()
                { 
                    Province = model.Province,
                };
                var affectedRow = await _unitOfWork.Provinces.AddNewAsync(provinceModel);
                if (affectedRow > 0)
                {
                    province.Message = "Province added successfully";
                    province.Success = true;
                    province.StatusCode = (int)HttpStatusCode.OK;
                    province.Result = affectedRow;
                }
                else
                {
                    province.Message = "Province added unsuccessfully";
                    province.Success = false;
                    province.StatusCode = (int)HttpStatusCode.BadRequest;
                }
            }
            catch (SqlException ex)
            {
                province.Message =$@"Sql Exception : {ex.Message}";
                Debug.WriteLine(ex.StackTrace);
                province.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            catch (Exception ex)
            {
                province.Message =$@"Error Exception : {ex.Message}";
                Debug.WriteLine(ex.StackTrace);
                province.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            return province;
        }
    }
}
