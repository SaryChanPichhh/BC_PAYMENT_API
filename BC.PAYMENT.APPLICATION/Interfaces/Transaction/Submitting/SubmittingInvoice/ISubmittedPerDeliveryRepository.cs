using BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DailyPayment;
using BC.PAYMENT.CORE.Entities.Transaction.Submitting.SubmittingInvoice;

namespace BC.PAYMENT.APPLICATION.Interfaces.Transaction.Submitting.SubmittingInvoice
{
    public interface ISubmittedPerDeliveryRepository
    {
        Task<List<ExpenseDetailModel>> GetSubmittedInvoicePerDeliveryAsync(string dbCode, string fromDate, string toDate);
        Task<List<ExpenseDetailModel>> GetSubmittedInvoicePerDeliveryByPeriodAsync(string dbCode, int month, int year);
        Task<int> DeleteSubmittedInvoiceAsync(string code);

    }
}
