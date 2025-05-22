using BC.PAYMENT.CORE.Entities.Transaction.Submitting.SubmittingInvoice;

namespace BC.PAYMENT.APPLICATION.Interfaces.Transaction.Submitting.SubmittingInvoice
{
    public interface ISubmittedRejectedInvoiceRepository
    {
        Task<List<RejectedInvoiceModel>> GetAllRejectedInvoicesByDateAsync(string dbCode, string fromDate, string toDate);
        Task<List<RejectedInvoiceModel>> GetAllRejectedInvoicesByPeriodAsync(string dbCode, int month, int year);
        Task<int> UpdateSubmittedInvoiceFromRejectToCancel(string createBy, string transactionCode, string invoiceId);
    }
}
