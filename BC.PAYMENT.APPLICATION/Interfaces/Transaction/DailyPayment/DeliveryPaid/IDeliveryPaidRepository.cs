using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BC.PAYMENT.CORE.Entities.Expense;
using BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DeliveryPaid;

namespace BC.PAYMENT.APPLICATION.Interfaces.Transaction.DailyPayment.DeliveryPaid
{
    public interface IDeliveryPaidRepository
    {
        Task<List<InvoicePaymentModel>> GetAllInvoiceByDeliveryIdAndDate(string dbCode,string deliveryId, DateTime date);
        Task<List<DeliveryDataObject>> GetAllInvoiceByDeliveryIdAndDateDataObjectAsync(string dbCode,string deliveryId, DateTime date);
        Task<List<InvoicePaymentModel>> LoadInvoicePaid(string dbCode,string deliveryId, DateTime date);
        Task<int> UpdateDeliveryInvoicePaid(DeliveryInvoicePaidUpdateModel model);
        Task<bool> CheckExistsPaymentHeaderByInvoiceDividendDateAndDeliveryId(DateTime invoiceDividendDate,
            string deliveryId, string dbCode);

        Task<int> CreatePaymentHeader(PaymentInvoiceHeaderModel headerModel);
        Task<bool> CreatePaymentExpense(List<ExpenseModel> expenseModel);
        Task<int> GetPaymentHeaderId(DateTime invoiceDividendDate, string deliveryId, string dbCode);
    }
}
