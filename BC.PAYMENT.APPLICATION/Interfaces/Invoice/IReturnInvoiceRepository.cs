using BC.PAYMENT.CORE.Contracts.Request.Invoice;
using BC.PAYMENT.CORE.Contracts.Response.Invoice;
using BC.PAYMENT.CORE.Entities;
using ReturnInvoiceModel =
    BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DividingInvoices.Invoice.ReturnInvoiceModel;

namespace BC.PAYMENT.APPLICATION.Interfaces.Invoice;

public interface IReturnInvoiceRepository
{
    Task<List<ReturnInvoiceResponse>> GetReturnInvoiceAsync(string dbCode);
    Task<List<ReturnInvoiceResponse>> GetReturnInvoiceByDateAsync(string dbCode, DateTime fromDate, DateTime toDate);

    Task<int> SaveReturnInvoiceAsync(List<ReturnInvoiceRequest> requests, string dbCode, string username,
        string entryCode);

    Task<int> CreatePcReturnInvoiceAsync(PcReturnInvoice returnInvoice);
    Task<List<PcReturnInvoice>> GetPcReturnInvoiceAsync(string dbCode);
    Task<List<ReturnInvoiceResponse>> GetPcReturnInvoiceByDateAsync(string dbCode, DateTime fromDate, DateTime toDate);
    Task<List<ReturnInvoiceResponse>> GetPcReturnInvoiceByPeriodAsync(string dbCode, string month, string year);
    Task<int> DeletePcReturnInvoiceAsync(int dividedId);

    Task<List<PcReturnInvoiceAuditResponse>> GetReturnChangeInvoiceByDateAsync(string dbCode, DateTime fromDate,
        DateTime toDate);

    Task<List<PcReturnInvoiceAuditResponse>> GetReturnChangeInvoiceByPeriodAsync(string dbCode, int month, int year);
    Task<int> InsertGetReturnInvoice(PcReturningInvoiceAudit model);
}