using BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DailyPayment;
using BC.PAYMENT.CORE.Entities.Transaction.Submitting.HistoryApproval;

namespace BC.PAYMENT.APPLICATION.Interfaces.Transaction.Submitting.HistoryApproval
{
    public interface ISubmissionHistoryRepository
    {
        Task<List<SubmissionHistoryModel>> GetHistoryApprovalByPeriodAsync(string dbCode, int month, int year);
        Task<List<SubmissionHistoryModel>> GetHistoryApprovalByDateAsync(string dbCode, string fromDate, string toDate);
    }
}
