namespace BC.PAYMENT.APPLICATION.Interfaces.Transaction.Submitting.SubmittingInvoice
{
    public interface ISubmittedInvoiceRepository
    {
        Task<List<SubmittedInvoiceModel>> GetAllNotSubmitPaidInvoice(string dbCode, string fromDate, string toDate);
        Task<int> UpdateInvoiceFromPendingToCancelAsync(string dbCode,string createBy, string submittedId);
    }
}
