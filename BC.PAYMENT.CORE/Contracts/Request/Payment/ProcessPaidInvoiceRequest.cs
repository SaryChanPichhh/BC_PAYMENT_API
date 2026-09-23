using BC.PAYMENT.CORE.Contracts.Request.Expense;
using BC.PAYMENT.CORE.Contracts.Request.Invoice;

namespace BC.PAYMENT.CORE.Contracts.Request.Payment;

public class ProcessPaidInvoiceRequest
{
    public List<PaymentInvoiceExpenseCreateRequest> Expenses { get; set; }
    public BcPaymentDetailCreateRequest PaymentDetail { get; set; }
    public List<PcPaymentInvoiceCreateRequest> PaidInvoices { get; set; }
    public List<PcReturnInvoiceCreateRequest> ReturnInvoices { get; set; }
}