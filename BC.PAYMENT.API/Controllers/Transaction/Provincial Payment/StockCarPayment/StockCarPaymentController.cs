using System.ComponentModel.DataAnnotations;
using System.Net;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.Invoice;
using BC.PAYMENT.CORE.DTO.Transaction.ProvincialPayment.StockCarPayment;
using BC.PAYMENT.CORE.Entities.Invoice;
using BC.PAYMENT.CORE.Entities.Transaction.ProvincialPayment.StockCarPayment;
using BC.PAYMENT.CORE.Enums;
using BC.PAYMENT.LOGGING;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BC.PAYMENT.API.Controllers.Transaction.Provincial_Payment.StockCarPayment
{
    public class StockCarPaymentController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public StockCarPaymentController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region SaleRepresent
        [HttpGet]
        [Route("getallsalerepresent")]
        public async Task<ApiResponse<List<SaleRepresentModel>>> GetAllSaleRepresentAsync()
        {
            var credential = Common.DecodeJwt(User);
            var saleRepresent = new ApiResponse<List<SaleRepresentModel>>();
            try
            {
                var saleRepresentList = await _unitOfWork.SaleRepresent.GetAsync(credential.DbCode!);
                if (saleRepresentList.Any())
                {
                    saleRepresent.StatusCode = (int)HttpStatusCode.OK;
                    saleRepresent.Success = true;
                    saleRepresent.Message = "Sale Represent fetched successfully";
                    saleRepresent.Result = saleRepresentList;
                }
                else
                {
                    saleRepresent.StatusCode = (int)HttpStatusCode.BadRequest;
                    saleRepresent.Success = false;
                    saleRepresent.Message = "Sale Represent fetched unsuccessfully";
                }
            }
            catch (SqlException ex)
            {
                saleRepresent.StatusCode = (int)HttpStatusCode.InternalServerError;
                saleRepresent.Success = false;
                saleRepresent.Message = $@"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql SqlException", ex);
            }
            catch (Exception ex)
            {
                saleRepresent.StatusCode = (int)HttpStatusCode.InternalServerError;
                saleRepresent.Success = false;
                saleRepresent.Message = $@"Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }
            return saleRepresent;
        }
        
        [HttpPost]
        [Route("addnewsalerepresent")]
        public async Task<ApiResponse<SaleRepresentDto>> AddNewSaleRepresentAsync([FromBody] SaleRepresentDto model)
        {
            var credential = Common.DecodeJwt(User);
            var saleRepresent = new ApiResponse<SaleRepresentDto>();
            try
            {
                var saleRepresentModel = new SaleRepresentModel
                {
                    EmployeeId = model.EmployeeId,
                    CreatedBy = credential.Username,
                    Description = model.Description,
                    DbCode = credential.DbCode
                };
                var affectedRow = await _unitOfWork.SaleRepresent.AddNewAsync(saleRepresentModel);
                if (affectedRow > 0 )
                {
                    saleRepresent.StatusCode = (int)HttpStatusCode.OK;
                    saleRepresent.Success = true;
                    saleRepresent.Message = "Sale Represent added successfully";
                    saleRepresent.Result = model;
                }
                else
                {
                    saleRepresent.StatusCode = (int)HttpStatusCode.BadRequest;
                    saleRepresent.Success = false;
                    saleRepresent.Message = "Sale Represent added unsuccessfully";
                }
            }
            catch (SqlException ex)
            {
                saleRepresent.StatusCode = (int)HttpStatusCode.InternalServerError;
                saleRepresent.Success = false;
                saleRepresent.Message = $@"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql SqlException", ex);
            }
            catch (Exception ex)
            {
                saleRepresent.StatusCode = (int)HttpStatusCode.InternalServerError;
                saleRepresent.Success = false;
                saleRepresent.Message = $@"Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }
            return saleRepresent;
        }
        [HttpDelete]
        [Route("deletesalerepresent/{employeeId}")]
        public async Task<ApiResponse<int>> DeleteSaleRepresentAsync([Required] string employeeId)
        {
            var credential = Common.DecodeJwt(User);
            var saleRepresent = new ApiResponse<int>();
            try
            {
                var affectedRow = await _unitOfWork.SaleRepresent.DeleteAsync(employeeId);
                if (affectedRow > 0 )
                {
                    saleRepresent.StatusCode = (int)HttpStatusCode.NoContent;
                    saleRepresent.Success = true;
                    saleRepresent.Message = "Sale Represent added successfully";
                    saleRepresent.Result = affectedRow;
                }
                else
                {
                    saleRepresent.StatusCode = (int)HttpStatusCode.BadRequest;
                    saleRepresent.Success = false;
                    saleRepresent.Message = "Sale Represent added unsuccessfully";
                }
            }
            catch (SqlException ex)
            {
                saleRepresent.StatusCode = (int)HttpStatusCode.InternalServerError;
                saleRepresent.Success = false;
                saleRepresent.Message = $@"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql SqlException", ex);
            }
            catch (Exception ex)
            {
                saleRepresent.StatusCode = (int)HttpStatusCode.InternalServerError;
                saleRepresent.Success = false;
                saleRepresent.Message = $@"Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }
            return saleRepresent;
        }
        
        [HttpPut]
        [Route("updatesalerepresent/{employeeId}")]
        public async Task<ApiResponse<SaleRepresentUpdateDto>> UpdateSaleRepresentAsync([FromBody] SaleRepresentUpdateDto model)
        {
            var credential = Common.DecodeJwt(User);
            var saleRepresent = new ApiResponse<SaleRepresentUpdateDto>();
            try
            {
                var saleRepresentModel = new SaleRepresentModel
                {
                    EmployeeId = model.EmployeeId,
                    CreatedBy = credential.Username,
                    Description = model.Description,
                    DbCode = credential.DbCode,
                    Id  = model.TemplateId
                };
                var affectedRow = await _unitOfWork.SaleRepresent.UpdateAsync(saleRepresentModel);
                if (affectedRow > 0 )
                {
                    saleRepresent.StatusCode = (int)HttpStatusCode.OK;
                    saleRepresent.Success = true;
                    saleRepresent.Message = "Sale Represent updated successfully";
                    saleRepresent.Result = model;
                }
                else
                {
                    saleRepresent.StatusCode = (int)HttpStatusCode.BadRequest;
                    saleRepresent.Success = false;
                    saleRepresent.Message = "Sale Represent updated unsuccessfully";
                }
            }
            catch (SqlException ex)
            {
                saleRepresent.StatusCode = (int)HttpStatusCode.InternalServerError;
                saleRepresent.Success = false;
                saleRepresent.Message = $@"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql SqlException", ex);
            }
            catch (Exception ex)
            {
                saleRepresent.StatusCode = (int)HttpStatusCode.InternalServerError;
                saleRepresent.Success = false;
                saleRepresent.Message = $@"Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }
            return saleRepresent;
        }


        #endregion

        #region Stock Car Invoice 
        [HttpGet]
        [Route("getinvoicebytemplateid/{invoiceTypes}/{templateId}")]
        public async Task<ApiResponse<List<StockCarInvoicesModel>>> GetNewInvoiceAsync([Required] InvoiceTypes invoiceTypes , [Required] int templateId)
        {
            var credential = Common.DecodeJwt(User);
            var stockCarInvoice = new ApiResponse<List<StockCarInvoicesModel>>();
            try
            {
                var stockCarInvoiceList = await _unitOfWork.SaleRepresent.GetAllInvoiceByInvoiceTypeAsync(credential.DbCode!,invoiceTypes,templateId);
                if (stockCarInvoiceList.Any())
                {
                    stockCarInvoice.StatusCode = (int)HttpStatusCode.OK;
                    stockCarInvoice.Success = true;
                    stockCarInvoice.Message = "Stock Car Invoice fetched successfully";
                    stockCarInvoice.Result = stockCarInvoiceList;
                }
                else
                {
                    stockCarInvoice.StatusCode = (int)HttpStatusCode.BadRequest;
                    stockCarInvoice.Success = false;
                    stockCarInvoice.Message = "Stock Car Invoice fetched unsuccessfully";
                }
            }
            catch (SqlException ex)
            {
                stockCarInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                stockCarInvoice.Success = false;
                stockCarInvoice.Message = $@"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql SqlException", ex);
            }
            catch (Exception ex)
            {
                stockCarInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                stockCarInvoice.Success = false;
                stockCarInvoice.Message = $@"Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }
            return stockCarInvoice;
        }

        #region All Invoice
        [HttpPost]
        [Route("getallinvoices")]
        public async Task<ApiResponse<List<OldInvoiceResponeDto>>> GetNewInvoiceAsync([FromBody] NewInvoiceGetDto model)
        {
            var credential = Common.DecodeJwt(User);
            var stockCarInvoice = new ApiResponse<List<OldInvoiceResponeDto>>();
            try
            {
                var stockCarInvoiceList = await _unitOfWork.SaleRepresent.GetAllInvoiceAsync(credential.DbCode!, model.FromSaleCode!, model.ToSaleCode!, Convert.ToDateTime(model.FromDate), Convert.ToDateTime(model.ToDate));
                if (stockCarInvoiceList.Any())
                {
                    stockCarInvoice.StatusCode = (int)HttpStatusCode.OK;
                    stockCarInvoice.Success = true;
                    stockCarInvoice.Message = "All invoices fetched successfully";
                    stockCarInvoice.Result = stockCarInvoiceList;
                }
                else
                {
                    stockCarInvoice.StatusCode = (int)HttpStatusCode.BadRequest;
                    stockCarInvoice.Success = false;
                    stockCarInvoice.Message = "All invoices fetched unsuccessfully";
                }
            }
            catch (SqlException ex)
            {
                stockCarInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                stockCarInvoice.Success = false;
                stockCarInvoice.Message = $@"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql SqlException", ex);
            }
            catch (Exception ex)
            {
                stockCarInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                stockCarInvoice.Success = false;
                stockCarInvoice.Message = $@"Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }
            return stockCarInvoice;
        }
        #endregion

        [HttpDelete]
        [Route("deleteinvoice/{templateId}")]
        public async Task<ApiResponse<int>> DeleteInvoiceAsync([Required] int templateId)
        {
            var credential = Common.DecodeJwt(User);
            var stockCarInvoice = new ApiResponse<int>();
            try
            {
                var affectedRow = await _unitOfWork.SaleRepresent.DeleteInvoiceByIdAsync(templateId);
                if (affectedRow > 0)
                {
                    stockCarInvoice.StatusCode = (int)HttpStatusCode.NoContent;
                    stockCarInvoice.Success = true;
                    stockCarInvoice.Message = "Stock Car Invoice deleted successfully";
                    stockCarInvoice.Result = affectedRow;
                }
                else
                {
                    stockCarInvoice.StatusCode = (int)HttpStatusCode.BadRequest;
                    stockCarInvoice.Success = false;
                    stockCarInvoice.Message = "Stock Car Invoice deleted unsuccessfully";
                }
            }
            catch (SqlException ex)
            {
                stockCarInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                stockCarInvoice.Success = false;
                stockCarInvoice.Message = $@"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql SqlException", ex);
            }
            catch (Exception ex)
            {
                stockCarInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                stockCarInvoice.Success = false;
                stockCarInvoice.Message = $@"Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }
            return stockCarInvoice;
        }
        [HttpPut]
        [Route("updateinvoice")]
        public async Task<ApiResponse<int>> UpdateInvoiceAsync([FromBody] InvoiceModelDto model)
        {
            var stockCarInvoice = new ApiResponse<int>();
            try
            {
                var stockCarInvoiceModel = new StockCarInvoicesModel
                {
                    InvoiceId = model.InvoiceId,
                    InvoiceValue = model.InvoiceValue,
                    CustomerCode = model.CustomerCode,
                    CustomerName = model.CustomerName,
                };
                var affectedRow = await _unitOfWork.SaleRepresent.UpdateInvoiceByIdAsync(stockCarInvoiceModel);
                if (affectedRow > 0)
                {
                    stockCarInvoice.StatusCode = (int)HttpStatusCode.NoContent;
                    stockCarInvoice.Success = true;
                    stockCarInvoice.Message = "Stock Car Invoice updated successfully";
                    stockCarInvoice.Result = affectedRow;
                }
                else
                {
                    stockCarInvoice.StatusCode = (int)HttpStatusCode.BadRequest;
                    stockCarInvoice.Success = false;
                    stockCarInvoice.Message = "Stock Car Invoice deleted unsuccessfully";
                }
            }
            catch (SqlException ex)
            {
                stockCarInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                stockCarInvoice.Success = false;
                stockCarInvoice.Message = $@"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql SqlException", ex);
            }
            catch (Exception ex)
            {
                stockCarInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                stockCarInvoice.Success = false;
                stockCarInvoice.Message = $@"Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }
            return stockCarInvoice;
        }

        #region OldInvoice
        [HttpPost]
        [Route("getoldinvoicesbyall")]
        public async Task<ApiResponse<List<OldInvoiceResponeDto>>> GetOldInvoiceByAllAsync([FromBody] OldInvoiceRequestDto model)
        {
            var credential = Common.DecodeJwt(User);
            var stockCarInvoice = new ApiResponse<List<OldInvoiceResponeDto>>();
            try
            {
                var stockCarInvoiceList = await _unitOfWork.SaleRepresent.GetOldInvoiceByAllAsync(model);
                if (stockCarInvoiceList.Any())
                {
                    stockCarInvoice.StatusCode = (int)HttpStatusCode.OK;
                    stockCarInvoice.Success = true;
                    stockCarInvoice.Message = "Old invoices by all fetched successfully";
                    stockCarInvoice.Result = stockCarInvoiceList;
                }
                else
                {
                    stockCarInvoice.StatusCode = (int)HttpStatusCode.BadRequest;
                    stockCarInvoice.Success = false;
                    stockCarInvoice.Message = "Old invoices by all fetched unsuccessfully";
                }
            }
            catch (SqlException ex)
            {
                stockCarInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                stockCarInvoice.Success = false;
                stockCarInvoice.Message = $@"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql SqlException", ex);
            }
            catch (Exception ex)
            {
                stockCarInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                stockCarInvoice.Success = false;
                stockCarInvoice.Message = $@"Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }
            return stockCarInvoice;
        }
        
        [HttpPost]
        [Route("getoldinvoicesbyrange")]
        public async Task<ApiResponse<List<OldInvoiceResponeDto>>> GetOldInvoiceByRangeAsync([FromBody] OldInvoiceRequestDto model)
        {
            var credential = Common.DecodeJwt(User);
            var stockCarInvoice = new ApiResponse<List<OldInvoiceResponeDto>>();
            try
            {
                var stockCarInvoiceList = await _unitOfWork.SaleRepresent.GetOldInvoiceByRangeAsync(model);
                if (stockCarInvoiceList.Any())
                {
                    stockCarInvoice.StatusCode = (int)HttpStatusCode.OK;
                    stockCarInvoice.Success = true;
                    stockCarInvoice.Message = "Old invoices by range fetched successfully";
                    stockCarInvoice.Result = stockCarInvoiceList;
                }
                else
                {
                    stockCarInvoice.StatusCode = (int)HttpStatusCode.BadRequest;
                    stockCarInvoice.Success = false;
                    stockCarInvoice.Message = "Old invoices by range fetched unsuccessfully";
                }
            }
            catch (SqlException ex)
            {
                stockCarInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                stockCarInvoice.Success = false;
                stockCarInvoice.Message = $@"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql SqlException", ex);
            }
            catch (Exception ex)
            {
                stockCarInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                stockCarInvoice.Success = false;
                stockCarInvoice.Message = $@"Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }
            return stockCarInvoice;
        }

        #endregion

        #region Transfer Money

        [HttpGet]
        [Route("gettransfermoneybytemplateid/{templateId}")]
        public async Task<ApiResponse<List<TransferMoneyModel>>> GetTransferMoneyByTemplateIdAsync([Required] int templateId)
        {
            var credential = Common.DecodeJwt(User);
            var transferMoney = new ApiResponse<List<TransferMoneyModel>>();
            try
            {
                var transferMoneyList = await _unitOfWork.SaleRepresent.GetTransferMoneyByTemplateIdAsync(credential.DbCode!, templateId);
                if (transferMoneyList.Any())
                {
                    transferMoney.StatusCode = (int)HttpStatusCode.OK;
                    transferMoney.Success = true;
                    transferMoney.Message = "Transfer Money fetched successfully";
                    transferMoney.Result = transferMoneyList;
                }
                else
                {
                    transferMoney.StatusCode = (int)HttpStatusCode.BadRequest;
                    transferMoney.Success = false;
                    transferMoney.Message = "Transfer Money fetched unsuccessfully";
                }
            }
            catch (SqlException ex)
            {
                transferMoney.StatusCode = (int)HttpStatusCode.InternalServerError;
                transferMoney.Success = false;
                transferMoney.Message = $@"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql SqlException", ex);
            }
            catch (Exception ex)
            {
                transferMoney.StatusCode = (int)HttpStatusCode.InternalServerError;
                transferMoney.Success = false;
                transferMoney.Message = $@"Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }
            return transferMoney;
        }
        
        [HttpPost]
        [Route("addnewtransfermoney")]
        public async Task<ApiResponse<TransferMoneyDto>> AddNewTransferMoneyAsync([FromBody] TransferMoneyDto model)
        {
            var credential = Common.DecodeJwt(User);
            var transferMoney = new ApiResponse<TransferMoneyDto>();
            try
            {
                var transferMoneyModel = new TransferMoneyModel
                {
                    TransactionDate = model.TransactionDate,
                    Description = model.Description,
                    Amount = model.Amount,
                    DollarFromEmployee = model.DollarFromEmployee,
                    RielFromEmployee = model.RielFromEmployee,
                    ExchangeRateEmployee = model.ExchangeRateEmployee,
                    DepositDollar = model.DepositDollar,
                    DepositRiel = model.DepositRiel,
                    DepositExchange = model.DepositExchange,
                    CreatedBy = credential.Username,
                    DbCode = credential.DbCode,
                    EmployeeId = model.EmployeeId,
                    TemplateId = model.TemplateId
                };
                var affectedRow = await _unitOfWork.SaleRepresent.AddNewTransferMoneyAsync(transferMoneyModel);
                if (affectedRow > 0 )
                {
                    transferMoney.StatusCode = (int)HttpStatusCode.OK;
                    transferMoney.Success = true;
                    transferMoney.Message = "Transfer Money added successfully";
                    transferMoney.Result = model;
                }
                else
                {
                    transferMoney.StatusCode = (int)HttpStatusCode.BadRequest;
                    transferMoney.Success = false;
                    transferMoney.Message = "Transfer Money added unsuccessfully";
                }
            }
            catch (SqlException ex)
            {
                transferMoney.StatusCode = (int)HttpStatusCode.InternalServerError;
                transferMoney.Success = false;
                transferMoney.Message = $@"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql SqlException", ex);
            }
            catch (Exception ex)
            {
                transferMoney.StatusCode = (int)HttpStatusCode.InternalServerError;
                transferMoney.Success = false;
                transferMoney.Message = $@"Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }
            return transferMoney;
        }

        [HttpPut]
        [Route("updatetransfermoney")]
        public async Task<ApiResponse<TransferMoneyUpdateDto>> UpdateTransferMoneyAsync([FromBody] TransferMoneyUpdateDto model)
        {
            var credential = Common.DecodeJwt(User);
            var transferMoney = new ApiResponse<TransferMoneyUpdateDto>();
            try
            {
                var transferMoneyModel = new TransferMoneyModel
                {
                    TransactionDate = model.TransactionDate,
                    Description = model.Description,
                    Amount = model.Amount,
                    DollarFromEmployee = model.DollarFromEmployee,
                    RielFromEmployee = model.RielFromEmployee,
                    ExchangeRateEmployee = model.ExchangeRateEmployee,
                    DepositDollar = model.DepositDollar,
                    DepositRiel = model.DepositRiel,
                    DepositExchange = model.DepositExchange,
                    CreatedBy = credential.Username,
                    DbCode = credential.DbCode,
                    EmployeeId = model.EmployeeId,
                    TemplateId = model.TemplateId,
                    Id = model.Id
                };
                var affectedRow = await _unitOfWork.SaleRepresent.UpdateTransferMoneyAsync(transferMoneyModel);
                if (affectedRow > 0 )
                {
                    transferMoney.StatusCode = (int)HttpStatusCode.OK;
                    transferMoney.Success = true;
                    transferMoney.Message = "Transfer Money updated successfully";
                    transferMoney.Result = model;
                }
                else
                {
                    transferMoney.StatusCode = (int)HttpStatusCode.BadRequest;
                    transferMoney.Success = false;
                    transferMoney.Message = "Transfer Money updated unsuccessfully";
                }
            }
            catch (SqlException ex)
            {
                transferMoney.StatusCode = (int)HttpStatusCode.InternalServerError;
                transferMoney.Success = false;
                transferMoney.Message = $@"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql SqlException", ex);
            }
            catch (Exception ex)
            {
                transferMoney.StatusCode = (int)HttpStatusCode.InternalServerError;
                transferMoney.Success = false;
                transferMoney.Message = $@"Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }
            return transferMoney;
        }
        
        [HttpDelete]
        [Route("deletetransfermoney/{transferId}")]
        public async Task<ApiResponse<int>> DeleteTransferMoneyAsync([Required] int transferId)
        {
            var transferMoney = new ApiResponse<int>();
            try
            {
                var affectedRow = await _unitOfWork.SaleRepresent.DeleteTransferMoneyAsync(transferId);
                if (affectedRow > 0 )
                {
                    transferMoney.StatusCode = (int)HttpStatusCode.NoContent;
                    transferMoney.Success = true;
                    transferMoney.Message = "Transfer Money deleted successfully";
                    transferMoney.Result = affectedRow;
                }
                else
                {
                    transferMoney.StatusCode = (int)HttpStatusCode.BadRequest;
                    transferMoney.Success = false;
                    transferMoney.Message = "Transfer Money deleted unsuccessfully";
                }
            }
            catch (SqlException ex)
            {
                transferMoney.StatusCode = (int)HttpStatusCode.InternalServerError;
                transferMoney.Success = false;
                transferMoney.Message = $@"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql SqlException", ex);
            }
            catch (Exception ex)
            {
                transferMoney.StatusCode = (int)HttpStatusCode.InternalServerError;
                transferMoney.Success = false;
                transferMoney.Message = $@"Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }
            return transferMoney;
        }

        #endregion

        #region Expense

        [HttpGet]
        [Route("getexpensebytemplateid/{templateId}")]
        public async Task<ApiResponse<List<StockCarExpenseModel>>> GetExpenseByTemplateIdAsync([Required] int templateId)
        {
            var credential = Common.DecodeJwt(User);
            var expense = new ApiResponse<List<StockCarExpenseModel>>();
            try
            {
                var execute = await _unitOfWork.SaleRepresent.GetExpenseByTemplateIdAsync(credential.DbCode!, templateId);
                if (execute.Any())
                {
                    expense.StatusCode = (int)HttpStatusCode.OK;
                    expense.Success = true;
                    expense.Message = "Stock car expense fetched successfully";
                    expense.Result = execute;
                }
                else
                {
                    expense.StatusCode = (int)HttpStatusCode.BadRequest;
                    expense.Success = false;
                    expense.Message = "Stock car expense fetched successfully";
                }
            }
            catch (SqlException ex)
            {
                expense.StatusCode = (int)HttpStatusCode.InternalServerError;
                expense.Success = false;
                expense.Message = $@"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql SqlException", ex);
            }
            catch (Exception ex)
            {
                expense.StatusCode = (int)HttpStatusCode.InternalServerError;
                expense.Success = false;
                expense.Message = $@"Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }
            return expense;
        }

        [HttpPost]
        [Route("addnewexpense")]
        public async Task<ApiResponse<StockCarExpenseDto>> AddNewExpenseAsync([FromBody] StockCarExpenseDto model)
        {
            var credential = Common.DecodeJwt(User);
            var transferMoney = new ApiResponse<StockCarExpenseDto>();
            try
            {
                var transferMoneyModel = new StockCarExpenseModel
                {
                    Employee = model.Employee,
                    ExpenseTypeId = model.ExpenseTypeId,
                    ProvinceId = model.ProvinceId,
                    ExpenseDate = model.ExpenseDate,
                    Quantity = model.Quantity,
                    UnitPrice = model.UnitPrice,
                    AmountDollar = model.AmountDollar,
                    AmountRiel = model.AmountRiel,
                    ExchangeRate = model.ExchangeRate,
                    ExpenseType = model.ExpenseType,
                    ProvinceName = model.ProvinceName,
                    CreateBy = credential.Username,
                    DbCode = credential.DbCode,
                    EmployeeId = model.EmployeeId,
                    TemplateId = model.TemplateId
                };
                var affectedRow = await _unitOfWork.SaleRepresent.AddNewExpenseAsync(transferMoneyModel);
                if (affectedRow > 0)
                {
                    transferMoney.StatusCode = (int)HttpStatusCode.OK;
                    transferMoney.Success = true;
                    transferMoney.Message = "Expense added successfully";
                    transferMoney.Result = model;
                }
                else
                {
                    transferMoney.StatusCode = (int)HttpStatusCode.BadRequest;
                    transferMoney.Success = false;
                    transferMoney.Message = "Expense added unsuccessfully";
                }
            }
            catch (SqlException ex)
            {
                transferMoney.StatusCode = (int)HttpStatusCode.InternalServerError;
                transferMoney.Success = false;
                transferMoney.Message = $@"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql SqlException", ex);
            }
            catch (Exception ex)
            {
                transferMoney.StatusCode = (int)HttpStatusCode.InternalServerError;
                transferMoney.Success = false;
                transferMoney.Message = $@"Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }
            return transferMoney;
        }

        [HttpPut]
        [Route("updateexpense")]
        public async Task<ApiResponse<StockCarExpenseUpdateDto>> UpdateExpenseAsync([FromBody] StockCarExpenseUpdateDto model)
        {
            var credential = Common.DecodeJwt(User);
            var transferMoney = new ApiResponse<StockCarExpenseUpdateDto>();
            try
            {
                var transferMoneyModel = new StockCarExpenseModel
                {
                    Employee = model.Employee,
                    ExpenseTypeId = model.ExpenseTypeId,
                    ProvinceId = model.ProvinceId,
                    ExpenseDate = model.ExpenseDate,
                    Quantity = model.Quantity,
                    UnitPrice = model.UnitPrice,
                    AmountDollar = model.AmountDollar,
                    AmountRiel = model.AmountRiel,
                    ExchangeRate = model.ExchangeRate,
                    ExpenseType = model.ExpenseType,
                    ProvinceName = model.ProvinceName,
                    CreateBy = credential.Username,
                    DbCode = credential.DbCode,
                    EmployeeId = model.EmployeeId,
                    TemplateId = model.TemplateId,
                    Id = model.Id
                };
                var affectedRow = await _unitOfWork.SaleRepresent.UpdateExpenseAsync(transferMoneyModel);
                if (affectedRow > 0)
                {
                    transferMoney.StatusCode = (int)HttpStatusCode.OK;
                    transferMoney.Success = true;
                    transferMoney.Message = "Expense updated successfully";
                    transferMoney.Result = model;
                }
                else
                {
                    transferMoney.StatusCode = (int)HttpStatusCode.BadRequest;
                    transferMoney.Success = false;
                    transferMoney.Message = "Expense updated unsuccessfully";
                }
            }
            catch (SqlException ex)
            {
                transferMoney.StatusCode = (int)HttpStatusCode.InternalServerError;
                transferMoney.Success = false;
                transferMoney.Message = $@"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql SqlException", ex);
            }
            catch (Exception ex)
            {
                transferMoney.StatusCode = (int)HttpStatusCode.InternalServerError;
                transferMoney.Success = false;
                transferMoney.Message = $@"Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }
            return transferMoney;
        }

        [HttpDelete]
        [Route("deleteexpense/{expenseId}")]
        public async Task<ApiResponse<int>> DeleteExpenseAsync([Required] int expenseId)
        {
            var transferMoney = new ApiResponse<int>();
            try
            {
                var affectedRow = await _unitOfWork.SaleRepresent.DeleteExpenseAsync(expenseId);
                if (affectedRow > 0)
                {
                    transferMoney.StatusCode = (int)HttpStatusCode.NoContent;
                    transferMoney.Success = true;
                    transferMoney.Message = "Expense deleted successfully";
                    transferMoney.Result = affectedRow;
                }
                else
                {
                    transferMoney.StatusCode = (int)HttpStatusCode.BadRequest;
                    transferMoney.Success = false;
                    transferMoney.Message = "Expense deleted unsuccessfully";
                }
            }
            catch (SqlException ex)
            {
                transferMoney.StatusCode = (int)HttpStatusCode.InternalServerError;
                transferMoney.Success = false;
                transferMoney.Message = $@"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql SqlException", ex);
            }
            catch (Exception ex)
            {
                transferMoney.StatusCode = (int)HttpStatusCode.InternalServerError;
                transferMoney.Success = false;
                transferMoney.Message = $@"Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }
            return transferMoney;
        }

        #endregion

        #region Payment Invoice

        [HttpGet]
        [Route("getpaymentinvoicebytemplateid/{templateId}")]
        public async Task<ApiResponse<List<PaymentInvoiceDto>>> GetPaymentInvoicesByTemplateIdAsync([Required] int templateId)
        {
            var credential = Common.DecodeJwt(User);
            var paymentInvoice = new ApiResponse<List<PaymentInvoiceDto>>();
            try
            {
                var execute = await _unitOfWork.SaleRepresent.GetPaymentInvoicesByTemplateIdAsync(credential.DbCode!, templateId);
                if (execute.Any())
                {
                    paymentInvoice.StatusCode = (int)HttpStatusCode.OK;
                    paymentInvoice.Success = true;
                    paymentInvoice.Message = "Payment invoices fetched successfully";
                    paymentInvoice.Result = execute;
                }
                else
                {
                    paymentInvoice.StatusCode = (int)HttpStatusCode.BadRequest;
                    paymentInvoice.Success = false;
                    paymentInvoice.Message = "Payment invoices fetched successfully";
                }
            }
            catch (SqlException ex)
            {
                paymentInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                paymentInvoice.Success = false;
                paymentInvoice.Message = $@"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql SqlException", ex);
            }
            catch (Exception ex)
            {
                paymentInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                paymentInvoice.Success = false;
                paymentInvoice.Message = $@"Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }
            return paymentInvoice;
        }

        [HttpPost]
        [Route("addnewpaymentinvoice")]
        public async Task<ApiResponse<PaymentInvoicePostDto>> AddNewPaymentInvoiceAsync([FromBody] PaymentInvoicePostDto model)
        {
            var credential = Common.DecodeJwt(User);
            var paymentInvoice = new ApiResponse<PaymentInvoicePostDto>();
            try
            {
                var paymentInvoiceModel = new PaymentInvoiceDto
                {
                    InvoiceId = model.InvoiceId,
                    AmountPaid = model.AmountPaid,
                };
                var affectedRow = await _unitOfWork.SaleRepresent.InsertPaymentInvoiceAsync(paymentInvoiceModel);
                if (affectedRow > 0)
                {
                    paymentInvoice.StatusCode = (int)HttpStatusCode.OK;
                    paymentInvoice.Success = true;
                    paymentInvoice.Message = "Payment invoice added successfully";
                    paymentInvoice.Result = model;
                }
                else
                {
                    paymentInvoice.StatusCode = (int)HttpStatusCode.BadRequest;
                    paymentInvoice.Success = false;
                    paymentInvoice.Message = "Payment invoice added unsuccessfully";
                }
            }
            catch (SqlException ex)
            {
                paymentInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                paymentInvoice.Success = false;
                paymentInvoice.Message = $@"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql SqlException", ex);
            }
            catch (Exception ex)
            {
                paymentInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                paymentInvoice.Success = false;
                paymentInvoice.Message = $@"Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }
            return paymentInvoice;
        }

        #endregion

        #region Returning Invoice

        [HttpGet]
        [Route("getreturninginvoicebytemplateid/{templateId}")]
        public async Task<ApiResponse<List<ReturningInvoiceDto>>> GetReturningInvoicesByTemplateIdAsync([Required] int templateId)
        {
            var credential = Common.DecodeJwt(User);
            var returningInvoice = new ApiResponse<List<ReturningInvoiceDto>>();
            try
            {
                var execute = await _unitOfWork.SaleRepresent.GetReturningInvoicesByTemplateIdAsync(credential.DbCode!, templateId);
                if (execute.Any())
                {
                    returningInvoice.StatusCode = (int)HttpStatusCode.OK;
                    returningInvoice.Success = true;
                    returningInvoice.Message = "Returning invoices fetched successfully";
                    returningInvoice.Result = execute;
                }
                else
                {
                    returningInvoice.StatusCode = (int)HttpStatusCode.BadRequest;
                    returningInvoice.Success = false;
                    returningInvoice.Message = "Returning invoices fetched successfully";
                }
            }
            catch (SqlException ex)
            {
                returningInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                returningInvoice.Success = false;
                returningInvoice.Message = $@"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql SqlException", ex);
            }
            catch (Exception ex)
            {
                returningInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                returningInvoice.Success = false;
                returningInvoice.Message = $@"Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }
            return returningInvoice;
        }

        [HttpPost]
        [Route("addnewreturninginvoice")]
        public async Task<ApiResponse<ReturningInvoicePostDto>> AddNewReturningInvoiceAsync([FromBody] ReturningInvoicePostDto model)
        {
            var credential = Common.DecodeJwt(User);
            var returningInvoice = new ApiResponse<ReturningInvoicePostDto>();
            try
            {
                var returningInvoiceModel = new ReturningInvoiceDto()
                {
                    InvoiceId = model.InvoiceId,
                    Description = model.Description,
                };
                var affectedRow = await _unitOfWork.SaleRepresent.AddNewReturningInvoiceAsync(returningInvoiceModel);
                if (affectedRow > 0)
                {
                    returningInvoice.StatusCode = (int)HttpStatusCode.OK;
                    returningInvoice.Success = true;
                    returningInvoice.Message = "Returning invoice added successfully";
                    returningInvoice.Result = model;
                }
                else
                {
                    returningInvoice.StatusCode = (int)HttpStatusCode.BadRequest;
                    returningInvoice.Success = false;
                    returningInvoice.Message = "Returning invoice added unsuccessfully";
                }
            }
            catch (SqlException ex)
            {
                returningInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                returningInvoice.Success = false;
                returningInvoice.Message = $@"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql SqlException", ex);
            }
            catch (Exception ex)
            {
                returningInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                returningInvoice.Success = false;
                returningInvoice.Message = $@"Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }
            return returningInvoice;
        }

        #endregion

        #region Payment Report

        [HttpGet]
        [Route("getpaymentreportbytemplateid/{templateId}")]
        public async Task<ApiResponse<List<PaymentModel>>> GetPaymentByTemplateIdAsync([Required] int templateId)
        {
            var paymentReport = new ApiResponse<List<PaymentModel>>();
            try
            {
                var execute = await _unitOfWork.SaleRepresent.GetPaymentByTemplateIdAsync(templateId);
                if (execute.Any())
                {
                    paymentReport.StatusCode = (int)HttpStatusCode.OK;
                    paymentReport.Success = true;
                    paymentReport.Message = "Payment's report fetched successfully";
                    paymentReport.Result = execute;
                }
                else
                {
                    paymentReport.StatusCode = (int)HttpStatusCode.BadRequest;
                    paymentReport.Success = false;
                    paymentReport.Message = "Payment's report fetched successfully";
                }
            }
            catch (SqlException ex)
            {
                paymentReport.StatusCode = (int)HttpStatusCode.InternalServerError;
                paymentReport.Success = false;
                paymentReport.Message = $@"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql SqlException", ex);
            }
            catch (Exception ex)
            {
                paymentReport.StatusCode = (int)HttpStatusCode.InternalServerError;
                paymentReport.Success = false;
                paymentReport.Message = $@"Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }
            return paymentReport;
        }
        [HttpGet]
        [Route("gettotalcollectionbytemplateid/{templateId}")]
        public async Task<ApiResponse<List<CollectionPaymentModel>>> GetAllTotalCollectionByTemplateIdAsync([Required] int templateId)
        {
            var paymentReport = new ApiResponse<List<CollectionPaymentModel>>();
            try
            {
                var execute = await _unitOfWork.SaleRepresent.GetAllTotalCollectionByTemplateIdAsync(templateId);
                if (execute.Any())
                {
                    paymentReport.StatusCode = (int)HttpStatusCode.OK;
                    paymentReport.Success = true;
                    paymentReport.Message = "Total collection report fetched successfully";
                    paymentReport.Result = execute;
                }
                else
                {
                    paymentReport.StatusCode = (int)HttpStatusCode.BadRequest;
                    paymentReport.Success = false;
                    paymentReport.Message = "Total collection report fetched successfully";
                }
            }
            catch (SqlException ex)
            {
                paymentReport.StatusCode = (int)HttpStatusCode.InternalServerError;
                paymentReport.Success = false;
                paymentReport.Message = $@"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql SqlException", ex);
            }
            catch (Exception ex)
            {
                paymentReport.StatusCode = (int)HttpStatusCode.InternalServerError;
                paymentReport.Success = false;
                paymentReport.Message = $@"Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }
            return paymentReport;
        }

        #endregion

        #region Credit Invoice
        [HttpGet]
        [Route("getcreditinvoicebytemplateid/{templateId}")]
        public async Task<ApiResponse<List<StockCarCreditInvoiceDto>>> GetAllCreditInvoiceByTemplateIdAsync([Required] int templateId)
        {
            var credential = Common.DecodeJwt(User);
            var creditInvoice = new ApiResponse<List<StockCarCreditInvoiceDto>>();
            try
            {
                var execute = await _unitOfWork.SaleRepresent.GetAllCreditInvoiceByTemplateIdAsync(credential.DbCode!,templateId);
                if (execute.Any())
                {
                    creditInvoice.StatusCode = (int)HttpStatusCode.OK;
                    creditInvoice.Success = true;
                    creditInvoice.Message = "Credit invoice fetched successfully";
                    creditInvoice.Result = execute;
                }
                else
                {
                    creditInvoice.StatusCode = (int)HttpStatusCode.BadRequest;
                    creditInvoice.Success = false;
                    creditInvoice.Message = "Credit invoice fetched successfully";
                }
            }
            catch (SqlException ex)
            {
                creditInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                creditInvoice.Success = false;
                creditInvoice.Message = $@"Sql Exception : {ex.Message}";
                Logger.Instance.Error("Sql SqlException", ex);
            }
            catch (Exception ex)
            {
                creditInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                creditInvoice.Success = false;
                creditInvoice.Message = $@"Exception : {ex.Message}";
                Logger.Instance.Error("Error Exception", ex);
            }
            return creditInvoice;
        }
        #endregion

        #endregion
    }
}
