using ReturnInvoiceModel = BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DividingInvoices.Invoice.ReturnInvoiceModel;

namespace BC.PAYMENT.APPLICATION.Interfaces.Transaction.DailyPayment.DividingInvoices.Invoice
{
    public interface IReturnInvoiceRepository
    {
        Task<List<ReturnInvoiceModel>> GetReturnInvoiceAsync(string dbCode);
        Task<List<ReturnInvoiceModel>> GetReturnInvoiceByDateAsync(string dbCode, DateTime fromDate, DateTime toDate);

    }
}
