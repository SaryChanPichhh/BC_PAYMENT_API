using System.ComponentModel.DataAnnotations;
using System.Net;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.CommondityExchange.RepairItem;
using BC.PAYMENT.CORE.DTO.General;
using BC.PAYMENT.CORE.DTO.Invoice;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BC.PAYMENT.API.Controllers.CommodityExchange.RepairItem;

public class RepairGoodController(IUnitOfWork unitOfWork) : BaseApiController
{
    #region Send Repair Good To Repairation

    [HttpGet]
    [Route("getrepairgoodbybranches")]
    public async Task<ApiResponse<List<RepairGoodsRespondDto>>> GetReceivedRepairGoodByBranchAsync()
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var result = await unitOfWork.RepairGoods.GetReceivedRepairGoodsByBranchAsync(credential.DbCode);
            if (result != null)
                return ApiResponse<List<RepairGoodsRespondDto>>.Builder()
                    .WithMessage("Received repair goods fetched successfully")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(result)
                    .Build();
            else
                return ApiResponse<List<RepairGoodsRespondDto>>.Builder()
                    .WithMessage("Received repair goods fetched unsuccessfully")
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .WithResult(new List<RepairGoodsRespondDto>())
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<RepairGoodsRespondDto>>(ex.Message);
        }
    }

    [HttpDelete]
    [Route("deletereceivedrepair/{receivedId}/{detailId}")]
    public async Task<ApiResponse<int>> DeleteReceivedRepairGoodAsync([Required] int receivedId,
        [Required] int detailId)
    {
        try
        {
            var result = await unitOfWork.RepairGoods.DeleteReceivedRepairGoodAsync(receivedId, detailId);
            if (result != null)
                return ApiResponse<int>.Builder()
                    .WithMessage("Received repair goods deleted successfully")
                    .WithStatusCode((int)HttpStatusCode.NoContent)
                    .WithResult(result)
                    .Build();
            else
                return ApiResponse<int>.Builder()
                    .WithMessage("Received repair goods deleted unsuccessfully")
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }

    [HttpPost]
    [Route("transferrepairgood")]
    public async Task<ApiResponse<int>> TransferRepairGoodsAsync([FromBody] RepairGoodsDto model)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var receivedModel = new RepairGoodsRespondDto
            {
                Id = model.ReceivedId,
                DbCode = credential.DbCode,
                CustomerCode = model.CustomerCode,
                ItemCode = model.ItemCode,
                Quantity = model.Quantity,
                Description = model.Description,
                CreateBy = credential.Username
            };
            var result = await unitOfWork.RepairGoods.TransferRepairGoodsAsync(receivedModel);
            if (result != null)
                return ApiResponse<int>.Builder()
                    .WithMessage("Received repair goods added successfully")
                    .WithStatusCode((int)HttpStatusCode.NoContent)
                    .WithResult(result)
                    .Build();
            else
                return ApiResponse<int>.Builder()
                    .WithMessage("Received repair goods added unsuccessfully")
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }

    #endregion

    #region Repair Good was received and reparing

    [HttpGet]
    [Route("getreparinggoodsorrepairerreceivedbydate/{fromDate}/{toDate}")]
    public async Task<ApiResponse<List<ItemRepairInprogressDto>>> GetRepairingGoodsByDateAsync(
        [Required] string fromDate, [Required] string toDate)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var result = await unitOfWork.RepairGoods.GetReparingGoodsByDateAsync(credential.DbCode,
                Convert.ToDateTime(fromDate), Convert.ToDateTime(toDate));
            if (result != null)
                return ApiResponse<List<ItemRepairInprogressDto>>.Builder()
                    .WithMessage("Received repair goods fetched successfully")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(result)
                    .Build();
            else
                return ApiResponse<List<ItemRepairInprogressDto>>.Builder()
                    .WithMessage("Received repair goods fetched unsuccessfully")
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .WithResult(new List<ItemRepairInprogressDto>())
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<ItemRepairInprogressDto>>(ex.Message);
        }
    }

    [HttpDelete]
    [Route("deletereparinggoods/{receivedId}/{repairId}")]
    public async Task<ApiResponse<int>> DeleteRepairItemAsync([Required] int receivedId, [Required] int repairId)
    {
        try
        {
            var result = await unitOfWork.RepairGoods.DeleteRepairItem(receivedId, repairId);
            if (result != null)
                return ApiResponse<int>.Builder()
                    .WithMessage("Received repair goods deleted successfully")
                    .WithStatusCode((int)HttpStatusCode.NoContent)
                    .WithResult(result)
                    .Build();
            else
                return ApiResponse<int>.Builder()
                    .WithMessage("Received repair goods deleted unsuccessfully")
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }

    #endregion

    #region RepairGood Payment

    [HttpGet]
    [Route("getsalecode")]
    public async Task<ApiResponse<List<string>>> GetSaleCodeAsync()
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var execute = await unitOfWork.Generators.GetSaleCodeAsync(credential.DbCode);
            if (execute.Any())
                return ApiResponse<List<string>>.Builder()
                    .WithMessage("Sale codes fetched successfully")
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithResult(execute)
                    .Build();
            else
                return ApiResponse<List<string>>.Builder()
                    .WithMessage("Sale codes fetched unsuccessfully")
                    .WithStatusCode(StatusCodes.Status400BadRequest)
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<string>>(ex.Message);
        }
    }

    [HttpGet]
    [Route("getnewtransactionifcost")]
    public async Task<ApiResponse<string>> GetNewTransactionIfCost([Required] string saleType)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var execute = await unitOfWork.Generators.PostSaleOrderAutoNumberAsync(saleType, credential.DbCode);
            if (execute.Any())
                return ApiResponse<string>.Builder()
                    .WithMessage("New transaction fetched successfully")
                    .WithStatusCode(StatusCodes.Status200OK)
                    .WithResult(execute)
                    .Build();
            else
                return ApiResponse<string>.Builder()
                    .WithMessage("New transaction fetched unsuccessfully")
                    .WithStatusCode(StatusCodes.Status400BadRequest)
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<string>(ex.Message);
        }
    }

    [HttpGet]
    [Route("getallcustomerhascompletedrepair")]
    public async Task<ApiResponse<List<CustomerRespondDto>>> GetAllCustomerHasCompletedRepair()
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var result = await unitOfWork.RepairGoods.GetAllCustomerHasCompletedRepair(credential.DbCode);
            var newRespond = result.Select(x => new CustomerRespondDto
            {
                UserCode = x.UserCode,
                CustomerName = x.CustomerName,
                CustomerCode = x.CustomerCode,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Store = x.Store,
                Area = x.Area,
                Market = x.Market
            }).ToList();
            if (result.Any())
                return ApiResponse<List<CustomerRespondDto>>.Builder()
                    .WithMessage("Customer fetched successfully")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(newRespond)
                    .Build();
            else
                return ApiResponse<List<CustomerRespondDto>>.Builder()
                    .WithMessage("Customer fetched unsuccessfully")
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<CustomerRespondDto>>(ex.Message);
        }
    }

    [HttpGet]
    [Route("getallitemhascompletedrepair/{customerCode}")]
    public async Task<ApiResponse<List<ItemRepairCompletedRespondDto>>> GetAllItemHasCompletedRepairByCustomerCode(
        [Required] string customerCode)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var result =
                await unitOfWork.RepairGoods.GetAllItemHasCompletedRepairByCustomerCode(credential.DbCode,
                    customerCode);
            var newRespond = result.Select(x => new ItemRepairCompletedRespondDto
            {
                RepairCompletedId = x.RepairCompletedId,
                Description = x.Description,
                ItemCode = x.ItemCode,
                Quantity = x.Quantity,
                RepairStatus = x.RepairStatus,
                CreatedDate = x.CreatedDate,
                Total = x.Total,
                RequestRepairId = x.RequestRepairId,
                Store = x.Store,
                Area = x.Area,
                Market = x.Market,
                RequestDetailId = x.RequestDetailId,
                Transaction = x.Transaction,
                Note = x.Note
            }).ToList();
            if (result.Any())
                return ApiResponse<List<ItemRepairCompletedRespondDto>>.Builder()
                    .WithMessage("Goods fetched successfully")
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(newRespond)
                    .Build();
            else
                return ApiResponse<List<ItemRepairCompletedRespondDto>>.Builder()
                    .WithMessage("Goods fetched unsuccessfully")
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<ItemRepairCompletedRespondDto>>(ex.Message);
        }
    }

    [HttpPost]
    [Route("switchitemtype")]
    public async Task<ApiResponse<int>> SwitchItemTypeAsync(SwitchItemTypeDto model)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            const string fromType = "REPAIR";
            const string toType = "EXCHANGE";
            var affectedRow = await unitOfWork.RepairGoods.SwitchItemType(credential.DbCode, credential.Username,
                model.RequestDetailId, model.Description, fromType, toType);
            if (affectedRow > 0)
                return ApiResponse<int>.Builder()
                    .WithMessage("Switch type updated successfully")
                    .WithStatusCode((int)HttpStatusCode.NoContent)
                    .WithResult(affectedRow)
                    .Build();
            else
                return ApiResponse<int>.Builder()
                    .WithMessage("Switch type updated unsuccessfully")
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }

    /// <summary>
    // sample param
    /*
     {
        "oldInvoiceIssuance": [
               {
                   "repairCompletedId": 3967,
                   "requestDetailId": 5745,
                   "requestRepairId": 4360,
                   "itemCode": "1200-PINK",
                   "quantity": 1,
                   "description": "ជួសជុលមានបញ្ហាខូចដុំខាងក្នុងម៉ាសុីន",
                   "unitPrice": 0,
                   "itemTransaction": "B16-B16-24050151"
               }
           ],
       "newInvoiceIssuance": {
               "transactionCode": "FF25050493",
               "saleType": "",
               "warehouse": "",
               "description": "",
               "invoiceDate": "2025-05-28"
           },
       "customerWhoRepairGoods": {
           "customerCode": "14-PV-000931",
           "customerName": "ចែ ណាវី",
           "area": "",
           "store": "",
           "market": "",
           "firstName": "កើត",
           "lastName": "វឌ្ឍនៈ",
           "userCode": "286"
       }
   }
    */
    /// 
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("issuancerepairgoodcompletedinvoice")]
    public async Task<ApiResponse<int>> IssuanceRepairGoodCompletedInvoiceAsync([FromBody] IssuanceParamDto param)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var affectedRow = 0;
            var checkifInStock = false;
            var qtyAndEpd = new Dictionary<string, List<string>>();
            var tempQtyAndEpd = new Dictionary<string, List<string>>();
            var itemQuantity = new Dictionary<string, int>();
            foreach (var items in param.OldInvoiceIssuance) qtyAndEpd[items.ItemCode] = new List<string> { "" };
            var total = Convert.ToDouble(param.OldInvoiceIssuance.Sum(x => x.Total));
            if (total is not { } or > 0)
            {
                var saleAnalysisByCustomerCodeAsync =
                    await unitOfWork.Generators.GetSaleAnalysisByCustomerCodeAsync(
                        param.CustomerWhoRepairGoods.CustomerCode, credential.DbCode);

                if (saleAnalysisByCustomerCodeAsync is null)
                    return ApiResponse<int>.Builder()
                        .WithMessage("Sale analysis not found for this customer")
                        .WithStatusCode((int)HttpStatusCode.NotFound)
                        .Build();
                var saleHeaderDto = new SaleHeaderDto
                {
                    RecType = "O",
                    InvoiceDate = param.NewInvoiceIssuance.InvoiceDate.ToString("MM/dd/yyyy"),
                    OrderDate = param.NewInvoiceIssuance.InvoiceDate.ToString("MM/dd/yyyy"),
                    CustomerCode = param.CustomerWhoRepairGoods.CustomerCode,
                    TransactionCode = param.NewInvoiceIssuance.SaleType,
                    Transaction = param.NewInvoiceIssuance.TransactionCode,
                    TransactionValue = total,
                    TransactionDate = param.NewInvoiceIssuance.InvoiceDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    OrderNo = param.NewInvoiceIssuance.TransactionCode,
                    AnalM0 = param.CustomerWhoRepairGoods.UserCode,
                    AnalM1 = "",
                    AnalM2 = "",
                    AnalM3 = saleAnalysisByCustomerCodeAsync.AnalysisC6,
                    AnalM4 = "",
                    AnalM5 = "",
                    AnalM6 = "Exchange Goods",
                    AnalM7 = "",
                    AnalM8 = saleAnalysisByCustomerCodeAsync.AnalysisC8,
                    AnalM9 = saleAnalysisByCustomerCodeAsync.AnalysisC9,
                    Comments = "",
                    InvoicePeriod = credential.Period,
                    UserCode = credential.Username
                };

                var tranLine = 0;
                double quantityExchange = 0;
                var count = 0;
                var checkingIfNotEnough = false;
                // Item Request Exchange
                var allDetailsDtos = new List<SaleDetailsDto>();

                do
                {
                    checkingIfNotEnough = false;
                    var execute = await unitOfWork.Invoices.GetItemExpiredDates(credential.DbCode, qtyAndEpd,
                        param.NewInvoiceIssuance.Warehouse);
                    foreach (var item in execute)
                    {
                        var checkIfOutOfStock = false;
                        var quantityFee = item.Fees;
                        var quantityOrder = param.OldInvoiceIssuance.Where(x => x.ItemCode == item.ItemCodeCopy)
                            .Select(x => x.Quantity).FirstOrDefault();
                        if (checkifInStock == false)
                            itemQuantity[item.ItemCodeCopy] = quantityOrder; // itemQuantity == SHP-94945 = 2
                        if (quantityFee < itemQuantity[item.ItemCodeCopy])
                        {
                            count++;
                            itemQuantity[item.ItemCodeCopy] -= quantityFee;
                            quantityExchange = quantityFee;
                            checkIfOutOfStock = true;
                            checkingIfNotEnough = true;
                            if (count > 1)
                            {
                                if (item.ItemCodeCopy != null)
                                {
                                    var checkExist = tempQtyAndEpd.ContainsKey(item.ItemCodeCopy);
                                    if (!checkExist)
                                        tempQtyAndEpd[item.ItemCodeCopy] = new List<string> { item.LineRef };
                                    else
                                        tempQtyAndEpd[item.ItemCodeCopy].Add(item.LineRef);
                                }
                            }
                            else
                            {
                                tempQtyAndEpd[item.ItemCodeCopy] = new List<string> { item.LineRef };
                            }
                        }

                        if (checkifInStock && checkIfOutOfStock == false)
                            quantityExchange = itemQuantity[item.ItemCodeCopy];
                        var DetailsDtos = param.OldInvoiceIssuance.Select(items => new SaleDetailsDto
                            {
                                TransType = param.NewInvoiceIssuance.SaleType,
                                TransRef = param.NewInvoiceIssuance.TransactionCode,
                                InvoiceDate = param.NewInvoiceIssuance.InvoiceDate.ToString("MM/dd/yyyy"),
                                DelDate = param.NewInvoiceIssuance.InvoiceDate.ToString("MM/dd/yyyy"),
                                DueDate = param.NewInvoiceIssuance.InvoiceDate.ToString("MM/dd/yyyy"),
                                TransLine = $"{tranLine += 1:D3}",
                                ItemCode = items.ItemCode,
                                Description = items.ItemCode,
                                Location = param.NewInvoiceIssuance.Warehouse,
                                InvoiceNo = param.NewInvoiceIssuance.TransactionCode,
                                AccountCode = "",
                                AnalM0 = param.CustomerWhoRepairGoods.UserCode,
                                AnalM1 = "",
                                AnalM2 = "",
                                AnalM3 = saleAnalysisByCustomerCodeAsync.AnalysisC6,
                                AnalM4 = "",
                                AnalM5 = "",
                                AnalM6 = "Exchange Goods",
                                AnalM7 = "",
                                AnalM8 = saleAnalysisByCustomerCodeAsync.AnalysisC8,
                                AnalM9 = saleAnalysisByCustomerCodeAsync.AnalysisC9,
                                Value1 = checkIfOutOfStock ? quantityExchange :
                                    checkifInStock ? quantityExchange : items.Quantity,
                                Value3 = Convert.ToDouble(items.UnitPrice),
                                UpdateStock = "M",
                                LineRef = item.LineRef,
                                OnHold = item.OnHold,
                                Physical = item.Physical,
                                Fees = item.Fees,
                                ItemCodeCopy = item.ItemCodeCopy,
                                OrdPeriod = credential.Period,
                                InvoicePeriod = credential.Period,
                                UserCode = credential.Username,
                                UserInvoice = credential.Username
                            })
                            .ToList();

                        allDetailsDtos.AddRange(DetailsDtos
                            .Where(x => x.ItemCode == item.ItemCodeCopy && x.ItemCode == x.ItemCodeCopy).ToList());
                    }

                    if (checkingIfNotEnough)
                    {
                        checkifInStock = true;
                        qtyAndEpd.Clear();
                        qtyAndEpd = tempQtyAndEpd.ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value.ToList()
                        );
                    }
                } while (checkingIfNotEnough);

                affectedRow +=
                    await unitOfWork.Invoices.CreateInvoiceSaleAsync(credential.DbCode, saleHeaderDto, allDetailsDtos);

                affectedRow += await unitOfWork.Invoices.InsertRecordInvoice(credential.DbCode, credential.Username,
                    param.NewInvoiceIssuance.TransactionCode, param.CustomerWhoRepairGoods.CustomerCode,
                    param.CustomerWhoRepairGoods.CustomerName, total, param.NewInvoiceIssuance.InvoiceDate,
                    credential.InvoiceEntryCode);
                var createInvoiceDetailDtos = param.OldInvoiceIssuance.Select(item => new CreateInvoiceDetailDto
                {
                    Description = item.Description,
                    ItemCode = item.ItemCode,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    RepairCompletedId = item.RepairCompletedId,
                    RequestDetailId = item.RequestDetailId,
                    ItemTransaction = item.ItemTransaction,
                    RequestRepairId = item.RequestRepairId
                }).ToList();
                affectedRow += await CreateRecordFixInvoice(createInvoiceDetailDtos,
                    param.NewInvoiceIssuance.Description, param.NewInvoiceIssuance.TransactionCode,
                    param.NewInvoiceIssuance.InvoiceDate,
                    total, param.CustomerWhoRepairGoods.CustomerCode, credential.DbCode, credential.Username);
                if (affectedRow > 6)
                    return ApiResponse<int>.Builder()
                        .WithMessage("Repair goods paid successfully")
                        .WithStatusCode(StatusCodes.Status204NoContent)
                        .WithResult(affectedRow)
                        .Build();
                else
                    return ApiResponse<int>.Builder()
                        .WithMessage("Repair goods paid unsuccessfully")
                        .WithStatusCode(StatusCodes.Status204NoContent)
                        .Build();

                //await unitOfWork.Invoices.GetInvoiceByCustomerCode(credential.DbCode, param.CustomerWhoRepairGoods.CustomerCode);
            }
            else
            {
                var generateFixInvoice = await unitOfWork.Generators.GenerateFixInvoice(credential.DbCode);
                affectedRow += await unitOfWork.Invoices.InsertRecordInvoice(credential.DbCode, credential.Username,
                    generateFixInvoice, param.CustomerWhoRepairGoods.CustomerCode,
                    param.CustomerWhoRepairGoods.CustomerName, total, param.NewInvoiceIssuance.InvoiceDate,
                    credential.InvoiceEntryCode);
                var createInvoiceDetailDtos = param.OldInvoiceIssuance.Select(item => new CreateInvoiceDetailDto
                {
                    Description = item.Description,
                    ItemCode = item.ItemCode,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    RepairCompletedId = item.RepairCompletedId,
                    RequestDetailId = item.RequestDetailId,
                    ItemTransaction = item.ItemTransaction,
                    RequestRepairId = item.RequestRepairId
                }).ToList();
                affectedRow += await CreateRecordFixInvoice(createInvoiceDetailDtos,
                    param.NewInvoiceIssuance.Description, generateFixInvoice, param.NewInvoiceIssuance.InvoiceDate,
                    0, param.CustomerWhoRepairGoods.CustomerCode, credential.DbCode, credential.Username);

                if (affectedRow > 4)
                    return ApiResponse<int>.Builder()
                        .WithMessage("Repair goods paid successfully")
                        .WithStatusCode(StatusCodes.Status204NoContent)
                        .WithResult(affectedRow)
                        .Build();
                else
                    return ApiResponse<int>.Builder()
                        .WithMessage("Repair goods paid unsuccessfully")
                        .WithStatusCode(StatusCodes.Status400BadRequest)
                        .Build();
            }
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }

    private async Task<int> CreateRecordFixInvoice(List<CreateInvoiceDetailDto> createInvoiceDetailDtos,
        string description,
        string transactionCode, DateTime date, double total, string customerCode, string dbCode, string userName)
    {
        var affectedRow = 0;
        var customer = customerCode;
        const int discount = 0;
        const int taxes = 0;
        var createInvoiceDto = new CreateInvoiceDto
        {
            DbCode = dbCode,
            CreatedBy = userName,
            InvoiceDate = date,
            InvoiceReference = transactionCode,
            CustomerCode = customer,
            Discount = discount,
            Taxes = taxes,
            InvoiceDue = date,
            Description = description,
            TotalAmount = Convert.ToDecimal(total)
        };
        var createInvoiceDetailDto = createInvoiceDetailDtos.Select(printItemRepairReadyDto =>
                new CreateInvoiceDetailDto
                {
                    ItemCode = printItemRepairReadyDto.ItemCode,
                    Description = printItemRepairReadyDto.Description,
                    UnitPrice = printItemRepairReadyDto.UnitPrice,
                    Quantity = printItemRepairReadyDto.Quantity,
                    RepairCompletedId = printItemRepairReadyDto.RepairCompletedId,
                    RequestDetailId = printItemRepairReadyDto.RequestDetailId,
                    ItemTransaction = printItemRepairReadyDto.ItemTransaction,
                    RequestRepairId = printItemRepairReadyDto.RequestRepairId
                })
            .ToList();

        affectedRow = await unitOfWork.Invoices.CreateInvoice(createInvoiceDto, createInvoiceDetailDto); // return 4
        return affectedRow;
    }

    [HttpPut]
    [Route("updateamountrepaircompleted")]
    public async Task<ApiResponse<int>> UpdateAmountRepairCompletedAsync([Required] double totalPrice,
        [Required] int repairCompletedId)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var affectedRow =
                await unitOfWork.RepairGoods.PaidRepairItemAsync(credential.Username, totalPrice, repairCompletedId);
            if (affectedRow > 0)
                return ApiResponse<int>.Builder()
                    .WithMessage("Amount updated successfully")
                    .WithStatusCode((int)HttpStatusCode.NoContent)
                    .WithResult(affectedRow)
                    .Build();
            else
                return ApiResponse<int>.Builder()
                    .WithMessage("Amount updated unsuccessfully")
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }

    #endregion
}