using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.Entities.Invoice;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Net;
using BC.PAYMENT.LOGGING;

namespace BC.PAYMENT.API.Controllers
{
    public class PaymentsController : BaseApiController
    {
        #region ===[ Private Members ]=============================================================

        private readonly IUnitOfWork _unitOfWork;
        private readonly AppSettings _appSettings;

        #endregion

        #region ===[ Constructor ]=================================================================

        /// <summary>
        /// Initialize PaymentsController by injecting an object type of IUnitOfWork
        /// </summary>
        public PaymentsController(IUnitOfWork unitOfWork, IOptions<AppSettings> appSettings)
        {
            this._unitOfWork = unitOfWork;
            this._appSettings = appSettings.Value;
        }

        #endregion

        [HttpGet("")]
        public async Task<ApiResponse<List<DividedInvoiceSummary>>> GetDeliveryPaidInvoice([FromQuery] DateTime date)
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
    }
}
