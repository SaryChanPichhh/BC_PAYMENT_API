namespace BC.PAYMENT.APPLICATION.Interfaces.CommondityExchange.ItemExchangeReport;

public interface IItemExchangeReportRepository
{
    Task<List<ExchangeItemReportDto>> GetExchangeItemInvoiceAsync(string dbCode, DateTime fromDate, DateTime toDate);
    Task<List<ItemReceivedDto>> GetReceivedExchangeItemInvoiceAsync(string dbCode, DateTime fromDate, DateTime toDate);
    Task<List<ReportExchangePendingItemDto>> GetPendingExchangeItemInvoiceAsync(string dbCode);

    Task<List<ReportExchangeDto>> GetExchangeInvoiceNotYetSendToCustomerReportAsync(string dbCode, DateTime fromDate,
        DateTime toDate);

    Task<List<ReportExchangeDto>> GetExchangeInvoiceAlreadySendToCustomerReportAsync(string dbCode, DateTime fromDate,
        DateTime toDate);
}