namespace BC.PAYMENT.APPLICATION.Interfaces.Transaction.DailyPayment.DeliveryPaid;

public interface IDeliveryPaidRepository
{
    Task<List<GeneralInvoicePaymentModel>> GetAllInvoiceByDeliveryIdAndDate(string dbCode, string deliveryId,
        DateTime date);

    Task<List<DeliveryDataObject>> GetAllInvoiceByDeliveryIdAndDateDataObjectAsync(string dbCode, string deliveryId,
        DateTime date);

    Task<List<GeneralInvoicePaymentModel>> LoadInvoicePaid(string dbCode, string deliveryId, DateTime date);
    Task<int> UpdateDeliveryInvoicePaid(DeliveryGeneralInvoicePaidUpdateModel model);

    Task<bool> CheckExistsPaymentHeaderByInvoiceDividendDateAndDeliveryId(DateTime invoiceDividendDate,
        string deliveryId, string dbCode);

    Task<int> CreatePaymentHeader(PaymentInvoiceHeaderModel headerModel);
    Task<bool> CreatePaymentExpense(List<ExpenseModel> expenseModel);
    Task<int> GetPaymentHeaderId(DateTime invoiceDividendDate, string deliveryId, string dbCode);
}