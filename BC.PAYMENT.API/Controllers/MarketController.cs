using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.Entities.General;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Net;
using BC.PAYMENT.LOGGING;

namespace BC.PAYMENT.API.Controllers
{
    [Helper.Authorize]
    public class MarketController : BaseApiController
    {
        #region ===[ Private Members ]=============================================================

        private readonly IUnitOfWork _unitOfWork;
        private readonly AppSettings _appSettings;

        #endregion

        #region ===[ Constructor ]=================================================================

        /// <summary>
        /// Initialize MarketController by injecting an object type of IUnitOfWork
        /// </summary>
        public MarketController(IUnitOfWork unitOfWork, IOptions<AppSettings> appSettings)
        {
            this._unitOfWork = unitOfWork;
            this._appSettings = appSettings.Value;
        }

        #endregion


        [HttpGet("")]
        public async Task<ApiResponse<List<Market>>> GetMarket()
        {
            var apiResponse = new ApiResponse<List<Market>>();

            try
            {
                var claim = Common.DecodeJwt(HttpContext.User);

                var data = await _unitOfWork.Markets.GetMarket(claim.DbCode!);
                if (!data.Any())
                {
                    apiResponse.Success = false;
                    apiResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                    apiResponse.Message = "No new markets";
                    apiResponse.Result = new List<Market>();
                    return apiResponse;
                }

                apiResponse.Success = true;
                apiResponse.Message = "Markets fetched successfully.";
                apiResponse.StatusCode = (int)HttpStatusCode.OK;
                data.ForEach(x =>
                {
                    var encryptedId = EncryptionHelper.EncryptAES(x.MarketID!.ToString());
                    encryptedId = Uri.EscapeDataString(encryptedId); // Ensure URL safety
                    x.ImagePath = $"api/markets/image/{encryptedId}".Trim();
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
        [HttpGet("Image/{marketId}")]
        public async Task<IActionResult> GetMarketImage(string marketId)
        {
            try
            {
                // helper = new EncryptionHelper(_configuration);

                // Decrypt the deliveryId from the request
                string decryptedId = EncryptionHelper.DecryptAES(Uri.UnescapeDataString(marketId));

                // Fetch the image using the decrypted deliveryId
                var data = await _unitOfWork.Deliveries.GetDeliveryImage(decryptedId);

                if (data == null || data.Image == null)
                {
                    return NotFound("Image not found");
                }

                return File(data.Image, "image/jpeg");
            }
            catch (Exception ex)
            {
                return BadRequest($"Invalid or tampered deliveryId: {ex.Message}");
            }
        }
    }
}
