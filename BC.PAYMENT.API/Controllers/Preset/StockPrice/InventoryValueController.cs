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
            var inventoryValue = new ApiResponse<List<InventoryValueModel>>();
            try
            {
                var execute = await _unitOfWork.InventoryValue.GetInventoryValueAsync(model.BranchDtos.ToDictionary(x => x.DbCode, x => x.DbName), model.Page, model.PageSize);
                if (execute.Any())
                {
                    inventoryValue.Result = execute;
                    inventoryValue.StatusCode = StatusCodes.Status200OK;
                    inventoryValue.Success = true;
                    inventoryValue.Message = "Account receivable fetched successfully";
                }
                else
                {
                    inventoryValue.StatusCode = StatusCodes.Status400BadRequest;
                    inventoryValue.Message = "Account receivable fetched unsuccessfully";
                }
            }
            catch (SqlException ex)
            {
                inventoryValue.StatusCode = StatusCodes.Status500InternalServerError;
                inventoryValue.Message = $"Sql Exception : ${ex.Message}";
            }
            catch (Exception ex)
            {
                inventoryValue.StatusCode = StatusCodes.Status500InternalServerError;
                inventoryValue.Message = $"Sql Exception : ${ex.Message}";
            }
            return inventoryValue;
        }
    }
}
