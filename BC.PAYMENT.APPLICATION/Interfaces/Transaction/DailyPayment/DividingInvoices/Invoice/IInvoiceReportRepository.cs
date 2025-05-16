using BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DividingInvoices.Invoice;

namespace BC.PAYMENT.APPLICATION.Interfaces.Transaction.DailyPayment.DividingInvoices.Invoice
{
    public interface IInvoiceReportRepository
    {
        Task<List<InvoiceReportModel>> GetInvoiceReportAsync(string dbCode, DateTime date);
    }
}
