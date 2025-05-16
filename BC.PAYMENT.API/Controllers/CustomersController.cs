using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.Entities.General;
using BC.PAYMENT.LOGGING;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Net;

namespace BC.PAYMENT.API.Controllers
{
    public class CustomersController : BaseApiController
    {
        #region ===[ Private Members ]=============================================================

        private readonly IUnitOfWork _unitOfWork;
        private readonly AppSettings _appSettings;

        #endregion

        #region ===[ Constructor ]=================================================================

        /// <summary>
        /// Initialize CustomersController by injecting an object type of IUnitOfWork
        /// </summary>
        public CustomersController(IUnitOfWork unitOfWork, IOptions<AppSettings> appSettings)
        {
            this._unitOfWork = unitOfWork;
            this._appSettings = appSettings.Value;
        }

        #endregion

        [Authorize]
        [HttpGet("")]
        public async Task<ApiResponse<PaginatedResponse<Customer>>> GetCustomer([FromQuery] int page, int pageSize)
        {
            var apiResponse = new ApiResponse<PaginatedResponse<Customer>>();

            try
            {
                var allRecords = await _unitOfWork.Customers.GetCustomer(page,pageSize);
                if (!allRecords.Any())
                {
                    apiResponse.Success = false;
                    apiResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                    apiResponse.Message = "No customers";
                    apiResponse.Result = new PaginatedResponse<Customer>(new List<Customer>(), 0, page, pageSize);
                    return apiResponse;
                }
                // Calculate pagination details
                var totalRecords = allRecords.Count;
                //var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

                // Paginate the data
                var paginatedData = allRecords
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // Set the API response
                apiResponse.Success = true;
                apiResponse.Message = "Account code fetched successfully.";
                apiResponse.StatusCode = (int)HttpStatusCode.OK;
                apiResponse.Result = new PaginatedResponse<Customer>(paginatedData, totalRecords, page, pageSize);

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

    }

}
