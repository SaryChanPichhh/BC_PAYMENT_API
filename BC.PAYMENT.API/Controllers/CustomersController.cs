using System.Net;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.Contracts.General;
using BC.PAYMENT.CORE.Entities.General;
using BC.PAYMENT.CORE.Contracts.Response.Customer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BC.PAYMENT.API.Controllers;

public class CustomersController(IUnitOfWork unitOfWork, IOptions<AppSettings> appSettings) : BaseApiController
{
    [Helper.Authorize]
    [HttpGet("")]
    public async Task<ApiResponse<PaginatedResponse<Customer>>> GetCustomer([FromQuery] int page, int pageSize)
    {
        try
        {
            var allRecords = await unitOfWork.Customers.GetCustomer(page, pageSize);
            if (!allRecords.Any())
                return ApiResponse<PaginatedResponse<Customer>>.Builder()
                    .WithMessage("No customers")
                    .WithStatusCode
                        ((int)HttpStatusCode.BadRequest)
                    .WithResult
                    (new PaginatedResponse<Customer>(new List<Customer>(),
                        0, page, pageSize))
                    .Build();

            // Calculate pagination details
            var totalRecords = allRecords.Count;

            // Paginate the data
            var paginatedData = allRecords
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return ApiResponse<PaginatedResponse<Customer>>.Builder()
                .WithMessage("Account code fetched successfully.")
                .WithStatusCode((int)HttpStatusCode.OK)
                .WithResult
                (new PaginatedResponse<Customer>(paginatedData,
                    totalRecords, page, pageSize))
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<PaginatedResponse<Customer>>(ex.Message);
        }
    }

    [Helper.Authorize]
    [HttpGet("by-market-id")]
    public async Task<ApiResponse<List<CustomerResponse>>> GetCustomerInfoByMarketId([FromQuery] string marketId)
    {
        try
        {
            var data = await unitOfWork.Customers.GetCustomerInfoByMarketIdAsync(marketId);
            return ApiResponse<List<CustomerResponse>>.Builder()
                .WithMessage(data.Any() ? "Customers fetched successfully." : "No customers found.")
                .WithStatusCode(data.Any() ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest)
                .WithResult(data)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<CustomerResponse>>(ex.Message);
        }
    }

    [Helper.Authorize]
    [HttpGet("info")]
    public async Task<ApiResponse<PaginatedResponse<CustomerResponse>>> GetAllCustomerInfo([FromQuery] int pageNumber,
        int pageSize)
    {
        try
        {
            var claim = Common.DecodeJwt(HttpContext.User);
            var paginatedData = await unitOfWork.Customers.GetAllCustomerInfoAsync(claim.DbCode!, pageNumber, pageSize);
            var totalRecords = paginatedData.Count;

            if (!paginatedData.Any())
                return ApiResponse<PaginatedResponse<CustomerResponse>>.Builder()
                    .WithMessage("No customers info found.")
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .WithResult(new PaginatedResponse<CustomerResponse>(new List<CustomerResponse>(), 0, pageNumber,
                        pageSize))
                    .Build();

            paginatedData.ForEach(x =>
            {
                if (!string.IsNullOrEmpty(x.CustomerCode))
                {
                    var payload = $"{claim.DbCode}|{x.CustomerCode}";
                    var encryptedId = EncryptionHelper.EncryptAES(payload);
                    encryptedId = Uri.EscapeDataString(encryptedId);
                    x.ImageUrl = $"/customers/image/{encryptedId}".Trim();
                }
            });

            return ApiResponse<PaginatedResponse<CustomerResponse>>.Builder()
                .WithMessage("Customers info fetched successfully.")
                .WithStatusCode((int)HttpStatusCode.OK)
                .WithResult(new PaginatedResponse<CustomerResponse>(paginatedData, totalRecords, pageNumber, pageSize))
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<PaginatedResponse<CustomerResponse>>(ex.Message);
        }
    }

    [Helper.Authorize]
    [HttpGet("wrong-analysis")]
    public async Task<ApiResponse<List<CustomerResponse>>> GetCustomerWhoWrongAreaAndMarketAsync()
    {
        try
        {
            var claim = Common.DecodeJwt(HttpContext.User);
            var data = await unitOfWork.Customers.GetCustomerWhoWrongAreaAndMarketAsync(claim.DbCode!);

            data.ForEach(x =>
            {
                if (!string.IsNullOrEmpty(x.CustomerCode))
                {
                    var payload = $"{claim.DbCode}|{x.CustomerCode}";
                    var encryptedId = EncryptionHelper.EncryptAES(payload);
                    encryptedId = Uri.EscapeDataString(encryptedId);
                    x.ImageUrl = $"/customers/image/{encryptedId}".Trim();
                }
            });

            return ApiResponse<List<CustomerResponse>>.Builder()
                .WithMessage(data.Any() ? "Customers fetched successfully." : "No customers found.")
                .WithStatusCode(data.Any() ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest)
                .WithResult(data)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<CustomerResponse>>(ex.Message);
        }
    }

    [AllowAnonymous]
    [HttpGet("image/{customerId}")]
    public async Task<IActionResult> GetCustomerImage(string customerId)
    {
        try
        {
            var decryptedPayload = EncryptionHelper.DecryptAES(Uri.UnescapeDataString(customerId));
            var parts = decryptedPayload.Split('|');

            if (parts.Length != 2)
                return BadRequest("Invalid image token format.");

            var dbCode = parts[0];
            var decryptedId = parts[1];

            var imageBytes = await unitOfWork.Customers.GetCustomerImageAsync(dbCode, decryptedId);

            if (imageBytes is null)
                return Ok("This customer does not have an image.");
            if (imageBytes.Length == 0)
                return NotFound("Image not found");

            return File(imageBytes, "image/jpeg");
        }
        catch (Exception ex)
        {
            return Ok(ex.Message);
        }
    }
}