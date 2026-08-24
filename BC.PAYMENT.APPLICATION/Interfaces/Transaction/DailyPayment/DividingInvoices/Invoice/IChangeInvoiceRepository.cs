namespace BC.PAYMENT.APPLICATION.Interfaces.Transaction.DailyPayment.DividingInvoices.Invoice
{
    public interface IChangeInvoiceRepository
    {
        Task<List<ChangeInvoiceModel>> GetLocalInvoiceAsync(string dbCode, DateTime fromDate, DateTime toDate);
        Task<List<ChangeInvoiceModel>> GetOtherBranchInvoiceAsync(string dbCode, DateTime fromDate, DateTime toDate);
        Task<int> InsertIfNotExistsInvoiceAsync(ChangeInvoiceModel model);
        Task<int> InsertIfExistsInvoiceAsync(ChangeInvoiceModel model);
    }
}
