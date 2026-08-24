using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.ClosingInventoryAndInvoice;
using BC.PAYMENT.CORE.DTO.Inventory;
using BC.PAYMENT.CORE.Entities.ClosingInventoryAndInvoice.ClosingInventory;
using BC.PAYMENT.CORE.Entities.Inventory;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BC.PAYMENT.API.Controllers.ClosingInventoryAndInvoice.ClosingInvoice
{
    public class OpeningBalanceController : BaseApiController
    {
   
        private readonly IUnitOfWork _unitOfWork;
        public OpeningBalanceController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        [Route("getitemsinstock/{dbCode}/{location}")]
        public async Task<ApiResponse<List<OpeningBalanceModel>>> LoadItemInStockByBranItemInStockByBranchCodeAsync(string dbCode,string location)
        {
            try
            {
                var execute = await _unitOfWork.OpeningBalance.LoadItemInStockByBranItemInStockByBranchCode(dbCode,location);
                execute.ForEach(x =>
                {
                    if (string.IsNullOrEmpty(x.DbCode)||string.IsNullOrEmpty(x.CreatedBy))
                    {
                        x.DbCode =x.CreatedBy = string.Empty;
                    }
                });
                if (execute.Any())
                {
                    return ApiResponse<List<OpeningBalanceModel>>.Builder()
                        .WithResult(execute)
                        .WithStatusCode(StatusCodes.Status200OK)
                        .WithMessage("Items fetched successfully")
                        .Build();
                }
                else
                {
                    return ApiResponse<List<OpeningBalanceModel>>.Builder()
                        .WithStatusCode(StatusCodes.Status404NotFound)
                        .WithMessage("Items fetched unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<OpeningBalanceModel>>(ex.Message);
            }
        }
        
        [HttpPost]
        [Route("createwarehousepreset")]
        public async Task<ApiResponse<int>> OpenClosingEntryInventoryAsync(List<OpeningBalanceDto> ls)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var model = ls.Select(x => new OpeningBalanceModel
                {
                    DbCode = x.DbCode,
                    ItemCode = x.ItemCode,
                    Location = x.Location,
                    ItemDesc = x.ItemDesc,
                    Physical = x.Physical,
                    CreatedBy = credential.Username

                }).ToList();
                var affectedRows = await _unitOfWork.OpeningBalance.OpenClosingEntryInventoryAsync(model);
                if (affectedRows > 0)
                {
                    return ApiResponse<int>.Builder()
                        .WithResult(affectedRows)
                        .WithStatusCode(StatusCodes.Status200OK)
                        .WithMessage("Operation did successfully")
                        .Build();
                }
                else
                {
                    return ApiResponse<int>.Builder()
                        .WithStatusCode(StatusCodes.Status404NotFound)
                        .WithMessage("Operation did unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
            }
        }
    }
}
