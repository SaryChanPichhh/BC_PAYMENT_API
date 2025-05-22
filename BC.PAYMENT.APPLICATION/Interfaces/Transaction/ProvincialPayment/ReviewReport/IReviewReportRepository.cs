
using BC.PAYMENT.CORE.DTO.Transaction.ProvincialPayment.ReviewReport;
using BC.PAYMENT.CORE.DTO.Transaction.ProvincialPayment.StockCarPayment;

namespace BC.PAYMENT.APPLICATION.Interfaces.Transaction.ProvincialPayment.ReviewReport
{
    public interface IReviewReportRepository
    {
        Task<List<RequestionActionDto>> GetAllActionAsync();
        Task<List<StockCarRequestDto>> GetAllRequestByActionIdAsync(string dbCode,int actionId);
        Task<List<StockCarRequestDto>> GetAllRequestByActionIdAndRoleIdAsync(string dbCode,int actionId, List<int> roleId);
        Task<List<ReviewReportCreditInvoice>> GetAllCreditInvoiceByRequestIdAndCheckingStatus(int requestId);
        Task<int> UpdateStatusCheckingCreditInvoiceBySubmitId(int submitId, bool checkingStatus);
        Task<List<PaidInvoiceRequestDto>> GetAllPaymentInvoiceByRequestIdAndCheckingStatus(int requestId, bool status);
    }
}
