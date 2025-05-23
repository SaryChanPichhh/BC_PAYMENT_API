using System.Net;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.General;
using BC.PAYMENT.CORE.DTO.Items;
using BC.PAYMENT.LOGGING;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BC.PAYMENT.API.Controllers.Transaction.Inventory
{

    public class CheckingStockByBranchController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly List<string> Types = new()
        {
            "Trasfer","Debit Note","Opening Balance","Purchase","Sale","Transaction","Credit Note"
        };
        public CheckingStockByBranchController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("gettypes")]
        public async Task<ApiResponse<List<string>>> GetTypes()
        {
            return new ApiResponse<List<string>>()
            {
                Result = Types,
                Message = "Types fetched successfully",
                StatusCode = (int)HttpStatusCode.OK,
                Success = true
            };
        }
        
        [HttpGet]
        [Route("getwarehouse")]
        public async Task<ApiResponse<List<WarehouseDto>>> GetWarehouseAsync()
        {
            var credential = Common.DecodeJwt(User);
            var warehouse = new ApiResponse<List<WarehouseDto>>();
            try
            {
                var execute = await _unitOfWork.Warehouses.GetWarehouseAsync(credential.DbCode);
                if (execute.Any())
                {
                    warehouse.StatusCode = (int)HttpStatusCode.OK;
                    warehouse.Message = $@"Warehouse fetched successfully";
                    warehouse.Result = execute;
                    warehouse.Success = true;
                }
                else
                {
                    warehouse.StatusCode = (int)HttpStatusCode.BadRequest;
                    warehouse.Message = $@"Warehouse fetched unsuccessfully";

                }
            }
            catch (SqlException ex)
            {
                warehouse.StatusCode = (int)HttpStatusCode.InternalServerError;
                warehouse.Message = $@"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql Exception",ex);

            }catch (Exception ex)
            {
                warehouse.StatusCode = (int)HttpStatusCode.InternalServerError;
                warehouse.Message = $@"Error Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }
            return warehouse;
        }
        
        [HttpGet]
        [Route("getitems")]
        public async Task<ApiResponse<List<ItemDto>>> GetItemListAsync()
        {
            var credential = Common.DecodeJwt(User);
            var warehouse = new ApiResponse<List<ItemDto>>();
            try
            {
                var execute = await _unitOfWork.Items.GetItemListAsync(credential.DbCode);
                if (execute.Any())
                {
                    warehouse.StatusCode = (int)HttpStatusCode.OK;
                    warehouse.Message = $@"Items fetched successfully";
                    warehouse.Result = execute;
                    warehouse.Success = true;
                }
                else
                {
                    warehouse.StatusCode = (int)HttpStatusCode.BadRequest;
                    warehouse.Message = $@"Items fetched unsuccessfully";

                }
            }
            catch (SqlException ex)
            {
                warehouse.StatusCode = (int)HttpStatusCode.InternalServerError;
                warehouse.Message = $@"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql Exception",ex);

            }catch (Exception ex)
            {
                warehouse.StatusCode = (int)HttpStatusCode.InternalServerError;
                warehouse.Message = $@"Error Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }
            return warehouse;
        }
    }
}
