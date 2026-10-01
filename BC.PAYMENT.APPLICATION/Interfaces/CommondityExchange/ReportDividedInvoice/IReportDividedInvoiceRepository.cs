namespace BC.PAYMENT.APPLICATION.Interfaces.CommondityExchange.ReportDividedInvoice;

public interface IReportDividedInvoiceRepository
{
    Task<List<ReportDividedInvoiceDto>> GetReportDividedInvoiceAsync(string dbCode, DateTime fromDate, DateTime toDate);
}