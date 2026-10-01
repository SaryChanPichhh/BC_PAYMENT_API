namespace BC.PAYMENT.APPLICATION.Interfaces.Report.OthersReport;

public interface IBillsOwedRepository
{
    Task<List<Dictionary<string, object>>> GetBillOwedByDateAsync(string dbCode, string month);
}