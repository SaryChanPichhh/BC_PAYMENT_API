using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.General;
using BC.PAYMENT.CORE.DTO.Preset.ExchangeItemAnalysis;
using BC.PAYMENT.CORE.DTO.Preset.OwedInvoiceDto;
using BC.PAYMENT.CORE.Entities.Preset.AnnualPurchase;
using BC.PAYMENT.CORE.Entities.Preset.OwedInvoice;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BC.PAYMENT.API.Controllers.Preset.OwedInvoice
{
 
    public class OwedInvoiceController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        public OwedInvoiceController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpPost]
        [Route("getaccountreceivableamount")]
        public async Task<ApiResponse<List<OwedInvoiceDto>>> GetAccountReceivableAmount([FromBody] List<BranchDTO> model)
        {
            var credential = Common.DecodeJwt(User);
            var oweInvoice = new ApiResponse<List<OwedInvoiceDto>>();
            try
            {
                var execute = await _unitOfWork.OwedInvoice.GetAccountReceivableAmount(model.ToDictionary(x=>x.DbCode,x=>x.DbName));
                if (execute.Any())
                {
                    oweInvoice.Result = execute;
                    oweInvoice.StatusCode = StatusCodes.Status200OK;
                    oweInvoice.Success = true;
                    oweInvoice.Message = "Amount fetched successfully";
                }
                else
                {
                    oweInvoice.StatusCode = StatusCodes.Status400BadRequest;
                    oweInvoice.Message = "Amount fetched unsuccessfully";
                }
            }
            catch (SqlException ex)
            {
                oweInvoice.StatusCode = StatusCodes.Status500InternalServerError;
                oweInvoice.Message = $"Sql Exception : ${ex.Message}";
            }
            catch (Exception ex)
            {
                oweInvoice.StatusCode = StatusCodes.Status500InternalServerError;
                oweInvoice.Message = $"Sql Exception : ${ex.Message}";
            }
            return oweInvoice;
        }
        [HttpPost]
        [Route("getaccountreceivabledetail")]
        public async Task<ApiResponse<List<SummaryAccountsReceivableModel>>> GetAccountReceivableSummaries([FromBody] OwedInvoiceFilterDto model)
        {
            var credential = Common.DecodeJwt(User);
            var oweInvoice = new ApiResponse<List<SummaryAccountsReceivableModel>>();
            try
            {
                var execute = await _unitOfWork.OwedInvoice.GetAccountReceivableSummaries(model.BranchDtos.ToDictionary(x=>x.DbCode,x=>x.DbName),model.Page,model.PageSize);
                if (execute.Any())
                {
                    oweInvoice.Result = execute;
                    oweInvoice.StatusCode = StatusCodes.Status200OK;
                    oweInvoice.Success = true;
                    oweInvoice.Message = "Account receivable fetched successfully";
                }
                else
                {
                    oweInvoice.StatusCode = StatusCodes.Status400BadRequest;
                    oweInvoice.Message = "Account receivable fetched unsuccessfully";
                }
            }
            catch (SqlException ex)
            {
                oweInvoice.StatusCode = StatusCodes.Status500InternalServerError;
                oweInvoice.Message = $"Sql Exception : ${ex.Message}";
            }
            catch (Exception ex)
            {
                oweInvoice.StatusCode = StatusCodes.Status500InternalServerError;
                oweInvoice.Message = $"Sql Exception : ${ex.Message}";
            }
            return oweInvoice;
        }
    }
}
