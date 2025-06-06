using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.INFRASTRUCTURE.Repository.General;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.CORE.DTO.Preset.ExchangeItemAnalysis;
using BC.PAYMENT.CORE.Entities.General;
using BC.PAYMENT.CORE.Entities.Preset.AnnualPurchase;
using BC.PAYMENT.CORE.DTO.General;

namespace BC.PAYMENT.API.Controllers.Preset.TotalAnnualPurchase
{

    public class AnnualPurchaseController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public AnnualPurchaseController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region Sale Of Year
        [HttpPost]
        [Route("getreportsaleofyear")]
        public async Task<ApiResponse<List<AnnualPurchaseModel>>> GetSalesReportListAsync([Required] List<int> years, [Required] int page, [Required] int pageSize)
        {
            var credential = Common.DecodeJwt(User);
            var itemTransaction = new ApiResponse<List<AnnualPurchaseModel>>();
            try
            {
                var execute = await _unitOfWork.AnnualPurchase.GetSalesReportListAsync(credential.DbCode, years, page, pageSize);
                if (execute.Any())
                {
                    itemTransaction.Result = execute;
                    itemTransaction.StatusCode = StatusCodes.Status200OK;
                    itemTransaction.Success = true;
                    itemTransaction.Message = "Items fetched successfully";
                }
                else
                {
                    itemTransaction.StatusCode = StatusCodes.Status400BadRequest;
                    itemTransaction.Message = "Items fetched unsuccessfully";
                }
            }
            catch (SqlException ex)
            {
                itemTransaction.StatusCode = StatusCodes.Status500InternalServerError;
                itemTransaction.Message = $"Sql Exception : ${ex.Message}";
            }
            catch (Exception ex)
            {
                itemTransaction.StatusCode = StatusCodes.Status500InternalServerError;
                itemTransaction.Message = $"Sql Exception : ${ex.Message}";
            }
            return itemTransaction;
        }

        [HttpPost]
        [Route("getreportsaleofyearbysaletype")]
        public async Task<ApiResponse<List<AnnualPurchaseModel>>> GetAllReportSalesPerYearsAsync(ReportSaleBySaleType model)
        {
            var credential = Common.DecodeJwt(User);
            var itemTransaction = new ApiResponse<List<AnnualPurchaseModel>>();
            try
            {
                var execute = await _unitOfWork.AnnualPurchase.GetAllReportSalesPerYearsAsync(credential.DbCode, model.Markets, model.CustomerCode, model.SaleTypes);
                if (execute.Any())
                {
                    itemTransaction.Result = execute;
                    itemTransaction.StatusCode = StatusCodes.Status200OK;
                    itemTransaction.Success = true;
                    itemTransaction.Message = "Items fetched successfully";
                }
                else
                {
                    itemTransaction.StatusCode = StatusCodes.Status400BadRequest;
                    itemTransaction.Message = "Items fetched unsuccessfully";
                }
            }
            catch (SqlException ex)
            {
                itemTransaction.StatusCode = StatusCodes.Status500InternalServerError;
                itemTransaction.Message = $"Sql Exception : ${ex.Message}";
            }
            catch (Exception ex)
            {
                itemTransaction.StatusCode = StatusCodes.Status500InternalServerError;
                itemTransaction.Message = $"Sql Exception : ${ex.Message}";
            }
            return itemTransaction;
        }
        #endregion
        
        [HttpPost]
        [Route("getcustomerbymarketcode" )]
        public async Task<ApiResponse<List<Customer>>> GetCustomerByMarketCodeAsync([FromForm]List<string> markets, [FromForm] List<string> saleTypes)
        {
            var credential = Common.DecodeJwt(User);
            var itemTransaction = new ApiResponse<List<Customer>>();
            try
            {
                var execute = await _unitOfWork.Customers.GetCustomerByMarketCodeAsync(credential.DbCode, markets, saleTypes);
                if (execute.Any())
                {
                    itemTransaction.Result = execute;
                    itemTransaction.StatusCode = StatusCodes.Status200OK;
                    itemTransaction.Success = true;
                    itemTransaction.Message = "Customer fetched successfully";
                }
                else
                {
                    itemTransaction.StatusCode = StatusCodes.Status400BadRequest;
                    itemTransaction.Message = "Customer fetched unsuccessfully";
                }
            }
            catch (SqlException ex)
            {
                itemTransaction.StatusCode = StatusCodes.Status500InternalServerError;
                itemTransaction.Message = $"Sql Exception : ${ex.Message}";
            }
            catch (Exception ex)
            {
                itemTransaction.StatusCode = StatusCodes.Status500InternalServerError;
                itemTransaction.Message = $"Sql Exception : ${ex.Message}";
            }
            return itemTransaction;
        }
        [HttpPost]
        [Route("getmarketbysaletypes" )]
        public async Task<ApiResponse<List<MarketDto>>> GetCustomerByMarketCodeAsync([FromBody] MarketFilterDto model)
        {
            var credential = Common.DecodeJwt(User);
            var itemTransaction = new ApiResponse<List<MarketDto>>();
            try
            {
                var execute = await _unitOfWork.Markets.LoadMarketBySaleTypesAsync(credential.DbCode, model.SaleTypes, model.FromMov, model.ToMov);
                if (execute.Any())
                {
                    itemTransaction.Result = execute.Select(x=> new MarketDto()
                    {
                        MarketName = x.MarketName,
                        MarketNameKhmer = x.MarketNameKhmer
                    }).ToList();
                    itemTransaction.StatusCode = StatusCodes.Status200OK;
                    itemTransaction.Success = true;
                    itemTransaction.Message = "Market fetched successfully";
                }
                else
                {
                    itemTransaction.StatusCode = StatusCodes.Status400BadRequest;
                    itemTransaction.Message = "Market fetched unsuccessfully";
                }
            }
            catch (SqlException ex)
            {
                itemTransaction.StatusCode = StatusCodes.Status500InternalServerError;
                itemTransaction.Message = $"Sql Exception : ${ex.Message}";
            }
            catch (Exception ex)
            {
                itemTransaction.StatusCode = StatusCodes.Status500InternalServerError;
                itemTransaction.Message = $"Sql Exception : ${ex.Message}";
            }
            return itemTransaction;
        }
        [HttpGet]
        [Route("getsaletype")]
        public async Task<ApiResponse<List<string>>> GetSalesReportListAsync()
        {
            var credential = Common.DecodeJwt(User);
            var itemTransaction = new ApiResponse<List<string>>();
            try
            {
                var execute = await _unitOfWork.AnnualPurchase.SaleCodesAsync(credential.DbCode);
                if (execute.Any())
                {
                    itemTransaction.Result = execute;
                    itemTransaction.StatusCode = StatusCodes.Status200OK;
                    itemTransaction.Success = true;
                    itemTransaction.Message = "Items fetched successfully";
                }
                else
                {
                    itemTransaction.StatusCode = StatusCodes.Status400BadRequest;
                    itemTransaction.Message = "Items fetched unsuccessfully";
                }
            }
            catch (SqlException ex)
            {
                itemTransaction.StatusCode = StatusCodes.Status500InternalServerError;
                itemTransaction.Message = $"Sql Exception : ${ex.Message}";
            }
            catch (Exception ex)
            {
                itemTransaction.StatusCode = StatusCodes.Status500InternalServerError;
                itemTransaction.Message = $"Sql Exception : ${ex.Message}";
            }
            return itemTransaction;
        }

        #region Sale Of The Day

        [HttpPost]
        [Route("getdailysalereportvalue")]
        public async Task<ApiResponse<List<DailySaleReportValueModel>>> GetDailySaleReportValueAsync([FromBody] DailySaleReportValueDto model)
        {
            var credential = Common.DecodeJwt(User);
            var itemTransaction = new ApiResponse<List<DailySaleReportValueModel>>();
            try
            {
                var execute = await _unitOfWork.AnnualPurchase.GetDailySaleReportValueAsync(model.Date,model.Branches);
                if (execute.Any())
                {
                    itemTransaction.Result = execute;
                    itemTransaction.StatusCode = StatusCodes.Status200OK;
                    itemTransaction.Success = true;
                    itemTransaction.Message = "Daily sale report fetched successfully";
                }
                else
                {
                    itemTransaction.StatusCode = StatusCodes.Status400BadRequest;
                    itemTransaction.Message = "Daily sale report fetched unsuccessfully";
                }
            }
            catch (SqlException ex)
            {
                itemTransaction.StatusCode = StatusCodes.Status500InternalServerError;
                itemTransaction.Message = $"Sql Exception : ${ex.Message}";
            }
            catch (Exception ex)
            {
                itemTransaction.StatusCode = StatusCodes.Status500InternalServerError;
                itemTransaction.Message = $"Sql Exception : ${ex.Message}";
            }
            return itemTransaction;
        }
        
        [HttpGet]
        [Route("getdailysalereportdetailvalue/{date}/{branchCode}")]
        public async Task<ApiResponse<List<DailySaleReportValueDetailsModel>>> GetDailySaleReportDetailsValueAsync([Required] string date, [Required] string branchCode)
        {
            var credential = Common.DecodeJwt(User);
            var itemTransaction = new ApiResponse<List<DailySaleReportValueDetailsModel>>();
            try
            {
                var execute = await _unitOfWork.AnnualPurchase.GetDailySaleReportDetailsValueAsync(Convert.ToDateTime(date), branchCode);
                if (execute.Any())
                {
                    itemTransaction.Result = execute;
                    itemTransaction.StatusCode = StatusCodes.Status200OK;
                    itemTransaction.Success = true;
                    itemTransaction.Message = "Daily sale report detail fetched successfully";
                }
                else
                {
                    itemTransaction.StatusCode = StatusCodes.Status400BadRequest;
                    itemTransaction.Message = "Daily sale report detail fetched unsuccessfully";
                }
            }
            catch (SqlException ex)
            {
                itemTransaction.StatusCode = StatusCodes.Status500InternalServerError;
                itemTransaction.Message = $"Sql Exception : ${ex.Message}";
            }
            catch (Exception ex)
            {
                itemTransaction.StatusCode = StatusCodes.Status500InternalServerError;
                itemTransaction.Message = $"Sql Exception : ${ex.Message}";
            }
            return itemTransaction;
        }

        #endregion
    }
}
