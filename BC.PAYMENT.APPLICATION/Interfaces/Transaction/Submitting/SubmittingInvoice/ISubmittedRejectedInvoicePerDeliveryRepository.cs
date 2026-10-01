namespace BC.PAYMENT.APPLICATION.Interfaces.Transaction.Submitting.SubmittingInvoice;

public interface ISubmittedRejectedInvoicePerDeliveryRepository
{
    Task<List<RejectedInvoicePerDelivery>> GetAllRejectedInvoicePerDeliveryByDateAsync(string dbCode, string fromDate,
        string toDate);

    Task<List<RejectedInvoicePerDelivery>> GetAllRejectedInvoicePerDeliveryByPeriodAsync(string dbCode, int month,
        int year);
}