
using BC.PAYMENT.API.Models.Deliveries;
using CoreDeliveryResponse = BC.PAYMENT.CORE.Contracts.Response.Delivery.DeliveryResponse;

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

        [HttpGet]
        [Route("by-permission")]
        public async Task<ApiResponse<List<CoreDeliveryResponse>>> GetDeliveryByPermissionAsync()
        {
            try
            {
                var claim = Common.DecodeJwt(HttpContext.User);
                var data = await unitOfWork.Deliveries.GetDeliveryByPermissionAsync(claim.DbCode!);
                if (!data.Any())
                {
                    return ApiResponse<List<CoreDeliveryResponse>>.Builder()
                        .WithStatusCode(StatusCodes.Status400BadRequest)
                        .WithMessage("No deliveries found")
                        .WithResult([])
                        .Build();
                }

                data.ForEach(x =>
                {
                    if (!string.IsNullOrEmpty(x.DeliveryId))
                    {
                        var encryptedId = EncryptionHelper.EncryptAES(x.DeliveryId);
                        encryptedId = Uri.EscapeDataString(encryptedId);
                        x.ImagePath = $"api/deliveries/image/{encryptedId}".Trim();
                    }
                });

                return ApiResponse<List<CoreDeliveryResponse>>.Builder()
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithMessage("Deliveries fetched successfully.")
                    .WithResult(data)
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<CoreDeliveryResponse>>(ex.Message);
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
        [HttpPost("")]
        public async Task<ApiResponse<DeliveryResponse>> CreateDelivery([FromForm] CreateDeliveryRequest request)
        {
            try
            {
                var claim = Common.DecodeJwt(HttpContext.User);

                byte[]? imageBytes = null;
                if (request.Image != null && request.Image.Length > 0)
                {
                    using var ms = new MemoryStream();
                    await request.Image.CopyToAsync(ms);
                    imageBytes = ms.ToArray();
                }

                var deliveryId = Guid.NewGuid().ToString();
                var delivery = new Delivery
                {
                    DeliveryId = deliveryId,
                    DbCode = claim.DbCode ?? request.DbCode,
                    DeliveryName = request.DeliveryName,
                    DeliveryNameKhmer = request.DeliveryNameKhmer,
                    Others = request.Others,
                    Image = imageBytes,
                    Status = request.Status ?? "ACTIVE",
                    UserCreate = claim.Username,
                    CreateDate = DateTime.Now
                };

                var success = await unitOfWork.Deliveries.CreateDelivery(delivery);
                if (!success)
                {
                    return ApiResponse<DeliveryResponse>.Builder()
                        .WithStatusCode(StatusCodes.Status400BadRequest)
                        .WithMessage("Failed to create delivery")
                        .Build();
                }

                var response = new DeliveryResponse
                {
                    DeliveryId = delivery.DeliveryId,
                    DbCode = delivery.DbCode,
                    DeliveryName = delivery.DeliveryName,
                    DeliveryNameKhmer = delivery.DeliveryNameKhmer,
                    Others = delivery.Others,
                    Status = delivery.Status,
                    UserCreate = delivery.UserCreate,
                    CreateDate = delivery.CreateDate
                };

                return ApiResponse<DeliveryResponse>.Builder()
                    .WithStatusCode((int)HttpStatusCode.Created)
                    .WithMessage("Delivery created successfully.")
                    .WithResult(response)
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<DeliveryResponse>(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ApiResponse<DeliveryResponse>> UpdateDelivery(string id, [FromForm] UpdateDeliveryRequest request)
        {
            try
            {
                var claim = Common.DecodeJwt(HttpContext.User);

                byte[]? imageBytes = null;
                if (request.Image != null && request.Image.Length > 0)
                {
                    using var ms = new MemoryStream();
                    await request.Image.CopyToAsync(ms);
                    imageBytes = ms.ToArray();
                }

                var delivery = new Delivery
                {
                    DeliveryId = id,
                    DbCode = request.DbCode ?? claim.DbCode,
                    DeliveryName = request.DeliveryName,
                    DeliveryNameKhmer = request.DeliveryNameKhmer,
                    Others = request.Others,
                    Image = imageBytes,
                    Status = request.Status,
                    UserUpdate = claim.Username,
                    UpdateDate = DateTime.Now
                };

                var success = await unitOfWork.Deliveries.UpdateDelivery(delivery);
                if (!success)
                {
                    return ApiResponse<DeliveryResponse>.Builder()
                        .WithStatusCode(StatusCodes.Status400BadRequest)
                        .WithMessage("Failed to update delivery or delivery not found")
                        .Build();
                }

                var response = new DeliveryResponse
                {
                    DeliveryId = delivery.DeliveryId,
                    DbCode = delivery.DbCode,
                    DeliveryName = delivery.DeliveryName,
                    DeliveryNameKhmer = delivery.DeliveryNameKhmer,
                    Others = delivery.Others,
                    Status = delivery.Status,
                    UserUpdate = delivery.UserUpdate,
                    UpdateDate = delivery.UpdateDate
                };

                return ApiResponse<DeliveryResponse>.Builder()
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithMessage("Delivery updated successfully.")
                    .WithResult(response)
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<DeliveryResponse>(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ApiResponse<bool>> DeleteDelivery(string id)
        {
            try
            {
                var success = await unitOfWork.Deliveries.DeleteDelivery(id);
                if (!success)
                {
                    return ApiResponse<bool>.Builder()
                        .WithStatusCode(StatusCodes.Status404NotFound)
                        .WithMessage("Delivery not found or failed to delete")
                        .WithResult(false)
                        .Build();
                }

                return ApiResponse<bool>.Builder()
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithMessage("Delivery deleted successfully.")
                    .WithResult(true)
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<bool>(ex.Message);
            }
        }
    }
}
