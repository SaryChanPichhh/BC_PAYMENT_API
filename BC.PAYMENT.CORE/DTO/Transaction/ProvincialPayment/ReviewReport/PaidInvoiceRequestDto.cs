

using BC.PAYMENT.CORE.DTO.Transaction.ProvincialPayment.StockCarPayment;

namespace BC.PAYMENT.CORE.DTO.Transaction.ProvincialPayment.ReviewReport
{
    public class PaidInvoiceRequestDto : InvoiceModelDto
    {
        public DateTime TransactionDate { get; set; }
        public string? TransactionCode { get; set; }
        public double HalfPayment { get; set; }
        public double FullPayment { get; set; }
        public double TotalPayment { get; set; }
        public string? InvoiceType { get; set; }

    }
}
