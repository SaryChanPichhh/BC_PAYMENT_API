namespace BC.PAYMENT.APPLICATION.Interfaces.Report.DailyPaymentReport.AmountCollected;

public interface IAmountCollectedRepository
{
    Task<List<ExpenseDetailModel>> GetAmountCollectedReportsByDateAsync(string dbCode, DateTime fromDate,
        DateTime toDate);
}