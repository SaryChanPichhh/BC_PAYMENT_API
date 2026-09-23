namespace BC.PAYMENT.APPLICATION.Interfaces.Transaction.Audit.VerifyInvoice
{
    public interface IVerifyInvoiceRepository
    {
        Task<List<VerifyInvoiceModel>> GetVerifyInvoicesAsync(string dbCode);
        Task<int> InsertVerifyInvoicesAsync(List<OldInvoiceModel> model);
    }
}
