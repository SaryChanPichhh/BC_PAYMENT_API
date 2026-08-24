using System.ComponentModel.DataAnnotations;
using System.Net;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.Expense;
using BC.PAYMENT.CORE.DTO.Invoice;
using BC.PAYMENT.CORE.DTO.Transaction.ProvincialPayment.StockCarPayment;
using BC.PAYMENT.CORE.Entities.Expense;
using BC.PAYMENT.CORE.Entities.Invoice;
using BC.PAYMENT.CORE.Entities.Transaction.ProvincialPayment.StockCarPayment;
using BC.PAYMENT.CORE.Enums;
using BC.PAYMENT.LOGGING;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BC.PAYMENT.API.Controllers.Transaction.Provincial_Payment.StockCarPayment
{
    public class StockCarPaymentController(IUnitOfWork unitOfWork) : BaseApiController
    {

        #region SaleRepresent
        [HttpGet]
        [Route("getallsalerepresent")]
        public async Task<ApiResponse<List<SaleRepresentModel>>> GetAllSaleRepresentAsync()
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var saleRepresentList = await unitOfWork.SaleRepresent.GetAsync(credential.DbCode!);
                if (saleRepresentList.Any())
                {
                    return ApiResponse<List<SaleRepresentModel>>.Builder()
                        .WithResult(saleRepresentList)
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithMessage("Sale Represent fetched successfully")
                        .Build();
                }
                else
                {
                    return ApiResponse<List<SaleRepresentModel>>.Builder()
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithMessage("Sale Represent fetched unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<SaleRepresentModel>>(ex.Message);
            }
        }
        
        [HttpPost]
        [Route("addnewsalerepresent")]
        public async Task<ApiResponse<SaleRepresentDto>> AddNewSaleRepresentAsync([FromBody] SaleRepresentDto model)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var saleRepresentModel = new SaleRepresentModel
                {
                    EmployeeId = model.EmployeeId,
                    CreatedBy = credential.Username,
                    Description = model.Description,
                    DbCode = credential.DbCode
                };
                var affectedRow = await unitOfWork.SaleRepresent.AddNewAsync(saleRepresentModel);
                if (affectedRow > 0 )
                {
                    return ApiResponse<SaleRepresentDto>.Builder()
                        .WithResult(model)
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithMessage("Sale Represent added successfully")
                        .Build();
                }
                else
                {
                    return ApiResponse<SaleRepresentDto>.Builder()
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithMessage("Sale Represent added unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<SaleRepresentDto>(ex.Message);
            }
        }
        [HttpDelete]
        [Route("deletesalerepresent/{employeeId}")]
        public async Task<ApiResponse<int>> DeleteSaleRepresentAsync([Required] string employeeId)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var affectedRow = await unitOfWork.SaleRepresent.DeleteAsync(employeeId);
                if (affectedRow > 0 )
                {
                    return ApiResponse<int>.Builder()
                        .WithResult(affectedRow)
                        .WithStatusCode((int)HttpStatusCode.NoContent)
                        .WithMessage("Sale Represent added successfully")
                        .Build();
                }
                else
                {
                    return ApiResponse<int>.Builder()
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithMessage("Sale Represent added unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
            }
        }
        
        [HttpPut]
        [Route("updatesalerepresent/{employeeId}")]
        public async Task<ApiResponse<SaleRepresentUpdateDto>> UpdateSaleRepresentAsync([FromBody] SaleRepresentUpdateDto model)
        {
            var credential = Common.DecodeJwt(User);
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
                var affectedRow = await unitOfWork.SaleRepresent.UpdateAsync(saleRepresentModel);
                if (affectedRow > 0 )
                {
                    return ApiResponse<SaleRepresentUpdateDto>.Builder()
                        .WithResult(model)
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithMessage("Sale Represent updated successfully")
                        .Build();
                }
                else
                {
                    return ApiResponse<SaleRepresentUpdateDto>.Builder()
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithMessage("Sale Represent updated unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<SaleRepresentUpdateDto>(ex.Message);
            }
        }


        #endregion

        #region Stock Car Invoice 
        [HttpGet]
        [Route("getinvoicebytemplateid/{invoiceTypes}/{templateId}")]
        public async Task<ApiResponse<List<StockCarInvoicesModel>>> GetNewInvoiceAsync([Required] InvoiceTypes invoiceTypes , [Required] int templateId)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var stockCarInvoiceList = await unitOfWork.SaleRepresent.GetAllInvoiceByInvoiceTypeAsync(credential.DbCode!,invoiceTypes,templateId);
                if (stockCarInvoiceList.Any())
                {
                    return ApiResponse<List<StockCarInvoicesModel>>.Builder()
                        .WithResult(stockCarInvoiceList)
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithMessage("Stock Car Invoice fetched successfully")
                        .Build();
                }
                else
                {
                    return ApiResponse<List<StockCarInvoicesModel>>.Builder()
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithMessage("Stock Car Invoice fetched unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<StockCarInvoicesModel>>(ex.Message);
            }
        }

        #region All Invoice
        [HttpPost]
        [Route("getallinvoices")]
        public async Task<ApiResponse<List<OldInvoiceResponeDto>>> GetNewInvoiceAsync([FromBody] NewInvoiceGetDto model)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var stockCarInvoiceList = await unitOfWork.SaleRepresent.GetAllInvoiceAsync(credential.DbCode!, model.FromSaleCode!, model.ToSaleCode!, Convert.ToDateTime(model.FromDate), Convert.ToDateTime(model.ToDate));
                if (stockCarInvoiceList.Any())
                {
                    return ApiResponse<List<OldInvoiceResponeDto>>.Builder()
                        .WithResult(stockCarInvoiceList)
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithMessage("All invoices fetched successfully")
                        .Build();
                }
                else
                {
                    return ApiResponse<List<OldInvoiceResponeDto>>.Builder()
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithMessage("All invoices fetched unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<OldInvoiceResponeDto>>(ex.Message);
            }
        }
        #endregion

        [HttpDelete]
        [Route("deleteinvoice/{templateId}")]
        public async Task<ApiResponse<int>> DeleteInvoiceAsync([Required] int templateId)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var affectedRow = await unitOfWork.SaleRepresent.DeleteInvoiceByIdAsync(templateId);
                if (affectedRow > 0)
                {
                    return ApiResponse<int>.Builder()
                        .WithResult(affectedRow)
                        .WithStatusCode((int)HttpStatusCode.NoContent)
                        .WithMessage("Stock Car Invoice deleted successfully")
                        .Build();
                }
                else
                {
                    return ApiResponse<int>.Builder()
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithMessage("Stock Car Invoice deleted unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
            }
        }
        [HttpPut]
        [Route("updateinvoice")]
        public async Task<ApiResponse<int>> UpdateInvoiceAsync([FromBody] InvoiceModelDto model)
        {
            try
            {
                var stockCarInvoiceModel = new StockCarInvoicesModel
                {
                    InvoiceId = model.InvoiceId,
                    InvoiceValue = model.InvoiceValue,
                    CustomerCode = model.CustomerCode,
                    CustomerName = model.CustomerName,
                };
                var affectedRow = await unitOfWork.SaleRepresent.UpdateInvoiceByIdAsync(stockCarInvoiceModel);
                if (affectedRow > 0)
                {
                    return ApiResponse<int>.Builder()
                        .WithResult(affectedRow)
                        .WithStatusCode((int)HttpStatusCode.NoContent)
                        .WithMessage("Stock Car Invoice updated successfully")
                        .Build();
                }
                else
                {
                    return ApiResponse<int>.Builder()
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithMessage("Stock Car Invoice deleted unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
            }
        }

        #region OldInvoice
        [HttpPost]
        [Route("getoldinvoicesbyall")]
        public async Task<ApiResponse<List<OldInvoiceResponeDto>>> GetOldInvoiceByAllAsync([FromBody] OldInvoiceRequestDto model)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var stockCarInvoiceList = await unitOfWork.SaleRepresent.GetOldInvoiceByAllAsync(model);
                if (stockCarInvoiceList.Any())
                {
                    return ApiResponse<List<OldInvoiceResponeDto>>.Builder()
                        .WithResult(stockCarInvoiceList)
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithMessage("Old invoices by all fetched successfully")
                        .Build();
                }
                else
                {
                    return ApiResponse<List<OldInvoiceResponeDto>>.Builder()
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithMessage("Old invoices by all fetched unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<OldInvoiceResponeDto>>(ex.Message);
            }
        }
        
        [HttpPost]
        [Route("getoldinvoicesbyrange")]
        public async Task<ApiResponse<List<OldInvoiceResponeDto>>> GetOldInvoiceByRangeAsync([FromBody] OldInvoiceRequestDto model)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var stockCarInvoiceList = await unitOfWork.SaleRepresent.GetOldInvoiceByRangeAsync(model);
                if (stockCarInvoiceList.Any())
                {
                    return ApiResponse<List<OldInvoiceResponeDto>>.Builder()
                        .WithResult(stockCarInvoiceList)
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithMessage("Old invoices by range fetched successfully")
                        .Build();
                }
                else
                {
                    return ApiResponse<List<OldInvoiceResponeDto>>.Builder()
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithMessage("Old invoices by range fetched unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<OldInvoiceResponeDto>>(ex.Message);
            }
        }

        #endregion

        #region Transfer Money

        [HttpGet]
        [Route("gettransfermoneybytemplateid/{templateId}")]
        public async Task<ApiResponse<List<TransferMoneyModel>>> GetTransferMoneyByTemplateIdAsync([Required] int templateId)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var transferMoneyList = await unitOfWork.SaleRepresent.GetTransferMoneyByTemplateIdAsync(credential.DbCode!, templateId);
                if (transferMoneyList.Any())
                {
                    return ApiResponse<List<TransferMoneyModel>>.Builder()
                        .WithResult(transferMoneyList)
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithMessage("Transfer Money fetched successfully")
                        .Build();
                }
                else
                {
                    return ApiResponse<List<TransferMoneyModel>>.Builder()
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithMessage("Transfer Money fetched unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<TransferMoneyModel>>(ex.Message);
            }
        }
        
        [HttpPost]
        [Route("addnewtransfermoney")]
        public async Task<ApiResponse<TransferMoneyDto>> AddNewTransferMoneyAsync([FromBody] TransferMoneyDto model)
        {
            var credential = Common.DecodeJwt(User);
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
                var affectedRow = await unitOfWork.SaleRepresent.AddNewTransferMoneyAsync(transferMoneyModel);
                if (affectedRow > 0 )
                {
                    return ApiResponse<TransferMoneyDto>.Builder()
                        .WithResult(model)
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithMessage("Transfer Money added successfully")
                        .Build();
                }
                else
                {
                    return ApiResponse<TransferMoneyDto>.Builder()
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithMessage("Transfer Money added unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<TransferMoneyDto>(ex.Message);
            }
        }

        [HttpPut]
        [Route("updatetransfermoney")]
        public async Task<ApiResponse<TransferMoneyUpdateDto>> UpdateTransferMoneyAsync([FromBody] TransferMoneyUpdateDto model)
        {
            var credential = Common.DecodeJwt(User);
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
                var affectedRow = await unitOfWork.SaleRepresent.UpdateTransferMoneyAsync(transferMoneyModel);
                if (affectedRow > 0 )
                {
                    return ApiResponse<TransferMoneyUpdateDto>.Builder()
                        .WithResult(model)
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithMessage("Transfer Money updated successfully")
                        .Build();
                }
                else
                {
                    return ApiResponse<TransferMoneyUpdateDto>.Builder()
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithMessage("Transfer Money updated unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<TransferMoneyUpdateDto>(ex.Message);
            }
        }
        
        [HttpDelete]
        [Route("deletetransfermoney/{transferId}")]
        public async Task<ApiResponse<int>> DeleteTransferMoneyAsync([Required] int transferId)
        {
            try
            {
                var affectedRow = await unitOfWork.SaleRepresent.DeleteTransferMoneyAsync(transferId);
                if (affectedRow > 0 )
                {
                    return ApiResponse<int>.Builder()
                        .WithResult(affectedRow)
                        .WithStatusCode((int)HttpStatusCode.NoContent)
                        .WithMessage("Transfer Money deleted successfully")
                        .Build();
                }
                else
                {
                    return ApiResponse<int>.Builder()
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithMessage("Transfer Money deleted unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
            }
        }

        #endregion

        #region Expense

        [HttpGet]
        [Route("getexpensebytemplateid/{templateId}")]
        public async Task<ApiResponse<List<ExpenseModel>>> GetExpenseByTemplateIdAsync([Required] int templateId)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var execute = await unitOfWork.SaleRepresent.GetExpenseByTemplateIdAsync(credential.DbCode!, templateId);
                if (execute.Any())
                {
                    return ApiResponse<List<ExpenseModel>>.Builder()
                        .WithResult([])
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithMessage("Expense fetched successfully")
                        .Build();
                }
                else
                {
                    return ApiResponse<List<ExpenseModel>>.Builder()
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithMessage("Expense fetched unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<ExpenseModel>>(ex.Message);
            }
        }

        // [HttpPost]
        // [Route("addnewexpense")]
        // public async Task<ApiResponse<ExpensePostDto>> AddNewExpenseAsync([FromBody] StockCarExpenseDto model)
        // {
        //     var credential = Common.DecodeJwt(User);
        //     try
        //     {
        //         var expenseModel = new ExpenseModel
        //         {
        //             ExpenseDate = model.ExpenseDate,
        //             Description = model.Description,
        //             TotalExpense = model.TotalExpense,
        //             CreatedBy = credential.Username,
        //             DbCode = credential.DbCode,
        //             EmployeeId = model.EmployeeId,
        //             TemplateId = model.TemplateId
        //         };
        //         var affectedRow = await unitOfWork.SaleRepresent.AddNewExpenseAsync(expenseModel);
        //         if (affectedRow > 0 )
        //         {
        //             return ApiResponse<ExpensePostDto>.Builder()
        //                 .WithResult(model)
        //                 .WithStatusCode((int)HttpStatusCode.OK)
        //                 .WithMessage("Expense added successfully")
        //                 .Build();
        //         }
        //         else
        //         {
        //             return ApiResponse<ExpensePostDto>.Builder()
        //                 .WithStatusCode((int)HttpStatusCode.BadRequest)
        //                 .WithMessage("Expense added unsuccessfully")
        //                 .Build();
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         return GlobalExceptionHandler.ExceptionError<ExpensePostDto>(ex.Message);
        //     }
        // }

        [HttpPut]
        [Route("updateexpense")]
        public async Task<ApiResponse<StockCarExpenseUpdateDto>> UpdateExpenseAsync([FromBody] StockCarExpenseUpdateDto model)
        {
            var credential = Common.DecodeJwt(User);
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
                var affectedRow = await unitOfWork.SaleRepresent.UpdateExpenseAsync(transferMoneyModel);
                if (affectedRow > 0)
                {
                    return ApiResponse<StockCarExpenseUpdateDto>.Builder()
                        .WithResult(model)
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithMessage("Expense updated successfully")
                        .Build();
                }
                else
                {
                    return ApiResponse<StockCarExpenseUpdateDto>.Builder()
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithMessage("Expense updated unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<StockCarExpenseUpdateDto>(ex.Message);
            }
        }

        [HttpDelete]
        [Route("deleteexpense/{expenseId}")]
        public async Task<ApiResponse<int>> DeleteExpenseAsync([Required] int expenseId)
        {
            try
            {
                var affectedRow = await unitOfWork.SaleRepresent.DeleteExpenseAsync(expenseId);
                if (affectedRow > 0)
                {
                    return ApiResponse<int>.Builder()
                        .WithResult(affectedRow)
                        .WithStatusCode((int)HttpStatusCode.NoContent)
                        .WithMessage("Expense deleted successfully")
                        .Build();
                }
                else
                {
                    return ApiResponse<int>.Builder()
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithMessage("Expense deleted unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
            }
        }

        #endregion

        #region Payment Invoice

        [HttpGet]
        [Route("getpaymentinvoicebytemplateid/{templateId}")]
        public async Task<ApiResponse<List<PaymentInvoiceDto>>> GetPaymentInvoicesByTemplateIdAsync([Required] int templateId)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var execute = await unitOfWork.SaleRepresent.GetPaymentInvoicesByTemplateIdAsync(credential.DbCode!, templateId);
                if (execute.Any())
                {
                    return ApiResponse<List<PaymentInvoiceDto>>.Builder()
                        .WithResult(execute)
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithMessage("Payment invoices fetched successfully")
                        .Build();
                }
                else
                {
                    return ApiResponse<List<PaymentInvoiceDto>>.Builder()
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithMessage("Payment invoices fetched successfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<PaymentInvoiceDto>>(ex.Message);
            }
        }

        [HttpPost]
        [Route("addnewpaymentinvoice")]
        public async Task<ApiResponse<PaymentInvoicePostDto>> AddNewPaymentInvoiceAsync([FromBody] PaymentInvoicePostDto model)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var paymentInvoiceModel = new PaymentInvoiceDto
                {
                    InvoiceId = model.InvoiceId,
                    AmountPaid = model.AmountPaid,
                };
                var affectedRow = await unitOfWork.SaleRepresent.InsertPaymentInvoiceAsync(paymentInvoiceModel);
                if (affectedRow > 0)
                {
                    return ApiResponse<PaymentInvoicePostDto>.Builder()
                        .WithResult(model)
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithMessage("Payment invoice added successfully")
                        .Build();
                }
                else
                {
                    return ApiResponse<PaymentInvoicePostDto>.Builder()
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithMessage("Payment invoice added unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<PaymentInvoicePostDto>(ex.Message);
            }
        }

        #endregion

        #region Returning Invoice

        [HttpGet]
        [Route("getreturninginvoicebytemplateid/{templateId}")]
        public async Task<ApiResponse<List<ReturningInvoiceDto>>> GetReturningInvoicesByTemplateIdAsync([Required] int templateId)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var execute = await unitOfWork.SaleRepresent.GetReturningInvoicesByTemplateIdAsync(credential.DbCode!, templateId);
                if (execute.Any())
                {
                    return ApiResponse<List<ReturningInvoiceDto>>.Builder()
                        .WithResult(execute)
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithMessage("Returning invoices fetched successfully")
                        .Build();
                }
                else
                {
                    return ApiResponse<List<ReturningInvoiceDto>>.Builder()
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithMessage("Returning invoices fetched successfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<ReturningInvoiceDto>>(ex.Message);
            }
        }

        [HttpPost]
        [Route("addnewreturninginvoice")]
        public async Task<ApiResponse<ReturningInvoicePostDto>> AddNewReturningInvoiceAsync([FromBody] ReturningInvoicePostDto model)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var returningInvoiceModel = new ReturningInvoiceDto()
                {
                    InvoiceId = model.InvoiceId,
                    Description = model.Description,
                };
                var affectedRow = await unitOfWork.SaleRepresent.AddNewReturningInvoiceAsync(returningInvoiceModel);
                if (affectedRow > 0)
                {
                    return ApiResponse<ReturningInvoicePostDto>.Builder()
                        .WithResult(model)
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithMessage("Returning invoice added successfully")
                        .Build();
                }
                else
                {
                    return ApiResponse<ReturningInvoicePostDto>.Builder()
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithMessage("Returning invoice added unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<ReturningInvoicePostDto>(ex.Message);
            }
        }

        #endregion

        #region Payment Report

        [HttpGet]
        [Route("getpaymentreportbytemplateid/{templateId}")]
        public async Task<ApiResponse<List<PaymentModel>>> GetPaymentByTemplateIdAsync([Required] int templateId)
        {
            try
            {
                var execute = await unitOfWork.SaleRepresent.GetPaymentByTemplateIdAsync(templateId);
                if (execute.Any())
                {
                    return ApiResponse<List<PaymentModel>>.Builder()
                        .WithResult(execute)
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithMessage("Payment's report fetched successfully")
                        .Build();
                }
                else
                {
                    return ApiResponse<List<PaymentModel>>.Builder()
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithMessage("Payment's report fetched successfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<PaymentModel>>(ex.Message);
            }
        }
        [HttpGet]
        [Route("gettotalcollectionbytemplateid/{templateId}")]
        public async Task<ApiResponse<List<CollectionPaymentModel>>> GetAllTotalCollectionByTemplateIdAsync([Required] int templateId)
        {
            try
            {
                var execute = await unitOfWork.SaleRepresent.GetAllTotalCollectionByTemplateIdAsync(templateId);
                if (execute.Any())
                {
                    return ApiResponse<List<CollectionPaymentModel>>.Builder()
                        .WithResult(execute)
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithMessage("Total collection report fetched successfully")
                        .Build();
                }
                else
                {
                    return ApiResponse<List<CollectionPaymentModel>>.Builder()
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithMessage("Total collection report fetched successfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<CollectionPaymentModel>>(ex.Message);
            }
        }

        #endregion

        #region Credit Invoice
        [HttpGet]
        [Route("getcreditinvoicebytemplateid/{templateId}")]
        public async Task<ApiResponse<List<StockCarCreditInvoiceDto>>> GetAllCreditInvoiceByTemplateIdAsync([Required] int templateId)
        {
            var credential = Common.DecodeJwt(User);
            try
            {
                var execute = await unitOfWork.SaleRepresent.GetAllCreditInvoiceByTemplateIdAsync(credential.DbCode!,templateId);
                if (execute.Any())
                {
                    return ApiResponse<List<StockCarCreditInvoiceDto>>.Builder()
                        .WithResult(execute)
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithMessage("Credit invoice fetched successfully")
                        .Build();
                }
                else
                {
                    return ApiResponse<List<StockCarCreditInvoiceDto>>.Builder()
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithMessage("Credit invoice fetched successfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<StockCarCreditInvoiceDto>>(ex.Message);
            }
        }
        #endregion

        #endregion
    }
}
