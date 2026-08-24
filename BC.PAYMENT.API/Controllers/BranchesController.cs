using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.General;
using BC.PAYMENT.LOGGING;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Net;

namespace BC.PAYMENT.API.Controllers
{
    [Helper.Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BranchesController(IUnitOfWork unitOfWork, IOptions<AppSettings> appSettings) : ControllerBase
    {

        #region ===[ Public Methods ]==============================================================
        [AllowAnonymous]
        [HttpGet("dbcodes")]
        public async Task<ApiResponse<List<BranchDTO>>> GetLoginBranch([FromQuery]string username,string appCode)
        {
            try
            {
                var data = await unitOfWork.Branches.GetLoginBranchAsync(username,appCode);
                return ApiResponse<List<BranchDTO>>.Builder()
                    .WithMessage(data.Any() ? "Branches fetched successfully." : "No branches")
                    .WithStatusCode(data.Any() ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest)
                    .WithResult(data.Any() ? data.ToList() : new List<BranchDTO>())
                    .Build();
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<BranchDTO>>(ex.Message);
            }
        }
        #endregion
    }
}
