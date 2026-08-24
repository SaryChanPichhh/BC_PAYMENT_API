using System.Net;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.Entities.General;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BC.PAYMENT.API.Controllers;

public class CustomersController(IUnitOfWork unitOfWork, IOptions<AppSettings> appSettings) : BaseApiController
{
    [Authorize]
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
}