using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.CommondityExchange.ExchangeItem;
using BC.PAYMENT.CORE.DTO.General;
using BC.PAYMENT.CORE.DTO.Invoice;
using BC.PAYMENT.CORE.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Logging;

namespace BC.PAYMENT.API.Controllers.CommodityExchange.ExchangeItem
{
    public class ExchangeItemController(IUnitOfWork unitOfWork) : BaseApiController
    {

        [HttpGet]
        [Route("getallcustomerhasexchangegoods")]
        public async Task<ApiResponse<List<CustomerDto>>> GetAllCustomerHasExchangeGoodsAsync()
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var execute = await unitOfWork.ExchangeItem.GetAllCustomerHasExchangeGoods(credential.DbCode);
                if (execute.Any())
                {
                    return ApiResponse<List<CustomerDto>>.Builder()
                        .WithResult(execute)
                        .WithMessage("Customer fetched successfully")
                        .WithStatusCode(StatusCodes.Status200OK)
                        .Build();
                }
                else
                {
                    return ApiResponse<List<CustomerDto>>.Builder()
                        .WithMessage("Customer fetched unsuccessfully")
                        .WithStatusCode(StatusCodes.Status400BadRequest)
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<CustomerDto>>(ex.Message);
            }
        }
        [HttpGet]
        [Route("getallitemexchangebycustomercode/{masterId}")]
        public async Task<ApiResponse<List<ItemExchangeDto>>> GetAllItemExchangeByCustomerCodeAsync([Required] int masterId)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var execute = await unitOfWork.ExchangeItem.GetAllItemExchangeByCustomerCodeAsync(credential.DbCode, masterId);
                if (execute.Any())
                {
                    return ApiResponse<List<ItemExchangeDto>>.Builder()
                        .WithResult(execute)
                        .WithMessage("Item fetched successfully")
                        .WithStatusCode(StatusCodes.Status200OK)
                        .Build();
                }
                else
                {
                    return ApiResponse<List<ItemExchangeDto>>.Builder()
                        .WithMessage("Item fetched unsuccessfully")
                        .WithStatusCode(StatusCodes.Status400BadRequest)
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<ItemExchangeDto>>(ex.Message);
            }
        }
        
        [HttpGet]
        [Route("getallitemcreditbycustomercode/{masterId}")]
        public async Task<ApiResponse<List<ItemExchangeDto>>> GetAllItemCreditByCustomerCodeAsync([Required] int masterId)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var execute = await unitOfWork.ExchangeItem.GetAllItemCreditByCustomerCodeAsync(credential.DbCode, masterId);
                if (execute.Any())
                {
                    return ApiResponse<List<ItemExchangeDto>>.Builder()
                        .WithResult(execute)
                        .WithMessage("Item fetched successfully")
                        .WithStatusCode(StatusCodes.Status200OK)
                        .Build();
                }
                else
                {
                    return ApiResponse<List<ItemExchangeDto>>.Builder()
                        .WithMessage("Item fetched unsuccessfully")
                        .WithStatusCode(StatusCodes.Status400BadRequest)
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<ItemExchangeDto>>(ex.Message);
            }
        }
        
        [HttpGet]
        [Route("getinvoiceinsixthmonths/{customerCode}/{itemCode}")]
        public async Task<ApiResponse<List<InvoiceForExchangeDto>>> GetInvoiceForExchangeByCustomerCodeAndItemCodeIn6MonthsAsync([Required] string customerCode,[Required]string itemCode)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var execute = await unitOfWork.ExchangeItem.GetInvoiceForExchangeByCustomerCodeAndItemCodeIn6MonthsAsync(credential.DbCode, customerCode, itemCode);
                if (execute.Any())
                {
                    return ApiResponse<List<InvoiceForExchangeDto>>.Builder()
                        .WithResult(execute)
                        .WithMessage("Item fetched successfully")
                        .WithStatusCode(StatusCodes.Status200OK)
                        .Build();
                }
                else
                {
                    return ApiResponse<List<InvoiceForExchangeDto>>.Builder()
                        .WithMessage("Item fetched unsuccessfully")
                        .WithStatusCode(StatusCodes.Status400BadRequest)
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<InvoiceForExchangeDto>>(ex.Message);
            }
        }
        
        [HttpGet]
        [Route("getinvoicebycustomercodeanditemcode/{customerCode}/{itemCode}")]
        public async Task<ApiResponse<List<InvoiceForExchangeDto>>> GetAllItemCreditByCustomerCodeAsync([Required] string customerCode,string itemCode)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var execute = await unitOfWork.ExchangeItem.GetInvoiceForExchangeByCustomerCodeAndItemCodeAsync(credential.DbCode, customerCode, itemCode);
                if (execute.Any())
                {
                    return ApiResponse<List<InvoiceForExchangeDto>>.Builder()
                        .WithResult(execute)
                        .WithMessage("Item fetched successfully")
                        .WithStatusCode(StatusCodes.Status200OK)
                        .Build();
                }
                else
                {
                    return ApiResponse<List<InvoiceForExchangeDto>>.Builder()
                        .WithMessage("Item fetched unsuccessfully")
                        .WithStatusCode(StatusCodes.Status400BadRequest)
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<InvoiceForExchangeDto>>(ex.Message);
            }
        }

        #region Sample Input
        //{
        //    "receivedId": "5298", from gridCustomer
        //    "oldTransaction": "BC22040182",
        //    "itemCode": "9935",
        //    "oldTranLine": "004",
        //    "quantity": 1,
        //    "customerCode": "14-PP-000758",
        //    "total": 26
        //}
        #endregion
    [HttpPost]
        [Route("createcreditnote")]
        public async Task<ApiResponse<CreditNoteItemDto>> CreateCreditNoteAsync([FromBody] CreditNoteItemDto model)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                CreditNoteItemModel creditNote = new()
                {
                    ItemCode = model.ItemCode,
                    Quantity = model.Quantity,
                    UserName = credential.Username,
                    DbCode = credential.DbCode,
                    OldTransaction = model.OldTransaction,
                    Total = model.Total,
                    CustomerCode = model.CustomerCode,
                    Period = credential.Period,
                    ReceivedId = model.ReceivedId,
                    OldTranLine = model.OldTranLine,
                };
                var newTransaction = await unitOfWork.Generators.PostCreditNoteAutoNumberAsync(credential.DbCode,"SALE-EXCH");
                creditNote.NewTransaction = newTransaction;
                var affectedRow = await unitOfWork.ExchangeItem.AddNewCreditNote(creditNote);
                if (affectedRow > 0 )
                {
                    return ApiResponse<CreditNoteItemDto>.Builder()
                        .WithResult(model)
                        .WithMessage("Credit Note added successfully")
                        .WithStatusCode(StatusCodes.Status204NoContent)
                        .Build();
                }
                else
                {
                    return ApiResponse<CreditNoteItemDto>.Builder()
                        .WithResult(model)
                        .WithMessage("Credit Note added unsuccessfully")
                        .WithStatusCode(StatusCodes.Status204NoContent)
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<CreditNoteItemDto>(ex.Message);
            }
        }
        
        [HttpPost]
        [Route("submitexchangeinvoices")]

        public async Task<ApiResponse<int>> SubmitExchangeInvoice([FromBody] ExchangeItemParamsDto model)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                bool checkifOutofStock = false;
                bool checkifInStock = false;
                try
                {
                    var totalExchangeItems = model.OutBoundItems.Sum(x => x.Total);
                    var totalRequestItems = model.InBoundItems.Sum(x => x.Total);
                    double total;
                    if (totalExchangeItems < totalRequestItems)
                        total = 0;
                    else
                        total = totalExchangeItems - totalRequestItems;

                    var saleAnalysisByCustomerCodeAsync =
                        await unitOfWork.Generators.GetSaleAnalysisByCustomerCodeAsync(model.Customer.CustomerCode,credential.DbCode);
                    var newTransaction = model.Invoices.TransactionCode;

                    var saleHeaderDto = new SaleHeaderDto
                    {
                        RecType = "O",
                        TransactionDate = model.Invoices.InvoiceDate.ToString("yyyy-MM-dd HH:mm:ss"),
                        InvoiceDate = model.Invoices.InvoiceDate.ToString("MM/dd/yyyy"),
                        OrderDate = model.Invoices.InvoiceDate.ToString("MM/dd/yyyy"),
                        CustomerCode = model.Customer.CustomerCode,
                        TransactionCode = model.Invoices.SaleType,
                        Transaction = newTransaction,
                        TransactionValue = totalExchangeItems,
                        OrderNo = newTransaction,
                        Status = "00",
                        AnalM0 = model.Customer.UserCode,
                        AnalM1 = "",
                        AnalM2 = "",
                        AnalM3 = saleAnalysisByCustomerCodeAsync.AnalysisC6,
                        AnalM4 = "",
                        AnalM5 = "",
                        AnalM6 = "Exchange Goods",
                        AnalM7 = "",
                        AnalM8 = saleAnalysisByCustomerCodeAsync.AnalysisC8,
                        AnalM9 = saleAnalysisByCustomerCodeAsync.AnalysisC9,
                        Comments = ""
                    };

                    var qtyAndEpd = new Dictionary<string, List<string>>();
                    var tempQtyAndEpd = new Dictionary<string, List<string>>();
                    var itemQuantity = new Dictionary<string, int>();
                    foreach (var item in model.OutBoundItems)
                    {
                        qtyAndEpd[item.ItemCode]= new List<string> { "" };
                        var exist = await unitOfWork.Invoices.CheckStockQuantityAsync(credential.DbCode,model.Invoices.Warehouse, item.ItemCode, item.Quantity);
                        if (!exist)
                        {
                            return ApiResponse<int>.Builder()
                                .WithResult(0)
                                .WithMessage($@"Stock of {item.ItemCode} is not available")
                                .WithStatusCode(StatusCodes.Status400BadRequest)
                                .Build();
                        }
                        return ApiResponse<int>.Builder().Build();
                    }
                    var tranLine = 0;
                    double quantityExchange = 0;
                    int count = 0;
                    int quantityOrder = 0;
                    int quantityFee = 0;
                    bool checkingIfNotEnought = false;

                    var allDetailsDtos = new List<SaleDetailsDto>();
                    do
                    {
                        checkingIfNotEnought = false;
                        var execute = await unitOfWork.Invoices.GetItemExpiredDates(credential.DbCode, qtyAndEpd, model.Invoices.Warehouse);
                        foreach (var item in execute)
                        {
                            checkifOutofStock = false;
                            quantityFee = item.Fees;
                            quantityOrder = model.OutBoundItems.Where(x => x.ItemCode == item.ItemCodeCopy)
                                .Select(x => x.Quantity).FirstOrDefault();
                            if (checkifInStock == false)
                            {
                                itemQuantity[item.ItemCodeCopy] = quantityOrder; // itemQuantity == SHP-94945 = 2
                            }

                            if (quantityFee < itemQuantity[item.ItemCodeCopy])
                            {
                                count++;
                                itemQuantity[item.ItemCodeCopy] -= quantityFee;
                                quantityExchange = quantityFee;
                                checkifOutofStock = true;
                                checkingIfNotEnought = true;
                                if (count > 1)
                                {
                                    if (item.ItemCodeCopy != null)
                                    {
                                        var checkExist = tempQtyAndEpd.ContainsKey(item.ItemCodeCopy);
                                        if (!checkExist)
                                        {
                                            tempQtyAndEpd[item.ItemCodeCopy] = new List<string> { item.LineRef };
                                        }
                                        else
                                        {
                                            tempQtyAndEpd[item.ItemCodeCopy].Add(item.LineRef);
                                        }
                                    }
                                }
                                else
                                {
                                    tempQtyAndEpd[item.ItemCodeCopy] = new List<string> { item.LineRef };
                                }
                            }

                            if (checkifInStock && checkifOutofStock == false)
                            {
                                quantityExchange = itemQuantity[item.ItemCodeCopy];
                            }

                            var DetailsDtos = model.OutBoundItems.Select(x=> new SaleDetailsDto
                            {
                                TransType = model.Invoices.SaleType,
                                TransRef = newTransaction,
                                TransLine = $"{tranLine += 1:D3}",
                                ItemCode = item.ItemCode,
                                Description = x.ItemCode,
                                Location = x.ItemName,
                                InvoiceNo = newTransaction,
                                Status = "05",
                                AccountCode = "",
                                AnalM0 = model.Customer.UserCode,
                                AnalM1 = "",
                                AnalM2 = "",
                                AnalM3 = saleAnalysisByCustomerCodeAsync.AnalysisC6,
                                AnalM4 = "",
                                AnalM5 = "",
                                AnalM6 = "Exchange Goods",
                                AnalM7 = "",
                                CreditStatus = "",
                                DelDate = model.Invoices.InvoiceDate.ToString("MM/dd/yyyy"),
                                InvoiceDate = model.Invoices.InvoiceDate.ToString("MM/dd/yyyy"),
                                DueDate = model.Invoices.InvoiceDate.ToString("MM/dd/yyyy"),
                                AnalM8 = saleAnalysisByCustomerCodeAsync.AnalysisC8,
                                AnalM9 = saleAnalysisByCustomerCodeAsync.AnalysisC9,
                                Value1 = (checkifOutofStock) ? quantityExchange : (checkifInStock) ? quantityExchange : x.Quantity,
                                Value3 = x.UnitPrice,
                                UpdateStock = "S",
                                LineRef = item.LineRef,
                                OnHold = item.OnHold,
                                Physical = item.Physical,
                                Fees = item.Fees,
                                ItemCodeCopy = item.ItemCodeCopy,
                            }).ToList();

                            allDetailsDtos.AddRange(DetailsDtos
                            .Where(x => x.ItemCode == item.ItemCodeCopy && x.ItemCode == x.ItemCodeCopy).ToList());
                        }
                        if (checkingIfNotEnought)
                        {
                            checkifInStock = true;
                            qtyAndEpd.Clear();
                            qtyAndEpd = tempQtyAndEpd.ToDictionary(
                                         kvp => kvp.Key,
                                         kvp => kvp.Value.ToList()
                                        );
                        }
                    } while (checkingIfNotEnought);
                    await unitOfWork.Invoices.InsertRecordInvoice(credential.DbCode,credential.Username,newTransaction, model.Customer.CustomerCode,
                        model.Customer.CustomerName, total, model.Invoices.InvoiceDate, credential.InvoiceEntryCode);
                    await unitOfWork.Invoices.CreateInvoiceSaleAsync(credential.DbCode,saleHeaderDto, allDetailsDtos);
                    var result = await unitOfWork.ExchangeItem.CreateInvoice(credential.DbCode,credential.Username,
                        model.Customer.MasterId,
                        model.Customer.CustomerCode,
                        newTransaction,
                        total,
                        model.OutBoundItems,
                        model.InBoundItems.Select(x=>x.ReceivedId).ToList());
                    if (string.IsNullOrEmpty(result)) return ApiResponse<int>.Builder().Build();
                    foreach (var itemExchangeDto in model.InBoundItems)
                    {
                        await unitOfWork.Invoices.UpdateStatusExchangeReceivedToCredit(itemExchangeDto.ReceivedId, 2);
                        await unitOfWork.Invoices.UpdateStatusRequestExchangeDetails(
                            itemExchangeDto.ReceivedId,
                            ExchangeStatus.Completed);
                        unitOfWork.Invoices.SaveRecordItemExchanged(credential.DbCode,credential.Username,newTransaction, itemExchangeDto.ItemCode,
                        itemExchangeDto.Quantity, itemExchangeDto.UnitPrice);
                    }
                    var isAllItemRequestCompletedByRequestIdAsync =
                        await unitOfWork.Invoices.IsAllItemRequestCompletedByRequestIdAsync(model.Customer.MasterId);
                    await unitOfWork.Invoices.IsAllItemRequestCompletedByRequestIdAsync(model.Customer.MasterId);
                    if (!isAllItemRequestCompletedByRequestIdAsync)
                        await unitOfWork.Invoices.UpdateReceivedToCompletedByIdAsync(credential.DbCode,model.Customer.MasterId);
                }
                finally
                {
                    checkifInStock = false;
                }

                var affectedRow = 0;
                if (affectedRow > 0 )
                {
                    return ApiResponse<int>.Builder()
                        .WithResult(affectedRow)
                        .WithMessage("Credit Note added successfully")
                        .WithStatusCode(StatusCodes.Status204NoContent)
                        .Build();
                }
                else
                {
                    return ApiResponse<int>.Builder()
                        .WithResult(affectedRow)
                        .WithMessage("Credit Note added unsuccessfully")
                        .WithStatusCode(StatusCodes.Status204NoContent)
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
