using BC.PAYMENT.CORE.DTO.Transaction.ProvincialPayment.ReviewReport;

namespace BC.PAYMENT.CORE.Entities.Transaction.ProvincialPayment.CheckingStockCarPayment
{
    public class CheckingStockSubmittingInvoiceModel : PaidInvoiceRequestDto
    {
        public string? Employee { get; set; }
    }
}
