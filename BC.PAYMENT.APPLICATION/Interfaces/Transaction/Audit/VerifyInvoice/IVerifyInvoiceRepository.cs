using BC.PAYMENT.CORE.Entities.Invoice;
using BC.PAYMENT.CORE.Entities.Transaction.Audit.VerifyInvoice;

namespace BC.PAYMENT.APPLICATION.Interfaces.Transaction.Audit.VerifyInvoice
{
    public interface IVerifyInvoiceRepository
    {
        Task<List<VerifyInvoiceModel>> GetVerifyInvoicesAsync(string dbCode);
        Task<int> InsertVerifyInvoicesAsync(List<OldInvoicesModel> model);
    }
}
