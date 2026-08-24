namespace BC.PAYMENT.APPLICATION.Interfaces.Report.DailyPaymentReport.Credit
{
    public interface ICreditInvoiceRepository
    {
        Task<List<CreditInvoiceReportModel>> GetCreditInvoiceAsync(string dbCode, DateTime fromDate, DateTime toDate);
    }
}
