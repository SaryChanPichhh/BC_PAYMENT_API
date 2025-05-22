using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Net;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.API.Models;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.DTO.Transaction.DailyPayment.DeliveryPaid;
using BC.PAYMENT.CORE.Entities.Expense;
using BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DeliveryPaid;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BC.PAYMENT.API.Controllers.Transaction.DailyPayment.DeliveryPaid
{
    public class DeliveryPaidController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeliveryPaidController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        [Route("updatedividedinvoice")]
        public async Task<ApiResponse<DeliveryInvoicePaidUpdateDto>> UpdateDividedInvoiceAsync(DeliveryInvoicePaidUpdateDto model)
        {
            var credential = Common.DecodeJwt(User);
            var dividedInvoice = new ApiResponse<DeliveryInvoicePaidUpdateDto>();
            try
            {
                var deliveryInvoicePaidModel = new DeliveryGeneralInvoicePaidUpdateModel
                {
                    DbCode = credential.DbCode,
                    CreateBy =  credential.Username,
                    DividedInvoiceId = model.DividedInvoiceId,
                    NewAmount = model.NewAmount,
                    OldAmount = model.OldAmount,
                    Description = model.Description,
                };
                var affectedRow = await _unitOfWork.DeliveryPaid.UpdateDeliveryInvoicePaid(deliveryInvoicePaidModel);
                if(affectedRow > 0)
                {
                    dividedInvoice.StatusCode = (int)HttpStatusCode.OK;
                    dividedInvoice.Success = true;
                    dividedInvoice.Message = "Delivery invoice updated successfully";
                    dividedInvoice.Result = model;
                }
                else
                {
                    dividedInvoice.StatusCode = (int)HttpStatusCode.BadRequest;
                    dividedInvoice.Success = false;
                    dividedInvoice.Message = "Delivery invoice updated unsuccessfully";
                }
            }
            catch (SqlException ex)
            {
                dividedInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                dividedInvoice.Success = false;
                dividedInvoice.Message = $@"Sql Exception : {ex.Message}";
            }
            catch (Exception ex)
            {
                dividedInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                dividedInvoice.Success = false;
                dividedInvoice.Message = $@"Error Exception : {ex.Message}";
            }
            return dividedInvoice;
        }
        [HttpGet]
        [Route("getinvoicebydeliveryid/{deliveryId}/{date}")]
        public async Task<ApiResponse<List<DeliveryDataObject>>> GetAllInvoiceByDeliveryId([Required] string deliveryId, [Required] DateTime date)
        {
            var credential = Common.DecodeJwt(User);
            var dividedInvoice = new ApiResponse<List<DeliveryDataObject>>();
            try
            {
                var execute = await _unitOfWork.DeliveryPaid.GetAllInvoiceByDeliveryIdAndDateDataObjectAsync(credential.DbCode!,deliveryId,date);
                execute.ForEach(x =>
                {
                    var encryptedId = EncryptionHelper.EncryptAES(x.DeliveryId.ToString());
                    encryptedId = Uri.EscapeDataString(encryptedId); // Ensure URL safety
                    x.ImagePath = $"api/deliveries/image/{encryptedId}".Trim();
                });
                if (execute.Any())
                {
                    dividedInvoice.StatusCode = (int)HttpStatusCode.OK;
                    dividedInvoice.Success = true;
                    dividedInvoice.Message = "Delivery invoice fetched successfully";
                    dividedInvoice.Result = execute;
                }
                else
                {
                    dividedInvoice.StatusCode = (int)HttpStatusCode.BadRequest;
                    dividedInvoice.Success = false;
                    dividedInvoice.Message = "Delivery invoice fetched unsuccessfully";
                }
            }
            catch (SqlException ex)
            {
                dividedInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                dividedInvoice.Success = false;
                dividedInvoice.Message = $@"Sql Exception : {ex.Message}";
            }
            catch (Exception ex)
            {
                dividedInvoice.StatusCode = (int)HttpStatusCode.InternalServerError;
                dividedInvoice.Success = false;
                dividedInvoice.Message = $@"Error Exception : {ex.Message}";
            }
            return dividedInvoice;
        }

        [HttpPost]
        [Route("payment")]
        public async Task<ApiResponse<DeliveryInvoicePaidPostDto>> CreatePaymentHeader(DeliveryInvoicePaidPostDto model)
        {
            var credential = Common.DecodeJwt(User);
            var payment = new ApiResponse<DeliveryInvoicePaidPostDto>();
            var expenses = new List<ExpenseModel>();
            try
            {
                var paymentHeader = new PaymentInvoiceHeaderModel
                {
                    DbCode = credential.DbCode,
                    CreatedBy = credential.Username,
                    CreatedDate = DateTime.Today.ToString(CultureInfo.InvariantCulture),
                    DeliveryId = model.PaymentInvoiceHeader.DeliveryId,
                    EntriesCode  = model.PaymentInvoiceHeader.EntriesCode,
                    Period = model.PaymentInvoiceHeader.Period,
                    InvoiceDividendDate = model.PaymentInvoiceHeader.InvoiceDividendDate,
                };
                var checkExists =
                    await _unitOfWork.DeliveryPaid.CheckExistsPaymentHeaderByInvoiceDividendDateAndDeliveryId(model.PaymentInvoiceHeader.InvoiceDividendDate,model.PaymentInvoiceHeader.DeliveryId!,credential.DbCode!);
                int paymentHeaderId;

                if (!checkExists)
                    paymentHeaderId = await _unitOfWork.DeliveryPaid.CreatePaymentHeader(paymentHeader);
                else
                    paymentHeaderId = await _unitOfWork.DeliveryPaid.GetPaymentHeaderId(model.PaymentInvoiceHeader.InvoiceDividendDate, model.PaymentInvoiceHeader.DeliveryId!, credential.DbCode!);

                foreach (var expense in model.Expenses)
                {
                    var expenseModel = new ExpenseModel()
                    {
                        DbCode = credential.DbCode,
                        CreatedBy = credential.Username,
                        Name = expense.Description,
                        Quantity = 1,
                        UnitPrice = expense.UnitPrice,
                        CurrencyType = "Riel",
                        PaymentInvoiceHeaderId = paymentHeaderId,
                        ExchangeRate = expense.ExchangeRate,
                        Total = expense.UnitPrice*1,
                        CreatedDate = DateTime.Today,
                    };
                    expenses.Add(expenseModel);
                }
                await _unitOfWork.DeliveryPaid.CreatePaymentExpense(expenses);


                var affectedRow = 1;
                if (affectedRow > 0)
                {
                    payment.StatusCode = (int)HttpStatusCode.OK;
                    payment.Success = true;
                    payment.Message = "Payment header created successfully";
                    payment.Result = model;
                }
                else
                {
                    payment.StatusCode = (int)HttpStatusCode.BadRequest;
                    payment.Success = false;
                    payment.Message = "Payment header created unsuccessfully";
                }
            }
            catch (SqlException ex)
            {
                payment.StatusCode = (int)HttpStatusCode.InternalServerError;
                payment.Success = false;
                payment.Message = $@"Sql Exception : {ex.Message}";
            }
            catch (Exception ex)
            {
                payment.StatusCode = (int)HttpStatusCode.InternalServerError;
                payment.Success = false;
                payment.Message = $@"Error Exception : {ex.Message}";
            }
            return payment;
        }
    }
}
