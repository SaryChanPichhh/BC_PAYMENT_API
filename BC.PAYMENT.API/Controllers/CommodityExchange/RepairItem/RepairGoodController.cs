using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.CommondityExchange.RepairItem;
using BC.PAYMENT.CORE.DTO.General;
using BC.PAYMENT.CORE.DTO.Invoice;
using BC.PAYMENT.CORE.DTO.Items;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BC.PAYMENT.API.Controllers.CommodityExchange.RepairItem
{
    public class RepairGoodController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public RepairGoodController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region Send Repair Good To Repairation

        [HttpGet]
        [Route("getrepairgoodbybranches")]
        public async Task<ApiResponse<List<RepairGoodsRespondDto>>> GetReceivedRepairGoodByBranchAsync()
        {
            var credential = Common.DecodeJwt(User);
            var response = new ApiResponse<List<RepairGoodsRespondDto>>();
            try
            {
                var result = await _unitOfWork.RepairGoods.GetReceivedRepairGoodsByBranchAsync(credential.DbCode);
                if (result != null)
                {
                    response.Result = result;
                    response.Message = "Received repair goods fetched successfully";
                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.Success = true;
                }
                else
                {
                    response.Result = new List<RepairGoodsRespondDto>();
                    response.Message = "Received repair goods fetched unsuccessfully";
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                }
            }
            catch (SqlException ex)
            {
                response.Message = $"Sql Exception : {ex.Message}";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            catch (Exception ex)
            {
                response.Message = $"Error Exception : {ex.Message}";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            return response;
        }
        [HttpDelete]
        [Route("deletereceivedrepair/{receivedId}/{detailId}")]
        public async Task<ApiResponse<int>> GetReceivedRepairGoodByBranchAsync([Required] int receivedId, [Required] int detailId)
        {
            var response = new ApiResponse<int>();
            try
            {
                var result = await _unitOfWork.RepairGoods.DeleteReceivedRepairGoodAsync(receivedId, detailId);
                if (result != null)
                {
                    response.Result = result;
                    response.Message = "Received repair goods deleted successfully";
                    response.StatusCode = (int)HttpStatusCode.NoContent;
                    response.Success = true;
                }
                else
                {
                    response.Message = "Received repair goods deleted unsuccessfully";
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                }
            }
            catch (SqlException ex)
            {
                response.Message = $"Sql Exception : {ex.Message}";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            catch (Exception ex)
            {
                response.Message = $"Error Exception : {ex.Message}";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            return response;
        }
        
        [HttpPost]
        [Route("transferrepairgood")]
        public async Task<ApiResponse<int>> TransferRepairGoodsAsync([FromBody]RepairGoodsDto model)
        {
            var credential = Common.DecodeJwt(User);
            var response = new ApiResponse<int>();
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
                    CreateBy = credential.Username,
                };
                var result = await _unitOfWork.RepairGoods.TransferRepairGoodsAsync(receivedModel);
                if (result != null)
                {
                    response.Result = result;
                    response.Message = "Received repair goods added successfully";
                    response.StatusCode = (int)HttpStatusCode.NoContent;
                    response.Success = true;
                }
                else
                {
                    response.Message = "Received repair goods added unsuccessfully";
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                }
            }
            catch (SqlException ex)
            {
                response.Message = $"Sql Exception : {ex.Message}";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            catch (Exception ex)
            {
                response.Message = $"Error Exception : {ex.Message}";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            return response;
        }

        #endregion

        #region Repair Good was received and reparing

        [HttpGet]
        [Route("getreparinggoodsorrepairerreceivedbydate/{fromDate}/{toDate}")]
        public async Task<ApiResponse<List<ItemRepairInprogressDto>>> GetRepairingGoodsByDateAsync([Required] string fromDate, [Required] string toDate)
        {
            var credential = Common.DecodeJwt(User);
            var response = new ApiResponse<List<ItemRepairInprogressDto>>();
            try
            {
                var result = await _unitOfWork.RepairGoods.GetReparingGoodsByDateAsync(credential.DbCode, Convert.ToDateTime(fromDate), Convert.ToDateTime(toDate));
                if (result != null)
                {
                    response.Result = result;
                    response.Message = "Received repair goods fetched successfully";
                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.Success = true;
                }
                else
                {
                    response.Result = new List<ItemRepairInprogressDto>();
                    response.Message = "Received repair goods fetched unsuccessfully";
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                }
            }
            catch (SqlException ex)
            {
                response.Message = $"Sql Exception : {ex.Message}";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            catch (Exception ex)
            {
                response.Message = $"Error Exception : {ex.Message}";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            return response;
        }
        
        [HttpDelete]
        [Route("deletereparinggoods/{receivedId}/{repairId}")]
        public async Task<ApiResponse<int>> DeleteRepairItemAsync([Required] int receivedId, [Required] int repairId)
        {
            var credential = Common.DecodeJwt(User);
            var response = new ApiResponse<int>();
            try
            {
                var result = await _unitOfWork.RepairGoods.DeleteRepairItem( receivedId, repairId);
                if (result != null)
                {
                    response.Result = result;
                    response.Message = "Received repair goods deleted successfully";
                    response.StatusCode = (int)HttpStatusCode.NoContent;
                    response.Success = true;
                }
                else
                {
                    response.Message = "Received repair goods deleted unsuccessfully";
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                }
            }
            catch (SqlException ex)
            {
                response.Message = $"Sql Exception : {ex.Message}";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            catch (Exception ex)
            {
                response.Message = $"Error Exception : {ex.Message}";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            return response;
        }

        #endregion

        #region RepairGood Payment

        [HttpGet]
        [Route("getsalecode")]
        public async Task<ApiResponse<List<string>>> GetSaleCodeAsync()
        {
            var credential = Common.DecodeJwt(User);
            var saleCode = new ApiResponse<List<string>>();
            try
            {
                var execute = await _unitOfWork.Generators.GetSaleCodeAsync(credential.DbCode);
                if (execute.Any())
                {
                    saleCode.Message = $@"Sale codes fetched successfully";
                    saleCode.Result = execute;
                    saleCode.StatusCode = StatusCodes.Status200OK;
                    saleCode.Success = true;
                }
                else
                {
                    saleCode.Message = $@"Sale codes fetched unsuccessfully";
                    saleCode.StatusCode = StatusCodes.Status400BadRequest;
                }
            }
            catch (SqlException ex)
            {
                saleCode.Message = $@"Sql Exception : {ex.Message}";
                saleCode.StatusCode = StatusCodes.Status500InternalServerError;
            }
            catch (Exception ex)
            {
                saleCode.Message = $@"Error Exception : {ex.Message}";
                saleCode.StatusCode = StatusCodes.Status500InternalServerError;
            }

            return saleCode;
        }

        [HttpGet]
        [Route("getnewtransactionifcost")]
        public async Task<ApiResponse<string>> GetNewTransactionIfCost([Required] string saleType)
        {
            var credential = Common.DecodeJwt(User);
            var newTransaction = new ApiResponse<string>();
            try
            {
                var execute = await _unitOfWork.Generators.PostSaleOrderAutoNumberAsync(saleType, credential.DbCode);
                if (execute.Any())
                {
                    newTransaction.Message = $@"New transaction fetched successfully";
                    newTransaction.Result = execute;
                    newTransaction.StatusCode = StatusCodes.Status200OK;
                    newTransaction.Success = true;
                }
                else
                {
                    newTransaction.Message = $@"New transaction fetched unsuccessfully";
                    newTransaction.StatusCode = StatusCodes.Status400BadRequest;
                }
            }
            catch (SqlException ex)
            {
                newTransaction.Message = $@"Sql Exception : {ex.Message}";
                newTransaction.StatusCode = StatusCodes.Status500InternalServerError;
            }
            catch (Exception ex)
            {
                newTransaction.Message = $@"Error Exception : {ex.Message}";
                newTransaction.StatusCode = StatusCodes.Status500InternalServerError;
            }

            return newTransaction;
        }
        [HttpGet]
        [Route("getallcustomerhascompletedrepair")]
        public async Task<ApiResponse<List<CustomerRespondDto>>> GetAllCustomerHasCompletedRepair()
        {
            var credential = Common.DecodeJwt(User);
            var response = new ApiResponse<List<CustomerRespondDto>>();
            try
            {
                var result = await _unitOfWork.RepairGoods.GetAllCustomerHasCompletedRepair(credential.DbCode);
                var newRespond = result.Select(x => new CustomerRespondDto
                {
                    UserCode = x.UserCode,
                    CustomerName = x.CustomerName,
                    CustomerCode = x.CustomerCode,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Store = x.Store,
                    Area = x.Area,
                    Market = x.Market,
                }).ToList();
                if (result.Any())
                {
                    response.Result = newRespond;
                    response.Message = "Customer fetched successfully";
                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.Success = true;
                }
                else
                {
                    response.Message = "Customer fetched unsuccessfully";
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                }
            }
            catch (SqlException ex)
            {
                response.Message = $"Sql Exception : {ex.Message}";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            catch (Exception ex)
            {
                response.Message = $"Error Exception : {ex.Message}";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            return response;
        }
        
        [HttpGet]
        [Route("getallitemhascompletedrepair/{customerCode}")]
        public async Task<ApiResponse<List<ItemRepairCompletedRespondDto>>> GetAllItemHasCompletedRepairByCustomerCode([Required] string customerCode)
        {
            var credential = Common.DecodeJwt(User);
            var response = new ApiResponse<List<ItemRepairCompletedRespondDto>>();
            try
            {
                var result = await _unitOfWork.RepairGoods.GetAllItemHasCompletedRepairByCustomerCode(credential.DbCode, customerCode);
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
                    Note = x.Note,
                }).ToList();
                if (result.Any())
                {
                    response.Result = newRespond;
                    response.Message = "Goods fetched successfully";
                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.Success = true;
                }
                else
                {
                    response.Message = "Goods fetched unsuccessfully";
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                }
            }
            catch (SqlException ex)
            {
                response.Message = $"Sql Exception : {ex.Message}";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            catch (Exception ex)
            {
                response.Message = $"Error Exception : {ex.Message}";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            return response;
        }
        
        [HttpPost]
        [Route("switchitemtype")]
        public async Task<ApiResponse<int>> SwitchItemTypeAsync(SwitchItemTypeDto model)
        {
            var credential = Common.DecodeJwt(User);
            var response = new ApiResponse<int>();
            try
            {
                const string fromType = "REPAIR";
                const string toType = "EXCHANGE";
                var affectedRow = await _unitOfWork.RepairGoods.SwitchItemType(credential.DbCode, credential.Username,model.RequestDetailId,model.Description,fromType,toType);
                if (affectedRow > 0 )
                {
                    response.Result = affectedRow;
                    response.Message = "Switch type updated successfully";
                    response.StatusCode = (int)HttpStatusCode.NoContent;
                    response.Success = true;
                }
                else
                {
                    response.Message = "Switch type updated unsuccessfully";
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                }
            }
            catch (SqlException ex)
            {
                response.Message = $"Sql Exception : {ex.Message}";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            catch (Exception ex)
            {
                response.Message = $"Error Exception : {ex.Message}";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            return response;
        }
        /// <summary>
        /// sample param
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
            var response = new ApiResponse<int>();
            try
            {
                var affectedRow = 0;
                bool checkifInStock = false;
                var qtyAndEpd = new Dictionary<string, List<string>>();
                var tempQtyAndEpd = new Dictionary<string, List<string>>();
                var itemQuantity = new Dictionary<string, int>(); 
                foreach (var items in param.OldInvoiceIssuance)
                {
                    qtyAndEpd[items.ItemCode] = new List<string>() { "" };
                }
                var total = Convert.ToDouble(param.OldInvoiceIssuance.Sum(x => x.Total));
                if (total is not { } or > 0)
                {
                    var saleAnalysisByCustomerCodeAsync =
                        await _unitOfWork.Generators.GetSaleAnalysisByCustomerCodeAsync(param.CustomerWhoRepairGoods.CustomerCode,credential.DbCode);

                    if (saleAnalysisByCustomerCodeAsync is null)
                    {
                        response.Message = "Sale analysis not found for this customer";
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        return response;
                    }
                    var saleHeaderDto = new SaleHeaderDto
                    {
                        RecType = "O",
                        InvoiceDate = param.NewInvoiceIssuance.InvoiceDate.ToString("MM/dd/yyyy"),
                        OrderDate =  param.NewInvoiceIssuance.InvoiceDate.ToString("MM/dd/yyyy"),
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
                        UserCode = credential.Username,
                    };

                    var tranLine = 0;
                    double quantityExchange = 0;
                    int count = 0;
                    bool checkingIfNotEnough = false;
                    // Item Request Exchange
                    var allDetailsDtos = new List<SaleDetailsDto>();

                    do
                    {
                        checkingIfNotEnough = false;
                        var execute = await _unitOfWork.Invoices.GetItemExpiredDates(credential.DbCode, qtyAndEpd, param.NewInvoiceIssuance.Warehouse);
                        foreach (var item in execute)
                        {
                            var checkIfOutOfStock = false;
                            var quantityFee = item.Fees;
                            int quantityOrder = param.OldInvoiceIssuance.Where(x => x.ItemCode == item.ItemCodeCopy).Select(x => x.Quantity).FirstOrDefault();
                            if (checkifInStock == false)
                            {
                                itemQuantity[item.ItemCodeCopy] = quantityOrder; // itemQuantity == SHP-94945 = 2
                            }
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
                            if (checkifInStock && checkIfOutOfStock == false)
                            {
                                quantityExchange = itemQuantity[item.ItemCodeCopy];
                            }
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
                                AnalM0 =  param.CustomerWhoRepairGoods.UserCode,
                                AnalM1 = "",
                                AnalM2 = "",
                                AnalM3 = saleAnalysisByCustomerCodeAsync.AnalysisC6,
                                AnalM4 = "",
                                AnalM5 = "",
                                AnalM6 = "Exchange Goods",
                                AnalM7 = "",
                                AnalM8 = saleAnalysisByCustomerCodeAsync.AnalysisC8,
                                AnalM9 = saleAnalysisByCustomerCodeAsync.AnalysisC9,
                                Value1 = (checkIfOutOfStock) ? quantityExchange : (checkifInStock) ? quantityExchange : items.Quantity,
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
                                UserInvoice = credential.Username,

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

                    affectedRow += await _unitOfWork.Invoices.CreateInvoiceSaleAsync(credential.DbCode,saleHeaderDto, allDetailsDtos);

                    affectedRow += await _unitOfWork.Invoices.InsertRecordInvoice(credential.DbCode,credential.Username,param.NewInvoiceIssuance.TransactionCode, param.CustomerWhoRepairGoods.CustomerCode,
                        param.CustomerWhoRepairGoods.CustomerName, total, param.NewInvoiceIssuance.InvoiceDate, credential.InvoiceEntryCode);
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
                    affectedRow += await CreateRecordFixInvoice(createInvoiceDetailDtos, param.NewInvoiceIssuance.Description, param.NewInvoiceIssuance.TransactionCode, param.NewInvoiceIssuance.InvoiceDate,
                        total,param.CustomerWhoRepairGoods.CustomerCode,credential.DbCode,credential.Username);
                    if (affectedRow > 6)
                    {
                        response.StatusCode = StatusCodes.Status204NoContent;
                        response.Message = "Repair goods paid successfully";
                        response.Success = true;
                        response.Result = affectedRow;
                    }
                    else
                    {
                        response.StatusCode = StatusCodes.Status204NoContent;
                        response.Message = "Repair goods paid unsuccessfully";
                    }
                    
                    //await _unitOfWork.Invoices.GetInvoiceByCustomerCode(credential.DbCode, param.CustomerWhoRepairGoods.CustomerCode);
                }
                else
                {
                    var generateFixInvoice = await _unitOfWork.Generators.GenerateFixInvoice(credential.DbCode);
                     affectedRow += await _unitOfWork.Invoices.InsertRecordInvoice(credential.DbCode,credential.Username,generateFixInvoice, param.CustomerWhoRepairGoods.CustomerCode,
                        param.CustomerWhoRepairGoods.CustomerName, total, param.NewInvoiceIssuance.InvoiceDate, credential.InvoiceEntryCode);
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
                    affectedRow +=await CreateRecordFixInvoice(createInvoiceDetailDtos, param.NewInvoiceIssuance.Description, generateFixInvoice, param.NewInvoiceIssuance.InvoiceDate,
                        0, param.CustomerWhoRepairGoods.CustomerCode,credential.DbCode,credential.Username);

                    if (affectedRow > 4)
                    {
                        response.StatusCode = StatusCodes.Status204NoContent;
                        response.Message = "Repair goods paid successfully";
                        response.Success = true;
                        response.Result = affectedRow;
                    }
                    else
                    {
                        response.StatusCode = StatusCodes.Status400BadRequest;
                        response.Message = "Repair goods paid unsuccessfully";
                        response.Success = false;
                    }
                    
                }

            }
            catch (SqlException ex)
            {
                response.Message = $"Sql Exception : {ex.Message}";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }catch (Exception ex)
            {
                response.Message = $"Error Exception : {ex.Message}";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            return response;
        }
        private async Task<int> CreateRecordFixInvoice(List<CreateInvoiceDetailDto> createInvoiceDetailDtos, string description,
            string transactionCode, DateTime date, double total,string customerCode,string dbCode,string userName)
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

            affectedRow = await _unitOfWork.Invoices.CreateInvoice(createInvoiceDto, createInvoiceDetailDto); // return 4
            return affectedRow;
        }

        [HttpPut]
        [Route("updateamountrepaircompleted")]
        public async Task<ApiResponse<int>> UpdateAmountRepairCompletedAsync([Required] double totalPrice, [Required] int repairCompletedId )
        {
            var credential = Common.DecodeJwt(User);
            var response = new ApiResponse<int>();
            try
            {
                var affectedRow = await _unitOfWork.RepairGoods.PaidRepairItemAsync(credential.Username, totalPrice, repairCompletedId);
                if (affectedRow > 0)
                {
                    response.Result = affectedRow;
                    response.Message = "Amount updated successfully";
                    response.StatusCode = (int)HttpStatusCode.NoContent;
                    response.Success = true;
                }
                else
                {
                    response.Message = "Amount updated unsuccessfully";
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                }
            }
            catch (SqlException ex)
            {
                response.Message = $"Sql Exception : {ex.Message}";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            catch (Exception ex)
            {
                response.Message = $"Error Exception : {ex.Message}";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            return response;
        }
        #endregion
    }
}
    