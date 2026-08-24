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
            try
            {
                var execute = await _unitOfWork.OwedInvoice.GetAccountReceivableAmount(model.ToDictionary(x=>x.DbCode,x=>x.DbName));
                if (execute.Any())
                {
                    return ApiResponse<List<OwedInvoiceDto>>.Builder()
                        .WithResult(execute)
                        .WithStatusCode(StatusCodes.Status200OK)
                        .WithMessage("Amount fetched successfully")
                        .Build();
                }
                else
                {
                    return ApiResponse<List<OwedInvoiceDto>>.Builder()
                        .WithStatusCode(StatusCodes.Status400BadRequest)
                        .WithMessage("Amount fetched unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<OwedInvoiceDto>>(ex.Message);
            }
        }
        [HttpPost]
        [Route("getaccountreceivabledetail")]
        public async Task<ApiResponse<List<SummaryAccountsReceivableModel>>> GetAccountReceivableSummaries([FromBody] OwedInvoiceFilterDto model)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var execute = await _unitOfWork.OwedInvoice.GetAccountReceivableSummaries(model.BranchDtos.ToDictionary(x=>x.DbCode,x=>x.DbName),model.Page,model.PageSize);
                if (execute.Any())
                {
                    return ApiResponse<List<SummaryAccountsReceivableModel>>.Builder()
                        .WithResult(execute)
                        .WithStatusCode(StatusCodes.Status200OK)
                        .WithMessage("Account receivable fetched successfully")
                        .Build();
                }
                else
                {
                    return ApiResponse<List<SummaryAccountsReceivableModel>>.Builder()
                        .WithStatusCode(StatusCodes.Status400BadRequest)
                        .WithMessage("Account receivable fetched unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<SummaryAccountsReceivableModel>>(ex.Message);
            }
        }
    }
}
