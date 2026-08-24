namespace BC.PAYMENT.APPLICATION.Interfaces.Report.Provincial_Payment
{
    public interface ICarPaymentReportRepository
    {
        Task<List<CarPaymentReportModel>> GetCarPaymentReportByDateAsync(DateTime fromDate,DateTime toDate);
        Task<List<CarPaymentReportModel>> GetCarPaymentReportByPeriodAsync(int period);
    }
}
