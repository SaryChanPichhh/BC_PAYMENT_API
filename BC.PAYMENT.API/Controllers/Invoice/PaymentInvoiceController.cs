using BC.PAYMENT.CORE.Contracts.Request.Invoice;
using BC.PAYMENT.CORE.Contracts.Request.Payment;
using BC.PAYMENT.CORE.Contracts.Response.Paid;
using BC.PAYMENT.CORE.Entities;
using BC.PAYMENT.CORE.Entities.Invoice;

namespace BC.PAYMENT.API.Controllers.Invoice;
public class PaymentInvoiceController(IUnitOfWork unitOfWork) : BaseApiController
{
    [HttpPost]
    [Route("save-payment-invoice")]
    public async Task<ApiResponse<int>> SavePaymentInvoiceAsync([FromBody] List<PaymentInvoiceRequest> request)
    {
        try
        {
            var rowsAffected = await unitOfWork.Invoices.SavePaymentInvoiceAsync(request);
            if (rowsAffected <= 0)
            {
                return ApiResponse<int>.Builder()
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .WithMessage("Payment invoice saved unsuccessfully.")
                    .Build();
            }
            return ApiResponse<int>.Builder()
                .WithStatusCode((int)HttpStatusCode.OK)
                .WithMessage("Payment invoice saved successfully.")
                .WithSuccess(true)
                .WithResult(rowsAffected)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<int>(ex.Message);
        }
    }
    
    [HttpGet]
    [Route("exists-headerId")]
    public async Task<ApiResponse<bool>> IsExistsHeaderIdAsync([FromQuery] DateTime  invoiceDate, [FromQuery] string deliveryId )
    {
        try
        {
            var credential = Common.DecodeJwt(User);
            var rowsAffected = await unitOfWork.PaymentInvoice.IsExistsPaymentHeaderId(credential?.DbCode, invoiceDate, deliveryId);
            if (rowsAffected)
            {
                return ApiResponse<bool>.Builder()
                    .WithStatusCode((int)HttpStatusCode.BadRequest)
                    .WithMessage("Header is exists.")
                    .Build();
            }
            return ApiResponse<bool>.Builder()
                .WithStatusCode((int)HttpStatusCode.OK)
                .WithMessage("Header is not exists.")
                .WithSuccess(true)
                .WithResult(rowsAffected)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<bool>(ex.Message);
        }
    }

    [HttpPost]
    [Route("process-paid")]
    public async Task<ApiResponse<bool>> PaidProcessingAsync([FromQuery] DateTime invoiceDate, [FromQuery] string deliveryId
        , [FromBody] ProcessPaidInvoiceRequest request)
    {
        try
        {
            var credential = Common.DecodeJwt(User);
            var checkExists =
                await unitOfWork.PaymentInvoice.IsExistsPaymentHeaderId(credential?.DbCode, invoiceDate, deliveryId);
            var paymentHeaderId = 0;
            var expenses = new List<PaymentInvoiceExpense>();
            if (!checkExists)
            {
                var paymentHeader = new PaymentInvoiceHeader
                {
                    DbCode = credential.DbCode,
                    CreatedBy = credential.Username,
                    CreatedDate = DateTime.Today,
                    DeliveryId = deliveryId,
                    EntriesCode = credential.InvoiceEntryCode,
                    Period = Convert.ToInt32(credential.Period),
                    InvoiceDividendDate = invoiceDate,
                };
                paymentHeaderId = await unitOfWork.PaymentInvoice.CreatePaymentHeader(paymentHeader);
            }
            else
                paymentHeaderId = await unitOfWork.PaymentInvoice.GetPaymentHeaderId(invoiceDate, deliveryId, credential.DbCode);

            foreach (var expense in request.Expenses)
            {
                var expenseModel = new PaymentInvoiceExpense
                {
                    DbCode = credential.DbCode,
                    CreatedBy = credential.Username,
                    Name = expense.Description,
                    Quantity = 1,
                    UnitPrice = expense.UnitPrice,
                    CurrencyType = "Riel",
                    PaymentHeaderId = paymentHeaderId,
                    ExchangeRate = expense.ExchangeRate,
                    Total = expense.UnitPrice * 1,
                    CreatedDate = DateTime.Today,
                };
                await unitOfWork.Expense.CreatePaymentExpense(expenseModel);
            }
            var paymentDetail = new BcPaymentDetail
            {
                DbCode = credential.DbCode,
                DeliveryId = deliveryId,
                Total = request.PaymentDetail.Total,
                Dollar = request.PaymentDetail.Dollar,
                Riel = request.PaymentDetail.Riel,
                Exchange = request.PaymentDetail.Exchange,
                DescExp1 = request.PaymentDetail.DescExp1,
                ExpAmount1 = request.PaymentDetail.ExpAmount1,
                DescExp2 = request.PaymentDetail.DescExp2,
                ExpAmount2 = request.PaymentDetail.ExpAmount2,
                DescExp3 = request.PaymentDetail.DescExp3,
                ExpAmount3 = request.PaymentDetail.ExpAmount3,
                MoneyBias = request.PaymentDetail.MoneyBias,
                CreatedDate = invoiceDate,
                CreatedBy = credential.Username,
                EntriesCode = credential.InvoiceEntryCode,
            };
            await unitOfWork.PaymentInvoice.CreatePaymentDetailAsync(paymentDetail);
            foreach (var payment in request.PaidInvoices)
            {
                var paidInvoice = new PcPaymentInvoice
                {
                    DbCode = credential.DbCode ?? string.Empty,
                    DividedInvoiceId = payment.DividedInvoiceId,
                    Amount = payment.Amount,
                    CreatedDate = DateTime.Today,
                    CreatedBy = credential.Username ?? string.Empty,
                    Status = true,
                    PaymentHeaderId = paymentHeaderId,
                };
                await  unitOfWork.PaymentInvoice.CreatePcPaymentInvoiceAsync(paidInvoice);
            }

            foreach (var returnInvoice in request.ReturnInvoices)
            {
                var returnInvoiceModel = new PcReturnInvoice
                {
                    DbCode = credential.DbCode ?? string.Empty,
                    DividedId = returnInvoice.DividedId,
                    Description = returnInvoice.Description,
                    CreatedDate = DateTime.Today,
                    CreatedBy = credential.Username ?? string.Empty,
                    Status = true,
                    HeaderId = paymentHeaderId,
                };
                await unitOfWork.ReturnInvoice.CreatePcReturnInvoiceAsync(returnInvoiceModel);
            }
            return ApiResponse<bool>.Builder()
                .WithStatusCode((int)HttpStatusCode.OK)
                .WithMessage("Invoices saved successfully.")
                .WithSuccess(true)
                .WithResult(true)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<bool>(ex.Message);
        }
    }

    [HttpGet]
    [Route("by-date")]
    public async Task<ApiResponse<List<PaymentInvoiceResponse>>> GetPaymentInvoiceDetailByDateAsync(
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var execute = await unitOfWork.PaymentInvoice.GetPaymentInvoiceDetailByDateAsync(credential.DbCode, fromDate, toDate);
            if (execute.Count > 0)
            {
                return ApiResponse<List<PaymentInvoiceResponse>>.Builder()
                    .WithMessage("Payment invoice details fetched successfully")
                    .WithSuccess(true)
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(execute)
                    .Build();
            }
            return ApiResponse<List<PaymentInvoiceResponse>>.Builder()
                .WithMessage("Payment invoice details fetched unsuccessfully")
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .WithResult([])
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<PaymentInvoiceResponse>>(ex.Message);
        }
    }
[HttpGet]
    [Route("by-date-exclude-submit")]
    public async Task<ApiResponse<List<PaymentInvoiceResponse>>> GetPaymentInvoiceDetailExcludeSubmitInvoiceAsync(
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var execute = await unitOfWork.PaymentInvoice.GetPaymentInvoiceDetailExcludeSubmitInvoiceAsync(credential.DbCode, fromDate, toDate);
            if (execute.Count > 0)
            {
                return ApiResponse<List<PaymentInvoiceResponse>>.Builder()
                    .WithMessage("Payment invoice details fetched successfully")
                    .WithSuccess(true)
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(execute)
                    .Build();
            }
            return ApiResponse<List<PaymentInvoiceResponse>>.Builder()
                .WithMessage("Payment invoice details fetched unsuccessfully")
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .WithResult([])
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<PaymentInvoiceResponse>>(ex.Message);
        }
    }

    [HttpGet]
    [Route("by-period")]
    public async Task<ApiResponse<List<PaymentInvoiceResponse>>> GetPaymentInvoiceDetailByPeriodAsync(
        [FromQuery] int month,
        [FromQuery] int year)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var execute = await unitOfWork.PaymentInvoice.GetPaymentInvoiceDetailByPeriodAsync(credential.DbCode, month, year);
            if (execute.Count > 0)
            {
                return ApiResponse<List<PaymentInvoiceResponse>>.Builder()
                    .WithMessage("Payment invoice details fetched successfully")
                    .WithSuccess(true)
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(execute)
                    .Build();
            }
            return ApiResponse<List<PaymentInvoiceResponse>>.Builder()
                .WithMessage("Payment invoice details fetched unsuccessfully")
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .WithResult([])
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<PaymentInvoiceResponse>>(ex.Message);
        }
    }
    [HttpGet]
    [Route("paid-history")]
    public async Task<ApiResponse<List<PaymentInvoiceResponse>>> GetPaymentInvoiceDetailByInvoiceCodeAsync(
        [FromQuery] DateTime date,
        [FromQuery] string invoiceCode)
    {
        var credential = Common.DecodeJwt(User);
        try
        {
            var execute = await unitOfWork.PaymentInvoice.GetPaymentInvoiceDetailByInvoiceCodeAsync(credential.DbCode, date:date,invoiceCode:invoiceCode);
            if (execute.Count > 0)
            {
                return ApiResponse<List<PaymentInvoiceResponse>>.Builder()
                    .WithMessage("Payment invoice details fetched successfully")
                    .WithSuccess(true)
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithResult(execute)
                    .Build();
            }
            return ApiResponse<List<PaymentInvoiceResponse>>.Builder()
                .WithMessage("Payment invoice details fetched unsuccessfully")
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .WithResult([])
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<List<PaymentInvoiceResponse>>(ex.Message);
        }
    }
    [HttpPut]
    [Route("value")]
    public async Task<ApiResponse<bool>> UpdatePaymentInvoiceAsync([FromBody] UpdatePaymentInvoiceRequest request)
    {
        try
        {
            var credential = Common.DecodeJwt(User);

            var newInvoice = new NewInvoiceModel
            {
                DbCode =  credential.DbCode ?? string.Empty,
                CreatedBy =  credential.Username ?? string.Empty,
                CreatedDate = credential.CurrectDate,
                Id = request.InvoiceId,
                InvoiceId = request.InvoiceId,
                InvoiceAmount = request.NewAmount,
                
            };
            var pcEditValue = new PcEditDividedInvoice
            {
                CreateDate = credential.CurrectDate,
                CreatedBy =  credential.Username ?? string.Empty,
                DbCode = credential.DbCode ?? string.Empty,
                DividedId =  request.DividedId,
                NewAmount = request.NewAmount,
                OldAmount = request.OldAmount,
                Description = request.Description,
            };
            var paymentInvoice = new PcPaymentInvoice
            {
                Amount = request.NewAmount,
                PaymentId =  request.PaymentId,
                DividedInvoiceId = request.DividedId,
                DbCode = credential.DbCode,
                CreatedBy =   credential.Username ?? string.Empty,
                CreatedDate =  credential.CurrectDate,
            };
            var affectedRow = await unitOfWork.PaymentInvoice.UpdatePaidValueAsync(paymentInvoice, newInvoice, pcEditValue);
            return ApiResponse<bool>.Builder()
                .WithStatusCode((int)HttpStatusCode.OK)
                .WithMessage("Invoices saved successfully.")
                .WithSuccess(true)
                .WithResult(true)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<bool>(ex.Message);
        }
    }

    [HttpDelete]
    [Route("")]
    public async Task<ApiResponse<bool>> DeletePaymentInvoiceAsync([FromQuery] int paymentId, [FromQuery] int dividedId)
    {
        try
        {
            var affectedRows = await unitOfWork.PaymentInvoice.DeletePaymentInvoiceAsync(paymentId, dividedId);
            if (affectedRows > 0)
            {
                return ApiResponse<bool>.Builder()
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithMessage("Payment invoice deleted successfully.")
                    .WithSuccess(true)
                    .WithResult(true)
                    .Build();
            }

            return ApiResponse<bool>.Builder()
                .WithStatusCode((int)HttpStatusCode.BadRequest)
                .WithMessage("Payment invoice deleted unsuccessfully.")
                .WithSuccess(false)
                .WithResult(false)
                .Build();
        }
        catch (Exception ex)
        {
            return GlobalExceptionHandler.ExceptionError<bool>(ex.Message);
        }
    }

}