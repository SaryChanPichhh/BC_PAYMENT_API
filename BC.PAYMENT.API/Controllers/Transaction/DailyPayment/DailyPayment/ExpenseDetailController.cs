using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DailyPayment;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Net;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.CORE.DTO.Filter;
using BC.PAYMENT.CORE.DTO.Transaction.DailyPayment.DailyPayment;

namespace BC.PAYMENT.API.Controllers.Transaction.DailyPayment.DailyPayment
{

    public class ExpenseDetailController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public ExpenseDetailController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpPost]
        [Route("getexpensedetailbydate")]
        public async Task<ApiResponse<PaginatedResponse<ExpenseDetailModel>>> GetExpenseDetailByDateAsync([FromBody] ByDateDto model)
        {
            var credential = Common.DecodeJwt(User);
            var expenseDetail = new ApiResponse<PaginatedResponse<ExpenseDetailModel>>();
            try
            {
                var expenseDetails =
                    await _unitOfWork.DailyPayment.GetExpenseDetailByDateAsync(credential.DbCode!, model.FromDate, model.ToDate);
                var newExpenseResponds =
                    expenseDetails.Skip(model.Page - 1 * model.PageSize).Take(model.PageSize).ToList();
                if (newExpenseResponds.Any())
                {
                    expenseDetail.StatusCode = (int)HttpStatusCode.OK;
                    expenseDetail.Success = true;
                    expenseDetail.Message = "Expense detail fetched successfully";
                    expenseDetail.Result = new PaginatedResponse<ExpenseDetailModel>(newExpenseResponds, newExpenseResponds.Count, model.Page, model.PageSize);
                }
                else
                {
                    expenseDetail.StatusCode = (int)HttpStatusCode.BadRequest;
                    expenseDetail.Success = false;
                    expenseDetail.Message = "Expense detail fetched unsuccessfully";
                }
            }
            catch (SqlException ex)
            {
                expenseDetail.StatusCode = (int)HttpStatusCode.InternalServerError;
                expenseDetail.Success = false;
                expenseDetail.Message = $@"Sql Exception : {ex.Message}";
            }
            catch (Exception ex)
            {
                expenseDetail.StatusCode = (int)HttpStatusCode.InternalServerError;
                expenseDetail.Success = false;
                expenseDetail.Message = $@"Error Exception : {ex.Message}";
            }
            return expenseDetail;
        }
        
        [HttpPost]
        [Route("getexpensedetailbyperiod")]
        public async Task<ApiResponse<PaginatedResponse<ExpenseDetailModel>>> GetExpenseDetailByPeriodAsync([FromBody] ByPeriodDto model)
        {
            var credential = Common.DecodeJwt(User);
            var expenseDetail = new ApiResponse<PaginatedResponse<ExpenseDetailModel>>();
            try
            {
                var expenseDetails =
                    await _unitOfWork.DailyPayment.GetExpenseDetailByPeriodAsync(credential.DbCode!, model.Month, model.Year);
                var newExpenseResponds =
                    expenseDetails.Skip(model.Page - 1 * model.PageSize).Take(model.PageSize).ToList();
                if (newExpenseResponds.Any())
                {
                    expenseDetail.StatusCode = (int)HttpStatusCode.OK;
                    expenseDetail.Success = true;
                    expenseDetail.Message = "Paid invoices fetched successfully";
                    expenseDetail.Result = new PaginatedResponse<ExpenseDetailModel>(newExpenseResponds, newExpenseResponds.Count, model.Page, model.PageSize);
                }
                else
                {
                    expenseDetail.StatusCode = (int)HttpStatusCode.BadRequest;
                    expenseDetail.Success = false;
                    expenseDetail.Message = "Paid invoices fetched unsuccessfully";
                }
            }
            catch (SqlException ex)
            {
                expenseDetail.StatusCode = (int)HttpStatusCode.InternalServerError;
                expenseDetail.Success = false;
                expenseDetail.Message = $@"Sql Exception : {ex.Message}";
            }
            catch (Exception ex)
            {
                expenseDetail.StatusCode = (int)HttpStatusCode.InternalServerError;
                expenseDetail.Success = false;
                expenseDetail.Message = $@"Error Exception : {ex.Message}";
            }
            return expenseDetail;
        }
        
        [HttpPut]
        [Route("")]
        public async Task<ApiResponse<ExpenseDetailDto>> UpdateExpenseDetailByPeriodAsync([FromBody] ExpenseDetailDto model)
        {
            var credential = Common.DecodeJwt(User);
            var expenseDetail = new ApiResponse<ExpenseDetailDto>();
            try
            {

                var affectedRow =
                    await _unitOfWork.DailyPayment.UpdateExpenseDetailAsync(model);
                if (affectedRow > 0 )
                {
                    expenseDetail.StatusCode = (int)HttpStatusCode.OK;
                    expenseDetail.Success = true;
                    expenseDetail.Message = "Expense detail updated successfully";
                    expenseDetail.Result = model;
                }
                else
                {
                    expenseDetail.StatusCode = (int)HttpStatusCode.BadRequest;
                    expenseDetail.Success = false;
                    expenseDetail.Message = "Expense detail updated unsuccessfully";
                }
            }
            catch (SqlException ex)
            {
                expenseDetail.StatusCode = (int)HttpStatusCode.InternalServerError;
                expenseDetail.Success = false;
                expenseDetail.Message = $@"Sql Exception : {ex.Message}";
            }
            catch (Exception ex)
            {
                expenseDetail.StatusCode = (int)HttpStatusCode.InternalServerError;
                expenseDetail.Success = false;
                expenseDetail.Message = $@"Error Exception : {ex.Message}";
            }
            return expenseDetail;
        }
    }
}
