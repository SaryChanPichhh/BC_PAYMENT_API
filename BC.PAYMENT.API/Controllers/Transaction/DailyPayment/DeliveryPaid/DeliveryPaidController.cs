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
    public class DeliveryPaidController(IUnitOfWork unitOfWork) : BaseApiController
    {
        [HttpPost]
        [Route("updatedividedinvoice")]
        public async Task<ApiResponse<DeliveryInvoicePaidUpdateDto>> UpdateDividedInvoiceAsync(DeliveryInvoicePaidUpdateDto model)
        {
            try
            {
                var credential = Common.DecodeJwt(User);
                var deliveryInvoicePaidModel = new DeliveryGeneralInvoicePaidUpdateModel
                {
                    DbCode = credential.DbCode,
                    CreateBy =  credential.Username,
                    DividedInvoiceId = model.DividedInvoiceId,
                    NewAmount = model.NewAmount,
                    OldAmount = model.OldAmount,
                    Description = model.Description,
                };
                var affectedRow = await unitOfWork.DeliveryPaid.UpdateDeliveryInvoicePaid(deliveryInvoicePaidModel);
                if(affectedRow > 0)
                {
                    return ApiResponse<DeliveryInvoicePaidUpdateDto>.Builder()
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithMessage("Delivery invoice updated successfully")
                        .WithResult(model)
                        .Build();
                }
                else
                {
                    return ApiResponse<DeliveryInvoicePaidUpdateDto>.Builder()
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithMessage("Delivery invoice updated unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<DeliveryInvoicePaidUpdateDto>(ex.Message);
            }
        }

        [HttpGet]
        [Route("getinvoicebydeliveryid/{deliveryId}/{date}")]
        public async Task<ApiResponse<List<DeliveryDataObject>>> GetAllInvoiceByDeliveryId([Required] string deliveryId, [Required] DateTime date)
        {
            try
            {
                var credential = Common.DecodeJwt(User);
                var execute = await unitOfWork.DeliveryPaid.GetAllInvoiceByDeliveryIdAndDateDataObjectAsync(credential.DbCode!,deliveryId,date);
                execute.ForEach(x =>
                {
                    var encryptedId = EncryptionHelper.EncryptAES(x.DeliveryId.ToString());
                    encryptedId = Uri.EscapeDataString(encryptedId); // Ensure URL safety
                    x.ImagePath = $"api/deliveries/image/{encryptedId}".Trim();
                });
                if (execute.Any())
                {
                    return ApiResponse<List<DeliveryDataObject>>.Builder()
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithMessage("Delivery invoice fetched successfully")
                        .WithResult(execute)
                        .Build();
                }
                else
                {
                    return ApiResponse<List<DeliveryDataObject>>.Builder()
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithMessage("Delivery invoice fetched unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<List<DeliveryDataObject>>(ex.Message);
            }
        }

        [HttpPost]
        [Route("payment")]
        public async Task<ApiResponse<DeliveryInvoicePaidPostDto>> CreatePaymentHeader(DeliveryInvoicePaidPostDto model)
        {
            try
            {
                var credential = Common.DecodeJwt(User);
                var expenses = new List<ExpenseModel>();
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
                    await unitOfWork.DeliveryPaid.CheckExistsPaymentHeaderByInvoiceDividendDateAndDeliveryId(model.PaymentInvoiceHeader.InvoiceDividendDate,model.PaymentInvoiceHeader.DeliveryId!,credential.DbCode!);
                int paymentHeaderId;

                if (!checkExists)
                    paymentHeaderId = await unitOfWork.DeliveryPaid.CreatePaymentHeader(paymentHeader);
                else
                    paymentHeaderId = await unitOfWork.DeliveryPaid.GetPaymentHeaderId(model.PaymentInvoiceHeader.InvoiceDividendDate, model.PaymentInvoiceHeader.DeliveryId!, credential.DbCode!);

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
                await unitOfWork.DeliveryPaid.CreatePaymentExpense(expenses);

                var affectedRow = 1;
                if (affectedRow > 0)
                {
                    return ApiResponse<DeliveryInvoicePaidPostDto>.Builder()
                        .WithStatusCode((int)HttpStatusCode.OK)
                        .WithMessage("Payment header created successfully")
                        .WithResult(model)
                        .Build();
                }
                else
                {
                    return ApiResponse<DeliveryInvoicePaidPostDto>.Builder()
                        .WithStatusCode((int)HttpStatusCode.BadRequest)
                        .WithMessage("Payment header created unsuccessfully")
                        .Build();
                }
            }
            catch (Exception ex)
            {
                return GlobalExceptionHandler.ExceptionError<DeliveryInvoicePaidPostDto>(ex.Message);
            }
        }
    }
}
