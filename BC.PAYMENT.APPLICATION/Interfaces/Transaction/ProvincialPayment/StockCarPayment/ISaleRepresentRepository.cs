using BC.PAYMENT.CORE.Contracts.Criteria;
using BC.PAYMENT.CORE.Contracts.Invoice;
using BC.PAYMENT.CORE.Contracts.Request.Invoice;
using BC.PAYMENT.CORE.Contracts.Transaction.ProvincialPayment.StockCarPayment;

namespace BC.PAYMENT.APPLICATION.Interfaces.Transaction.ProvincialPayment.StockCarPayment
{
    public  interface ISaleRepresentRepository : IBaseRepository<SaleRepresentModel>
    {
        Task<int> DisableTemplateById(string createBy,int templateId);
        Task<SaleRepresentModel> GetAllTemplateByEmployeId(int employeeId);
        Task<List<OldInvoiceResponeDto>> GetAllInvoiceAsync(string dbCode,string fromSaleCode,string toSaleCode,DateTime fromDate ,DateTime toDate);
        Task<List<StockCarInvoiceModel>> GetAllInvoiceByInvoiceTypeAsync(string dbCode, InvoiceStatus invoiceType,int templateId);
        Task<int> DeleteInvoiceByIdAsync(int invoiceId);
        Task<int> UpdateInvoiceByIdAsync(StockCarInvoiceModel model);

        Task<List<OldInvoiceResponeDto>> GetOldInvoiceByRangeAsync(OldInvoiceCriteria model);
        Task<List<OldInvoiceResponeDto>> GetOldInvoiceByAllAsync(OldInvoiceCriteria model);
        Task<int> InsertInvoiceAsync(InvoicesModel model);

        // Transfer Money
        Task<List<TransferMoneyModel>> GetTransferMoneyByTemplateIdAsync(string dbCode,int templateId);
        Task<int> AddNewTransferMoneyAsync(TransferMoneyModel model);
        Task<int> UpdateTransferMoneyAsync(TransferMoneyModel model);
        Task<int> DeleteTransferMoneyAsync(int transferId);

        // Expense 
        Task<List<StockCarExpenseModel>> GetExpenseByTemplateIdAsync(string dbCode, int templateId);
        Task<int> AddNewExpenseAsync(StockCarExpenseModel model);
        Task<int> UpdateExpenseAsync(StockCarExpenseModel model);
        Task<int> DeleteExpenseAsync(int expenseId);

        // Payment Invoice
        Task<List<PaymentInvoiceDto>> GetPaymentInvoicesByTemplateIdAsync(string dbCode, int templateId);
        Task<PaymentInvoiceDto> GetPaymentInvoicesByTransactionCodeAsync(string dbCode, string transactionCode);
        Task<int> InsertPaymentInvoiceAsync(PaymentInvoiceDto model);

        // Returning Invoice
        Task<List<ReturningInvoiceDto>> GetReturningInvoicesByTemplateIdAsync(string dbCode, int templateId);
        Task<int> AddNewReturningInvoiceAsync(ReturningInvoiceDto model);

        // Payment Invoice
        Task<List<PaymentModel>> GetPaymentByTemplateIdAsync( int templateId);
        Task<List<CollectionPaymentModel>> GetAllTotalCollectionByTemplateIdAsync( int templateId);

        // Credit Invoice
        Task<List<StockCarCreditInvoiceDto>> GetAllCreditInvoiceByTemplateIdAsync(string dbCode,int templateId);
    }
}
