using BC.PAYMENT.CORE.Contracts.Invoice;
using BC.PAYMENT.CORE.Contracts.Request.Invoice;
using BC.PAYMENT.CORE.Contracts.Response.Invoice;
using NewInvoiceModel = BC.PAYMENT.CORE.Entities.Invoice.NewInvoiceModel;

namespace BC.PAYMENT.APPLICATION.Interfaces.Invoice
{
    public interface IInvoiceRepository
    {
        Task<int> SavePaymentInvoiceAsync(List<PaymentInvoiceRequest> request);
        Task<List<DividedInvoiceTransactionResponse>> GetInvoiceTransactionByDeliveryAndDateAsync(string dbCode, string deliveryId, DateTime date);

        Task<List<InvoiceResponse>> GetInvoiceByAreaAndTransCodeAsync(string dbCode,string areaId,string  transCode);
        
        #region Submitted Invoice
        Task<bool> PostPrintInvoiceAsync(string transactionInvoice, RequestType type, string dbCode,string period,string userName);
        Task<bool> IsAlreadyPosted(string dbCode,string transactionCode);
        Task<bool> SaveRecordPostInvoice(string dbCode,string userName,int invoiceId, string transactionCode, string transCode);
        #endregion

        Task<List<SaleDetailsDto>> GetItemExpiredDates(string dbCode,
            Dictionary<string, List<string>> itemCodeAndExpireDate, string wareHouse);

        Task<int> CreateInvoiceSaleAsync(string dbCode,SaleHeaderDto saleHeader, List<SaleDetailsDto> detailsDtos);

        Task<int> InsertRecordInvoice(string dbCode, string userName, string transaction, string customerCode, string customerName,
            double value, DateTime date, string entryCode);

        Task<int> CreateInvoice(CreateInvoiceDto createInvoiceDto,
            List<CreateInvoiceDetailDto> createInvoiceDetailDto);

        Task<InvoiceDetailDto> GetInvoiceByCustomerCode(string dbCode, string customerCode);
        Task<bool> CheckStockQuantityAsync(string dbCode,string location, string itemCode, int quantityRequest);
        Task UpdateStatusExchangeReceivedToCredit(int id, int status);
        Task UpdateStatusRequestExchangeDetails( int id, ExchangeStatus exchangeStatus);
        Task SaveRecordItemExchanged(string dbCode, string userName, string transaction, string itemCode, int quantity, double unitPrice);
        Task<bool> IsAllItemRequestCompletedByRequestIdAsync(int requestId);
        Task<bool> UpdateReceivedToCompletedByIdAsync(string dbCode,int requestId);
        Task<int> UpdateValue6ToZero(
            List<(string newTransaction, string TransLine, string itemCode, string oldTransaction)> tupleValues);
    }
}
