using System.Net;
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

namespace BC.PAYMENT.API.Controllers;

[Authorize]
public class InvoicesController : BaseApiController
{
    #region ===[ Constructor ]=================================================================

    public InvoicesController(IUnitOfWork unitOfWork, IOptions<AppSettings> appSettings)
    {
        _unitOfWork = unitOfWork;
        _appSettings = appSettings.Value;
    }

    #endregion

    #region New Invoices

    [HttpPost("new/download")]
    public async Task<ApiResponse<List<InvoicesModel>>> GetInvoiceByInvoiceCode([FromBody] NewInvoicesRequestDTO newInvoiceRequest)
    {
        try
        {
            var claim = Common.DecodeJwt(HttpContext.User);
            newInvoiceRequest.DbCode = claim.DbCode;
            newInvoiceRequest.EntriesCode = claim.InvoiceEntryCode;
            newInvoiceRequest.Username = claim.Username;

            var data = await _unitOfWork.Invoices.GetInvoiceByInvoiceCode(newInvoiceRequest);
            if (!data.Any())
            {
                return ApiResponse<List<InvoicesModel>>.Builder()
                    .WithResult(new List<InvoicesModel>())
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .WithMessage("No new invoices")
                    .Build();
            }

            await _unitOfWork.Invoices.SaveInvoices(data, claim.CurrectDate);

            var invoicesModels = await _unitOfWork.Invoices.GetInvoices(InvoiceTypes.NewInvoice, newInvoiceRequest.DbCode!, claim.CurrectDate);
            if (!invoicesModels.Any())
            {
                return ApiResponse<List<InvoicesModel>>.Builder()
                    .WithResult(new List<InvoicesModel>())
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .WithMessage("No invoices reloaded")
                    .Build();
            }

            return ApiResponse<List<InvoicesModel>>.Builder()
                .WithResult(invoicesModels)
                .WithStatusCode((int)HttpStatusCode.OK)
                .WithMessage("Invoices fetched successfully.")
                .Build();
        }
        catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<InvoicesModel>>(ex.Message);
            }
    }

    #endregion

    #region ===[ Private Members ]=============================================================

    private readonly IUnitOfWork _unitOfWork;
    private readonly AppSettings _appSettings;

    #endregion

    #region Old Invoices

    [HttpPost("old")]
    public async Task<ApiResponse<PaginatedResponse<OldInvoicesModel>>> GetOldInvoice([FromBody] OldInvoiceRequestDto dto, int page = 1, int pageSize = 10)
    {


        try
        {
            var claim = Common.DecodeJwt(HttpContext.User);
            dto.DbCode = claim.DbCode;
            dto.Date = claim.CurrectDate;
            // Fetch all records
            var allRecords = await _unitOfWork.Invoices.GetAllOldInvoices(dto);

            if (!allRecords.Any())
            {
                return ApiResponse<PaginatedResponse<OldInvoicesModel>>.Builder()
                    .WithResult(new PaginatedResponse<OldInvoicesModel>(new List<OldInvoicesModel>(), 0, page, pageSize))
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .WithMessage("No old invoices")
                    .Build();
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
            return ApiResponse<PaginatedResponse<OldInvoicesModel>>.Builder()
                .WithResult(new PaginatedResponse<OldInvoicesModel>(paginatedData, totalRecords, page, pageSize))
                .WithStatusCode((int)HttpStatusCode.OK)
                .WithMessage("Old invoice fetched successfully.")
                .Build();
        }
        catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<PaginatedResponse<OldInvoicesModel>>(ex.Message);
            }
    }

    [HttpPost("old/{transactionCode}/verify")]
    public async Task<ApiResponse<OldInvoicesModel>> UpdateAndGetOldInvoice([FromBody] string transactionCode)
    {
        try
        {
            var claim = Common.DecodeJwt(HttpContext.User);

            if (string.IsNullOrWhiteSpace(transactionCode))
            {
                return ApiResponse<OldInvoicesModel>.Builder()
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .WithMessage("Transaction code is required.")
                    .Build();
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
                return ApiResponse<OldInvoicesModel>.Builder()
                    .WithStatusCode((int)HttpStatusCode.NotFound)
                    .WithMessage("No invoice found to update.")
                    .Build();
            }

            // Get the old invoice
            var oldInvoice = await _unitOfWork.Invoices.GetOneOldInvoiceByTransactionCode(claim.DbCode!, transactionCode);
            if (oldInvoice == null)
            {
                return ApiResponse<OldInvoicesModel>.Builder()
                    .WithStatusCode((int)HttpStatusCode.NotFound)
                    .WithMessage("Invoices not found.")
                    .Build();
            }

            return ApiResponse<OldInvoicesModel>.Builder()
                .WithResult(oldInvoice)
                .WithStatusCode((int)HttpStatusCode.OK)
                .WithMessage("Invoices updated and retrieved successfully.")
                .Build();
        }
        catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<OldInvoicesModel>(ex.Message);
            }
    }

    // [HttpPatch("old/{transactionCode}/verify-status")]
    // public async Task<ApiResponse<int>> UpdateInvoiceStatus(string transactionCode)
    // {
    //     try
    //     {
    //         var claim = Common.DecodeJwt(HttpContext.User);
    //
    //         if (string.IsNullOrWhiteSpace(transactionCode))
    //         {
    //             return ApiResponse<int>.Builder()
    //                 .WithStatusCode((int)HttpStatusCode.BadRequest)
    //                 .WithMessage("Transaction code is required.")
    //                 .Build();
    //         }
    //
    //         transactionCode = transactionCode.ToUpper();
    //
    //         // Call the UpdateStatus method
    //         var result = await _unitOfWork.Invoices.UpdateStatus(claim.DbCode!, transactionCode);
    //
    //         if (result > 0)
    //         {
    //             response.Success = true;
    //             response.StatusCode = (int)HttpStatusCode.OK;
    //             response.Message = "Invoices status updated successfully.";
    //             response.Result = result;
    //         }
    //         else
    //         {
    //             response.Success = false;
    //             response.StatusCode = (int)HttpStatusCode.NotFound;
    //             response.Message = "No invoice was updated.";
    //             response.Result = result;
    //         }
    //     }
    //     catch (Exception ex)
    //         {
    //             return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
    //         }
    //
    //     return response;
    // }

    // [HttpPost("old/save")]
    // public async Task<ApiResponse<int>> SaveOldInvoices([FromBody] List<OldInvoiceDTO> dto)
    // {
    //
    //
    //     try
    //     {
    //         var claim = Common.DecodeJwt(HttpContext.User);
    //         dto.ForEach(d =>
    //         {
    //             d.DbCode = claim.DbCode;
    //             d.CreatedBy = claim.Username;
    //             d.Period = claim.Period;
    //         });
    //
    //
    //         // Call the UpdateStatus method
    //         var result = await _unitOfWork.Invoices.SaveOldInvoice(dto);
    //
    //         if (result > 0)
    //         {
    //             response.Success = true;
    //             response.StatusCode = (int)HttpStatusCode.OK;
    //             response.Message = "Invoices status updated successfully.";
    //             response.Result = result;
    //         }
    //         else
    //         {
    //             response.Success = false;
    //             response.StatusCode = (int)HttpStatusCode.NotFound;
    //             response.Message = "No invoice was updated.";
    //             response.Result = result;
    //         }
    //     }
    //     catch (Exception ex)
    //         {
    //             return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
    //         }
    //
    //     return response;
    // }

    [HttpGet("old/invoice-by-status")]
    public async Task<ApiResponse<PaginatedResponse<OldInvoicesModel>>> GetOldInvoiceByStatus(int page = 1, int pageSize = 10)
    {
        try
        {
            var claim = Common.DecodeJwt(HttpContext.User);

            // Fetch all records
            var allRecords = await _unitOfWork.Invoices.GetAllOldInvoiceByStatus(claim.DbCode!, page, pageSize);

            if (!allRecords.Any())
            {
                return ApiResponse<PaginatedResponse<OldInvoicesModel>>.Builder()
                    .WithResult(new PaginatedResponse<OldInvoicesModel>(new List<OldInvoicesModel>(), 0, page, pageSize))
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .WithMessage("No old invoices")
                    .Build();
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
            return ApiResponse<PaginatedResponse<OldInvoicesModel>>.Builder()
                .WithResult(new PaginatedResponse<OldInvoicesModel>(paginatedData, totalRecords, page, pageSize))
                .WithStatusCode((int)HttpStatusCode.OK)
                .WithMessage("Old invoice fetched successfully.")
                .Build();
        }
        catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<PaginatedResponse<OldInvoicesModel>>(ex.Message);
            }
    }

    #endregion

    #region Change and Fix Invoices

    [HttpPost("change-fix/local")]
    public async Task<ApiResponse<PaginatedResponse<ChangeInvoice>>> GetLocalChangeFixInvoice([FromBody] FilterDTO dto)
    {


        try
        {
            var claim = Common.DecodeJwt(HttpContext.User);
            dto.DbCode = claim.DbCode;
            // Fetch all records
            var allRecords = await _unitOfWork.Invoices.GetLocalChangeAndFixInvoice(dto);

            if (!allRecords.Any())
            {
                return ApiResponse<PaginatedResponse<ChangeInvoice>>.Builder()
                    .WithResult(new PaginatedResponse<ChangeInvoice>(new List<ChangeInvoice>(), 0, dto.Page, dto.PageSize))
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .WithMessage("No change invoices")
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
            return ApiResponse<PaginatedResponse<ChangeInvoice>>.Builder()
                .WithResult(new PaginatedResponse<ChangeInvoice>(paginatedData, totalRecords, dto.Page, dto.PageSize))
                .WithStatusCode((int)HttpStatusCode.OK)
                .WithMessage("Change invoice fetched successfully.")
                .Build();
        }
        catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<PaginatedResponse<ChangeInvoice>>(ex.Message);
            }
    }

    [HttpPost("change-fix")]
    public async Task<ApiResponse<PaginatedResponse<ChangeInvoice>>> GetChangeFixInvoice([FromBody] FilterDTO dto)
    {


        try
        {
            var claim = Common.DecodeJwt(HttpContext.User);
            dto.DbCode = claim.DbCode;
            // Fetch all records
            var allRecords = await _unitOfWork.Invoices.GetChangeAndFixInvoice(dto);

            if (!allRecords.Any())
            {
                return ApiResponse<PaginatedResponse<ChangeInvoice>>.Builder()
                    .WithResult(new PaginatedResponse<ChangeInvoice>(new List<ChangeInvoice>(), 0, dto.Page, dto.PageSize))
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .WithMessage("No change invoices")
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
            return ApiResponse<PaginatedResponse<ChangeInvoice>>.Builder()
                .WithResult(new PaginatedResponse<ChangeInvoice>(paginatedData, totalRecords, dto.Page, dto.PageSize))
                .WithStatusCode((int)HttpStatusCode.OK)
                .WithMessage("Change invoice fetched successfully.")
                .Build();
        }
        catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<PaginatedResponse<ChangeInvoice>>(ex.Message);
            }
    }

    [HttpPost("change-fix/save")]
    public async Task<ApiResponse<int>> SaveFixInvoice([FromBody] SaveInvoiceDTO dto)
    {


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
                return ApiResponse<int>.Builder()
                    .WithResult(result)
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithMessage("Invoices saved successfully.")
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

    #endregion

    #region Return Invoices

    [HttpPost("returns")]
    public async Task<ApiResponse<PaginatedResponse<ReturnInvoice>>> GetReturnInvoice([FromBody] FilterDTO dto)
    {


        try
        {
            var claim = Common.DecodeJwt(HttpContext.User);
            dto.DbCode = claim.DbCode;
            // Fetch all records
            var allRecords = await _unitOfWork.Invoices.GetReturnInvoice(dto);

            if (!allRecords.Any())
            {
                return ApiResponse<PaginatedResponse<ReturnInvoice>>.Builder()
                    .WithResult(new PaginatedResponse<ReturnInvoice>(new List<ReturnInvoice>(), 0, dto.Page, dto.PageSize))
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .WithMessage("No return invoices")
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
            return ApiResponse<PaginatedResponse<ReturnInvoice>>.Builder()
                .WithResult(new PaginatedResponse<ReturnInvoice>(paginatedData, totalRecords, dto.Page, dto.PageSize))
                .WithStatusCode((int)HttpStatusCode.OK)
                .WithMessage("Return invoice fetched successfully.")
                .Build();
        }
        catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<PaginatedResponse<ReturnInvoice>>(ex.Message);
            }
    }

    [HttpGet("returns/today")]
    public async Task<ApiResponse<PaginatedResponse<ReturnInvoice>>> GetTodayReturnInvoice([FromQuery] int pageNumber = 1, int pagesize = 10)
    {


        try
        {
            var claim = Common.DecodeJwt(HttpContext.User);

            // Fetch all records
            var allRecords = await _unitOfWork.Invoices.GetTodayReturnInvoice(claim.DbCode!, pageNumber, pagesize);

            if (!allRecords.Any())
            {
                return ApiResponse<PaginatedResponse<ReturnInvoice>>.Builder()
                    .WithResult(new PaginatedResponse<ReturnInvoice>(new List<ReturnInvoice>(), 0, pageNumber, pagesize))
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .WithMessage("No return invoices")
                    .Build();
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
            return ApiResponse<PaginatedResponse<ReturnInvoice>>.Builder()
                .WithResult(new PaginatedResponse<ReturnInvoice>(paginatedData, totalRecords, pageNumber, pagesize))
                .WithStatusCode((int)HttpStatusCode.OK)
                .WithMessage("Return invoice fetched successfully.")
                .Build();
        }
        catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<PaginatedResponse<ReturnInvoice>>(ex.Message);
            }
    }

    [HttpPost("returns/save")]
    public async Task<ApiResponse<int>> SaveReturnInvoice([FromBody] SaveInvoiceDTO dto)
    {


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
                return ApiResponse<int>.Builder()
                    .WithResult(result)
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithMessage("Invoices saved successfully.")
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

    #endregion
}