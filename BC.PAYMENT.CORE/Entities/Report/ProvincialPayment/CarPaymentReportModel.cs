

namespace BC.PAYMENT.CORE.Entities.Report.ProvincialPayment
{
    public class CarPaymentReportModel
    {
        public string Id { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionCode { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public double InvoiceValue { get; set; }
        public double HalfPayment { get; set; }
        public double FullPayment { get; set; }
        public double Total { get; set; }
        public string Invoice { get; set; }

    }
}
