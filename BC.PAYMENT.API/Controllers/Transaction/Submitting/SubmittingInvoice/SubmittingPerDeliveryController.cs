using System.Net;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.Filter;
using BC.PAYMENT.CORE.DTO.Transaction.DailyPayment.DailyPayment;
using BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DailyPayment;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BC.PAYMENT.API.Controllers.Transaction.Submitting.SubmittingInvoice
{

    public class SubmittingPerDeliveryController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubmittingPerDeliveryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        [Route("getallsubmittingperdelivery")]
        public async Task<ApiResponse<PaginatedResponse<ExpenseDetailModel>>> GetAllSubmittedInvoices([FromBody] ByDateDto model)
        {
            var credential = Common.DecodeJwt(User);
            var submittedInvoice = new ApiResponse<PaginatedResponse<ExpenseDetailModel>>();
            try
            {
                var submittedInvoices = await _unitOfWork.SubmittingPerDelivery.GetSubmittedInvoicesAsync(credential.DbCode!,model.FromDate,model.ToDate);
                var newResponds = submittedInvoices.Skip(model.Page-1*model.PageSize).Take(model.PageSize).ToList();
                if (submittedInvoices.Any())
                {
                    submittedInvoice.Result = new PaginatedResponse<ExpenseDetailModel>(newResponds,submittedInvoices.Count,model.Page,model.PageSize);
                    submittedInvoice.StatusCode = (int)HttpStatusCode.OK;
                    submittedInvoice.Message = "Submitted Invoices fetched successfully";
                    submittedInvoice.Success = true;
                }
                else
                {
                    submittedInvoice.StatusCode = (int)HttpStatusCode.BadRequest;
                    submittedInvoice.Message = "Submitted Invoices fetched unsuccessfully";
                }
                
            }catch (SqlException ex)
            {
                submittedInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                submittedInvoice.Message = $@"Sql Exception : {ex.Message}";
            }
            catch (Exception ex)
            {
                submittedInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                submittedInvoice.Message = $@"Error Exception : {ex.Message}";
            }
            return submittedInvoice;
        }
        
        [HttpPost]
        [Route("addnewsubmittedinvoice")]
        public async Task<ApiResponse<ExpenseDetailPostDto>> AddNewSubmittedInvoiceAynsc([FromBody] ExpenseDetailPostDto model)
        {
            var credential = Common.DecodeJwt(User);
            var submittedInvoice = new ApiResponse<ExpenseDetailPostDto>();
            try
            {
                var submittedInvoicesModel = new ExpenseDetailModel
                {
                    Id = model.Id,
                    Dollar = model.Dollar,
                    Riel = model.Riel,
                    ExchangeRate = model.Exchange,
                    Total = model.Total,
                    ExpenseRiel = model.ExpenseRiel,
                    Misaligned = model.Misaligned,
                    ExpenseDollar = model.ExpenseDollar,
                    CreateBy = credential.Username!,
                    CreateDate = DateTime.Now,
                    DbCode = credential.DbCode!
                };
                var affectedRow = await _unitOfWork.SubmittingPerDelivery.AddNewSubmittedInvoicesAsync(submittedInvoicesModel);
                if (affectedRow > 0)
                {
                    submittedInvoice.Result = model;
                    submittedInvoice.StatusCode = (int)HttpStatusCode.OK;
                    submittedInvoice.Message = "Submitted Invoices added successfully";
                    submittedInvoice.Success = true;
                }
                else
                {
                    submittedInvoice.StatusCode = (int)HttpStatusCode.BadRequest;
                    submittedInvoice.Message = "Submitted Invoices added unsuccessfully";
                }
                
            }catch (SqlException ex)
            {
                submittedInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                submittedInvoice.Message = $@"Sql Exception : {ex.Message}";
            }
            catch (Exception ex)
            {
                submittedInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                submittedInvoice.Message = $@"Error Exception : {ex.Message}";
            }
            return submittedInvoice;
        }

    }
}
