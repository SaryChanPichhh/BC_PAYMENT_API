using BC.PAYMENT.CORE.Contracts.Response.StockCar;
using BC.PAYMENT.CORE.Entities;

namespace BC.PAYMENT.APPLICATION.Interfaces.StockCar;

public interface IStockCarInvoiceRepository
{
    Task<List<StockCarInvoiceResponse>> GetStockCarInvoicesByInvoiceTypeAsync(string dbCode,int templateId,InvoiceStatus invoiceStatus);
    Task<List<PaymentInvoiceResponse>> GetPaymentInvoicesByTemplateIdAsync(string dbCode, int templateId);
    Task<int> DeleteStockCarInvoiceByIdAsync(int id);
    Task<int> AddNewStockCarInvoiceAsync(BcStockCar model);
}