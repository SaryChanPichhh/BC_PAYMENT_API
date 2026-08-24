using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.Entities.General;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Net;
using BC.PAYMENT.API.Constants;
using BC.PAYMENT.CORE.Contracts.Request.Market;
using BC.PAYMENT.LOGGING;
using BC.PAYMENT.CORE.Contracts.Response.Market;
using BC.PAYMENT.CORE.Entities.Setting.Preset;
using BC.PAYMENT.CORE.Mappers;

namespace BC.PAYMENT.API.Controllers
{
    [Helper.Authorize]
    public class MarketController : BaseApiController
    {
        #region ===[ Private Members ]=============================================================

        private readonly IUnitOfWork _unitOfWork;

        #endregion

        #region ===[ Constructor ]=================================================================

        /// <summary>
        /// Initialize MarketController by injecting an object type of IUnitOfWork
        /// </summary>
        public MarketController(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
            
        }

        #endregion


        [HttpGet("")]
        public async Task<ApiResponse<List<Market>>> GetMarket()
        {
            try
            {
                var claim = Common.DecodeJwt(HttpContext.User);

                var data = await _unitOfWork.Markets.GetMarket(claim.DbCode!);
                if (data.Count == 0)
                {
                    return ApiResponse<List<Market>>.Builder()
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithMessage("No new markets")
                        .WithResult(new List<Market>())
                        .Build();
                }
                data.ForEach(x =>
                {
                    var encryptedId = EncryptionHelper.EncryptAES(x.MarketID!.ToString());
                    encryptedId = Uri.EscapeDataString(encryptedId); // Ensure URL safety
                    x.ImagePath = $"api/v2/markets/image/{encryptedId}".Trim();
                });
                return ApiResponse<List<Market>>.Builder()
                    .WithMessage("Markets fetched successfully.")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(data)
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<Market>>(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet("Image/{marketId}")]
        public async Task<IActionResult> GetMarketImage(string marketId)
        {
            try
            {
                // helper = new EncryptionHelper(_configuration);

                // Decrypt the deliveryId from the request
                var decryptedId = EncryptionHelper.DecryptAES(Uri.UnescapeDataString(marketId));

                // Fetch the image using the decrypted marketId
                var imageBytes = await _unitOfWork.Markets.GetMarketImageAsync(decryptedId);

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

        [HttpPost("By-SaleTypes")]
        public async Task<ApiResponse<List<Market>>> LoadMarketBySaleTypes([FromForm] List<string> saleTypes, [FromForm] int fromMov, [FromForm] int toMov)
        {
            try
            {
                var claim = Common.DecodeJwt(HttpContext.User);
                
                var data = await _unitOfWork.Markets.LoadMarketBySaleTypesAsync(claim.DbCode!, saleTypes, fromMov, toMov);
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

        [HttpGet("By-DbCode")]
        public async Task<ApiResponse<List<MarketResponse>>> GetMarketByDbCode()
        {
            try
            {
                var claim = Common.DecodeJwt(HttpContext.User);
                var data = await _unitOfWork.Markets.GetMarketByDbCodeAsync(claim.DbCode!);
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
                var execute = await _unitOfWork.Markets.AddNewMarketAsync(data);
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
        public async Task<ApiResponse<int>> UpdateMarket([FromBody] MarketModel model)
        {
            try
            {
                var claim = Common.DecodeJwt(HttpContext.User);
                model.DbCode = claim.DbCode;
                model.UpdatedBy = claim.Username;
                model.UpdatedAt = DateTime.Now;
                var execute = await _unitOfWork.Markets.UpdateMarketAsync(model);
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
                var execute = await _unitOfWork.Markets.DeleteMarketAsync(marketId);
                return ApiResponse<int>.Builder()
                    .WithMessage("Market deleted successfully.")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(execute)
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
            }
        }
    }
}
