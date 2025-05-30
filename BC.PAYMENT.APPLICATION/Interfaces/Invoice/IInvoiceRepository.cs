using BC.PAYMENT.CORE.DTO.Filter;
using BC.PAYMENT.CORE.DTO.Invoice;
using BC.PAYMENT.CORE.Entities.Invoice;
using BC.PAYMENT.CORE.Enums;

namespace BC.PAYMENT.APPLICATION.Interfaces.Invoice
{
    public interface IInvoiceRepository
    {
        #region NewInvoice
        Task<List<NewInvoiceModel>> GetInvoiceByInvoiceCode(NewInvoicesRequestDTO newInvoiceRequestCodeDto);
        Task SaveInvoices(List<NewInvoiceModel> invoices, DateTime createdDate);
        Task<List<InvoicesModel>> GetInvoices(InvoiceTypes type, string dbCode, DateTime createdDate);

        #endregion

        #region OldInvoice
        Task<List<OldInvoicesModel>> GetAllOldInvoiceByStatus(string dbCode, int offset, int pageSize);
        Task<List<OldInvoicesModel>> GetAllOldInvoices(OldInvoiceRequestDto oldInvoiceRequestDto);
        Task<int> SaveOldInvoice(List<OldInvoiceDTO> dto);
        Task<int> UpdateStatusOldInvoiceByTransactionCode(InvoiceDTO invoiceDto);
        Task<OldInvoicesModel> GetOneOldInvoiceByTransactionCode(string dbCode, string transactionCode);
        Task<int> UpdateStatus(string dbCode, string transaction);

        #endregion

        #region Change and fix invoice
        Task<List<ChangeInvoice>> GetLocalChangeAndFixInvoice(FilterDTO dto);
        Task<List<ChangeInvoice>> GetChangeAndFixInvoice(FilterDTO dto);
        Task<int> SaveFixInvoice(SaveInvoiceDTO dto);

        #endregion

        #region Return Invoices
        Task<List<ReturnInvoice>> GetReturnInvoice(FilterDTO dto);
        Task<List<ReturnInvoice>> GetTodayReturnInvoice(string dbCode, int offset, int pagesize);
        Task<int> SaveReturnInvoice(SaveInvoiceDTO dto);

        #endregion

        #region Submitted Invoice

        Task<bool> PostPrintInvoiceAsync(string transactionInvoice, RequestType type, string dbCode,string period,string userName);

        #endregion

        Task<List<SaleDetailsDto>> GetItemExpiredDates(string dbCode,
            Dictionary<string, List<string>> itemCodeAndExpireDate, string wareHouse);

        Task<int> CreateInvoiceSaleAsync(string dbCode,SaleHeaderDto saleHeader, List<SaleDetailsDto> detailsDtos);

        Task<int> InsertRecordInvoice(string dbCode, string userName, string transaction, string customerCode, string customerName,
            double value, DateTime date, string entryCode);

        Task<int> CreateInvoice(CreateInvoiceDto createInvoiceDto,
            List<CreateInvoiceDetailDto> createInvoiceDetailDto);

        Task<InvoiceDetailDto> GetInvoiceByCustomerCode(string dbCode, string customerCode);
    }
}
