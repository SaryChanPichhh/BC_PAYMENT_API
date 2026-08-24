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
    public class ReviewReportController(IUnitOfWork unitOfWork, ILogger<ReviewReportController> logger) : BaseApiController
    {

        #region Requestion Action

        [HttpGet]
        [Route("getallrequestionaction")]
        public async Task<ApiResponse<List<RequestionActionDto>>> GetRequestActionAsync()
        {
            try
            {
                var result = await unitOfWork.ReviewReport.GetAllActionAsync();
                if (result.Any())
                {
                    return ApiResponse<List<RequestionActionDto>>.Builder()
                        .WithMessage("Request action fetched successfully")
                        .WithStatusCode(StatusCodes.Status200OK)
                        .WithResult(result)
                        .Build();
                }
                else
                {
                    return ApiResponse<List<RequestionActionDto>>.Builder()
                        .WithMessage("Request action fetched unsuccessfully")
                        .WithStatusCode(StatusCodes.Status400BadRequest)
                        .WithResult(result)
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<RequestionActionDto>>(ex.Message);
            }
        }
        [HttpGet]
        [Route("getallrequestbyactionid/{actionId}")]
        public async Task<ApiResponse<List<StockCarRequestDto>>> GetAllRequestByActionIdAsync([Required] int actionId)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var result = await unitOfWork.ReviewReport.GetAllRequestByActionIdAsync(credential.DbCode!,actionId);
                if (result.Any())
                {
                    return ApiResponse<List<StockCarRequestDto>>.Builder()
                        .WithMessage("Requests fetched successfully")
                        .WithStatusCode(StatusCodes.Status200OK)
                        .WithResult(result)
                        .Build();
                }
                else
                {
                    return ApiResponse<List<StockCarRequestDto>>.Builder()
                        .WithMessage("Requests fetched unsuccessfully")
                        .WithStatusCode(StatusCodes.Status400BadRequest)
                        .WithResult(result)
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<StockCarRequestDto>>(ex.Message);
            }
        }
        
        [HttpGet]
        [Route("getallrequestbyactionidandroleid")]
        public async Task<ApiResponse<List<StockCarRequestDto>>> GetAllRequestByActionIdAndRoleIdAsync([FromBody]  StockCarRequestParamDto model)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var result = await unitOfWork.ReviewReport.GetAllRequestByActionIdAndRoleIdAsync(credential.DbCode!,model.ActionId,model.RoleId);
                if (result.Any())
                {
                    return ApiResponse<List<StockCarRequestDto>>.Builder()
                        .WithMessage("Requests fetched successfully")
                        .WithStatusCode(StatusCodes.Status200OK)
                        .WithResult(result)
                        .Build();
                }
                else
                {
                    return ApiResponse<List<StockCarRequestDto>>.Builder()
                        .WithMessage("Requests fetched unsuccessfully")
                        .WithStatusCode(StatusCodes.Status400BadRequest)
                        .WithResult(result)
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<StockCarRequestDto>>(ex.Message);
            }
        }

        #endregion

        #region Review Credit Invoice
        [HttpGet]
        [Route("getallcreditinvoicebyrequestid/{requestId}/{checkingStatus}")]
        public async Task<ApiResponse<List<ReviewReportCreditInvoice>>> GetAllCreditInvoiceByRequestIdAndCheckingStatusAsync([Required] int requestId,[Required] bool checkingStatus)
        {
            try
            {
                var result = await unitOfWork.ReviewReport.GetAllCreditInvoiceByRequestIdAndCheckingStatus(requestId, checkingStatus);
                if (result.Any())
                {
                    return ApiResponse<List<ReviewReportCreditInvoice>>.Builder()
                        .WithMessage("Credit invoices fetched successfully")
                        .WithStatusCode(StatusCodes.Status200OK)
                        .WithResult(result)
                        .Build();
                }
                else
                {
                    return ApiResponse<List<ReviewReportCreditInvoice>>.Builder()
                        .WithMessage("Credit invoices fetched unsuccessfully")
                        .WithStatusCode(StatusCodes.Status400BadRequest)
                        .WithResult(result)
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<ReviewReportCreditInvoice>>(ex.Message);
            }
        }
        
        [HttpPut]
        [Route("updatestatuscreditinvoice/{requestId}/{checkingStatus}")]
        public async Task<ApiResponse<int>> UpdateStatusCheckingCreditInvoiceBySubmitId([Required] int requestId, [Required] bool checkingStatus)
        {
            try
            {
                var affectedRow = await unitOfWork.ReviewReport.UpdateStatusCheckingCreditInvoiceBySubmitId(requestId, checkingStatus);
                if (affectedRow > 0)
                {
                    return ApiResponse<int>.Builder()
                        .WithMessage("Checking status of credit invoices updated successfully")
                        .WithStatusCode(StatusCodes.Status204NoContent)
                        .WithResult(affectedRow)
                        .Build();
                }
                else
                {
                    return ApiResponse<int>.Builder()
                        .WithMessage("Checking status of credit invoices updated unsuccessfully")
                        .WithStatusCode(StatusCodes.Status400BadRequest)
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
            }
        }

        #endregion
        
        #region Review Paid Invoice
        [HttpGet]
        [Route("getallpaidinvoicebyrequestid/{requestId}/{checkingStatus}")]
        public async Task<ApiResponse<List<PaidInvoiceRequestDto>>> GetAllPaidInvoiceByRequestIdAndCheckingStatus([Required] int requestId, [Required] bool checkingStatus)
        {
            try
            {
                var result = await unitOfWork.ReviewReport.GetAllPaidInvoiceByRequestIdAndCheckingStatus(requestId, checkingStatus);
                if (result.Any())
                {
                    return ApiResponse<List<PaidInvoiceRequestDto>>.Builder()
                        .WithMessage("Paid invoices fetched successfully")
                        .WithStatusCode(StatusCodes.Status200OK)
                        .WithResult(result)
                        .Build();
                }
                else
                {
                    return ApiResponse<List<PaidInvoiceRequestDto>>.Builder()
                        .WithMessage("Paid invoices fetched unsuccessfully")
                        .WithStatusCode(StatusCodes.Status400BadRequest)
                        .WithResult(result)
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<PaidInvoiceRequestDto>>(ex.Message);
            }
        }
        
        [HttpPut]
        [Route("updatestatuspaidinvoice/{requestId}/{checkingStatus}")]
        public async Task<ApiResponse<int>> UpdateStatusCheckingPaymentInvoiceByInvoiceId([Required] int requestId, [Required] bool checkingStatus)
        {
            try
            {
                var affectedRow = await unitOfWork.ReviewReport.UpdateStatusCheckingPaymentInvoiceByInvoiceId(requestId, checkingStatus);
                if (affectedRow > 0)
                {
                    return ApiResponse<int>.Builder()
                        .WithMessage("Checking status of paid invoices updated successfully")
                        .WithStatusCode(StatusCodes.Status204NoContent)
                        .WithResult(affectedRow)
                        .Build();
                }
                else
                {
                    return ApiResponse<int>.Builder()
                        .WithMessage("Checking status of paid invoices updated unsuccessfully")
                        .WithStatusCode(StatusCodes.Status400BadRequest)
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
            }
        }

        #endregion
        
        #region Review Transfer Invoice
        [HttpGet]
        [Route("getalltransferinvoicebyrequestid/{requestId}/{checkingStatus}")]
        public async Task<ApiResponse<List<TransferMoneyModel>>> GetAllTransferByRequestIdAndCheckingStatus([Required] int requestId, [Required] bool checkingStatus)
        {
            try
            {
                var result = await unitOfWork.ReviewReport.GetAllTransferByRequestIdAndCheckingStatus(requestId, checkingStatus);
                if (result.Any())
                {
                    return ApiResponse<List<TransferMoneyModel>>.Builder()
                        .WithMessage("Transfer invoices fetched successfully")
                        .WithStatusCode(StatusCodes.Status200OK)
                        .WithResult(result)
                        .Build();
                }
                else
                {
                    return ApiResponse<List<TransferMoneyModel>>.Builder()
                        .WithMessage("Transfer invoices fetched unsuccessfully")
                        .WithStatusCode(StatusCodes.Status400BadRequest)
                        .WithResult(result)
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<TransferMoneyModel>>(ex.Message);
            }
        }
        
        [HttpPut]
        [Route("updatestatustransferinvoice/{requestId}/{checkingStatus}")]
        public async Task<ApiResponse<int>> UpdateStatusCheckingTransferBySubmitTransferId([Required] int requestId, [Required] bool checkingStatus)
        {
            try
            {
                var affectedRow = await unitOfWork.ReviewReport.UpdateStatusCheckingTransferBySubmitTransferId(requestId, checkingStatus);
                if (affectedRow > 0)
                {
                    return ApiResponse<int>.Builder()
                        .WithMessage("Checking status of transfer invoices updated successfully")
                        .WithStatusCode(StatusCodes.Status204NoContent)
                        .WithResult(affectedRow)
                        .Build();
                }
                else
                {
                    return ApiResponse<int>.Builder()
                        .WithMessage("Checking status of transfer invoices updated unsuccessfully")
                        .WithStatusCode(StatusCodes.Status400BadRequest)
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
            }
        }
        #endregion

        #region Expense Invoice

        [HttpGet]
        [Route("getallexpenseinvoicebyrequestid/{requestId}/{checkingStatus}")]
        public async Task<ApiResponse<List<ReviewReportExpenseDto>>> GetAllExpenseByRequestIdAndCheckingStatus([Required] int requestId, [Required] bool checkingStatus)
        {
            try
            {
                var result = await unitOfWork.ReviewReport.GetAllExpenseByRequestIdAndCheckingStatus(requestId, checkingStatus);
                if (result.Any())
                {
                    return ApiResponse<List<ReviewReportExpenseDto>>.Builder()
                        .WithMessage("Expense invoices fetched successfully")
                        .WithStatusCode(StatusCodes.Status200OK)
                        .WithResult(result)
                        .Build();
                }
                else
                {
                    return ApiResponse<List<ReviewReportExpenseDto>>.Builder()
                        .WithMessage("Expense invoices fetched unsuccessfully")
                        .WithStatusCode(StatusCodes.Status400BadRequest)
                        .WithResult(result)
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<ReviewReportExpenseDto>>(ex.Message);
            }
        }

        [HttpPut]
        [Route("updatestatusexpenseinvoice/{requestId}/{checkingStatus}")]
        public async Task<ApiResponse<int>> UpdateStatusCheckingExpenseByInvoiceId([Required] int requestId, [Required] bool checkingStatus)
        {
            try
            {
                var affectedRow = await unitOfWork.ReviewReport.UpdateStatusCheckingExpenseByInvoiceId(requestId, checkingStatus);
                if (affectedRow > 0)
                {
                    return ApiResponse<int>.Builder()
                        .WithMessage("Checking status of expense invoices updated successfully")
                        .WithStatusCode(StatusCodes.Status204NoContent)
                        .WithResult(affectedRow)
                        .Build();
                }
                else
                {
                    return ApiResponse<int>.Builder()
                        .WithMessage("Checking status of expense invoices updated unsuccessfully")
                        .WithStatusCode(StatusCodes.Status400BadRequest)
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
            }
        }

        #endregion
        #region Approval Invoice History
        [HttpGet]
        [Route("getallapprovalinvoicehistorybyrequestid/{requestId}/{checkingStatus}")]
        public async Task<ApiResponse<List<ApprovalInvoiceHistoryModel>>> GetAllApprovalHistoryByRequestIdAsync([Required] int requestId, [Required] bool checkingStatus)
        {
            try
            {
                var result = await unitOfWork.ReviewReport.GetAllApprovalHistoryByRequestIdAsync(requestId, checkingStatus);
                if (result.Any())
                {
                    return ApiResponse<List<ApprovalInvoiceHistoryModel>>.Builder()
                        .WithMessage("Approval invoices fetched successfully")
                        .WithStatusCode(StatusCodes.Status200OK)
                        .WithResult(result)
                        .Build();
                }
                else
                {
                    return ApiResponse<List<ApprovalInvoiceHistoryModel>>.Builder()
                        .WithMessage("Approval invoices fetched unsuccessfully")
                        .WithStatusCode(StatusCodes.Status400BadRequest)
                        .WithResult(result)
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<ApprovalInvoiceHistoryModel>>(ex.Message);
            }
        }


        #endregion
    }
}
