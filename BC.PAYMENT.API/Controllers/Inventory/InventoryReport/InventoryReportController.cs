using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.Filter;
using BC.PAYMENT.CORE.DTO.Inventory;
using BC.PAYMENT.CORE.Entities.General;
using BC.PAYMENT.CORE.Entities.Inventory.InventoryReport;
using BC.PAYMENT.CORE.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BC.PAYMENT.API.Controllers.Inventory.InventoryReport;

[Route("api/[controller]")]
[ApiController]
public class InventoryReportController : BaseApiController
{
    private readonly IUnitOfWork _unitOfWork;

    public InventoryReportController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }


    [HttpGet]
    [Route("getproductssalebydate")]
    public async Task<ApiResponse<List<InventoryReportModel>>> GetProductsSaleByDateAsync(
        [FromBody] InventoryReportRequestDto.RequestByDateDto model)
    {
        var credential = Common.DecodeJwt(User);

        try
        {
            var response = await _unitOfWork.InventoryReport.GetAllStatusProductsSaleByDateAsync(model);
            if (response.Any())
                return ApiResponse<List<InventoryReportModel>>.Builder()
                    .WithResult(response)
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithMessage("Daily Sale Report fetched successfully")
                    .Build();
            else
                return ApiResponse<List<InventoryReportModel>>.Builder()
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithMessage("Daily Sale Report fetched unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<InventoryReportModel>>(ex.Message);
        }
    }

    [HttpGet]
    [Route("getproductssalebyperiod")]
    public async Task<ApiResponse<List<InventoryReportModel>>> GetProductsSaleByPeriodAsync(
        [FromBody] InventoryReportRequestDto.RequestByPeriodDto model)
    {
        var credential = Common.DecodeJwt(User);

        try
        {
            var response = await _unitOfWork.InventoryReport.GetAllStatusProductsSaleByPeriodAsync(model);
            if (response.Any())
                return ApiResponse<List<InventoryReportModel>>.Builder()
                    .WithResult(response)
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithMessage("Daily Sale Report fetched successfully")
                    .Build();
            else
                return ApiResponse<List<InventoryReportModel>>.Builder()
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithMessage("Daily Sale Report fetched unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<InventoryReportModel>>(ex.Message);
        }
    }

    [HttpGet]
    [Route("getproductssalebydatewithoutsalefix")]
    public async Task<ApiResponse<List<InventoryReportModel>>> GetProductsSaleByDateWithOutSaleFixAsync(
        [FromBody] InventoryReportRequestDto.RequestByDateDto model)
    {
        var credential = Common.DecodeJwt(User);

        try
        {
            var response = await _unitOfWork.InventoryReport.GetAllStatusProductsSaleByDateWithOutSaleFixAsync(model);
            if (response.Any())
                return ApiResponse<List<InventoryReportModel>>.Builder()
                    .WithResult(response)
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithMessage("Daily Sale Report fetched successfully")
                    .Build();
            else
                return ApiResponse<List<InventoryReportModel>>.Builder()
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithMessage("Daily Sale Report fetched unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<InventoryReportModel>>(ex.Message);
        }
    }

    [HttpGet]
    [Route("getproductssalebyperiodwithoutsalefix")]
    public async Task<ApiResponse<List<InventoryReportModel>>> GetProductsSaleByPeriodWithOutSaleFixAsync(
        [FromBody] InventoryReportRequestDto.RequestByPeriodDto model)
    {
        var credential = Common.DecodeJwt(User);

        try
        {
            var response = await _unitOfWork.InventoryReport.GetAllStatusProductsSaleByPeriodWithOutSaleFixAsync(model);
            if (response.Any())
                return ApiResponse<List<InventoryReportModel>>.Builder()
                    .WithResult(response)
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithMessage("Daily Sale Report WithOut SaleFix fetched successfully")
                    .Build();
            else
                return ApiResponse<List<InventoryReportModel>>.Builder()
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithMessage("Daily Sale Report WithOut SaleFix fetched unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<InventoryReportModel>>(ex.Message);
        }
    }

    [HttpPost]
    [Route("addwarehousedata")]
    public async Task<ApiResponse<int>> AddWarehouseData([FromBody] List<InventoryTrackingWarehouseDto> model)
    {
        var credential = Common.DecodeJwt(User);

        try
        {
            var data = model.Select(x => new InventoryTrackingWarehouseDataModel
            {
                DbCode = x.DbCode,
                Warehouse = x.Warehouse,
                ToWarehouse = x.ToWarehouse,
                ItemCode = x.ItemCode,
                Quantity = x.Quantity,
                CreatedBy = credential.Username,
                InventoryTrackingTypes = InventoryTrackingTypes.Subtract,
                ItemDescription = "",
                TransactionDate = DateTime.Now,
                CreatedDate = DateTime.Now
            }).ToList();
            var affectedRow = await _unitOfWork.InventoryReport.AddWarehouseData(data);
            if (affectedRow > 0)
                return ApiResponse<int>.Builder()
                    .WithResult(affectedRow)
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithMessage("Warehouse added successfully")
                    .Build();
            else
                return ApiResponse<int>.Builder()
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithMessage("Warehouse added unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }

    #region InventoryReceive

    [HttpGet]
    [Route("getinventoryreceive")]
    public async Task<ApiResponse<List<InventoryReceiveModel>>> GetInventoryReceiveAsync(DatePagedRequestDto dto)
    {
        var credential = Common.DecodeJwt(User);

        try
        {
            var execute = await _unitOfWork.InventoryReport.GetInventoryReceiveAsync(Convert.ToDateTime(dto.FromDate),
                Convert.ToDateTime(dto.ToDate));
            if (execute.Any())
                return ApiResponse<List<InventoryReceiveModel>>.Builder()
                    .WithResult(execute)
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithMessage("Inventory added successfully")
                    .Build();
            else
                return ApiResponse<List<InventoryReceiveModel>>.Builder()
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithMessage("Inventory added unsuccessfully")
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<InventoryReceiveModel>>(ex.Message);
        }
    }

    #endregion
}