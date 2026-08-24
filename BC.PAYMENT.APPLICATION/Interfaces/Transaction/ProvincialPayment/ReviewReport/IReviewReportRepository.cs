namespace BC.PAYMENT.APPLICATION.Interfaces.Transaction.ProvincialPayment.ReviewReport
{
    public interface IReviewReportRepository
    {
        Task<List<RequestionActionDto>> GetAllActionAsync();
        Task<List<StockCarRequestDto>> GetAllRequestByActionIdAsync(string dbCode,int actionId);
        Task<List<StockCarRequestDto>> GetAllRequestByActionIdAndRoleIdAsync(string dbCode,int actionId, List<int> roleId);
        // Credit Invoice
        Task<List<ReviewReportCreditInvoice>> GetAllCreditInvoiceByRequestIdAndCheckingStatus(int requestId,bool checkingStatus);
        Task<int> UpdateStatusCheckingCreditInvoiceBySubmitId(int submitId, bool checkingStatus);

        // Paid Invoice
        Task<List<PaidInvoiceRequestDto>> GetAllPaidInvoiceByRequestIdAndCheckingStatus(int requestId, bool status);
        Task<int> UpdateStatusCheckingPaymentInvoiceByInvoiceId(int invoiceId , Boolean status);
        Task<List<HistoryPaymentInvoiceRespondDto>> GetAllHistoryPaymentInvoiceByTransactionCode(string transactionCode);

        // Transfer Money
        Task<List<TransferMoneyModel>> GetAllTransferByRequestIdAndCheckingStatus(int requestId, bool status);
        Task<int> UpdateStatusCheckingTransferBySubmitTransferId(int transferId, bool checkingStatus);


        // Expense
         Task<List<ReviewReportExpenseDto>>GetAllExpenseByRequestIdAndCheckingStatus(int requestId, bool status);
         Task<int> UpdateStatusCheckingExpenseByInvoiceId(int requestExpenseId, bool checkingStatus);

         // Approval History
         Task <List<ApprovalInvoiceHistoryModel>> GetAllApprovalHistoryByRequestIdAsync(int requestId,bool status);
    }
}
