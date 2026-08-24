namespace BC.PAYMENT.APPLICATION.Interfaces.Transaction.DailyPayment.DividingInvoices.Invoice
{
    public interface IDividedInvoiceRepository
    {
        Task<List<DividedInvoiceModel>> GetDividedInvoicesByDeliveryIdAndDateAsync(string dbCode, string deliveryId, DateTime date);
        Task<int> DeleteDividedInvoiceAsync(string invoiceId, string transactionCode, string deliveryId, string note);
    }
}
