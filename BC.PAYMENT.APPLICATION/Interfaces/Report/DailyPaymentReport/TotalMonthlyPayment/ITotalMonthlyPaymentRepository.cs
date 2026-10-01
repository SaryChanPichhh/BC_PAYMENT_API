namespace BC.PAYMENT.APPLICATION.Interfaces.Report.DailyPaymentReport.TotalMonthlyPayment;

public interface ITotalMonthlyPaymentRepository
{
    Task<List<MonthlyPaymentModel>> GetAllPaidInvoiceByDateAsync(string dbCode, DateTime fromDate, DateTime toDate);

    Task<List<Dictionary<string, object>>> GetMonthlyHistoryPaidInvoiceByDateAsync(string dbCode, DateTime fromDate,
        DateTime toDate);
}