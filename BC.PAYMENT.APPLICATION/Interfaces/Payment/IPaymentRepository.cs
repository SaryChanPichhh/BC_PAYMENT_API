using BC.PAYMENT.CORE.DTO.Payment;
using BC.PAYMENT.CORE.Entities.Payment;

namespace BC.PAYMENT.APPLICATION.Interfaces.Payment
{
    public interface IPaymentRepository
    {
        Task<List<DeliveryPayment>> GetDeliveryPaidInvoice(string dbCode, string deliveryId , DateTime divideDate);
        Task<int> CreatePaymentHeader(PaymentInvoiceHeaderDTO headerModel);
        Task<int> GetPaymentHeaderId(DateTime invoiceDividendDate, string deliveryId, string dbCode);
        Task<bool> CheckExistsPaymentHeader(DateTime invoiceDividendDate, string deliveryId, string dbCode);
        Task<int> CreatePaymentExpense(List<InvoiceExpenseDTO> dto);
    }
}
