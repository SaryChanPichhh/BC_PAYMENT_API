using System.ComponentModel.DataAnnotations;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.CommondityExchange.RepairItem;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Net;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.CORE.DTO.CommondityExchange.CreditNote;

namespace BC.PAYMENT.API.Controllers.CommodityExchange.CreditNoteReport
{
    public class CreditNoteReportController(IUnitOfWork unitOfWork) : BaseApiController
    {

        [HttpGet]
        [Route("getcreditnotereportbydate/{fromDate}/{toDate}")]
        public async Task<ApiResponse<List<CreditNoteReportDto>>> GetReceivedRepairGoodByBranchAsync([Required] string fromDate, [Required] string toDate)
        {
            try
            {
                var credential = Common.DecodeJwt(User);
                var result = await unitOfWork.CreditNoteReport.GetCreditNoteReportsAsync(credential.DbCode,Convert.ToDateTime(fromDate),Convert.ToDateTime(toDate));
                
                return ApiResponse<List<CreditNoteReportDto>>.Builder()
                    .WithMessage(result != null ? "Credit note reports fetched successfully" : "Credit note reports goods fetched unsuccessfully")
                    .WithStatusCode(result != null ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest)
                    .WithResult(result != null ? result : new List<CreditNoteReportDto>())
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<CreditNoteReportDto>>(ex.Message);
            }
        }
    }
}
