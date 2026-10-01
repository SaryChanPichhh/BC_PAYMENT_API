namespace BC.PAYMENT.APPLICATION.Interfaces.General;

public interface IInvoiceClosingEntryRepository
{
    Task<bool> CheckIsEntriesIsAlreadyOpenAsync(string dbCode);
    Task<int> CreateClosingEntryAsync(InvoiceClosingEntriesModel closingEntry);
    Task<string> GenerateOpeningEntryCodeAsync(string dbCode);
    Task<string> GetOpeningEntryCodeByDbCodeAsync(string dbCode);
}