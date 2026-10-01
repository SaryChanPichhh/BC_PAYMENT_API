namespace BC.PAYMENT.APPLICATION.Interfaces.CommondityExchange.ExchangeItem;

public interface IExchangeItemRepository
{
    Task<List<CustomerDto>> GetAllCustomerHasExchangeGoods(string dbCode);
    Task<List<ItemExchangeDto>> GetAllItemExchangeByCustomerCodeAsync(string dbCode, int requestId);
    Task<List<ItemExchangeDto>> GetAllItemCreditByCustomerCodeAsync(string dbCode, int requestId);

    Task<List<InvoiceForExchangeDto>> GetInvoiceForExchangeByCustomerCodeAndItemCodeIn6MonthsAsync(string dbCode,
        string customerCode, string itemCode);

    Task<List<InvoiceForExchangeDto>> GetInvoiceForExchangeByCustomerCodeAndItemCodeAsync(string dbCode,
        string customerCode, string itemCode);

    Task<int> AddNewCreditNote(CreditNoteItemModel model);
    Task<int> SaveRecordItemExchanged(string transaction, string itemCode, int quantity, double unitPrice);
    Task<bool> CheckIfInvoiceAlreadyPostInvoiceAsync(int masterId);

    Task<string> CreateInvoice(string dbCode, string userName, int requestId, string customerCode,
        string transactionCode, double subTotal,
        List<ItemForExchaneDto> items, List<int> receivedId);
}