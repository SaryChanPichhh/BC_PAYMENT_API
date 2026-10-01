namespace BC.PAYMENT.APPLICATION.Interfaces.Report.OthersReport;

public interface ICustomerReportRepository
{
    Task<List<CustomerReportModel>> GetCustomerReportByDateAsync(string dbCode, DateTime fromDate, DateTime toDate);
}