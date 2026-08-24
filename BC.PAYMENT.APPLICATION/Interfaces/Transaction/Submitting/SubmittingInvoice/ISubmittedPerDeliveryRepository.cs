namespace BC.PAYMENT.APPLICATION.Interfaces.Transaction.Submitting.SubmittingInvoice
{
    public interface ISubmittedPerDeliveryRepository
    {
        Task<List<ExpenseDetailModel>> GetSubmittedInvoicePerDeliveryAsync(string dbCode, string fromDate, string toDate);
        Task<List<ExpenseDetailModel>> GetSubmittedInvoicePerDeliveryByPeriodAsync(string dbCode, int month, int year);
        Task<int> DeleteSubmittedInvoiceAsync(string code);

    }
}
