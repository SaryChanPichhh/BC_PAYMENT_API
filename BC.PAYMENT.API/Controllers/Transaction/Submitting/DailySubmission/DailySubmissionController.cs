using System.ComponentModel.DataAnnotations;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.Filter;
using BC.PAYMENT.CORE.DTO.Transaction.Submitting.DailySubmission;
using BC.PAYMENT.CORE.Entities.Transaction.Submitting.DailySubmission;
using BC.PAYMENT.LOGGING;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BC.PAYMENT.API.Controllers.Transaction.Submitting.DailySubmission
{
    public class DailySubmissionController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public DailySubmissionController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        private static IEnumerable<DateTime> EachDay(DateTime startDate, DateTime endDate)
        {
            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                yield return date;
            }
        }
        [HttpGet]
        [Route("getallapprovaldate")]
        public async Task<ApiResponse<ApprovalHistoryResponeDto>> GetAllApprovalDateAsync()
        {   
            var credential = Common.DecodeJwt(User);
            var approvalHistory = new ApiResponse<ApprovalHistoryResponeDto>();
            try
            {
                var approvalHistoryModel = new ApprovalHistoryResponeDto();
                approvalHistoryModel.ApprovedInvoices = await _unitOfWork.Approve.GetApprovedInvoiceAsync(credential.DbCode!);
                approvalHistoryModel.ReSubmittedInvoices = await _unitOfWork.Approve.GetReSubmitInvoiceAsync(credential.DbCode!);
                approvalHistoryModel.RejectedInvoices  = await _unitOfWork.Approve.GetRejectedInvoiceAsync(credential.DbCode!);
                var publicHoliday = await _unitOfWork.PublicHoliday.GetListHoliday();
                publicHoliday.ForEach(x => {
                    foreach (var date in EachDay(x.StartDate, x.EndDate))
                    {
                        approvalHistoryModel.PublicHolidays[date] = x.Remark!;
                    }
                });

                if (approvalHistoryModel != null)
                {
                    approvalHistory.Result = approvalHistoryModel;
                    approvalHistory.StatusCode = StatusCodes.Status200OK;
                    approvalHistory.Message = "Approval History fetched successfully";
                    approvalHistory.Success = true;
                }
                else
                {
                    approvalHistory.StatusCode = StatusCodes.Status400BadRequest;
                    approvalHistory.Message = "Approval History fetched unsuccessfully";
                    approvalHistory.Success = false;
                }
            }catch (SqlException ex)
            {
                approvalHistory.StatusCode = StatusCodes.Status500InternalServerError;
                approvalHistory.Message = $"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql Exception", ex);
            }
            catch (Exception ex)
            {
                approvalHistory.StatusCode = StatusCodes.Status500InternalServerError;
                approvalHistory.Message = $"Error Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }

            return approvalHistory;
        } 
        [HttpPost]
        [Route("getsubmittedinvoicebydate")]
        public async Task<ApiResponse<List<DailySubmissionModel>>> GetSubmittedInvoiceByDateAsync([FromBody] ByDateDto model)
        {
            var credential = Common.DecodeJwt(User);
            var submittedInvoice = new ApiResponse<List<DailySubmissionModel>>();
            try
            {
                var execute = await _unitOfWork.Approve.GetSubmittedInvoiceByDateAsync(credential.DbCode!, model.FromDate, model.ToDate,model.Page,model.PageSize);
                if (execute.Any())
                {
                    submittedInvoice.Result = execute;
                    submittedInvoice.StatusCode = StatusCodes.Status200OK;
                    submittedInvoice.Message = "Submitted Invoice fetched successfully";
                    submittedInvoice.Success = true;
                }
                else
                {
                    submittedInvoice.StatusCode = StatusCodes.Status400BadRequest;
                    submittedInvoice.Message = "Submitted Invoice fetched unsuccessfully";
                    submittedInvoice.Success = false;
                }
            }catch (SqlException ex)
            {
                submittedInvoice.StatusCode = StatusCodes.Status500InternalServerError;
                submittedInvoice.Message = $"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql Exception", ex);
            }
            catch (Exception ex)
            {
                submittedInvoice.StatusCode = StatusCodes.Status500InternalServerError;
                submittedInvoice.Message = $"Error Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }
            return submittedInvoice;
        }
        [HttpPost]
        [Route("getsubmittedinvoicebyperiod")]
        public async Task<ApiResponse<List<DailySubmissionModel>>> GetSubmittedInvoiceByPeriodAsync([FromBody] ByPeriodDto model)
        {
            var credential = Common.DecodeJwt(User);
            var submittedInvoice = new ApiResponse<List<DailySubmissionModel>>();
            try
            {
                var execute = await _unitOfWork.Approve.GetSubmittedInvoiceByPeriodAsync(credential.DbCode!, model.Month, model.Year,model.Page,model.PageSize);
                if (execute.Any())
                {
                    submittedInvoice.Result = execute;
                    submittedInvoice.StatusCode = StatusCodes.Status200OK;
                    submittedInvoice.Message = "Submitted Invoices fetched successfully";
                    submittedInvoice.Success = true;
                }
                else
                {
                    submittedInvoice.StatusCode = StatusCodes.Status400BadRequest;
                    submittedInvoice.Message = "Submitted Invoices fetched unsuccessfully";
                    submittedInvoice.Success = false;
                }
            }catch (SqlException ex)
            {
                submittedInvoice.StatusCode = StatusCodes.Status500InternalServerError;
                submittedInvoice.Message = $"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql Exception", ex);
            }
            catch (Exception ex)
            {
                submittedInvoice.StatusCode = StatusCodes.Status500InternalServerError;
                submittedInvoice.Message = $"Error Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }
            return submittedInvoice;
        }
        
        [HttpGet]
        [Route("getapprovalinvoicebydate/{fromDate}/{toDate}")]
        public async Task<ApiResponse<List<ApprovalInvoiceModel>>> GetApprovalInvoiceByDateAsync([Required] string fromDate,[Required]string toDate )
        {
            var credential = Common.DecodeJwt(User);
            var submittedInvoice = new ApiResponse<List<ApprovalInvoiceModel>>();
            try
            {
                var execute = await _unitOfWork.Approve.GetApprovalListByDateAsync(credential.DbCode!, fromDate, toDate);
                if (execute.Any())
                {
                    submittedInvoice.Result = execute;
                    submittedInvoice.StatusCode = StatusCodes.Status200OK;
                    submittedInvoice.Message = "Approved Invoices fetched successfully";
                    submittedInvoice.Success = true;
                }
                else
                {
                    submittedInvoice.StatusCode = StatusCodes.Status400BadRequest;
                    submittedInvoice.Message = "Approved Invoices fetched unsuccessfully";
                    submittedInvoice.Success = false;
                }
            }catch (SqlException ex)
            {
                submittedInvoice.StatusCode = StatusCodes.Status500InternalServerError;
                submittedInvoice.Message = $"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql Exception", ex);
            }
            catch (Exception ex)
            {
                submittedInvoice.StatusCode = StatusCodes.Status500InternalServerError;
                submittedInvoice.Message = $"Error Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }
            return submittedInvoice;
        }
        [HttpGet]
        [Route("getsubmittedinvoicebyperiod/{year}/{month}")]
        public async Task<ApiResponse<List<ApprovalInvoiceModel>>> GetSubmittedInvoiceByPeriodAsync([Required] int year, [Required] int month)
        {
            var credential = Common.DecodeJwt(User);
            var submittedInvoice = new ApiResponse<List<ApprovalInvoiceModel>>();
            try
            {
                var execute = await _unitOfWork.Approve.GetApprovalListByPeriodAsync(credential.DbCode!,month,year );
                if (execute.Any())
                {
                    submittedInvoice.Result = execute;
                    submittedInvoice.StatusCode = StatusCodes.Status200OK;
                    submittedInvoice.Message = "Approved Invoices fetched successfully";
                    submittedInvoice.Success = true;
                }
                else
                {
                    submittedInvoice.StatusCode = StatusCodes.Status400BadRequest;
                    submittedInvoice.Message = "Approved Invoices fetched unsuccessfully";
                    submittedInvoice.Success = false;
                }
            }catch (SqlException ex)
            {
                submittedInvoice.StatusCode = StatusCodes.Status500InternalServerError;
                submittedInvoice.Message = $"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql Exception", ex);
            }
            catch (Exception ex)
            {
                submittedInvoice.StatusCode = StatusCodes.Status500InternalServerError;
                submittedInvoice.Message = $"Error Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }
            return submittedInvoice;
        }
        
        [HttpPut]
        [Route("updatesubmittedinvoicestatus/{submittedInvoiceId}")]
        public async Task<ApiResponse<int>> UpdateSubmittedInvoiceStatusAsync([Required] int submittedInvoiceId )
        {
            var credential = Common.DecodeJwt(User);
            var submittedInvoice = new ApiResponse<int>();
            try
            {
                var affectedRow = await _unitOfWork.Approve.UpdateStatusSubmittedInvoiceBySubmittedInvoiceId(submittedInvoiceId,credential.Username! );
                if (affectedRow > 0)
                {
                    submittedInvoice.Result = affectedRow;
                    submittedInvoice.StatusCode = StatusCodes.Status200OK;
                    submittedInvoice.Message = "Submitted Invoices updated to processing successfully";
                    submittedInvoice.Success = true;
                }
                else
                {
                    submittedInvoice.StatusCode = StatusCodes.Status400BadRequest;
                    submittedInvoice.Message = "Submitted Invoices updated to processing unsuccessfully";
                    submittedInvoice.Success = false;
                }
            }catch (SqlException ex)
            {
                submittedInvoice.StatusCode = StatusCodes.Status500InternalServerError;
                submittedInvoice.Message = $"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql Exception", ex);
            }
            catch (Exception ex)
            {
                submittedInvoice.StatusCode = StatusCodes.Status500InternalServerError;
                submittedInvoice.Message = $"Error Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }
            return submittedInvoice;
        }
        
        [HttpPost]
        [Route("findsubmitedinvoicebytransactioncode/{transactionCode}")]
        public async Task<ApiResponse<int>> UpdateSubmittedInvoiceStatusAsync([Required] string transactionCode)
        {
            var credential = Common.DecodeJwt(User);
            var submittedInvoice = new ApiResponse<int>();
            try
            {
                var affectedRow = await _unitOfWork.Approve.FindSubmittedInvoiceByTransactionCodeAsync(credential.DbCode!,transactionCode );
                if (affectedRow > 0)
                {
                    submittedInvoice.Result = affectedRow;
                    submittedInvoice.StatusCode = StatusCodes.Status200OK;
                    submittedInvoice.Message = "Submitted Invoices Id fetched successfully";
                    submittedInvoice.Success = true;
                }
                else
                {
                    submittedInvoice.StatusCode = StatusCodes.Status400BadRequest;
                    submittedInvoice.Message = "Submitted Invoices Id fetched unsuccessfully";
                    submittedInvoice.Success = false;
                }
            }catch (SqlException ex)
            {
                submittedInvoice.StatusCode = StatusCodes.Status500InternalServerError;
                submittedInvoice.Message = $"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql Exception", ex);
            }
            catch (Exception ex)
            {
                submittedInvoice.StatusCode = StatusCodes.Status500InternalServerError;
                submittedInvoice.Message = $"Error Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }
            return submittedInvoice;
        }
        
        [HttpGet]
        [Route("findhistorypaymentbytransactioncode/{transactionCode}")]
        public async Task<ApiResponse<List<HistoryPaymentModel>>> FindHistoryPaymentByTransactionCodeAsync([Required] string transactionCode)
        {
            var credential = Common.DecodeJwt(User);
            var submittedInvoice = new ApiResponse<List<HistoryPaymentModel>>();
            try
            {
                var execute = await _unitOfWork.Approve.FindHistoryPaymentByTransactionCode(credential.DbCode!,transactionCode );
                if (execute.Any())
                {
                    submittedInvoice.Result = execute;
                    submittedInvoice.StatusCode = StatusCodes.Status200OK;
                    submittedInvoice.Message = "Submitted Invoices fetched successfully";
                    submittedInvoice.Success = true;
                }
                else
                {
                    submittedInvoice.StatusCode = StatusCodes.Status400BadRequest;
                    submittedInvoice.Message = "Submitted Invoices fetched unsuccessfully";
                    submittedInvoice.Success = false;
                }
            }catch (SqlException ex)
            {
                submittedInvoice.StatusCode = StatusCodes.Status500InternalServerError;
                submittedInvoice.Message = $"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql Exception", ex);
            }
            catch (Exception ex)
            {
                submittedInvoice.StatusCode = StatusCodes.Status500InternalServerError;
                submittedInvoice.Message = $"Error Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }
            return submittedInvoice;
        }
        [HttpPost]
        [Route("scansubmittedinvoice/{transactionCode}")]
        public async Task<ApiResponse<List<HistoryPaymentModel>>> ScanBarCodeAsync([Required] string transactionCode)
        {
            var credential = Common.DecodeJwt(User);
            var submittedInvoice = new ApiResponse<List<HistoryPaymentModel>>();
            try
            {
                var submittedId = await _unitOfWork.Approve.FindSubmittedInvoiceByTransactionCodeAsync(credential.DbCode!, transactionCode);
                if (submittedId > 0)
                {
                    await _unitOfWork.Approve.UpdateStatusSubmittedInvoiceBySubmittedInvoiceId(submittedId,
                        credential.Username!);

                    var execute =
                        await _unitOfWork.Approve.FindHistoryPaymentByTransactionCode(credential.DbCode!,
                            transactionCode);
                    if (execute.Any())
                    {
                        submittedInvoice.Result = execute;
                        submittedInvoice.StatusCode = StatusCodes.Status200OK;
                        submittedInvoice.Message = "Submitted Invoices Id fetched successfully";
                        submittedInvoice.Success = true;
                    }
                    else
                    {
                        submittedInvoice.StatusCode = StatusCodes.Status400BadRequest;
                        submittedInvoice.Message = "Submitted Invoices Id fetched unsuccessfully";
                        submittedInvoice.Success = false;
                    }
                }

            }
            catch (SqlException ex)
            {
                submittedInvoice.StatusCode = StatusCodes.Status500InternalServerError;
                submittedInvoice.Message = $"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql Exception", ex);
            }
            catch (Exception ex)
            {
                submittedInvoice.StatusCode = StatusCodes.Status500InternalServerError;
                submittedInvoice.Message = $"Error Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }
            return submittedInvoice;
        }

    }
}
