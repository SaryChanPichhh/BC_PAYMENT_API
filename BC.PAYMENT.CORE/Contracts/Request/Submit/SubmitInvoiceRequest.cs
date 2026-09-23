using BC.PAYMENT.CORE.Contracts.Request.AccountReceivable;
using BC.PAYMENT.CORE.Contracts.Request.Invoice;

namespace BC.PAYMENT.CORE.Contracts.Request.Submit;

public class SubmitInvoiceRequest
{
    public List<BcInvoiceSubmittedRequest> InvoiceRequest { get; set; } = [];
    public List<SiLedgerRequest> OldInvoiceRequest { get; set; } = [];
    public bool IsAutoSettleAccountReceivable { get; set; } = false;
}