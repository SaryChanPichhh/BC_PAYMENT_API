using BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DividingInvoices.Invoice;
using BC.PAYMENT.CORE.Enums;

namespace BC.PAYMENT.APPLICATION.Interfaces.Transaction.DailyPayment.DividingInvoices.Invoice
{
    public interface INewInvoiceRepository
    {
        Task<List<NewInvoiceModel>> GetInvoices(InvoiceTypes type, string dbCode, DateTime createDate);
        Task<List<NewInvoiceModel>> GetInvoicesByInvoiceCode(string startInvoiceCode, string endInvoiceCode, string fromDate, string endDate, string dbCode);
        Task<int> SaveInvoices(List<NewInvoiceModel> invoices);
        Task<List<string>> GetSaleTypes(string dbCode);
    }
}
