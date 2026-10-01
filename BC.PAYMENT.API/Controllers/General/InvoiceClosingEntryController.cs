using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.Entities.General;
using BC.PAYMENT.CORE.Contracts.Request.General;
using BC.PAYMENT.CORE.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BC.PAYMENT.API.Controllers.General;

public class InvoiceClosingEntryController(IUnitOfWork unitOfWork) : BaseApiController
{
    [HttpGet]
    [Route("is-open")]
    public async Task<ApiResponse<bool>> CheckIsEntriesIsAlreadyOpenAsync()
    {
        var claim = Common.DecodeJwt(HttpContext.User);
        var result = await unitOfWork.InvoiceClosingEntry.CheckIsEntriesIsAlreadyOpenAsync(claim?.DbCode);
        return ApiResponseFactory.SuccessResponse(result,
            result ? "Entries has already opened." : "Entries has not opened.");
    }

    [HttpPost]
    [Route("")]
    public async Task<ApiResponse<int>> CreateClosingEntryAsync([FromBody] InvoiceClosingEntriesRequest request)
    {
        var claim = Common.DecodeJwt(HttpContext.User);
        var closingEntry = request.ToInvoiceClosingEntriesModel();
        closingEntry.CreatedBy = "System"; // Set creator from JWT
        closingEntry.DbCode = claim?.DbCode;
        var result = await unitOfWork.InvoiceClosingEntry.CreateClosingEntryAsync(closingEntry);
        return ApiResponseFactory.SuccessResponse(result, "Closing entry created successfully.");
    }

    [HttpGet]
    [Route("generate-opening-code")]
    public async Task<ApiResponse<string>> GenerateOpeningEntryCodeAsync()
    {
        var claim = Common.DecodeJwt(HttpContext.User);
        var result = await unitOfWork.InvoiceClosingEntry.GenerateOpeningEntryCodeAsync(claim?.DbCode);
        return ApiResponseFactory.SuccessResponse(result, "Opening entry code generated successfully.");
    }

    [HttpGet]
    [Route("")]
    public async Task<ApiResponse<string>> GetOpeningEntryCodeByDbCodeAsync()
    {
        var claim = Common.DecodeJwt(HttpContext.User);
        var result = await unitOfWork.InvoiceClosingEntry.GetOpeningEntryCodeByDbCodeAsync(claim?.DbCode);
        return ApiResponseFactory.SuccessResponse(result, "Opening entry code retrieved successfully.");
    }
}