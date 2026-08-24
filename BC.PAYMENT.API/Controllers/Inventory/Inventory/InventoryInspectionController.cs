using System.ComponentModel.DataAnnotations;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.General;
using BC.PAYMENT.CORE.DTO.Inventory;
using BC.PAYMENT.CORE.Entities.General;
using BC.PAYMENT.CORE.Entities.Inventory.Inventory;
using BC.PAYMENT.CORE.Entities.Preset.ItemTransaction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BC.PAYMENT.API.Controllers.Inventory.Inventory
{
    public class InventoryInspectionController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        public InventoryInspectionController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        [Route("getinventoryallbranches")]
        public async Task<ApiResponse<List<InventoryModel>>> GetInventoryAllBranchesAsync()
        {
            try
            {
                var data = await _unitOfWork.Inventory.GetInventoryAllBranchesAsync();
                var execute =  data.OrderBy(x=>x.DbCode).ToList();
                if (execute.Any())
                {
                    return ApiResponse<List<InventoryModel>>.Builder()
                        .WithResult(execute)
                        .WithStatusCode(StatusCodes.Status200OK)
                        .WithMessage("Inventory fetched successfully")
                        .Build();
                }
                else
                {
                    return ApiResponse<List<InventoryModel>>.Builder()
                        .WithStatusCode(StatusCodes.Status200OK)
                        .WithMessage("Inventory fetched unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<InventoryModel>>(ex.Message);
            }
        }
        
        [HttpGet]
        [Route("getinventorybymultiwarehouse")]
        public async Task<ApiResponse<List<dynamic>>> GetInventoryByMultiBranchesAsync([Required][FromBody] List<InventoryDto> data)
        {
            try
            {
                var dataInDict = data.ToDictionary(x => x.DbCode, x => x.Warehouses);
                var results = await _unitOfWork.Inventory.GetInventoryByMultiWarehouseAsync(dataInDict);
               

                var execute = results.OrderBy(x=>x.Location).ToList();    
                if (execute.Any())
                {
                    return ApiResponse<List<dynamic>>.Builder()
                        .WithResult(execute)
                        .WithStatusCode(StatusCodes.Status200OK)
                        .WithMessage("Inventory fetched successfully")
                        .Build();
                }
                else
                {
                    return ApiResponse<List<dynamic>>.Builder()
                        .WithStatusCode(StatusCodes.Status200OK)
                        .WithMessage("Inventory fetched unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<dynamic>>(ex.Message);
            }
        }
        [HttpGet]
        [Route("getinventorybybranches")]
        public async Task<ApiResponse<List<dynamic>>> GetInventoryByBranchesAsync()
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                if (credential.DbCode is not string)
                {
                    return ApiResponse<List<dynamic>>.Builder()
                        .WithStatusCode(StatusCodes.Status500InternalServerError)
                        .WithMessage("DbCode must be string!!!!")
                        .Build();
                }
                var getWarehousesByBranches =await _unitOfWork.Warehouses.GetWarehouseAsync(credential.DbCode);
                var dataInDict = new Dictionary<string, List<string>>{
                    { credential.DbCode, getWarehousesByBranches.Select(x => x.WarehouseCode).ToList() }};
                var results = await _unitOfWork.Inventory.GetInventoryByMultiWarehouseAsync(dataInDict);
                
                var execute = results.OrderBy(x=>x.Location).ToList();    
                if (execute.Any())
                {
                    return ApiResponse<List<dynamic>>.Builder()
                        .WithResult(execute)
                        .WithStatusCode(StatusCodes.Status200OK)
                        .WithMessage("Inventory fetched successfully")
                        .Build();
                }
                else
                {
                    return ApiResponse<List<dynamic>>.Builder()
                        .WithStatusCode(StatusCodes.Status200OK)
                        .WithMessage("Inventory fetched unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<dynamic>>(ex.Message);
            }
        }
        
        [HttpGet]
        [Route("getproductsdetail")]
        public async Task<ApiResponse<List<ProductModel>>> GetProductDetailsAsync()
        {
            var credential = Common.DecodeJwt(User);
            const string IMAGE_PATH = @"D:\BC Payment\Photos\BC PHOTOS";
            try
            {
                if (credential.DbCode is not string)
                {
                    return ApiResponse<List<ProductModel>>.Builder()
                        .WithStatusCode(StatusCodes.Status500InternalServerError)
                        .WithMessage("DbCode must be string!!!!")
                        .Build();
                }
                var results =await _unitOfWork.Products.GetAllProductsAsync(credential.DbCode);
                results.ForEach(x =>
                {
                    x.ImagePath = $@"{IMAGE_PATH}\{x.ItemCode}.jpg";
                });
                if (results.Any())
                {
                    return ApiResponse<List<ProductModel>>.Builder()
                        .WithResult(results.OrderBy(x => x.ItemCode).ToList())
                        .WithStatusCode(StatusCodes.Status200OK)
                        .WithMessage("Products fetched successfully")
                        .Build();
                }
                else
                {
                    return ApiResponse<List<ProductModel>>.Builder()
                        .WithStatusCode(StatusCodes.Status200OK)
                        .WithMessage("Products fetched unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<ProductModel>>(ex.Message);
            }
        }
    }
}
