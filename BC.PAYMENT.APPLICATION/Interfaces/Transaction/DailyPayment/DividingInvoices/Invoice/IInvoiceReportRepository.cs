namespace BC.PAYMENT.APPLICATION.Interfaces.Transaction.DailyPayment.DividingInvoices.Invoice
{
    public interface IInvoiceReportRepository
    {
        Task<List<InvoiceReportModel>> GetInvoiceReportAsync(string dbCode, DateTime date);
    }
}
