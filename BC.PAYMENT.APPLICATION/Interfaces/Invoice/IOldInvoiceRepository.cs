using BC.PAYMENT.CORE.Contracts.Criteria;
using BC.PAYMENT.CORE.Contracts.Invoice;
using BC.PAYMENT.CORE.Contracts.Request.Invoice;
using BC.PAYMENT.CORE.Contracts.Response.Invoice;
using BC.PAYMENT.CORE.Contracts.TablesType;

namespace BC.PAYMENT.APPLICATION.Interfaces.Invoice;

public interface IOldInvoiceRepository
{
    Task<List<OldInvoiceResponse>> GetOldInvoiceAsync(string dbCode, int offset, int pageSize,List<string>? transCodes = null);
    Task<List<OldInvoiceResponse>> GetAllOldInvoicesFromLedgerAsync(OldInvoiceCriteria crit);
    Task<int> SaveOldInvoice(List<OldInvoiceTableType> dto);
    Task<int> RecreateOldInvoiceAsync(InvoiceDTO invoiceDto);
    Task<OldInvoiceResponse> GetOldInvoiceByTransactionCode(string dbCode, string transactionCode);
    Task<int> UpdateStatus(string dbCode, string transaction);
    Task<int> DeleteOldInvoiceByIdAsync(int id);
}   