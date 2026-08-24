namespace BC.PAYMENT.APPLICATION.Interfaces.Report.DailyPaymentReport.PaidInvoice
{
    public interface IPaidInvoiceRepository
    {
        Task<List<PaidInvoiceReportModel>> GetPaidInvoiceByDateAsync(string dbCode, DateTime fromDate, DateTime toDate);
    }
}
