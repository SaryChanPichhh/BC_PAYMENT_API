

using BC.PAYMENT.CORE.Entities.Transaction.Submitting.SubmittingInvoice;

namespace BC.PAYMENT.APPLICATION.Interfaces.Transaction.Submitting.SubmittingInvoice
{
    public interface ISubmittingInvoiceRepository
    {
        Task<List<SubmittedInvoiceModel>> GetAllNotSubmitPaidInvoice(string dbCode,string fromDate,string toDate);
        Task<int> AddSubmittedInvoices(List<SubmittedInvoiceModel> submittedInvoices);
        Task<int> AddSubmittedInvoices(SubmittedInvoiceModel submittedInvoice);
    }
}
