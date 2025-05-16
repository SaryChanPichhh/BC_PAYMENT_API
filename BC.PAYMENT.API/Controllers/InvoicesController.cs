using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.Filter;
using BC.PAYMENT.CORE.DTO.Invoice;
using BC.PAYMENT.CORE.Entities.Invoice;
using BC.PAYMENT.CORE.Enums;
using BC.PAYMENT.LOGGING;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Net;

namespace BC.PAYMENT.API.Controllers
{
    [Authorize]
    public class InvoicesController : BaseApiController
    {

        #region ===[ Private Members ]=============================================================

        private readonly IUnitOfWork _unitOfWork;
        private readonly AppSettings _appSettings;
       

        #endregion

        #region ===[ Constructor ]=================================================================
        public InvoicesController(IUnitOfWork unitOfWork, IOptions<AppSettings> appSettings)
        {
            this._unitOfWork = unitOfWork;
            this._appSettings = appSettings.Value;
        }
        #endregion

        #region New Invoices
        [HttpPost("new/download")]
        public async Task<ApiResponse<List<InvoicesModel>>> GetInvoiceByInvoiceCode([FromBody] NewInvoicesRequestDTO newInvoiceRequest)
        {
            var apiResponse = new ApiResponse<List<InvoicesModel>>();

            try
            {
                var claim = Common.DecodeJwt(HttpContext.User);
                newInvoiceRequest.DbCode = claim.DbCode;
                newInvoiceRequest.EntriesCode = claim.InvoiceEntryCode;
                newInvoiceRequest.Username = claim.Username;
                
                var data = await _unitOfWork.Invoices.GetInvoiceByInvoiceCode(newInvoiceRequest);
                if (!data.Any())
                {
                    apiResponse.Success = false;
                    apiResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                    apiResponse.Message = "No new invoices";
                    apiResponse.Result = new List<InvoicesModel>();
                    return apiResponse;
                }
                await _unitOfWork.Invoices.SaveInvoices(data, claim.CurrectDate);

                var invoicesModels = await _unitOfWork.Invoices.GetInvoices(InvoiceTypes.NewInvoice, newInvoiceRequest.DbCode!, claim.CurrectDate);
                if (!invoicesModels.Any())
                {
                    apiResponse.Success = false;
                    apiResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                    apiResponse.Message = "No invoices reloaded";
                    apiResponse.Result = new List<InvoicesModel>();
                    return apiResponse;
                }
                apiResponse.Success = true;
                apiResponse.Message = "Invoices fetched successfully.";
                apiResponse.StatusCode = (int)HttpStatusCode.OK;
                apiResponse.Result = invoicesModels;

            }
            catch (SqlException ex)
            {
                apiResponse.Success = false;
                apiResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                apiResponse.Message = ex.Message;
                Logger.Instance.Error("SQL Exception:", ex);
            }
            catch (Exception ex)
            {
                apiResponse.Success = false;
                apiResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                apiResponse.Message = ex.Message;
                Logger.Instance.Error("Exception:", ex);
            }

            return apiResponse;
        }

        #endregion

        #region Old Invoices
        [HttpPost("old")]
        public async Task<ApiResponse<PaginatedResponse<OldInvoicesModel>>> GetOldInvoice([FromBody] OldInvoiceRequestDto dto,int page = 1, int pageSize = 10)
        {
            var apiResponse = new ApiResponse<PaginatedResponse<OldInvoicesModel>>();

            try
            {
                var claim = Common.DecodeJwt(HttpContext.User);
                dto.DbCode = claim.DbCode;
                dto.Date = claim.CurrectDate;
                // Fetch all records
                var allRecords = await _unitOfWork.Invoices.GetAllOldInvoices(dto);

                if (!allRecords.Any())
                {
                    apiResponse.Success = false;
                    apiResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                    apiResponse.Message = "No old invoices";
                    apiResponse.Result = new PaginatedResponse<OldInvoicesModel>(new List<OldInvoicesModel>(), 0, page, pageSize);
                    return apiResponse;
                }

                // Calculate pagination details
                var totalRecords = allRecords.Count;
                var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

                // Paginate the data
                var paginatedData = allRecords
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // Set the API response
                apiResponse.Success = true;
                apiResponse.Message = "Old invoice fetched successfully.";
                apiResponse.StatusCode = (int)HttpStatusCode.OK;
                apiResponse.Result = new PaginatedResponse<OldInvoicesModel>(paginatedData, totalRecords, page, pageSize);
            }
            catch (SqlException ex)
            {
                apiResponse.Success = false;
                apiResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                apiResponse.Message = ex.Message;
                Logger.Instance.Error("SQL Exception:", ex);
            }
            catch (Exception ex)
            {
                apiResponse.Success = false;
                apiResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                apiResponse.Message = ex.Message;
                Logger.Instance.Error("Exception:", ex);
            }

            return apiResponse;
        }

        [HttpPost("old/{transactionCode}/verify")]
        public async Task<ApiResponse<OldInvoicesModel>> UpdateAndGetOldInvoice([FromBody] string transactionCode)
        {
            var response = new ApiResponse<OldInvoicesModel>();

            try
            {
                var claim = Common.DecodeJwt(HttpContext.User);
               
                if (string.IsNullOrWhiteSpace(transactionCode))
                {
                    response.Success = false;
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Message = "Transaction code is required.";
                    return response;
                }
                
                transactionCode = transactionCode.ToUpper();

                var invoiceDto = new InvoiceDTO
                {
                    DbCode = claim.DbCode,
                    CreatedBy = claim.Username,
                    EntryCode = claim.InvoiceEntryCode,
                    TransactionCode = transactionCode
                };
                // Update the status
                var rowsAffected = await _unitOfWork.Invoices.UpdateStatusOldInvoiceByTransactionCode(invoiceDto);
                if (rowsAffected <= 0)
                {
                    response.Success = false;
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.Message = "No invoice found to update.";
                    return response;
                }

                // Get the old invoice
                var oldInvoice = await _unitOfWork.Invoices.GetOneOldInvoiceByTransactionCode(claim.DbCode!,transactionCode);
                if (oldInvoice == null)
                {
                    response.Success = false;
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.Message = "Invoices not found.";
                    return response;
                }

                response.Success = true;
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Message = "Invoices updated and retrieved successfully.";
                response.Result = oldInvoice;
            }
            catch (SqlException ex)
            {
                response.Success = false;
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Message = "A database error occurred.";
                Logger.Instance.Error("SQL Exception:", ex);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Message = "An unexpected error occurred.";
                Logger.Instance.Error("Exception:", ex);
            }

            return response;
        }

        [HttpPatch("old/{transactionCode}/verify-status")]
        public async Task<ApiResponse<int>> UpdateInvoiceStatus(string transactionCode)
        {
            var response = new ApiResponse<int>();

            try
            {
                var claim = Common.DecodeJwt(HttpContext.User);

                if (string.IsNullOrWhiteSpace(transactionCode))
                {
                    response.Success = false;
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Message = "Transaction code is required.";
                    return response;
                }

                transactionCode = transactionCode.ToUpper();

                // Call the UpdateStatus method
                var result = await _unitOfWork.Invoices.UpdateStatus(claim.DbCode!, transactionCode);

                if (result > 0)
                {
                    response.Success = true;
                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.Message = "Invoices status updated successfully.";
                    response.Result = result;
                }
                else
                {
                    response.Success = false;
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.Message = "No invoice was updated.";
                    response.Result = result;
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Message = "An unexpected error occurred.";
                Logger.Instance.Error("Exception:", ex);
            }

            return response;
        }

        [HttpPost("old/save")]
        public async Task<ApiResponse<int>> SaveOldInvoices([FromBody] List<OldInvoiceDTO> dto)
        {
            var response = new ApiResponse<int>();

            try
            {
                var claim = Common.DecodeJwt(HttpContext.User);
                dto.ForEach(d =>
                {
                    d.DbCode = claim.DbCode;
                    d.CreatedBy = claim.Username;
                    d.Period = claim.Period;
                });


                // Call the UpdateStatus method
                var result = await _unitOfWork.Invoices.SaveOldInvoice(dto);

                if (result > 0)
                {
                    response.Success = true;
                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.Message = "Invoices status updated successfully.";
                    response.Result = result;
                }
                else
                {
                    response.Success = false;
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.Message = "No invoice was updated.";
                    response.Result = result;
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Message = "An unexpected error occurred.";
                Logger.Instance.Error("Exception:", ex);
            }

            return response;
        }

        [HttpGet("old/invoice-by-status")]
        public async Task<ApiResponse<PaginatedResponse<OldInvoicesModel>>> GetOldInvoiceByStatus(int page = 1, int pageSize = 10)
        {
            var apiResponse = new ApiResponse<PaginatedResponse<OldInvoicesModel>>();

            try
            {
                var claim = Common.DecodeJwt(HttpContext.User);
             
                // Fetch all records
                var allRecords = await _unitOfWork.Invoices.GetAllOldInvoiceByStatus(claim.DbCode!,page,pageSize);

                if (!allRecords.Any())
                {
                    apiResponse.Success = false;
                    apiResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                    apiResponse.Message = "No old invoices";
                    apiResponse.Result = new PaginatedResponse<OldInvoicesModel>(new List<OldInvoicesModel>(), 0, page, pageSize);
                    return apiResponse;
                }

                // Calculate pagination details
                var totalRecords = allRecords.Count;
                var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

                // Paginate the data
                var paginatedData = allRecords
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // Set the API response
                apiResponse.Success = true;
                apiResponse.Message = "Old invoice fetched successfully.";
                apiResponse.StatusCode = (int)HttpStatusCode.OK;
                apiResponse.Result = new PaginatedResponse<OldInvoicesModel>(paginatedData, totalRecords, page, pageSize);
            }
            catch (SqlException ex)
            {
                apiResponse.Success = false;
                apiResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                apiResponse.Message = ex.Message;
                Logger.Instance.Error("SQL Exception:", ex);
            }
            catch (Exception ex)
            {
                apiResponse.Success = false;
                apiResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                apiResponse.Message = ex.Message;
                Logger.Instance.Error("Exception:", ex);
            }

            return apiResponse;
        }

        #endregion

        #region Change and Fix Invoices
        [HttpPost("change-fix/local")]
        public async Task<ApiResponse<PaginatedResponse<ChangeInvoice>>> GetLocalChangeFixInvoice([FromBody] FilterDTO dto)
        {
            var apiResponse = new ApiResponse<PaginatedResponse<ChangeInvoice>>();

            try
            {
                var claim = Common.DecodeJwt(HttpContext.User);
                dto.DbCode = claim.DbCode;
                // Fetch all records
                var allRecords = await _unitOfWork.Invoices.GetLocalChangeAndFixInvoice(dto);

                if (!allRecords.Any())
                {
                    apiResponse.Success = false;
                    apiResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                    apiResponse.Message = "No change invoices";
                    apiResponse.Result = new PaginatedResponse<ChangeInvoice>(new List<ChangeInvoice>(), 0, dto.Page, dto.PageSize);
                    return apiResponse;
                }

                // Calculate pagination details
                var totalRecords = allRecords.Count;
                var totalPages = (int)Math.Ceiling((double)totalRecords / dto.PageSize);

                // Paginate the data
                var paginatedData = allRecords
                    .Skip((dto.Page - 1) * dto.PageSize)
                    .Take(dto.PageSize)
                    .ToList();

                // Set the API response
                apiResponse.Success = true;
                apiResponse.Message = "Change invoice fetched successfully.";
                apiResponse.StatusCode = (int)HttpStatusCode.OK;
                apiResponse.Result = new PaginatedResponse<ChangeInvoice>(paginatedData, totalRecords, dto.Page, dto.PageSize);
            }
            catch (SqlException ex)
            {
                apiResponse.Success = false;
                apiResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                apiResponse.Message = ex.Message;
                Logger.Instance.Error("SQL Exception:", ex);
            }
            catch (Exception ex)
            {
                apiResponse.Success = false;
                apiResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                apiResponse.Message = ex.Message;
                Logger.Instance.Error("Exception:", ex);
            }

            return apiResponse;
        }

        [HttpPost("change-fix")]
        public async Task<ApiResponse<PaginatedResponse<ChangeInvoice>>> GetChangeFixInvoice([FromBody] FilterDTO dto)
        {
            var apiResponse = new ApiResponse<PaginatedResponse<ChangeInvoice>>();

            try
            {
                var claim = Common.DecodeJwt(HttpContext.User);
                dto.DbCode = claim.DbCode;
                // Fetch all records
                var allRecords = await _unitOfWork.Invoices.GetChangeAndFixInvoice(dto);

                if (!allRecords.Any())
                {
                    apiResponse.Success = false;
                    apiResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                    apiResponse.Message = "No change invoices";
                    apiResponse.Result = new PaginatedResponse<ChangeInvoice>(new List<ChangeInvoice>(), 0, dto.Page, dto.PageSize);
                    return apiResponse;
                }

                // Calculate pagination details
                var totalRecords = allRecords.Count;
                var totalPages = (int)Math.Ceiling((double)totalRecords / dto.PageSize);

                // Paginate the data
                var paginatedData = allRecords
                    .Skip((dto.Page - 1) * dto.PageSize)
                    .Take(dto.PageSize)
                    .ToList();

                // Set the API response
                apiResponse.Success = true;
                apiResponse.Message = "Change invoice fetched successfully.";
                apiResponse.StatusCode = (int)HttpStatusCode.OK;
                apiResponse.Result = new PaginatedResponse<ChangeInvoice>(paginatedData, totalRecords, dto.Page, dto.PageSize);
            }
            catch (SqlException ex)
            {
                apiResponse.Success = false;
                apiResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                apiResponse.Message = ex.Message;
                Logger.Instance.Error("SQL Exception:", ex);
            }
            catch (Exception ex)
            {
                apiResponse.Success = false;
                apiResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                apiResponse.Message = ex.Message;
                Logger.Instance.Error("Exception:", ex);
            }

            return apiResponse;
        }

        [HttpPost("change-fix/save")]
        public async Task<ApiResponse<int>> SaveFixInvoice([FromBody] SaveInvoiceDTO dto)
        {
            var response = new ApiResponse<int>();

            try
            {
                var claim = Common.DecodeJwt(HttpContext.User);
                dto.DbCode = claim.DbCode;
                dto.CreatedBy = claim.Username;
                dto.EntryCode = claim.InvoiceEntryCode;
                

                // Call the UpdateStatus method
                var result = await _unitOfWork.Invoices.SaveFixInvoice(dto);

                if (result > 0)
                {
                    response.Success = true;
                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.Message = "Invoices saved successfully.";
                    response.Result = result;
                }
                else
                {
                    response.Success = false;
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.Message = "No invoice was updated.";
                    response.Result = result;
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Message = "An unexpected error occurred.";
                Logger.Instance.Error("Exception:", ex);
            }

            return response;
        }

        #endregion

        #region Return Invoices

        [HttpPost("returns")]
        public async Task<ApiResponse<PaginatedResponse<ReturnInvoice>>> GetReturnInvoice([FromBody] FilterDTO dto)
        {
            var apiResponse = new ApiResponse<PaginatedResponse<ReturnInvoice>>();

            try
            {
                var claim = Common.DecodeJwt(HttpContext.User);
                dto.DbCode = claim.DbCode;
                // Fetch all records
                var allRecords = await _unitOfWork.Invoices.GetReturnInvoice(dto);

                if (!allRecords.Any())
                {
                    apiResponse.Success = false;
                    apiResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                    apiResponse.Message = "No return invoices";
                    apiResponse.Result = new PaginatedResponse<ReturnInvoice>(new List<ReturnInvoice>(), 0, dto.Page, dto.PageSize);
                    return apiResponse;
                }

                // Calculate pagination details
                var totalRecords = allRecords.Count;
                var totalPages = (int)Math.Ceiling((double)totalRecords / dto.PageSize);

                // Paginate the data
                var paginatedData = allRecords
                    .Skip((dto.Page - 1) * dto.PageSize)
                    .Take(dto.PageSize)
                    .ToList();

                // Set the API response
                apiResponse.Success = true;
                apiResponse.Message = "Return invoice fetched successfully.";
                apiResponse.StatusCode = (int)HttpStatusCode.OK;
                apiResponse.Result = new PaginatedResponse<ReturnInvoice>(paginatedData, totalRecords, dto.Page, dto.PageSize);
            }
            catch (SqlException ex)
            {
                apiResponse.Success = false;
                apiResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                apiResponse.Message = ex.Message;
                Logger.Instance.Error("SQL Exception:", ex);
            }
            catch (Exception ex)
            {
                apiResponse.Success = false;
                apiResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                apiResponse.Message = ex.Message;
                Logger.Instance.Error("Exception:", ex);
            }

            return apiResponse;
        }

        [HttpGet("returns/today")]
        public async Task<ApiResponse<PaginatedResponse<ReturnInvoice>>> GetTodayReturnInvoice([FromQuery] int pageNumber = 1, int pagesize = 10)
        {
            var apiResponse = new ApiResponse<PaginatedResponse<ReturnInvoice>>();

            try
            {
                var claim = Common.DecodeJwt(HttpContext.User);
                
                // Fetch all records
                var allRecords = await _unitOfWork.Invoices.GetTodayReturnInvoice(claim.DbCode!, pageNumber, pagesize);

                if (!allRecords.Any())
                {
                    apiResponse.Success = false;
                    apiResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                    apiResponse.Message = "No return invoices";
                    apiResponse.Result = new PaginatedResponse<ReturnInvoice>(new List<ReturnInvoice>(), 0, pageNumber, pagesize);
                    return apiResponse;
                }

                // Calculate pagination details
                var totalRecords = allRecords.Count;
                var totalPages = (int)Math.Ceiling((double)totalRecords / pagesize);

                // Paginate the data
                var paginatedData = allRecords
                    .Skip((pageNumber - 1) * pagesize)
                    .Take(pagesize)
                    .ToList();

                // Set the API response
                apiResponse.Success = true;
                apiResponse.Message = "Return invoice fetched successfully.";
                apiResponse.StatusCode = (int)HttpStatusCode.OK;
                apiResponse.Result = new PaginatedResponse<ReturnInvoice>(paginatedData, totalRecords, pageNumber, pagesize);
            }
            catch (SqlException ex)
            {
                apiResponse.Success = false;
                apiResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                apiResponse.Message = ex.Message;
                Logger.Instance.Error("SQL Exception:", ex);
            }
            catch (Exception ex)
            {
                apiResponse.Success = false;
                apiResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                apiResponse.Message = ex.Message;
                Logger.Instance.Error("Exception:", ex);
            }

            return apiResponse;
        }

        [HttpPost("returns/save")]
        public async Task<ApiResponse<int>> SaveReturnInvoice([FromBody] SaveInvoiceDTO dto)
        {
            var response = new ApiResponse<int>();

            try
            {
                var claim = Common.DecodeJwt(HttpContext.User);
                dto.DbCode = claim.DbCode;
                dto.CreatedBy = claim.Username;
                dto.EntryCode = claim.InvoiceEntryCode;


                // Call the UpdateStatus method
                var result = await _unitOfWork.Invoices.SaveReturnInvoice(dto);

                if (result > 0)
                {
                    response.Success = true;
                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.Message = "Invoices saved successfully.";
                    response.Result = result;
                }
                else
                {
                    response.Success = false;
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.Message = "No invoice was updated.";
                    response.Result = result;
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Message = "An unexpected error occurred.";
                response.Errors!.Add(ex.Message);
                Logger.Instance.Error("Exception:", ex);
            }

            return response;
        }
        #endregion
    }
}
