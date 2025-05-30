using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.CashFlow.CashFlowData;
using BC.PAYMENT.CORE.Entities.CashFlow.CashFlowData;
using BC.PAYMENT.CORE.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BC.PAYMENT.API.Controllers.CashFlow.CashFlowData
{
    public class CashFlowDataController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public CashFlowDataController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region Cash Flow Header

        [HttpGet]
        [Route("getcashflowheader")]
        public async Task<ApiResponse<List<CashFlowDataHeader>>> GetCashFlowHeaderAsync()
        {
            var credential = Common.DecodeJwt(User);
            var cashFlowHeader = new ApiResponse<List<CashFlowDataHeader>>() { Timestamp = DateTime.Today };
            try
            {
                var execute = await _unitOfWork.CashFlowData.GetCashFlowHeaderAsync(credential.DbCode);
    
                if (execute.Any())
                {
                    cashFlowHeader.Result = execute;
                    cashFlowHeader.Message = $"Cash flow header fetched successfully";
                    cashFlowHeader.StatusCode = StatusCodes.Status200OK;
                    cashFlowHeader.Success = true;
                }
                else
                {
                    cashFlowHeader.Result = new List<CashFlowDataHeader>();
                    cashFlowHeader.Message = $"Cash flow header fetched unsuccessfully";
                    cashFlowHeader.StatusCode = StatusCodes.Status400BadRequest;
                }
            }
            catch (SqlException ex)
            {
                cashFlowHeader.Message = $@"Sql Exception : {ex.Message}";
                cashFlowHeader.StatusCode = StatusCodes.Status500InternalServerError;

            }
            catch (Exception ex)
            {
                cashFlowHeader.Message = $@"Error Exception : {ex.Message}";
                cashFlowHeader.StatusCode = StatusCodes.Status500InternalServerError;
            }

            return cashFlowHeader;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="date"></param>
        /// <value>2025-05-26</value>
        /// <returns></returns>
        [HttpPost]
        [Route("addnewcashflowheader/{date}")]
        public async Task<ApiResponse<string>> AddNewCashFlowHeaderAsync([Required] string date)
        {
            var credential = Common.DecodeJwt(User);
            var cashFlowHeader = new ApiResponse<string>() { Timestamp = DateTime.Today };
            try
            {
                var cashFlowHeaderModel = new CashFlowDataModel
                {
                    Date = Convert.ToDateTime(date),
                    DbCode = credential.DbCode,
                    CreateBy = credential.Username,
                    Period = credential.InvoiceEntryCode,
                };
                var affectedRow = await _unitOfWork.CashFlowData.AddNewCashFlowHeaderAsync(cashFlowHeaderModel);
                if (affectedRow > 0)
                {
                    cashFlowHeader.Result = date;
                    cashFlowHeader.Message = $"Cash flow header added successfully";
                    cashFlowHeader.StatusCode = StatusCodes.Status200OK;
                    cashFlowHeader.Success = true;
                }
                else
                {
                    cashFlowHeader.Message = $"Cash flow header added unsuccessfully";
                    cashFlowHeader.StatusCode = StatusCodes.Status400BadRequest;
                }
            }
            catch (SqlException ex)
            {
                cashFlowHeader.Message = $@"Sql Exception : {ex.Message}";
                cashFlowHeader.StatusCode = StatusCodes.Status500InternalServerError;

            }
            catch (Exception ex)
            {
                cashFlowHeader.Message = $@"Error Exception : {ex.Message}";
                cashFlowHeader.StatusCode = StatusCodes.Status500InternalServerError;
            }

            return cashFlowHeader;
        }

        [HttpPut]
        [Route("cancelcashflowheader/{headerId}")]
        public async Task<ApiResponse<int>> CancelCashFlowHeaderAsync([Required] int headerId)
        {
            var credential = Common.DecodeJwt(User);
            var cashFlowHeader = new ApiResponse<int>() { Timestamp = DateTime.Today };
            try
            {

                var affectedRow = await _unitOfWork.CashFlowData.CancelCashFlowHeaderAsync(headerId);
                if (affectedRow > 0)
                {
                    cashFlowHeader.Result = headerId;
                    cashFlowHeader.Message = $"Cash flow header set to pending successfully";
                    cashFlowHeader.StatusCode = StatusCodes.Status200OK;
                    cashFlowHeader.Success = true;
                }
                else
                {
                    cashFlowHeader.Message = $"Cash flow header set to pending unsuccessfully";
                    cashFlowHeader.StatusCode = StatusCodes.Status400BadRequest;
                }
            }
            catch (SqlException ex)
            {
                cashFlowHeader.Message = $@"Sql Exception : {ex.Message}";
                cashFlowHeader.StatusCode = StatusCodes.Status500InternalServerError;

            }
            catch (Exception ex)
            {
                cashFlowHeader.Message = $@"Error Exception : {ex.Message}";
                cashFlowHeader.StatusCode = StatusCodes.Status500InternalServerError;
            }

            return cashFlowHeader;
        }
        [HttpDelete]
        [Route("deletecashflowheader/{id}")]
        public async Task<ApiResponse<int>> DeleteCashFlowHeaderAsync([Required] int id)
        {
            var credential = Common.DecodeJwt(User);
            var cashFlowHeader = new ApiResponse<int>() { Timestamp = DateTime.Today };
            try
            {
                var affectedRow = await _unitOfWork.CashFlowData.DeleteCashFlowHeaderAsync(id);
                if (affectedRow > 0)
                {
                    cashFlowHeader.Result = id;
                    cashFlowHeader.Message = $"Cash flow header deleted successfully";
                    cashFlowHeader.StatusCode = StatusCodes.Status200OK;
                    cashFlowHeader.Success = true;
                }
                else
                {
                    cashFlowHeader.Message = $"Cash flow header deleted unsuccessfully";
                    cashFlowHeader.StatusCode = StatusCodes.Status400BadRequest;
                }
            }
            catch (SqlException ex)
            {
                cashFlowHeader.Message = $@"Sql Exception : {ex.Message}";
                cashFlowHeader.StatusCode = StatusCodes.Status500InternalServerError;

            }
            catch (Exception ex)
            {
                cashFlowHeader.Message = $@"Error Exception : {ex.Message}";
                cashFlowHeader.StatusCode = StatusCodes.Status500InternalServerError;
            }

            return cashFlowHeader;
        }

        [HttpPut]
        [Route("updatecashflowheader/{id}/{date}")]
        public async Task<ApiResponse<int>> UpdateCashFlowHeaderAsync([Required] int id, [Required] string date)
        {
            var credential = Common.DecodeJwt(User);
            var cashFlowHeader = new ApiResponse<int>() { Timestamp = DateTime.Today };
            try
            {
                var affectedRow = await _unitOfWork.CashFlowData.UpdateCashFlowHeaderAsync(id, date);
                if (affectedRow > 0)
                {
                    cashFlowHeader.Result = id;
                    cashFlowHeader.Message = $"Cash flow header updated successfully";
                    cashFlowHeader.StatusCode = StatusCodes.Status200OK;
                    cashFlowHeader.Success = true;
                }
                else
                {
                    cashFlowHeader.Message = $"Cash flow header updated unsuccessfully";
                    cashFlowHeader.StatusCode = StatusCodes.Status400BadRequest;
                }
            }
            catch (SqlException ex)
            {
                cashFlowHeader.Message = $@"Sql Exception : {ex.Message}";
                cashFlowHeader.StatusCode = StatusCodes.Status500InternalServerError;

            }
            catch (Exception ex)
            {
                cashFlowHeader.Message = $@"Error Exception : {ex.Message}";
                cashFlowHeader.StatusCode = StatusCodes.Status500InternalServerError;
            }

            return cashFlowHeader;
        }
        #endregion

        #region Cash Flow Detail

        [HttpGet]
        [Route("getcashflowdetailbyheaderid/{id}")]
        public async Task<ApiResponse<List<CashFlowDataDetailModel>>> GetPaymentCashFlowDetailByIdAsync([Required] int id)
        {
            var credential = Common.DecodeJwt(User);
            var cashFlowDetail = new ApiResponse<List<CashFlowDataDetailModel>>() { Timestamp = DateTime.Today };
            try
            {
                var execute = await _unitOfWork.CashFlowData.GetPaymentCashFlowDetailByIdAsync(credential.DbCode, id);
                if (execute.Any())
                {
                    cashFlowDetail.Result = execute;
                    cashFlowDetail.Message = $"Cash flow detail fetched successfully";
                    cashFlowDetail.StatusCode = StatusCodes.Status200OK;
                    cashFlowDetail.Success = true;
                }
                else
                {
                    cashFlowDetail.Message = $"Cash flow detail fetched unsuccessfully";
                    cashFlowDetail.StatusCode = StatusCodes.Status400BadRequest;
                }
            }
            catch (SqlException ex)
            {
                cashFlowDetail.Message = $@"Sql Exception : {ex.Message}";
                cashFlowDetail.StatusCode = StatusCodes.Status500InternalServerError;

            }
            catch (Exception ex)
            {
                cashFlowDetail.Message = $@"Error Exception : {ex.Message}";
                cashFlowDetail.StatusCode = StatusCodes.Status500InternalServerError;
            }

            return cashFlowDetail;
        }
        
        [HttpDelete]
        [Route("deletecashflowdetail/{id}")]
        public async Task<ApiResponse<int>> DeleteCashFlowDetailAsync([Required] int id)
        {
            var credential = Common.DecodeJwt(User);
            var cashFlowDetail = new ApiResponse<int>() { Timestamp = DateTime.Today };
            try
            {
                var affectedRow = await _unitOfWork.CashFlowData.DeleteCashFlowDetailAsync( id);
                if (affectedRow > 0)
                {
                    cashFlowDetail.Result = id;
                    cashFlowDetail.Message = $"Cash flow detail deleted successfully";
                    cashFlowDetail.StatusCode = StatusCodes.Status200OK;
                    cashFlowDetail.Success = true;
                }
                else
                {
                    cashFlowDetail.Message = $"Cash flow detail deleted unsuccessfully";
                    cashFlowDetail.StatusCode = StatusCodes.Status400BadRequest;
                }
            }
            catch (SqlException ex)
            {
                cashFlowDetail.Message = $@"Sql Exception : {ex.Message}";
                cashFlowDetail.StatusCode = StatusCodes.Status500InternalServerError;

            }
            catch (Exception ex)
            {
                cashFlowDetail.Message = $@"Error Exception : {ex.Message}";
                cashFlowDetail.StatusCode = StatusCodes.Status500InternalServerError;
            }

            return cashFlowDetail;
        }
        
        [HttpGet]
        [Route("getdescriptioncashflowdetail")]
        public async Task<ApiResponse<List<string>>> GetDescriptionCashFlowDetailAsync()
        {
            var credential = Common.DecodeJwt(User);
            var cashFlowDetail = new ApiResponse<List<string>>() { Timestamp = DateTime.Today };
            try
            {
                var execute = await _unitOfWork.CashFlowData.GetDescriptionCashFlowDetailAsync( credential.DbCode);
                if (execute.Any())
                {
                    cashFlowDetail.Result = execute;
                    cashFlowDetail.Message = $"Cash flow detail fetched successfully";
                    cashFlowDetail.StatusCode = StatusCodes.Status200OK;
                    cashFlowDetail.Success = true;
                }
                else
                {
                    cashFlowDetail.Message = $"Cash flow detail fetched unsuccessfully";
                    cashFlowDetail.StatusCode = StatusCodes.Status400BadRequest;
                }
            }
            catch (SqlException ex)
            {
                cashFlowDetail.Message = $@"Sql Exception : {ex.Message}";
                cashFlowDetail.StatusCode = StatusCodes.Status500InternalServerError;

            }
            catch (Exception ex)
            {
                cashFlowDetail.Message = $@"Error Exception : {ex.Message}";
                cashFlowDetail.StatusCode = StatusCodes.Status500InternalServerError;
            }

            return cashFlowDetail;
        }

        [HttpPut]
        [Route("updatecashflowdetail")]
        public async Task<ApiResponse<PaymentCashFlowDto>> UpdateCashFlowDetailAsync([FromBody] PaymentCashFlowUpdateDto model)
        {
            var credential = Common.DecodeJwt(User);
            var cashFlowDetail = new ApiResponse<PaymentCashFlowDto>() { Timestamp = DateTime.Today };
            try
            {
                var cashFlowDetailModel = new CashFlowDataDetailModel
                {
                    DbCode = credential.DbCode,
                    Name = model.Name,
                    Date = model.Date,
                    Amount = model.Amount,
                    CurrencyFormat = model.CurrencyFormat.GetDescription(),
                    ExchangeRate = model.ExchangeRate,
                    CreatedBy = credential.Username,
                    Id = model.Id,

                };
                var affectedRow = await _unitOfWork.CashFlowData.UpdateCashFlowDetailAsync(cashFlowDetailModel);
                if (affectedRow > 0 )
                {
                    cashFlowDetail.Result = model;
                    cashFlowDetail.Message = $"Cash flow detail updated successfully";
                    cashFlowDetail.StatusCode = StatusCodes.Status200OK;
                    cashFlowDetail.Success = true;
                }
                else
                {
                    cashFlowDetail.Message = $"Cash flow detail updated unsuccessfully";
                    cashFlowDetail.StatusCode = StatusCodes.Status400BadRequest;
                }
            }
            catch (SqlException ex)
            {
                cashFlowDetail.Message = $@"Sql Exception : {ex.Message}";
                cashFlowDetail.StatusCode = StatusCodes.Status500InternalServerError;

            }
            catch (Exception ex)
            {
                cashFlowDetail.Message = $@"Error Exception : {ex.Message}";
                cashFlowDetail.StatusCode = StatusCodes.Status500InternalServerError;
            }

            return cashFlowDetail;
        }
        [HttpPost]
        [Route("addnewcashflowdetail")]
        public async Task<ApiResponse<PaymentCashFlowDto>> UpdateCashFlowDetailAsync([FromBody] PaymentCashFlowPostDto model)
        {
            var credential = Common.DecodeJwt(User);
            var cashFlowDetail = new ApiResponse<PaymentCashFlowDto>() { Timestamp = DateTime.Today };
            try
            {
                var cashFlowDetailModel = new CashFlowDataDetailModel
                {
                    DbCode = credential.DbCode,
                    Name = model.Name,
                    Date = model.Date,
                    Amount = model.Amount,
                    CurrencyFormat = model.CurrencyFormat.GetDescription(),
                    ExchangeRate = model.ExchangeRate,
                    CreatedBy = credential.Username,
                    HeaderId = model.HeaderId

                };
                var affectedRow = await _unitOfWork.CashFlowData.AddNewCashFlowDetailAsync(cashFlowDetailModel);
                if (affectedRow > 0 )
                {
                    cashFlowDetail.Result = model;
                    cashFlowDetail.Message = $"Cash flow detail added successfully";
                    cashFlowDetail.StatusCode = StatusCodes.Status200OK;
                    cashFlowDetail.Success = true;
                }
                else
                {
                    cashFlowDetail.Message = $"Cash flow detail added unsuccessfully";
                    cashFlowDetail.StatusCode = StatusCodes.Status400BadRequest;
                }
            }
            catch (SqlException ex)
            {
                cashFlowDetail.Message = $@"Sql Exception : {ex.Message}";
                cashFlowDetail.StatusCode = StatusCodes.Status500InternalServerError;

            }
            catch (Exception ex)
            {
                cashFlowDetail.Message = $@"Error Exception : {ex.Message}";
                cashFlowDetail.StatusCode = StatusCodes.Status500InternalServerError;
            }

            return cashFlowDetail;
        }
        
        
        [HttpPost]
        [Route("submittingcashflowheader")]
        public async Task<ApiResponse<List<SubmittedCashFlowPostDto>>> AddNewCashFlowSubmittedAsync([FromBody] List<SubmittedCashFlowPostDto> model)
        {
            var credential = Common.DecodeJwt(User);
            var cashFlowDetail = new ApiResponse<List<SubmittedCashFlowPostDto>>() { Timestamp = DateTime.Today };
            try
            {
                var lsCashFlowDetailModel = model.Select(x => new CashFlowDataDetailModel()
                {
                    DbCode = credential.DbCode,
                    Name =x.Name,
                    Date = x.Date,
                    Amount = x.Amount,
                    CurrencyFormat = x.CurrencyFormat.GetDescription(),
                    ExchangeRate = x.ExchangeRate,
                    CreatedBy = credential.Username,
                    HeaderId = x.HeaderId,
                    Id = x.Id
                }).ToList();
          
                var affectedRow = await _unitOfWork.CashFlowData.AddNewCashFlowSubmittedAsync(lsCashFlowDetailModel);
                if (affectedRow > 0 )
                {
                    cashFlowDetail.Result = model;
                    cashFlowDetail.Message = $"Cash flow detail submitted added successfully";
                    cashFlowDetail.StatusCode = StatusCodes.Status200OK;
                    cashFlowDetail.Success = true;
                }
                else
                {
                    cashFlowDetail.Message = $"Cash flow detail submitted unsuccessfully";
                    cashFlowDetail.StatusCode = StatusCodes.Status400BadRequest;
                }
            }
            catch (SqlException ex)
            {
                cashFlowDetail.Message = $@"Sql Exception : {ex.Message}";
                cashFlowDetail.StatusCode = StatusCodes.Status500InternalServerError;

            }
            catch (Exception ex)
            {
                cashFlowDetail.Message = $@"Error Exception : {ex.Message}";
                cashFlowDetail.StatusCode = StatusCodes.Status500InternalServerError;
            }

            return cashFlowDetail;
        }

        #endregion

    }
}
