using BC.PAYMENT.CORE.Contracts.Transaction.ProvincialPayment.StockCarPayment;

namespace BC.PAYMENT.CORE.DTO.Transaction.ProvincialPayment.StockCarPayment
{
    public class StockCarCreditInvoiceDto : Customer
    {
        public string? TransactionCode { get; set; }
        public double InvoiceValue { get; set; }
        public string? Type { get; set; }

        public string InvoiceType => Type switch
        {
            "N" => "ថ្មី",
            "C" => "ដូរ",
            _ => "ចាស់"
        };

        public int? Credit { get; set; }
        public int? Return { get; set; }
        public string? Description { get; set; }
    }

    public class ReviewReportCreditInvoice : InvoiceModelDto
    {
        public DateTime TransactionDate { get; set; }
        public string? IsCheck { get; set; }
    }
}
