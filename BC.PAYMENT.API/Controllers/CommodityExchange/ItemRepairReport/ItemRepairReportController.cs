using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.CommondityExchange.ItemRepairReport;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.Net;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.CORE.Entities.CommondityExchange.ItemRepairReport;

namespace BC.PAYMENT.API.Controllers.CommodityExchange.ItemRepairReport
{
    public class ItemExchangeReportController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        public ItemExchangeReportController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("getitemrepairreport")]
        public async Task<ApiResponse<List<ItemRepairAnalysis>>> GetItemRepairReportAsync()
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var result = await _unitOfWork.ItemRepairReport.GetItemRepairReportAsync(credential.DbCode);
                if (result.Any())
                {
                    return ApiResponse<List<ItemRepairAnalysis>>.Builder()
                        .WithResult(result)
                        .WithMessage("Items repair fetched successfully")
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .Build();
                }
                else
                {
                    return ApiResponse<List<ItemRepairAnalysis>>.Builder()
                        .WithResult(new List<ItemRepairAnalysis>())
                        .WithMessage("Items repair invoices fetched unsuccessfully")
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<ItemRepairAnalysis>>(ex.Message);
            }
        }


        [HttpGet]
        [Route("getitemrepairreceivedreport")]
        public async Task<ApiResponse<List<ItemRepairReceivedDto>>> GetItemRepairReceivedAsync([Required] string fromdate, [Required] string todate)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var result = await _unitOfWork.ItemRepairReport.GetItemRepairReceivedAsync(credential.DbCode,Convert.ToDateTime(fromdate),Convert.ToDateTime(todate));
                if (result.Any())
                {
                    return ApiResponse<List<ItemRepairReceivedDto>>.Builder()
                        .WithResult(result)
                        .WithMessage("Items repair received fetched successfully")
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .Build();
                }
                else
                {
                    return ApiResponse<List<ItemRepairReceivedDto>>.Builder()
                        .WithResult(new List<ItemRepairReceivedDto>())
                        .WithMessage("Items repair received fetched unsuccessfully")
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<ItemRepairReceivedDto>>(ex.Message);
            }
        }
        
        [HttpGet]
        [Route("getitemrepairsendtorepairerreport")]
        public async Task<ApiResponse<List<ReportRepairItemInHandDto>>> GetItemRepairSendToRepairerReportAsync()
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var result = await _unitOfWork.ItemRepairReport.GetItemRepairSendToRepairerReportAsync(credential.DbCode);
                if (result.Any())
                {
                    return ApiResponse<List<ReportRepairItemInHandDto>>.Builder()
                        .WithResult(result)
                        .WithMessage("Items repairer in hand fetched successfully")
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .Build();
                }
                else
                {
                    return ApiResponse<List<ReportRepairItemInHandDto>>.Builder()
                        .WithResult(new List<ReportRepairItemInHandDto>())
                        .WithMessage("Items repairer in hand fetched unsuccessfully")
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<ReportRepairItemInHandDto>>(ex.Message);
            }
        }
        
        
        [HttpGet]
        [Route("getitemrepairingreport")]
        public async Task<ApiResponse<List<ReportItemInRepairDto>>> GetItemRepairingReportAsync()
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var result = await _unitOfWork.ItemRepairReport.GetItemRepairingReportAsync(credential.DbCode);
                if (result.Any())
                {
                    return ApiResponse<List<ReportItemInRepairDto>>.Builder()
                        .WithResult(result)
                        .WithMessage("Items repairing fetched successfully")
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .Build();
                }
                else
                {
                    return ApiResponse<List<ReportItemInRepairDto>>.Builder()
                        .WithResult(new List<ReportItemInRepairDto>())
                        .WithMessage("Items repairing fetched unsuccessfully")
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<ReportItemInRepairDto>>(ex.Message);
            }
        }
    }
}
