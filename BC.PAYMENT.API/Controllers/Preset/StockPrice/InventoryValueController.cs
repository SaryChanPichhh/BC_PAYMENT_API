using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.Preset.InventoryValue;
using BC.PAYMENT.CORE.DTO.Preset.OwedInvoiceDto;
using BC.PAYMENT.CORE.Entities.Preset.InventoryValue;
using BC.PAYMENT.CORE.Entities.Preset.OwedInvoice;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BC.PAYMENT.API.Controllers.Preset.StockPrice
{

    public class InventoryValueController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public InventoryValueController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        [Route("getinvetoryvalue")]
        public async Task<ApiResponse<List<InventoryValueModel>>> GetInventoryValueAsync([FromBody] InventoryValueDto model)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var execute = await _unitOfWork.InventoryValue.GetInventoryValueAsync(model.BranchDtos.ToDictionary(x => x.DbCode, x => x.DbName), model.Page, model.PageSize);
                if (execute.Any())
                {
                    return ApiResponse<List<InventoryValueModel>>.Builder()
                        .WithResult(execute)
                        .WithStatusCode(StatusCodes.Status200OK)
                        .WithMessage("Account receivable fetched successfully")
                        .Build();
                }
                else
                {
                    return ApiResponse<List<InventoryValueModel>>.Builder()
                        .WithStatusCode(StatusCodes.Status400BadRequest)
                        .WithMessage("Account receivable fetched unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<InventoryValueModel>>(ex.Message);
            }
        }
    }
}
