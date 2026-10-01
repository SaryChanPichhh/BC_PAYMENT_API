namespace BC.PAYMENT.APPLICATION.Interfaces.Report.DailyPaymentReport.SummaryInvoice;

public interface ISummaryInvoiceRepository
{
    Task<List<SummaryInvoiceReportModel>> GetSummaryInvoiceReportsByDateAsync(string dbCode, DateTime fromDate,
        DateTime toDate);
}