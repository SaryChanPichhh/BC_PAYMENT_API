using static BC.PAYMENT.CORE.Entities.Report.ProvincialPayment.CarPaymentModel;
namespace BC.PAYMENT.APPLICATION.Interfaces.Report.Provincial_Payment
{
    public interface ICarPaymentRepository
    {
        Task<List<CarPaymentCreditInvoiceModel>>  ReportAllCreditInvoiceByEmployeeIdAndDate(int employeeId, DateTime dateFrom, DateTime dateTo);
        Task<List<CarPaymentCreditInvoiceModel>>  ReportAllCreditInvoiceByEmployeeIdAndTemplateId(int employeeId, int templateId);

        Task<List<CarPaymentInvoiceModel>> ReportAllPaymentInvoiceByEmployeeIdAndTemplateId(string dbCode,int employeeId, int templateId);
        Task<List<CarPaymentInvoiceModel>> ReportAllPaymentInvoiceByEmployeeIdAndDate(int employeeId, DateTime dateFrom, DateTime dateTo);
        Task<List<CarPaymentExpenseModel>> ReportAllExpenseByEmployeeIdAndTemplateId(int employeeId, int templateId);
        Task<List<CarPaymentExpenseModel>>  ReportAllExpenseByEmployeeIdAndDate(int employeeId, DateTime dateFrom, DateTime dateTo);

        Task<List<TransferMoney>> ReportAllTransferByEmployeeIdAndDate(string dbCode,int employeeId, DateTime dateFrom,
            DateTime dateTo);

        Task<List<TransferMoney>> ReportAllTransferByEmployeeIdAndTemplateId(string dbCode,int employeeId, int templateId);


        Task<List<AmountInvoiceReportModel>> GetAllTotalCollectionByRequestId(int requestId);
        Task<List<TotalInvoiceReportModel>> GetAllTotalInvoiceByRequestId(int requestId);


        Task<List<TotalCollectionModel>> ReportAllTotalCollectionByEmployeeIdAndDate(int employeeId, DateTime dateFrom, DateTime dateTo);
        Task<List<TotalPaymentModel>> ReportAllPaymentByEmployeeIdAndDate(int employeeId, DateTime dateFrom, DateTime dateTo);
        Task<List<TotalCollectionModel>> ReportAllTotalCollectionByEmployeeIdAndTemplateId(int employeeId, int templateId);
        Task<List<TotalPaymentModel>> ReportAllPaymentByEmployeeIdAndTemplateId(int employeeId, int templateId);
        Task<List<TotalCollectionModel>> GetAllTotalCollectionByTemplateId(int templateId);
        Task<List<TotalPaymentModel>> GetAllPaymentByTemplateId(int templateId);
    }
}
