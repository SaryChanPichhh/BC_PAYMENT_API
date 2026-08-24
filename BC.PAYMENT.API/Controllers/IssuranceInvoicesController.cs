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
                    return ApiResponse<int>.Builder()
                        .WithResult(result)
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithMessage("Invoices divdied successfully.")
                        .Build();
                }
                else
                {
                    return ApiResponse<int>.Builder()
                        .WithResult(result)
                        .WithStatusCode((int)HttpStatusCode.NotFound)
                        .WithMessage("No invoice was updated.")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
            }
        }

        [HttpPost("invoices")]
        public async Task<ApiResponse<PaginatedResponse<Invoices>>> GetInvoices([FromBody] IssueInvoiceFilterDTO dto)
        {


            try
            {
                var claim = Common.DecodeJwt(HttpContext.User);
                dto.DbCode = claim.DbCode;

                var allRecords = await _unitOfWork.DividedInvoices.GetInvoices(dto);
                if (!allRecords.Any())
                {
                    return ApiResponse<PaginatedResponse<Invoices>>.Builder()
                        .WithResult(new PaginatedResponse<Invoices>(new List<Invoices>(), 0, dto.Page, dto.PageSize))
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithMessage("No invoices")
                        .Build();
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
                return ApiResponse<PaginatedResponse<Invoices>>.Builder()
                    .WithResult(new PaginatedResponse<Invoices>(paginatedData, totalRecords, dto.Page, dto.PageSize))
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithMessage("Invoices fetched successfully.")
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<PaginatedResponse<Invoices>>(ex.Message);
            }
        }

        [HttpPost("invoices/exclude")]
        public async Task<ApiResponse<PaginatedResponse<Invoices>>> GetInvoicesExclusion([FromBody] IssueInvoiceExclusionFilterDTO dto)
        {


            try
            {
                var claim = Common.DecodeJwt(HttpContext.User);
                dto.DbCode = claim.DbCode;

                var allRecords = await _unitOfWork.DividedInvoices.GetInvoices(dto);
                if (!allRecords.Any())
                {
                    return ApiResponse<PaginatedResponse<Invoices>>.Builder()
                        .WithResult(new PaginatedResponse<Invoices>(new List<Invoices>(), 0, dto.Page, dto.PageSize))
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithMessage("No invoices")
                        .Build();
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
                return ApiResponse<PaginatedResponse<Invoices>>.Builder()
                    .WithResult(new PaginatedResponse<Invoices>(paginatedData, totalRecords, dto.Page, dto.PageSize))
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithMessage("Invoices fetched successfully.")
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<PaginatedResponse<Invoices>>(ex.Message);
            }
        }

        [HttpGet("divided-delivery/filter")]
        public async Task<ApiResponse<List<Delivery>>> GetDividedDelivery([FromQuery] DateTime date)
        {


            try
            {
                var claim = Common.DecodeJwt(HttpContext.User);
                
                var deliveries = await _unitOfWork.DividedInvoices.GetDividedDeliveryInfo(claim.DbCode!, date.Date);
                if (!deliveries.Any())
                {
                    return ApiResponse<List<Delivery>>.Builder()
                        .WithResult(new List<Delivery>())
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithMessage("No delivery")
                        .Build();
                }
                
                return ApiResponse<List<Delivery>>.Builder()
                    .WithResult(deliveries)
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithMessage("Deliveries fetched successfully.")
                    .Build();

            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<Delivery>>(ex.Message);
            }
        }

        [HttpGet("divided-invoice")]
        public async Task<ApiResponse<List<Invoices>>> GetDividedInvoice([FromQuery] string deliveryId, DateTime date)
        {


            try
            {
                var claim = Common.DecodeJwt(HttpContext.User);

                var deliveries = await _unitOfWork.DividedInvoices.GetDividedInvoice(claim.DbCode!, deliveryId,date.Date);
                if (!deliveries.Any())
                {
                    return ApiResponse<List<Invoices>>.Builder()
                        .WithResult(new List<Invoices>())
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithMessage("No invoices")
                        .Build();
                }

                return ApiResponse<List<Invoices>>.Builder()
                    .WithResult(deliveries)
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithMessage("Deliveries fetched successfully.")
                    .Build();

            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<Invoices>>(ex.Message);
            }
        }

        [HttpGet("divided-Invoice/summary")]
        public async Task<ApiResponse<List<DividedInvoiceSummary>>> GetDividedInvoiceSummary([FromQuery] DateTime date)
        {


            try
            {
                var claim = Common.DecodeJwt(HttpContext.User);

                var deliveries = await _unitOfWork.DividedInvoices.GetDividedInvoiceSummary(claim.DbCode!, date.Date);
                if (!deliveries.Any())
                {
                    return ApiResponse<List<DividedInvoiceSummary>>.Builder()
                        .WithResult(new List<DividedInvoiceSummary>())
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithMessage("No summary")
                        .Build();
                }

                return ApiResponse<List<DividedInvoiceSummary>>.Builder()
                    .WithResult(deliveries)
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithMessage("Summary fetched successfully.")
                    .Build();

            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<DividedInvoiceSummary>>(ex.Message);
            }
        }
        #endregion

    }
}
