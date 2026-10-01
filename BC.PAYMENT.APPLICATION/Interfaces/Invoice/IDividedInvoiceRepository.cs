using BC.PAYMENT.CORE.Contracts.Invoice;
using BC.PAYMENT.CORE.Contracts.Request.Invoice;
using BC.PAYMENT.CORE.Contracts.Response.Invoice;

namespace BC.PAYMENT.APPLICATION.Interfaces.Invoice;

public interface IDividedInvoiceRepository
{
    Task<List<DividedInvoiceResponse>> GetDividedInvoicesByDeliveryIdAndDateAsync(string dbCode, string deliveryId,
        DateTime date);

    Task<List<DividedInvoiceDetailResponse>> GetDividedInvoiceDetailByDateAsync(string dbCode, DateTime fromDate,
        DateTime toDate);

    Task<int> DeleteDividedInvoiceAsync(int invoiceId, string transactionCode, string deliveryId, string note);
    Task<int> SaveDividedInvoiceAsync(List<CreateDividedInvoiceRequest> requests, string dbCode, string userName);
    Task<bool> CheckExistsDividedInvoice(int invoiceId);
    Task<List<DividedInvoiceSummaryResponse>> GetDividedInvoiceReportAsync(string dbCode, DateTime date);

    Task<List<DividedInvoiceStatusResponse>> GetDividedInvoiceStatusByDateAsync(string dbCode, DateTime dividedDate,
        string deliveryId);
}