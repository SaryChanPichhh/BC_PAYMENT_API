namespace BC.PAYMENT.APPLICATION.Interfaces.Payment
{
    public interface IDividedInvoiceRepository
    {
        Task<List<CORE.Entities.Invoice.Invoice>> GetInvoices(IssueInvoiceExclusionFilterDTO dto);
        Task<List<CORE.Entities.Invoice.Invoice>> GetInvoices(IssueInvoiceFilterDTO dto);
        Task<int> SaveDividedInvoice(List<IssueInvoiceDTO> dto);
        Task<List<Delivery>> GetDividedDeliveryInfo(string dbCode, DateTime date);
        Task<List<CORE.Entities.Invoice.Invoice>> GetDividedInvoice(string dbCode, string deliveryId, DateTime date);
        Task<List<DividedInvoiceSummary>> GetDividedInvoiceSummary(string dbCode, DateTime date);
    }
}
