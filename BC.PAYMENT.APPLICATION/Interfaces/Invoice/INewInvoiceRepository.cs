using BC.PAYMENT.CORE.Contracts.Request.Invoice;
using BC.PAYMENT.CORE.Contracts.Response.Invoice;
using BC.PAYMENT.CORE.Entities;
using NewInvoiceModel = BC.PAYMENT.CORE.Entities.Invoice.NewInvoiceModel;

namespace BC.PAYMENT.APPLICATION.Interfaces.Invoice;

public interface INewInvoiceRepository
{
    Task<List<NewInvoiceResponse>> GetInvoices(InvoiceStatus type, string dbCode, DateTime createDate);

    Task<List<NewInvoiceResponse>> GetInvoicesByInvoiceCode(string startInvoiceCode, string endInvoiceCode,
        DateTime fromDate, DateTime endDate, string dbCode);

    Task<int> SaveInvoices(List<NewInvoiceModel> invoices);
    Task<int> DeleteInvoiceAsync(int invoiceId);
    Task<int> UpdateHeaderValueAsync(PcEditDividedInvoice request);
}