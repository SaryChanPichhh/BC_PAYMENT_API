namespace BC.PAYMENT.APPLICATION.Interfaces.Preset.OwedInvoice
{
    public interface IOwedInvoiceRepository
    {
        Task<List<SummaryAccountsReceivableModel>> GetAccountReceivableSummaries(Dictionary<string, string> dbCodes, int page, int pageSize);
        Task<List<OwedInvoiceDto>> GetAccountReceivableAmount(Dictionary<string, string> dbCodes);
    }
}
