using System.Net;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.Contracts.Items;
using BC.PAYMENT.CORE.Contracts.Response.Item;
using BC.PAYMENT.CORE.DTO.General;
using BC.PAYMENT.CORE.DTO.Transaction.Inventory.VerificationStock;
using BC.PAYMENT.LOGGING;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BC.PAYMENT.API.Controllers.Transaction.Inventory;

public class CheckingStockByBranchController : BaseApiController
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly Dictionary<string, string> Types = new()
    {
        { "T", "Trasfer" }, { "D", "Debit Note" }, { "O", "Opening Balance" }, { "P", "Purchase" }, { "S", "Sale" },
        { "M", "Transaction" }, { "C", "Credit Note" }
    };

    public CheckingStockByBranchController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    [Route("gettypes")]
    public async Task<ApiResponse<List<string>>> GetTypes()
    {
        return ApiResponse<List<string>>.Builder()
            .WithResult(Types.Values.ToList())
            .WithMessage("Types fetched successfully")
            .WithStatusCode((int)HttpStatusCode.OK)
            .WithSuccess(true)
            .Build();
    }

    [HttpGet]
    [Route("getwarehouse")]
    public async Task<ApiResponse<List<WarehouseDto>>> GetWarehouseAsync()
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var execute = await _unitOfWork.Warehouses.GetWarehouseAsync(credential.DbCode!);
            if (execute.Any())
                return ApiResponse<List<WarehouseDto>>.Builder()
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithMessage("Warehouse fetched successfully")
                    .WithResult(execute)
                    .WithSuccess(true)
                    .Build();
            else
                return ApiResponse<List<WarehouseDto>>.Builder()
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .WithMessage("Warehouse fetched unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<WarehouseDto>>(ex.Message);
        }
    }

    [HttpGet]
    [Route("getitems")]
    public async Task<ApiResponse<List<ItemResponse>>> GetItemListAsync()
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var execute = await _unitOfWork.Items.GetItemListAsync(credential.DbCode);
            if (execute.Any())
                return ApiResponse<List<ItemResponse>>.Builder()
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithMessage("Items fetched successfully")
                    .WithResult(execute)
                    .WithSuccess(true)
                    .Build();
            else
                return ApiResponse<List<ItemResponse>>.Builder()
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .WithMessage("Items fetched unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<ItemResponse>>(ex.Message);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="model">
    //{
    //"period": 202501,
    //"location": "1-LOC-OFFICE",
    //"recType": "Sale",
    //"fromDate": "",
    //"toDate": "",
    //"fromItemCode": "0001",
    //"toItemCode": "301",
    //"byAccountPeriod": true
    // }
    /// </param>
    /// <returns></returns>
    [HttpPost]
    [Route("checkingStockItemByBranch")]
    public async Task<ApiResponse<List<CheckingStockByBranch>>> GetItemListAsync(
        [FromBody] CheckingStockByBranchDto model)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            if (Types.ContainsValue(model.RecType))
                model.RecType = Types.Where(x => x.Value == model.RecType).Select(x => x.Key).FirstOrDefault();
            var execute = await _unitOfWork.VerificationStock.GetCheckingStockByBranchAsync(model);
            if (execute != null)
                return ApiResponse<List<CheckingStockByBranch>>.Builder()
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithMessage("Items fetched successfully")
                    .WithResult(execute)
                    .WithSuccess(true)
                    .Build();
            else
                return ApiResponse<List<CheckingStockByBranch>>.Builder()
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .WithMessage("Items fetched unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<CheckingStockByBranch>>(ex.Message);
        }
    }
}