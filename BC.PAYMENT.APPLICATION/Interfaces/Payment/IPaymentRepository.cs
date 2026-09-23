using BC.PAYMENT.CORE.Contracts.Payment;
using BC.PAYMENT.CORE.Entities;
using NewInvoiceModel = BC.PAYMENT.CORE.Entities.Invoice.NewInvoiceModel;

namespace BC.PAYMENT.APPLICATION.Interfaces.Payment
{
    public interface IPaymentRepository
    {
        Task<List<DeliveryPayment>> GetDeliveryPaidInvoice(string dbCode, string deliveryId , DateTime divideDate);
        Task<int> CreatePaymentHeader(PaymentInvoiceHeaderDTO headerModel);
        Task<int> GetPaymentHeaderId(DateTime invoiceDividendDate, string deliveryId, string dbCode);
        Task<bool> CheckExistsPaymentHeader(DateTime invoiceDividendDate, string deliveryId, string dbCode);
        Task<int> CreatePaymentExpense(List<InvoiceExpenseDTO> dto);
        Task<int> UpdatePaidValueAsync(PcPaymentInvoice paymentInvoice, NewInvoiceModel invoice, PcEditDividedInvoice editDividedInvoice);
        Task<int> DeletePaymentInvoiceAsync(int paymentId, int dividedId);
    }
}
