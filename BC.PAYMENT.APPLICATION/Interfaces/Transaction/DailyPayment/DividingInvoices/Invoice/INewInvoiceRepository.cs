using NewInvoiceModel = BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DividingInvoices.Invoice.NewInvoiceModel;

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
