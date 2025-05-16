using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.Invoice;
using BC.PAYMENT.CORE.Entities.Invoice;
using BC.PAYMENT.CORE.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Net;
using BC.PAYMENT.LOGGING;
using BC.PAYMENT.CORE.DTO.Filter;
using BC.PAYMENT.CORE.Entities.General;

namespace BC.PAYMENT.API.Controllers
{
    [Authorize]
    public class IssuranceInvoicesController : BaseApiController
    {

        #region ===[ Private Members ]=============================================================

        private readonly IUnitOfWork _unitOfWork;
        private readonly AppSettings _appSettings;


        #endregion

        #region ===[ Constructor ]=================================================================
        public IssuranceInvoicesController(IUnitOfWork unitOfWork, IOptions<AppSettings> appSettings)
        {
            this._unitOfWork = unitOfWork;
            this._appSettings = appSettings.Value;
        }
        #endregion

        #region IssuranceInvoice

        [HttpPost("invoices/save")]
        public async Task<ApiResponse<int>> SaveDivideInvoices([FromBody] List<IssueInvoiceDTO> dto)
        {
            var response = new ApiResponse<int>();

            try
            {
                var claim = Common.DecodeJwt(HttpContext.User);
                dto.ForEach(d =>
                {
                    d.DbCode = claim.DbCode;
                    d.CreatedDate = claim.CurrectDate;
                    d.CreatedBy = claim.Username;
                });

                var result = await _unitOfWork.DividedInvoices.SaveDividedInvoice(dto);

                if (result > 0)
                {
                    response.Success = true;
                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.Message = "Invoices divdied successfully.";
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

        [HttpPost("invoices")]
        public async Task<ApiResponse<PaginatedResponse<Invoices>>> GetInvoices([FromBody] IssueInvoiceFilterDTO dto)
        {
            var apiResponse = new ApiResponse<PaginatedResponse<Invoices>>();

            try
            {
                var claim = Common.DecodeJwt(HttpContext.User);
                dto.DbCode = claim.DbCode;

                var allRecords = await _unitOfWork.DividedInvoices.GetInvoices(dto);
                if (!allRecords.Any())
                {
                    apiResponse.Success = false;
                    apiResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                    apiResponse.Message = "No invoices";
                    apiResponse.Result = new PaginatedResponse<Invoices>(new List<Invoices>(), 0, dto.Page, dto.PageSize);
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
                apiResponse.Message = "Invoices fetched successfully.";
                apiResponse.StatusCode = (int)HttpStatusCode.OK;
                apiResponse.Result = new PaginatedResponse<Invoices>(paginatedData, totalRecords, dto.Page, dto.PageSize);
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

        [HttpPost("invoices/exclude")]
        public async Task<ApiResponse<PaginatedResponse<Invoices>>> GetInvoicesExclusion([FromBody] IssueInvoiceExclusionFilterDTO dto)
        {
            var apiResponse = new ApiResponse<PaginatedResponse<Invoices>>();

            try
            {
                var claim = Common.DecodeJwt(HttpContext.User);
                dto.DbCode = claim.DbCode;

                var allRecords = await _unitOfWork.DividedInvoices.GetInvoices(dto);
                if (!allRecords.Any())
                {
                    apiResponse.Success = false;
                    apiResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                    apiResponse.Message = "No invoices";
                    apiResponse.Result = new PaginatedResponse<Invoices>(new List<Invoices>(), 0, dto.Page, dto.PageSize);
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
                apiResponse.Message = "Invoices fetched successfully.";
                apiResponse.StatusCode = (int)HttpStatusCode.OK;
                apiResponse.Result = new PaginatedResponse<Invoices>(paginatedData, totalRecords, dto.Page, dto.PageSize);
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

        [HttpGet("divided-delivery/filter")]
        public async Task<ApiResponse<List<Delivery>>> GetDividedDelivery([FromQuery] DateTime date)
        {
            var apiResponse = new ApiResponse<List<Delivery>>();

            try
            {
                var claim = Common.DecodeJwt(HttpContext.User);
                
                var deliveries = await _unitOfWork.DividedInvoices.GetDividedDeliveryInfo(claim.DbCode!, date.Date);
                if (!deliveries.Any())
                {
                    apiResponse.Success = false;
                    apiResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                    apiResponse.Message = "No delivery";
                    apiResponse.Result = new List<Delivery>();
                    return apiResponse;
                }
                
                apiResponse.Success = true;
                apiResponse.Message = "Deliveries fetched successfully.";
                apiResponse.StatusCode = (int)HttpStatusCode.OK;
                apiResponse.Result = deliveries;

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

        [HttpGet("divided-invoice")]
        public async Task<ApiResponse<List<Invoices>>> GetDividedInvoice([FromQuery] string deliveryId, DateTime date)
        {
            var apiResponse = new ApiResponse<List<Invoices>>();

            try
            {
                var claim = Common.DecodeJwt(HttpContext.User);

                var deliveries = await _unitOfWork.DividedInvoices.GetDividedInvoice(claim.DbCode!, deliveryId,date.Date);
                if (!deliveries.Any())
                {
                    apiResponse.Success = false;
                    apiResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                    apiResponse.Message = "No invoices";
                    apiResponse.Result = new List<Invoices>();
                    return apiResponse;
                }

                apiResponse.Success = true;
                apiResponse.Message = "Deliveries fetched successfully.";
                apiResponse.StatusCode = (int)HttpStatusCode.OK;
                apiResponse.Result = deliveries;

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

        [HttpGet("divided-Invoice/summary")]
        public async Task<ApiResponse<List<DividedInvoiceSummary>>> GetDividedInvoiceSummary([FromQuery] DateTime date)
        {
            var apiResponse = new ApiResponse<List<DividedInvoiceSummary>>();

            try
            {
                var claim = Common.DecodeJwt(HttpContext.User);

                var deliveries = await _unitOfWork.DividedInvoices.GetDividedInvoiceSummary(claim.DbCode!, date.Date);
                if (!deliveries.Any())
                {
                    apiResponse.Success = false;
                    apiResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                    apiResponse.Message = "No summary";
                    apiResponse.Result = new List<DividedInvoiceSummary>();
                    return apiResponse;
                }

                apiResponse.Success = true;
                apiResponse.Message = "Summary fetched successfully.";
                apiResponse.StatusCode = (int)HttpStatusCode.OK;
                apiResponse.Result = deliveries;

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

    }
}
