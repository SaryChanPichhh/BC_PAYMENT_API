using BC.PAYMENT.CORE.Enums;

namespace BC.PAYMENT.CORE.Contracts.Request.Invoice;

public class PostInvoiceRequest
{
    public string TransactionInvoice { get; set; } = string.Empty;
    public RequestType Type { get; set; } = RequestType.Invoice;
}

public class PostPrintInvoiceRequest : PostInvoiceRequest
{
}
