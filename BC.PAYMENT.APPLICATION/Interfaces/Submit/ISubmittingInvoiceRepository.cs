using BC.PAYMENT.CORE.Contracts.Response.Paid;
using BC.PAYMENT.CORE.Contracts.Response.SubmitInvoice;
using BC.PAYMENT.CORE.Entities;

namespace BC.PAYMENT.APPLICATION.Interfaces.Submit;

public interface ISubmittingInvoiceRepository
{
    Task<List<PaymentInvoiceResponse>> GetAllNotSubmitPaidInvoice(string dbCode, string fromDate, string toDate);
    Task<int> AddSubmittedInvoices(List<BcInvoiceSubmitted> submittedInvoices);
    Task<int> CancelingSubmittedInvoiceAsync(string dbCode, string createBy, string submittedId);
    Task<List<SubmitInvoiceResponse>> GetSubmittedInvoiceByDateAsync(string dbCode, string fromDate, string toDate);

    Task<List<SubmitInvoiceResponse>> GetSubmittedPendingInvoiceByDateAsync(string dbCode, string fromDate,
        string toDate);

    Task<List<ApproveSubmitInvoiceResponse>> GetApprovedSubmittedInvoiceByDateAsync(string dbCode, string fromDate,
        string toDate);

    Task<List<ApproveSubmitInvoiceResponse>> GetApprovedSubmittedInvoicePeriodAsync(string dbCode, int year, int month);

    Task<List<ApproveSubmitInvoiceResponse>> GetApprovedSubmittedInvoiceByInvoiceCodeAndDateAsync(string dbCode,
        DateTime date, string invoiceCode);

    Task<bool> UpdateStatusBcInvoiceSubmittedAsync(string submittedInvoiceId, string status);
    Task<int> AddNewApprovalInvoice(BcApprovalInvoice request);
}