
using BC.PAYMENT.CORE.Contracts.Request.Market;
using BC.PAYMENT.CORE.Contracts.Response.Market;
using BC.PAYMENT.CORE.Entities.Setting.Preset;
using BC.PAYMENT.CORE.Mappers;

namespace BC.PAYMENT.API.Controllers
{
    [Helper.Authorize]
    public class MarketController(IUnitOfWork unitOfWork) : BaseApiController
    {
        [HttpGet("")]
        public async Task<ApiResponse<List<MarketResponse>>> GetMarket()
        {
            try
            {
                var claim = Common.DecodeJwt(HttpContext.User); 

                var data = await unitOfWork.Markets.GetMarket(claim.DbCode!);
                if (data.Count == 0)
                {
                    return ApiResponse<List<MarketResponse>>.Builder()
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithMessage("No new markets")
                        .WithResult(new List<MarketResponse>())
                        .Build();
                }
                data.ForEach(x =>
                {
                    var encryptedId = EncryptionHelper.EncryptAES(x.MarketId!.ToString());
                    encryptedId = Uri.EscapeDataString(encryptedId); // Ensure URL safety
                    x.Image = $"api/v2/market/image/{encryptedId}".Trim();
                });
                return ApiResponse<List<MarketResponse>>.Builder()
                    .WithMessage("Markets fetched successfully.")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(data)
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<MarketResponse>>(ex.Message);
            }
        }
        [AllowAnonymous]
        [HttpGet("image/{marketId}")]
        public async Task<IActionResult> GetMarketImage(string marketId)
        {
            try
            {
                // helper = new EncryptionHelper(_configuration);

                // Decrypt the deliveryId from the request
                var decryptedId = EncryptionHelper.DecryptAES(Uri.UnescapeDataString(marketId));

                // Fetch the image using the decrypted marketId
                var imageBytes = await unitOfWork.Markets.GetMarketImageAsync(decryptedId);
                if (imageBytes == null)
                    return Ok("This market does not has image.");
                if (imageBytes.Length == 0)
                {
                    return NotFound("Image not found");
                }

                return File(imageBytes, "image/jpeg");
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpGet("by-saletypes")]
        public async Task<ApiResponse<List<Market>>> LoadMarketBySaleTypes([FromForm] List<string> saleTypes, [FromForm] int fromMov, [FromForm] int toMov)
        {
            try
            {
                var claim = Common.DecodeJwt(HttpContext.User);
                
                var data = await unitOfWork.Markets.LoadMarketBySaleTypesAsync(claim.DbCode!, saleTypes, fromMov, toMov);
                return ApiResponse<List<Market>>.Builder()
                    .WithMessage("Markets loaded successfully.")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(data)
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<Market>>(ex.Message);
            }
        }
        
        [HttpGet("by-market-id")]
        public async Task<ApiResponse<MarketResponse>> GetMarketByMarketIdAsync([FromQuery] string marketId)
        {
            try
            {
                var claim = Common.DecodeJwt(HttpContext.User);
                var data = await unitOfWork.Markets.GetMarketByMarketIdAsync(claim.DbCode!, marketId);
                return ApiResponse<MarketResponse>.Builder()
                    .WithMessage("Markets fetched successfully.")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(data)
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<MarketResponse>(ex.Message);
            }
        }

        [HttpGet("by-area-id")]
        public async Task<ApiResponse<List<MarketResponse>>> GetMarketByAreaIdAsync([FromQuery] string areaId)
        {
            try
            {
                var claim = Common.DecodeJwt(HttpContext.User);
                var data = await unitOfWork.Markets.GetMarketByAreaIdAsync(claim.DbCode!, areaId);
                return ApiResponse<List<MarketResponse>>.Builder()
                    .WithMessage("Markets fetched successfully.")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(data)
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<MarketResponse>>(ex.Message);
            }
        }

        [HttpGet("by-dbcode")]
        public async Task<ApiResponse<List<MarketResponse>>> GetMarketByDbCode()
        {
            try
            {
                var claim = Common.DecodeJwt(HttpContext.User);
                var data = await unitOfWork.Markets.GetMarketByDbCodeAsync(claim.DbCode!);
                return ApiResponse<List<MarketResponse>>.Builder()
                    .WithMessage("Markets loaded successfully.")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(data)
                    .Build();
            }
            catch (SqlException ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<MarketResponse>>(ex.Message);
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<MarketResponse>>(ex.Message);
            }
        }
        [HttpPost("")]
        public async Task<ApiResponse<int>> AddNewMarket([FromBody] MarketCreateRequest model)
        {
            try
            {
                var claim = Common.DecodeJwt(HttpContext.User);
                var data = model.ToMarketModel();
                data.DbCode = claim.DbCode;
                data.CreatedBy = claim.Username;
                data.CreatedAt = DateTime.Now;
                var execute = await unitOfWork.Markets.AddNewMarketAsync(data);
                return ApiResponse<int>.Builder()
                    .WithMessage(execute > 0 ? "Market added successfully." :"Market added unsuccessfully.")
                    .WithStatusCode(execute > 0 ? (int)HttpStatusCode.Created : (int)HttpStatusCode.BadRequest)
                    .WithResult(execute)
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
            }
        }

        [HttpPut("")]
        public async Task<ApiResponse<int>> UpdateMarket([FromBody] MarketUpdateRequest req)
        {
            try
            {
                var claim = Common.DecodeJwt(HttpContext.User);
                var model = req.ToMarketModel();
                model.DbCode = claim.DbCode;
                model.UpdatedBy = claim.Username;
                model.UpdatedAt = DateTime.Now;
                var execute = await unitOfWork.Markets.UpdateMarketAsync(model);
                return ApiResponse<int>.Builder()
                    .WithMessage("Market updated successfully.")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(execute)
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
            }
        }

        [HttpDelete("{marketId}")]
        public async Task<ApiResponse<int>> DeleteMarket(string marketId)
        {
            try
            {
                var execute = await unitOfWork.Markets.DeleteMarketAsync(marketId);
                return ApiResponse<int>.Builder()
                    .WithMessage(execute>0?"Market deleted successfully.":"Market deleted unsuccessfully.")
                    .WithStatusCode(execute>0?(int)HttpStatusCode.OK:(int)HttpStatusCode.BadRequest)
                    .WithResult(execute)
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
            }
        }
        [HttpGet("market-id")]
        public async Task<ApiResponse<string>> GenerateMarketIdAsync()
        {
            try
            {
                var marketId = await unitOfWork.Markets.GenerateMarketIdAsync();
                return ApiResponse<string>.Builder()
                    .WithMessage(!string.IsNullOrEmpty(marketId)?"Generate market id successfully.":"Generate market id unsuccessfully.")
                    .WithStatusCode(!string.IsNullOrEmpty(marketId)?(int)HttpStatusCode.OK:(int)HttpStatusCode.BadRequest)
                    .WithResult(marketId)
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<string>(ex.Message);
            }
        }
    }
}
