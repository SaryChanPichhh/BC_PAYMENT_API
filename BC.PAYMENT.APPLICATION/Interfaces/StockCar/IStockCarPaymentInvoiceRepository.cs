using BC.PAYMENT.CORE.Contracts.Response.StockCar;
using BC.PAYMENT.CORE.Entities.StockCar;

namespace BC.PAYMENT.APPLICATION.Interfaces.StockCar;

public interface IStockCarPaymentInvoiceRepository
{
    Task<List<PaymentInvoiceResponse>> GetPaymentInvoicesByTemplateIdAsync(string dbCode, int templateId);
    Task<int> PaymentInvoiceAsync(int invoiceId, double amount, string createdBy = "");
    int PaymentInvoice(int invoiceId, double amount, string createdBy = "");
    Task<int> InsertPaymentInvoiceAsync(int invoiceId, double amountPaid, string createdBy);
    Task<int> DeletePaymentInvoiceByInvoiceIdAsync(int invoiceId);
    Task<int> DeletePaymentInvoiceByInvoiceId(int invoiceId);
    Task<int> UpdatePaymentInvoiceByIdAsync(int id, double amount);
    Task<int> UpdatePaymentInvoiceById(int id, double amount);
    Task<List<InvoicesPayment>> GetPaymentHistoryByInvoiceIdAsync(int invoiceId);
    Task<List<InvoicesPayment>> GetPaymentHistoryByInvoiceId(int invoiceId);
}
