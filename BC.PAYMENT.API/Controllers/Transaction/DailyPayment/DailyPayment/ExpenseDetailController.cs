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

    public class ExpenseDetailController(IUnitOfWork unitOfWork) : BaseApiController
    {
        [HttpPost]
        [Route("getexpensedetailbydate")]
        public async Task<ApiResponse<PaginatedResponse<ExpenseDetailModel>>> GetExpenseDetailByDateAsync([FromBody] ByDateDto model)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var expenseDetails =
                    await unitOfWork.DailyPayment.GetExpenseDetailByDateAsync(credential.DbCode!, model.FromDate, model.ToDate);
                var newExpenseResponds =
                    expenseDetails.Skip(model.Page - 1 * model.PageSize).Take(model.PageSize).ToList();
                if (newExpenseResponds.Any())
                {
                    return ApiResponse<PaginatedResponse<ExpenseDetailModel>>.Builder()
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithMessage("Expense detail fetched successfully")
                        .WithResult(new PaginatedResponse<ExpenseDetailModel>(newExpenseResponds, newExpenseResponds.Count, model.Page, model.PageSize))
                        .Build();
                }
                
                return ApiResponse<PaginatedResponse<ExpenseDetailModel>>.Builder()
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .WithMessage("Expense detail fetched unsuccessfully")
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<PaginatedResponse<ExpenseDetailModel>>(ex.Message);
            }
        }
        
        [HttpPost]
        [Route("getexpensedetailbyperiod")]
        public async Task<ApiResponse<PaginatedResponse<ExpenseDetailModel>>> GetExpenseDetailByPeriodAsync([FromBody] ByPeriodDto model)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var expenseDetails =
                    await unitOfWork.DailyPayment.GetExpenseDetailByPeriodAsync(credential.DbCode!, model.Month, model.Year);
                var newExpenseResponds =
                    expenseDetails.Skip(model.Page - 1 * model.PageSize).Take(model.PageSize).ToList();
                if (newExpenseResponds.Any())
                {
                    return ApiResponse<PaginatedResponse<ExpenseDetailModel>>.Builder()
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithMessage("Paid invoices fetched successfully")
                        .WithResult(new PaginatedResponse<ExpenseDetailModel>(newExpenseResponds, newExpenseResponds.Count, model.Page, model.PageSize))
                        .Build();
                }
                
                return ApiResponse<PaginatedResponse<ExpenseDetailModel>>.Builder()
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .WithMessage("Paid invoices fetched unsuccessfully")
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<PaginatedResponse<ExpenseDetailModel>>(ex.Message);
            }
        }
        
        [HttpPut]
        [Route("")]
        public async Task<ApiResponse<ExpenseDetailDto>> UpdateExpenseDetailByPeriodAsync([FromBody] ExpenseDetailDto model)
        {
            var credential = Common.DecodeJwt(User);
            try
            {

                var affectedRow =
                    await unitOfWork.DailyPayment.UpdateExpenseDetailAsync(model);
                if (affectedRow > 0 )
                {
                    return ApiResponse<ExpenseDetailDto>.Builder()
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithMessage("Expense detail updated successfully")
                        .WithResult(model)
                        .Build();
                }
                
                return ApiResponse<ExpenseDetailDto>.Builder()
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .WithMessage("Expense detail updated unsuccessfully")
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<ExpenseDetailDto>(ex.Message);
            }
        }
    }
}
