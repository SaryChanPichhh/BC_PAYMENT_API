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

namespace BC.PAYMENT.API.Controllers;

[Helper.Authorize]
[Route("api/v2/[controller]")]
[ApiController]
public class BranchesController(IUnitOfWork unitOfWork, IOptions<AppSettings> appSettings) : ControllerBase
{
    #region ===[ Public Methods ]==============================================================

    [AllowAnonymous]
    [HttpGet("")]
    public async Task<ApiResponse<List<BranchDTO>>> GetLoginBranch([FromQuery] string userName, string appCode)
    {
        try
        {
            var data = await unitOfWork.Branches.GetLoginBranchAsync(userName, appCode);
            return ApiResponse<List<BranchDTO>>.Builder()
                .WithMessage(data.Count != 0 ? "Branches fetched successfully." : "No branches")
                .WithStatusCode(data.Count != 0 ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest)
                .WithResult(data.Count != 0 ? data.ToList() : new List<BranchDTO>())
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<BranchDTO>>(ex.Message);
        }
    }

    #endregion
}