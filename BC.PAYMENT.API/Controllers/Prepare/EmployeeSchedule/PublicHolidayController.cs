using System.Net;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.Prepare.EmployeeSchedule;
using BC.PAYMENT.CORE.Entities.Prepare.EmployeeSchedule;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BC.PAYMENT.API.Controllers.Prepare.EmployeeSchedule
{
    public class PublicHolidayController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public PublicHolidayController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("")]
        public async Task<ApiResponse<List<PublicHolidayModel>>> GetPublicHolidayAsync()
        {
            var credential = Common.DecodeJwt(HttpContext.User);
            var publicHoliday = new ApiResponse<List<PublicHolidayModel>>();
            try
            {
                var execute = await _unitOfWork.PublicHoliday.GetAsync(credential.DbCode!);
                if (execute.Count > 0)
                {
                    publicHoliday.Message = "Public Holiday fetched successfully";
                    publicHoliday.Success = true;
                    publicHoliday.StatusCode = (int)HttpStatusCode.OK;
                    publicHoliday.Result = execute;
                }
                else
                {
                    publicHoliday.Message = "Public Holiday fetched unsuccessfully";
                    publicHoliday.StatusCode = (int)HttpStatusCode.BadRequest;
                    publicHoliday.Result = new List<PublicHolidayModel>();
                }
            }catch(SqlException ex)
            {
                publicHoliday.Message = ex.Message;
                publicHoliday.StatusCode = (int)HttpStatusCode.InternalServerError;
                publicHoliday.Result = new List<PublicHolidayModel>();
            }
            catch (Exception ex)
            {
                publicHoliday.Message = ex.Message;
                publicHoliday.StatusCode = (int)HttpStatusCode.InternalServerError;
                publicHoliday.Result = new List<PublicHolidayModel>();
            }   

            return publicHoliday;
        }
        
        [HttpGet]
        [Route("getmaxcode")]
        public async Task<ApiResponse<string>> GetMaxCodePublicHolidayAsync()
        {
            var publicHoliday = new ApiResponse<string>();
            try
            {
                var execute = await _unitOfWork.PublicHoliday.GetMaxCodePublicHolidayAsync();
                if (execute.Any())
                {
                    publicHoliday.Message = "Max code fetched successfully";
                    publicHoliday.Success = true;
                    publicHoliday.StatusCode = (int)HttpStatusCode.OK;
                    publicHoliday.Result = execute;
                }
                else
                {
                    publicHoliday.Message = "Max code fetched unsuccessfully";
                    publicHoliday.StatusCode = (int)HttpStatusCode.BadRequest;
                    publicHoliday.Result = string.Empty;
                }
            }catch(SqlException ex)
            {
                publicHoliday.Message = ex.Message;
                publicHoliday.StatusCode = (int)HttpStatusCode.InternalServerError;
                publicHoliday.Result = string.Empty;
            }
            catch (Exception ex)
            {
                publicHoliday.Message = ex.Message;
                publicHoliday.StatusCode = (int)HttpStatusCode.InternalServerError;
                publicHoliday.Result = string.Empty;
            }   

            return publicHoliday;
        }
        
        [HttpDelete]
        [Route("{code}")]
        public async Task<ApiResponse<string>> DeletePublicHolidayAsync(string code)
        {
            var credential = Common.DecodeJwt(HttpContext.User);
            var publicHoliday = new ApiResponse<string>();
            try
            {
                var affectedRow = await _unitOfWork.PublicHoliday.DeleteAsync(code);
                if (affectedRow > 0)
                {
                    publicHoliday.Message = "Public Holiday deleted successfully";
                    publicHoliday.Success = true;
                    publicHoliday.StatusCode = (int)HttpStatusCode.OK;
                    publicHoliday.Result = code;
                }
                else
                {
                    publicHoliday.Message = "Public Holiday deleted unsuccessfully";
                    publicHoliday.StatusCode = (int)HttpStatusCode.BadRequest;
                }
            }catch(SqlException ex)
            {
                publicHoliday.Message = ex.Message;
                publicHoliday.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            catch (Exception ex)
            {
                publicHoliday.Message = ex.Message;
                publicHoliday.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            return publicHoliday;
        }
        
        [HttpPut]
        [Route("")]
        public async Task<ApiResponse<PublicHolidayDto>> UpdatePublicHolidayAsync(PublicHolidayDto model)
        {
            var credential = Common.DecodeJwt(HttpContext.User);
            var publicHoliday = new ApiResponse<PublicHolidayDto>();
            try
            {
                var publicHolidayModel = new PublicHolidayModel
                {
                    Code = model.Code,
                    Description = model.Description,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    Remark = model.Remark,
                    UserName = credential.Username,
                };
                var affectedRow = await _unitOfWork.PublicHoliday.UpdateAsync(publicHolidayModel);
                if (affectedRow > 0)
                {
                    publicHoliday.Message = "Public Holiday updated successfully";
                    publicHoliday.Success = true;
                    publicHoliday.StatusCode = (int)HttpStatusCode.OK;
                    publicHoliday.Result = model;
                }
                else
                {
                    publicHoliday.Message = "Public Holiday updated unsuccessfully";
                    publicHoliday.StatusCode = (int)HttpStatusCode.BadRequest;
                }
            }catch(SqlException ex)
            {
                publicHoliday.Message = ex.Message;
                publicHoliday.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            catch (Exception ex)
            {
                publicHoliday.Message = ex.Message;
                publicHoliday.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            return publicHoliday;
        }
        
        [HttpPost]
        [Route("")]
        public async Task<ApiResponse<PublicHolidayDto>> AddNewPublicHolidayAsync(PublicHolidayDto model)
        {
            var credential = Common.DecodeJwt(HttpContext.User);
            var publicHoliday = new ApiResponse<PublicHolidayDto>();
            try
            {
                var publicHolidayModel = new PublicHolidayModel
                {
                    Code = model.Code,
                    Description = model.Description,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    Remark = model.Remark,
                    UserName = credential.Username,
                };

                var affectedRow = await _unitOfWork.PublicHoliday.AddNewAsync(publicHolidayModel);
                if (affectedRow > 0)
                {
                    publicHoliday.Message = "Public Holiday added successfully";
                    publicHoliday.Success = true;
                    publicHoliday.StatusCode = (int)HttpStatusCode.OK;
                    publicHoliday.Result = model;
                }
                else
                {
                    publicHoliday.Message = "Public Holiday added unsuccessfully";
                    publicHoliday.StatusCode = (int)HttpStatusCode.BadRequest;
                }
            }catch(SqlException ex)
            {
                publicHoliday.Message = ex.Message;
                publicHoliday.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            catch (Exception ex)
            {
                publicHoliday.Message = ex.Message;
                publicHoliday.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            return publicHoliday;
        }
    }
}
