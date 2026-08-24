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
    public class CashFlowDataController(IUnitOfWork unitOfWork) : BaseApiController
    {

        #region Cash Flow Header

        [HttpGet]
        [Route("getcashflowheader")]
        public async Task<ApiResponse<List<CashFlowDataHeader>>> GetCashFlowHeaderAsync()
        {
            try
            {
                var credential = Common.DecodeJwt(User);
                var execute = await unitOfWork.CashFlowData.GetCashFlowHeaderAsync(credential.DbCode);
                
                return ApiResponse<List<CashFlowDataHeader>>.Builder()
                    .WithMessage(execute.Any() ? "Cash flow header fetched successfully" : "Cash flow header fetched unsuccessfully")
                    .WithStatusCode(execute.Any() ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest)
                    .WithResult(execute.Any() ? execute : new List<CashFlowDataHeader>())
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<CashFlowDataHeader>>(ex.Message);
            }
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
            try
            {
                var credential = Common.DecodeJwt(User);
                var cashFlowHeaderModel = new CashFlowDataModel
                {
                    Date = Convert.ToDateTime(date),
                    DbCode = credential.DbCode,
                    CreateBy = credential.Username,
                    Period = credential.InvoiceEntryCode,
                };
                var affectedRow = await unitOfWork.CashFlowData.AddNewCashFlowHeaderAsync(cashFlowHeaderModel);
                
                return ApiResponse<string>.Builder()
                    .WithMessage(affectedRow > 0 ? "Cash flow header added successfully" : "Cash flow header added unsuccessfully")
                    .WithStatusCode(affectedRow > 0 ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest)
                    .WithResult(affectedRow > 0 ? date : null)
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<string>(ex.Message);
            }
        }

        [HttpPut]
        [Route("cancelcashflowheader/{headerId}")]
        public async Task<ApiResponse<int>> CancelCashFlowHeaderAsync([Required] int headerId)
        {
            try
            {
                var credential = Common.DecodeJwt(User);
                var affectedRow = await unitOfWork.CashFlowData.CancelCashFlowHeaderAsync(headerId);
                
                return ApiResponse<int>.Builder()
                    .WithMessage(affectedRow > 0 ? "Cash flow header set to pending successfully" : "Cash flow header set to pending unsuccessfully")
                    .WithStatusCode(affectedRow > 0 ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest)
                    .WithResult(affectedRow > 0 ? headerId : 0)
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
            }
        }
        [HttpDelete]
        [Route("deletecashflowheader/{id}")]
        public async Task<ApiResponse<int>> DeleteCashFlowHeaderAsync([Required] int id)
        {
            try
            {
                var credential = Common.DecodeJwt(User);
                var affectedRow = await unitOfWork.CashFlowData.DeleteCashFlowHeaderAsync(id);
                
                return ApiResponse<int>.Builder()
                    .WithMessage(affectedRow > 0 ? "Cash flow header deleted successfully" : "Cash flow header deleted unsuccessfully")
                    .WithStatusCode(affectedRow > 0 ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest)
                    .WithResult(affectedRow > 0 ? id : 0)
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
            }
        }

        [HttpPut]
        [Route("updatecashflowheader/{id}/{date}")]
        public async Task<ApiResponse<int>> UpdateCashFlowHeaderAsync([Required] int id, [Required] string date)
        {
            try
            {
                var credential = Common.DecodeJwt(User);
                var affectedRow = await unitOfWork.CashFlowData.UpdateCashFlowHeaderAsync(id, date);
                
                return ApiResponse<int>.Builder()
                    .WithMessage(affectedRow > 0 ? "Cash flow header updated successfully" : "Cash flow header updated unsuccessfully")
                    .WithStatusCode(affectedRow > 0 ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest)
                    .WithResult(affectedRow > 0 ? id : 0)
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
            }
        }
        #endregion

        #region Cash Flow Detail

        [HttpGet]
        [Route("getcashflowdetailbyheaderid/{id}")]
        public async Task<ApiResponse<List<CashFlowDataDetailModel>>> GetPaymentCashFlowDetailByIdAsync([Required] int id)
        {
            try
            {
                var credential = Common.DecodeJwt(User);
                var execute = await unitOfWork.CashFlowData.GetPaymentCashFlowDetailByIdAsync(credential.DbCode, id);
                
                return ApiResponse<List<CashFlowDataDetailModel>>.Builder()
                    .WithMessage(execute.Any() ? "Cash flow detail fetched successfully" : "Cash flow detail fetched unsuccessfully")
                    .WithStatusCode(execute.Any() ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest)
                    .WithResult(execute.Any() ? execute : new List<CashFlowDataDetailModel>())
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<CashFlowDataDetailModel>>(ex.Message);
            }
        }
        
        [HttpDelete]
        [Route("deletecashflowdetail/{id}")]
        public async Task<ApiResponse<int>> DeleteCashFlowDetailAsync([Required] int id)
        {
            try
            {
                var credential = Common.DecodeJwt(User);
                var affectedRow = await unitOfWork.CashFlowData.DeleteCashFlowDetailAsync( id);
                
                return ApiResponse<int>.Builder()
                    .WithMessage(affectedRow > 0 ? "Cash flow detail deleted successfully" : "Cash flow detail deleted unsuccessfully")
                    .WithStatusCode(affectedRow > 0 ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest)
                    .WithResult(affectedRow > 0 ? id : 0)
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
            }
        }
        
        [HttpGet]
        [Route("getdescriptioncashflowdetail")]
        public async Task<ApiResponse<List<string>>> GetDescriptionCashFlowDetailAsync()
        {
            try
            {
                var credential = Common.DecodeJwt(User);
                var execute = await unitOfWork.CashFlowData.GetDescriptionCashFlowDetailAsync( credential.DbCode);
                
                return ApiResponse<List<string>>.Builder()
                    .WithMessage(execute.Any() ? "Cash flow detail fetched successfully" : "Cash flow detail fetched unsuccessfully")
                    .WithStatusCode(execute.Any() ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest)
                    .WithResult(execute.Any() ? execute : new List<string>())
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<string>>(ex.Message);
            }
        }
        
            [HttpPost]
        [Route("addnewcashflowdetail")]
        public async Task<ApiResponse<int>> AddNewCashFlowDetailAsync(CashFlowDataDetailModel data)
        {
            try
            {
                var credential = Common.DecodeJwt(User);
                // data.CreateBy = credential.Username;
                var affectedRow = await unitOfWork.CashFlowData.AddNewCashFlowDetailAsync(data);
                
                return ApiResponse<int>.Builder()
                    .WithMessage(affectedRow > 0 ? "Cash flow detail added successfully" : "Cash flow detail added unsuccessfully")
                    .WithStatusCode(affectedRow > 0 ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest)
                    .WithResult(affectedRow > 0 ? affectedRow : 0)
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
            }
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
          
                var affectedRow = await unitOfWork.CashFlowData.AddNewCashFlowSubmittedAsync(lsCashFlowDetailModel);
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
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<SubmittedCashFlowPostDto>>(ex.Message);
            }

            return cashFlowDetail;
        }

        #endregion

    }
            
}
