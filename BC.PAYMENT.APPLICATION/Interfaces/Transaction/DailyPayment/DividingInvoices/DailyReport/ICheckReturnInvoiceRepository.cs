namespace BC.PAYMENT.APPLICATION.Interfaces.Transaction.DailyPayment.DividingInvoices.DailyReport
{
    public interface ICheckReturnInvoiceRepository
    {
        Task<List<PaymentInvoiceModel>> GetAllNewAndChangeDividedInvoiceByDate(string dbCode, DateTime fromDate, DateTime toDate);
        //Task<int> AddNewPaymentInvoiceByDate(string dbCode,DateTime fromDate, DateTime toDate);
    }
}
