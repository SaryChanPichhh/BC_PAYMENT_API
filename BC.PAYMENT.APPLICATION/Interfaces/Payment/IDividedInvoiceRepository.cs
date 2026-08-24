namespace BC.PAYMENT.APPLICATION.Interfaces.Payment
{
    public interface IDividedInvoiceRepository
    {
        Task<List<Invoices>> GetInvoices(IssueInvoiceExclusionFilterDTO dto);
        Task<List<Invoices>> GetInvoices(IssueInvoiceFilterDTO dto);
        Task<int> SaveDividedInvoice(List<IssueInvoiceDTO> dto);

        Task<List<Delivery>> GetDividedDeliveryInfo(string dbCode, DateTime date);
        Task<List<Invoices>> GetDividedInvoice(string dbCode, string deliveryId, DateTime date);
        Task<List<DividedInvoiceSummary>> GetDividedInvoiceSummary(string dbCode, DateTime date);
    }
}
