namespace BC.PAYMENT.APPLICATION.Interfaces.CommondityExchange.CreditNoteReport;

public interface ICreditNoteReportRepository
{
    Task<List<CreditNoteReportDto>> GetCreditNoteReportsAsync(string dbCode, DateTime fromDate, DateTime toDate);
}