using System.Net;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.Transaction.Inventory.VerificationStock;
using BC.PAYMENT.CORE.Entities.Transaction.Inventory.VerificationStock;
using BC.PAYMENT.LOGGING;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BC.PAYMENT.API.Controllers.Transaction.Inventory
{

    public class VerificationStockReportController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public VerificationStockReportController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        [Route("getverificationinventorystockbydate/{fromDate}/{toDate}")]
        public async Task<ApiResponse<List<VerificationStockReportDto>>> GetVerificationInventoryStockByDate( string fromDate, string toDate)
        {
            var credential = Common.DecodeJwt(User);
            var verificationStock = new ApiResponse<List<VerificationStockReportDto>>();
            try
            {
                var verificationStocks = await _unitOfWork.VerificationStock.GetVerificationStockReport(credential.DbCode, Convert.ToDateTime(fromDate), Convert.ToDateTime(toDate
                ));
                if (verificationStocks.Any())
                {
                    verificationStock.Result = verificationStocks;
                    verificationStock.StatusCode = (int)HttpStatusCode.OK;
                    verificationStock.Message = "Verification Stock report fetched successfully";
                    verificationStock.Success = true;
                }
                else
                {
                    verificationStock.StatusCode = (int)HttpStatusCode.BadRequest;
                    verificationStock.Message = "Verification Stock report fetched unsuccessfully";
                }
            }
            catch (SqlException ex)
            {
                verificationStock.StatusCode = (int)HttpStatusCode.InternalServerError;
                verificationStock.Message = $@"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql Exception",ex);
            }
            catch (Exception ex)
            {
                verificationStock.StatusCode = (int)HttpStatusCode.InternalServerError;
                verificationStock.Message = $@"Error Exception : {ex.Message}";
                Logger.Instance.Error("Sql Exception", ex);
            }
            return verificationStock;
        } 
        
        [HttpGet]
        [Route("getverificationinventorystockbyperiod/{year}/{month}")]
        public async Task<ApiResponse<List<VerificationStockReportDto>>> GetVerificationStockReportByPeriod( int year, int month)
        {
            var credential = Common.DecodeJwt(User);
            var verificationStock = new ApiResponse<List<VerificationStockReportDto>>();
            try
            {
                var verificationStocks = await _unitOfWork.VerificationStock.GetVerificationStockReportByPeriod(credential.DbCode,month,year);
                if (verificationStocks.Any())
                {
                    verificationStock.Result = verificationStocks;
                    verificationStock.StatusCode = (int)HttpStatusCode.OK;
                    verificationStock.Message = "Verification Stock report fetched successfully";
                    verificationStock.Success = true;
                }
                else
                {
                    verificationStock.StatusCode = (int)HttpStatusCode.BadRequest;
                    verificationStock.Message = "Verification Stock report fetched unsuccessfully";
                }
            }
            catch (SqlException ex)
            {
                verificationStock.StatusCode = (int)HttpStatusCode.InternalServerError;
                verificationStock.Message = $@"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql Exception",ex);
            }
            catch (Exception ex)
            {
                verificationStock.StatusCode = (int)HttpStatusCode.InternalServerError;
                verificationStock.Message = $@"Error Exception : {ex.Message}";
                Logger.Instance.Error("Sql Exception", ex);
            }
            return verificationStock;
        }

    }
}
