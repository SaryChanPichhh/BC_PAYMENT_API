using BC.PAYMENT.CORE.Contracts.Response.Expense;
using BC.PAYMENT.CORE.Contracts.Response.Paid;
using BC.PAYMENT.CORE.Entities;
using NewInvoiceModel = BC.PAYMENT.CORE.Entities.Invoice.NewInvoiceModel;

namespace BC.PAYMENT.APPLICATION.Interfaces.Invoice;

public interface IPaymentInvoiceRepository
{
    Task<bool> IsExistsPaymentHeaderId(string dbCode,DateTime invoiceDate,string deliveryId);       
    Task<int> CreatePaymentHeader(PaymentInvoiceHeader headerModel);
    Task<int> GetPaymentHeaderId(DateTime invoiceDividendDate, string deliveryId, string dbCode);
    Task<int> CreatePaymentDetailAsync(BcPaymentDetail model);
    Task<int> CreatePcPaymentInvoiceAsync(PcPaymentInvoice paymentInvoices);
    Task<List<PaymentInvoiceResponse>> GetPaymentInvoiceDetailByDateAsync(string dbCode, DateTime fromDate, DateTime toDate);
    Task<List<PaymentInvoiceResponse>> GetPaymentInvoiceDetailByPeriodAsync(string dbCode, int month, int year);
    Task<List<PaymentInvoiceResponse>> GetPaymentInvoiceDetailByInvoiceCodeAsync(string dbCode, DateTime date,string invoiceCode);
    Task<int> UpdatePaidValueAsync(PcPaymentInvoice paymentInvoice, NewInvoiceModel invoice, PcEditDividedInvoice editDividedInvoice);
    Task<int> DeletePaymentInvoiceAsync(int paymentId, int dividedId);
    Task<List<BcPaymentDetailResponse>> LoadBcPaymentDetailAsync(
        string dbCode,
        string? deliveryId = null,
        DateTime? date = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int? month = null,
        int? year = null,
        int? period = null);

    Task<List<PaymentInvoiceResponse>> GetPaymentInvoiceDetailExcludeSubmitInvoiceAsync(string dbCode,DateTime fromDate, DateTime toDate);
}