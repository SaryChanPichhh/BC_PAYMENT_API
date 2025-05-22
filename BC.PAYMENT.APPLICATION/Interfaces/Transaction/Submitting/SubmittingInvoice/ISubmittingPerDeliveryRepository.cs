
using BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DailyPayment;

namespace BC.PAYMENT.APPLICATION.Interfaces.Transaction.Submitting.SubmittingInvoice
{
    public interface ISubmittingPerDeliveryRepository
    {
        Task<List<ExpenseDetailModel>> GetSubmittedInvoicesAsync( string dbCode, string fromDate, string toDate);
        Task<int> AddNewSubmittedInvoicesAsync(ExpenseDetailModel model);
    }
}
