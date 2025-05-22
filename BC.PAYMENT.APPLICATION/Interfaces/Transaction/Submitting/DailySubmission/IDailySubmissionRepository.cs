using BC.PAYMENT.CORE.Entities.Transaction.Submitting.DailySubmission;

namespace BC.PAYMENT.APPLICATION.Interfaces.Transaction.Submitting.DailySubmission
{
    public interface IDailySubmissionRepository
    {
        Task<List<DailySubmissionModel>> GetSubmittedInvoiceByDateAsync(string dbCode,string fromDate,string toDate,int page,int pageSize);
        Task<List<DailySubmissionModel>> GetSubmittedInvoiceByPeriodAsync(string dbCode,int month,int year, int page, int pageSize);
        Task<List<ApprovalInvoiceModel>> GetApprovalListByDateAsync(string dbCode,string fromDate,string toDate);
        Task<List<ApprovalInvoiceModel>> GetApprovalListByPeriodAsync(string dbCode,int month, int year);
        Task<int> FindSubmittedInvoiceByTransactionCodeAsync(string dbCode,string transactionCoed);
        Task<int> UpdateStatusSubmittedInvoiceBySubmittedInvoiceId(int submittedId,string createBy);
        Task<List<HistoryPaymentModel>> FindHistoryPaymentByTransactionCode(string dbCode,string transactionCode);
        Task<List<string>> GetApprovedInvoiceAsync(string dbCode);
        Task<List<string>> GetRejectedInvoiceAsync(string dbCode);
        Task<List<string>> GetReSubmitInvoiceAsync(string dbCode);
    }
}
