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
    public class DeliveriesController(IUnitOfWork unitOfWork, IOptions<AppSettings> appSettings) : BaseApiController
    {


        [HttpGet("")]
        public async Task<ApiResponse<List<Delivery>>> GetDelivery()
        {
            try
            {
                var claim = Common.DecodeJwt(HttpContext.User);

                var data = await unitOfWork.Deliveries.GetDelivery(claim.DbCode!);
                if (!data.Any())
                {
                    return ApiResponse<List<Delivery>>.Builder()
                        .WithStatusCode(StatusCodes.Status400BadRequest)
                        .WithMessage("No new deliveries")
                        .WithResult(new List<Delivery>())
                        .Build();
                }

                data.ForEach(x =>
                {
                    var encryptedId = EncryptionHelper.EncryptAES(x.DeliveryId.ToString());
                    encryptedId = Uri.EscapeDataString(encryptedId); // Ensure URL safety
                    x.ImagePath = $"api/deliveries/image/{encryptedId}".Trim();
                });

                return ApiResponse<List<Delivery>>.Builder()
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithMessage("Deliveries fetched successfully.")
                    .WithResult(data)
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<Delivery>>(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet("Image/{deliveryId}")]
        public async Task<IActionResult> GetDeliveryImage(string deliveryId)
        {
            try
            {
                string decryptedId = EncryptionHelper.DecryptAES(Uri.UnescapeDataString(deliveryId));
                var data = await unitOfWork.Deliveries.GetDeliveryImage(decryptedId);

                if (data == null || data.Image == null)
                {
                    return NotFound("Image not found");
                }

                return File(data.Image, "image/jpeg");
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }
    }
}
