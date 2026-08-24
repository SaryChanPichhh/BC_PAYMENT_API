namespace BC.PAYMENT.APPLICATION.Interfaces.Transaction.Submitting.InvoiceVerify
{
    public interface IMonthlyInvoiceRepository
    {
        Task<List<MonthlyInvoiceModel>> GetInvoiceVerifyByDateAsync(string dbCode, string fromDate, string toDate);
        Task<List<MonthlyInvoiceModel>> GetInvoiceVerifyByPeriodAsync(string dbCode, int month, int year);
    }
}
