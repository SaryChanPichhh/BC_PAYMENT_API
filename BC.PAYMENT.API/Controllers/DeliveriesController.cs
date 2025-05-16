using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.Entities.General;
using BC.PAYMENT.LOGGING;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Net;

namespace BC.PAYMENT.API.Controllers
{
    [Helper.Authorize]
    public class DeliveriesController : BaseApiController
    {
        #region ===[ Private Members ]=============================================================

        private readonly IUnitOfWork _unitOfWork;
        private readonly AppSettings _appSettings;
        #endregion

        #region ===[ Constructor ]=================================================================

        /// <summary>
        /// Initialize DeliveriesController by injecting an object type of IUnitOfWork
        /// </summary>
        public DeliveriesController(IUnitOfWork unitOfWork, IOptions<AppSettings> appSettings)
        {
            this._unitOfWork = unitOfWork;
            this._appSettings = appSettings.Value;
        }

        #endregion


        [HttpGet("")]
        public async Task<ApiResponse<List<Delivery>>> GetDelivery()
        {
            var apiResponse = new ApiResponse<List<Delivery>>();

            try
            {
                var claim = Common.DecodeJwt(HttpContext.User);

                var data = await _unitOfWork.Deliveries.GetDelivery(claim.DbCode!);
                if (!data.Any())
                {
                    apiResponse.Success = false;
                    apiResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                    apiResponse.Message = "No new deliveries";
                    apiResponse.Result = new List<Delivery>();
                    return apiResponse;
                }

                apiResponse.Success = true;
                apiResponse.Message = "Deliveries fetched successfully.";
                apiResponse.StatusCode = (int)HttpStatusCode.OK;
                data.ForEach(x =>
                {
                    var encryptedId = EncryptionHelper.EncryptAES(x.DeliveryId.ToString());
                    encryptedId = Uri.EscapeDataString(encryptedId); // Ensure URL safety
                    x.ImagePath = $"api/deliveries/image/{encryptedId}".Trim();
                });

                apiResponse.Result = data;

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

        [AllowAnonymous]
        [HttpGet("Image/{deliveryId}")]
        public async Task<IActionResult> GetDeliveryImage(string deliveryId)
        {
            try
            {
                string decryptedId = EncryptionHelper.DecryptAES(Uri.UnescapeDataString(deliveryId));
                var data = await _unitOfWork.Deliveries.GetDeliveryImage(decryptedId);

                if (data == null || data.Image == null)
                {
                    return NotFound("Image not found");
                }

                return File(data.Image, "image/jpeg");
            }
            catch (Exception ex)
            {
                return BadRequest("Image not found");
            }
        }
    }
}
