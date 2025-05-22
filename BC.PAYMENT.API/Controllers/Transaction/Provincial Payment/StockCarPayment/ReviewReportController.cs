using System.ComponentModel.DataAnnotations;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.Transaction.ProvincialPayment.ReviewReport;
using BC.PAYMENT.CORE.DTO.Transaction.ProvincialPayment.StockCarPayment;
using BC.PAYMENT.CORE.Entities.Transaction.ProvincialPayment.StockCarPayment;
using BC.PAYMENT.LOGGING;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BC.PAYMENT.API.Controllers.Transaction.Provincial_Payment.StockCarPayment
{
    public class ReviewReportController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ReviewReportController> _logger;
        public ReviewReportController(IUnitOfWork unitOfWork, ILogger<ReviewReportController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #region Requestion Action

        [HttpGet]
        [Route("getallrequestionaction")]
        public async Task<ApiResponse<List<RequestionActionDto>>> GetRequestActionAsync()
        {
            var requestAction = new ApiResponse<List<RequestionActionDto>>();
            try
            {
                var result = await _unitOfWork.ReviewReport.GetAllActionAsync();
                if (result.Any())
                {
                    requestAction.Result = result;
                    requestAction.StatusCode = StatusCodes.Status200OK;
                    requestAction.Message = "Request action fetched successfully";
                    requestAction.Success = true;
                }
                else
                {
                    requestAction.Result = result;
                    requestAction.StatusCode = StatusCodes.Status400BadRequest;
                    requestAction.Message = "Request action fetched unsuccessfully";
                    requestAction.Success = false;
                }
                
            }catch (SqlException ex)
            {
                requestAction.StatusCode = StatusCodes.Status500InternalServerError;
                requestAction.Message = $"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql Exception",ex);
                _logger.LogError($@"Sql Exception : {ex.Message}");
            }
            catch (Exception ex)
            {
                requestAction.StatusCode = StatusCodes.Status500InternalServerError;
                requestAction.Message = $"Error Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
                _logger.LogError($@"Error Exception : {ex.Message}");
            }
            return requestAction;
        }
        [HttpGet]
        [Route("getallrequestbyactionid/{actionId}")]
        public async Task<ApiResponse<List<StockCarRequestDto>>> GetAllRequestByActionIdAsync([Required] int actionId)
        {
            var credential = Common.DecodeJwt(User);
            var requestAction = new ApiResponse<List<StockCarRequestDto>>();
            try
            {
                var result = await _unitOfWork.ReviewReport.GetAllRequestByActionIdAsync(credential.DbCode!,actionId);
                if (result.Any())
                {
                    requestAction.Result = result;
                    requestAction.StatusCode = StatusCodes.Status200OK;
                    requestAction.Message = "Requests fetched successfully";
                    requestAction.Success = true;
                }
                else
                {
                    requestAction.Result = result;
                    requestAction.StatusCode = StatusCodes.Status400BadRequest;
                    requestAction.Message = "Requests fetched unsuccessfully";
                    requestAction.Success = false;
                }
                
            }
            catch (SqlException ex)
            {
                requestAction.StatusCode = StatusCodes.Status500InternalServerError;
                requestAction.Message = $"Sql Exception : {ex.Message}";
                //Logger.Instance.Error("Sql Exception", ex);
                //_logger.LogError($@"Sql Exception ", ex);
            }
            catch (Exception ex)
            {
                requestAction.StatusCode = StatusCodes.Status500InternalServerError;
                requestAction.Message = $"Error Exception : {ex.Message}";
                //Logger.Instance.Error("Error Exception", ex);
                _logger.LogInformation($@"Error Exception ", ex);
            }
            return requestAction;
        }
        
        [HttpGet]
        [Route("getallrequestbyactionidandroleid")]
        public async Task<ApiResponse<List<StockCarRequestDto>>> GetAllRequestByActionIdAndRoleIdAsync([FromBody]  StockCarRequestParamDto model)
        {
            var credential = Common.DecodeJwt(User);
            var requestAction = new ApiResponse<List<StockCarRequestDto>>();
            try
            {
                var result = await _unitOfWork.ReviewReport.GetAllRequestByActionIdAndRoleIdAsync(credential.DbCode!,model.ActionId,model.RoleId);
                if (result.Any())
                {
                    requestAction.Result = result;
                    requestAction.StatusCode = StatusCodes.Status200OK;
                    requestAction.Message = "Requests fetched successfully";
                    requestAction.Success = true;
                }
                else
                {
                    requestAction.Result = result;
                    requestAction.StatusCode = StatusCodes.Status400BadRequest;
                    requestAction.Message = "Requests fetched unsuccessfully";
                    requestAction.Success = false;
                }
                
            }catch (SqlException ex)
            {
                requestAction.StatusCode = StatusCodes.Status500InternalServerError;
                requestAction.Message = $"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql Exception",ex);
            }
            catch (Exception ex)
            {
                requestAction.StatusCode = StatusCodes.Status500InternalServerError;
                requestAction.Message = $"Error Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }
            return requestAction;
        }

        #endregion

        #region Review Credit Invoice
        [HttpGet]
        [Route("getallcreditinvoicebyrequestid/{requestId}/{checkingStatus}")]
        public async Task<ApiResponse<List<ReviewReportCreditInvoice>>> GetAllCreditInvoiceByRequestIdAndCheckingStatusAsync([Required] int requestId,[Required] bool checkingStatus)
        {
            var creditInvoice = new ApiResponse<List<ReviewReportCreditInvoice>>();
            try
            {
                var result = await _unitOfWork.ReviewReport.GetAllCreditInvoiceByRequestIdAndCheckingStatus(requestId, checkingStatus);
                if (result.Any())
                {
                    creditInvoice.Result = result;
                    creditInvoice.StatusCode = StatusCodes.Status200OK;
                    creditInvoice.Message = "Credit invoices fetched successfully";
                    creditInvoice.Success = true;
                }
                else
                {
                    creditInvoice.Result = result;
                    creditInvoice.StatusCode = StatusCodes.Status400BadRequest;
                    creditInvoice.Message = "Credit invoices fetched unsuccessfully";
                    creditInvoice.Success = false;
                }

            }
            catch (SqlException ex)
            {
                creditInvoice.StatusCode = StatusCodes.Status500InternalServerError;
                creditInvoice.Message = $"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql Exception", ex);
            }
            catch (Exception ex)
            {
                creditInvoice.StatusCode = StatusCodes.Status500InternalServerError;
                creditInvoice.Message = $"Error Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }
            return creditInvoice;
        }
        
        [HttpPut]
        [Route("updatestatuscreditinvoice/{requestId}/{checkingStatus}")]
        public async Task<ApiResponse<int>> UpdateStatusCheckingCreditInvoiceBySubmitId([Required] int requestId, [Required] bool checkingStatus)
        {
            var creditInvoice = new ApiResponse<int>();
            try
            {
                var affectedRow = await _unitOfWork.ReviewReport.UpdateStatusCheckingCreditInvoiceBySubmitId(requestId, checkingStatus);
                if (affectedRow > 0)
                {
                    creditInvoice.Result = affectedRow;
                    creditInvoice.StatusCode = StatusCodes.Status204NoContent;
                    creditInvoice.Message = "Checking status of credit invoices updated successfully";
                    creditInvoice.Success = true;
                }
                else
                {
                    creditInvoice.StatusCode = StatusCodes.Status400BadRequest;
                    creditInvoice.Message = "Checking status of credit invoices updated unsuccessfully";
                    creditInvoice.Success = false;
                }

            }
            catch (SqlException ex)
            {
                creditInvoice.StatusCode = StatusCodes.Status500InternalServerError;
                creditInvoice.Message = $"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql Exception", ex);
            }
            catch (Exception ex)
            {
                creditInvoice.StatusCode = StatusCodes.Status500InternalServerError;
                creditInvoice.Message = $"Error Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }
            return creditInvoice;
        }

        #endregion
        
        #region Review Paid Invoice
        [HttpGet]
        [Route("getallpaidinvoicebyrequestid/{requestId}/{checkingStatus}")]
        public async Task<ApiResponse<List<PaidInvoiceRequestDto>>> GetAllPaidInvoiceByRequestIdAndCheckingStatus([Required] int requestId, [Required] bool checkingStatus)
        {
            var paidInvoice = new ApiResponse<List<PaidInvoiceRequestDto>>();
            try
            {
                var result = await _unitOfWork.ReviewReport.GetAllPaidInvoiceByRequestIdAndCheckingStatus(requestId, checkingStatus);
                if (result.Any())
                {
                    paidInvoice.Result = result;
                    paidInvoice.StatusCode = StatusCodes.Status200OK;
                    paidInvoice.Message = "Paid invoices fetched successfully";
                    paidInvoice.Success = true;
                }
                else
                {
                    paidInvoice.Result = result;
                    paidInvoice.StatusCode = StatusCodes.Status400BadRequest;
                    paidInvoice.Message = "Paid invoices fetched unsuccessfully";
                    paidInvoice.Success = false;
                }

            }
            catch (SqlException ex)
            {
                paidInvoice.StatusCode = StatusCodes.Status500InternalServerError;
                paidInvoice.Message = $"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql Exception", ex);
            }
            catch (Exception ex)
            {
                paidInvoice.StatusCode = StatusCodes.Status500InternalServerError;
                paidInvoice.Message = $"Error Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }
            return paidInvoice;
        }
        
        [HttpPut]
        [Route("updatestatuspaidinvoice/{requestId}/{checkingStatus}")]
        public async Task<ApiResponse<int>> UpdateStatusCheckingPaymentInvoiceByInvoiceId([Required] int requestId, [Required] bool checkingStatus)
        {
            var paidInvoice = new ApiResponse<int>();
            try
            {
                var affectedRow = await _unitOfWork.ReviewReport.UpdateStatusCheckingPaymentInvoiceByInvoiceId(requestId, checkingStatus);
                if (affectedRow > 0)
                {
                    paidInvoice.Result = affectedRow;
                    paidInvoice.StatusCode = StatusCodes.Status204NoContent;
                    paidInvoice.Message = "Checking status of paid invoices updated successfully";
                    paidInvoice.Success = true;
                }
                else
                {
                    paidInvoice.StatusCode = StatusCodes.Status400BadRequest;
                    paidInvoice.Message = "Checking status of paid invoices updated unsuccessfully";
                    paidInvoice.Success = false;
                }

            }
            catch (SqlException ex)
            {
                paidInvoice.StatusCode = StatusCodes.Status500InternalServerError;
                paidInvoice.Message = $"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql Exception", ex);
            }
            catch (Exception ex)
            {
                paidInvoice.StatusCode = StatusCodes.Status500InternalServerError;
                paidInvoice.Message = $"Error Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }
            return paidInvoice;
        }

        #endregion
        
        #region Review Transfer Invoice
        [HttpGet]
        [Route("getalltransferinvoicebyrequestid/{requestId}/{checkingStatus}")]
        public async Task<ApiResponse<List<TransferMoneyModel>>> GetAllTransferByRequestIdAndCheckingStatus([Required] int requestId, [Required] bool checkingStatus)
        {
            var transferInvoice = new ApiResponse<List<TransferMoneyModel>>();
            try
            {
                var result = await _unitOfWork.ReviewReport.GetAllTransferByRequestIdAndCheckingStatus(requestId, checkingStatus);
                if (result.Any())
                {
                    transferInvoice.Result = result;
                    transferInvoice.StatusCode = StatusCodes.Status200OK;
                    transferInvoice.Message = "Transfer invoices fetched successfully";
                    transferInvoice.Success = true;
                }
                else
                {
                    transferInvoice.Result = result;
                    transferInvoice.StatusCode = StatusCodes.Status400BadRequest;
                    transferInvoice.Message = "Transfer invoices fetched unsuccessfully";
                    transferInvoice.Success = false;
                }

            }
            catch (SqlException ex)
            {
                transferInvoice.StatusCode = StatusCodes.Status500InternalServerError;
                transferInvoice.Message = $"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql Exception", ex);
            }
            catch (Exception ex)
            {
                transferInvoice.StatusCode = StatusCodes.Status500InternalServerError;
                transferInvoice.Message = $"Error Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }
            return transferInvoice;
        }
        
        [HttpPut]
        [Route("updatestatustransferinvoice/{requestId}/{checkingStatus}")]
        public async Task<ApiResponse<int>> UpdateStatusCheckingTransferBySubmitTransferId([Required] int requestId, [Required] bool checkingStatus)
        {
            var transferInvoice = new ApiResponse<int>();
            try
            {
                var affectedRow = await _unitOfWork.ReviewReport.UpdateStatusCheckingTransferBySubmitTransferId(requestId, checkingStatus);
                if (affectedRow > 0)
                {
                    transferInvoice.Result = affectedRow;
                    transferInvoice.StatusCode = StatusCodes.Status204NoContent;
                    transferInvoice.Message = "Checking status of transfer invoices updated successfully";
                    transferInvoice.Success = true;
                }
                else
                {
                    transferInvoice.StatusCode = StatusCodes.Status400BadRequest;
                    transferInvoice.Message = "Checking status of transfer invoices updated unsuccessfully";
                    transferInvoice.Success = false;
                }

            }
            catch (SqlException ex)
            {
                transferInvoice.StatusCode = StatusCodes.Status500InternalServerError;
                transferInvoice.Message = $"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql Exception", ex);
            }
            catch (Exception ex)
            {
                transferInvoice.StatusCode = StatusCodes.Status500InternalServerError;
                transferInvoice.Message = $"Error Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }
            return transferInvoice;
        }
        #endregion

        #region Expense Invoice

        [HttpGet]
        [Route("getallexpenseinvoicebyrequestid/{requestId}/{checkingStatus}")]
        public async Task<ApiResponse<List<ReviewReportExpenseDto>>> GetAllExpenseByRequestIdAndCheckingStatus([Required] int requestId, [Required] bool checkingStatus)
        {
            var transferInvoice = new ApiResponse<List<ReviewReportExpenseDto>>();
            try
            {
                var result = await _unitOfWork.ReviewReport.GetAllExpenseByRequestIdAndCheckingStatus(requestId, checkingStatus);
                if (result.Any())
                {
                    transferInvoice.Result = result;
                    transferInvoice.StatusCode = StatusCodes.Status200OK;
                    transferInvoice.Message = "Expense invoices fetched successfully";
                    transferInvoice.Success = true;
                }
                else
                {
                    transferInvoice.Result = result;
                    transferInvoice.StatusCode = StatusCodes.Status400BadRequest;
                    transferInvoice.Message = "Expense invoices fetched unsuccessfully";
                    transferInvoice.Success = false;
                }

            }
            catch (SqlException ex)
            {
                transferInvoice.StatusCode = StatusCodes.Status500InternalServerError;
                transferInvoice.Message = $"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql Exception", ex);
            }
            catch (Exception ex)
            {
                transferInvoice.StatusCode = StatusCodes.Status500InternalServerError;
                transferInvoice.Message = $"Error Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }
            return transferInvoice;
        }

        [HttpPut]
        [Route("updatestatusexpenseinvoice/{requestId}/{checkingStatus}")]
        public async Task<ApiResponse<int>> UpdateStatusCheckingExpenseByInvoiceId([Required] int requestId, [Required] bool checkingStatus)
        {
            var transferInvoice = new ApiResponse<int>();
            try
            {
                var affectedRow = await _unitOfWork.ReviewReport.UpdateStatusCheckingExpenseByInvoiceId(requestId, checkingStatus);
                if (affectedRow > 0)
                {
                    transferInvoice.Result = affectedRow;
                    transferInvoice.StatusCode = StatusCodes.Status204NoContent;
                    transferInvoice.Message = "Checking status of expense invoices updated successfully";
                    transferInvoice.Success = true;
                }
                else
                {
                    transferInvoice.StatusCode = StatusCodes.Status400BadRequest;
                    transferInvoice.Message = "Checking status of expense invoices updated unsuccessfully";
                    transferInvoice.Success = false;
                }
            }
            catch (SqlException ex)
            {
                transferInvoice.StatusCode = StatusCodes.Status500InternalServerError;
                transferInvoice.Message = $"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql Exception", ex);
            }
            catch (Exception ex)
            {
                transferInvoice.StatusCode = StatusCodes.Status500InternalServerError;
                transferInvoice.Message = $"Error Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }
            return transferInvoice;
        }

        #endregion
        #region Approval Invoice History
        [HttpGet]
        [Route("getallapprovalinvoicehistorybyrequestid/{requestId}/{checkingStatus}")]
        public async Task<ApiResponse<List<ApprovalInvoiceHistoryModel>>> GetAllApprovalHistoryByRequestIdAsync([Required] int requestId, [Required] bool checkingStatus)
        {
            var transferInvoice = new ApiResponse<List<ApprovalInvoiceHistoryModel>>();
            try
            {
                var result = await _unitOfWork.ReviewReport.GetAllApprovalHistoryByRequestIdAsync(requestId, checkingStatus);
                if (result.Any())
                {
                    transferInvoice.Result = result;
                    transferInvoice.StatusCode = StatusCodes.Status200OK;
                    transferInvoice.Message = "Approval invoices fetched successfully";
                    transferInvoice.Success = true;
                }
                else
                {
                    transferInvoice.Result = result;
                    transferInvoice.StatusCode = StatusCodes.Status400BadRequest;
                    transferInvoice.Message = "Approval invoices fetched unsuccessfully";
                    transferInvoice.Success = false;
                }
            }
            catch (SqlException ex)
            {
                transferInvoice.StatusCode = StatusCodes.Status500InternalServerError;
                transferInvoice.Message = $"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql Exception", ex);
            }
            catch (Exception ex)
            {
                transferInvoice.StatusCode = StatusCodes.Status500InternalServerError;
                transferInvoice.Message = $"Error Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }
            return transferInvoice;
        }


        #endregion
    }
}
