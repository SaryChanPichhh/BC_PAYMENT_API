namespace BC.PAYMENT.APPLICATION.Interfaces.Report.DailyPaymentReport.ExpenseInvoice
{
    public interface IExpenseInvoiceReportRepository
    {
        Task<List<ExpenseInvoiceReportModel>> GetExpenseInvoiceReportsByDateAsync(string dbCode, DateTime fromDate, DateTime toDate);
    }
}
