namespace BC.PAYMENT.APPLICATION.Interfaces.Transaction.DailyPayment.DividingInvoices.Invoice
{
    public interface IIssuanceInvoiceRepository
    {
        Task<List<IssuanceModel>> GetIssuanceInvoiceAsync(string dbCode, string areaId);

        Task<List<IssuanceModel>> GetIssuanceInvoiceByTransactionAsync(string dbCode, string areaId,
            string transactionCode);
    }
}
