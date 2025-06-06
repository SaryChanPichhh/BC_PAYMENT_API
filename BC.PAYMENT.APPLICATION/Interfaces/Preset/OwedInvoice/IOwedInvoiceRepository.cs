
using BC.PAYMENT.CORE.DTO.Preset.OwedInvoiceDto;
using BC.PAYMENT.CORE.Entities.Preset.OwedInvoice;
using System.Data;

namespace BC.PAYMENT.APPLICATION.Interfaces.Preset.OwedInvoice
{
    public interface IOwedInvoiceRepository
    {
        Task<List<SummaryAccountsReceivableModel>> GetAccountReceivableSummaries(Dictionary<string, string> dbCodes, int page, int pageSize);
        Task<List<OwedInvoiceDto>> GetAccountReceivableAmount(Dictionary<string, string> dbCodes);
    }
}
