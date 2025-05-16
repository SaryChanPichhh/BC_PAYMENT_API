
using BC.PAYMENT.CORE.DTO.Transaction.DailyPayment.DailyPayment;
using BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DailyPayment;
using BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DeliveryPaid;
using ReturnInvoiceModel = BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DailyPayment.ReturnInvoiceModel;

namespace BC.PAYMENT.APPLICATION.Interfaces.Transaction.DailyPayment.DailyPayment
{
    public interface IDailyPaymentRepository
    {
        Task<List<PaidInvoiceModel>> GetPaidInvoiceByPeriodAsync(string dbCode,int month,int year);
        Task<List<PaidInvoiceModel>> GetPaidInvoiceByDateAsync(string dbCode,string fromDate,string toDate);
        Task<int> DeletePaidInvoiceAsync(int paymentId,int dividedId);
        Task<int> UpdatePaidInvoiceAsync(DeliveryInvoicePaidUpdateModel model);

        // Money Control
        Task<List<ExpenseDetailModel>> GetExpenseDetailByPeriodAsync(string dbCode, int month, int year);
        Task<List<ExpenseDetailModel>> GetExpenseDetailByDateAsync(string dbCode, string fromDate, string toDate);
        Task<int> UpdateExpenseDetailAsync(ExpenseDetailDto model);

        // Return Invoice Control
        Task<List<ReturnInvoiceModel>> GetReturnInvoiceByDateAsync(string dbCode,DateTime fromDate,DateTime toDate);
        Task<List<ReturnInvoiceModel>> GetReturnInvoiceByPeriodAsync(string dbCode,string month,string year);
        Task<int> UpdateReturnInvoiceAsync(string dividedId);

        // Return Change Invoice 
        Task<List<ReturnChangeInvoiceModel>> GetReturnChangeInvoiceByDateAsync(string dbCode, DateTime fromDate, DateTime toDate);
        Task<List<ReturnChangeInvoiceModel>> GetReturnChangeInvoiceByPeriodAsync(string dbCode,int month, int year);
        Task<int> InsertGetReturnInvoice(ReturnChangeInvoiceModel model);
    }
}
