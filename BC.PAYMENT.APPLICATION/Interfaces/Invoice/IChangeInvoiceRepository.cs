using BC.PAYMENT.CORE.Contracts.Response.Invoice;

namespace BC.PAYMENT.APPLICATION.Interfaces.Invoice;

public interface
    IChangeInvoiceRepository
{
    Task<List<ChangeInvoiceResponse>> GetChangeInvoicesAsync(string dbCode, DateTime createDate);
    Task<bool> CheckExistInvoiceAsync(string transaction, string dbCode);
    Task<List<ChangeInvoiceResponse>> GetLocalInvoiceAsync(string dbCode, DateTime fromDate, DateTime toDate);
    Task<List<ChangeInvoiceResponse>> GetOtherBranchInvoiceAsync(string dbCode, DateTime fromDate, DateTime toDate);
    Task<int> AddChangeInvoiceAsync(ChangeInvoiceModel model);
}