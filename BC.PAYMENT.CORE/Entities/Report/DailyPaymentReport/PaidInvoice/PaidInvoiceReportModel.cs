namespace BC.PAYMENT.CORE.Entities.Report.DailyPaymentReport.PaidInvoice
{
    public class PaidInvoiceReportModel : Customer
    {
        public DateTime CreatedDate { get; set; }
        public string Delivery { get; set; }
        public string TransactionCode { get; set; }
        public double InvoiceValue { get; set; }
        public double Paid { get; set; }
        public double? HalfPaid => InvoiceValue > Paid ? Paid : null;
        public double? FullPaid => Math.Abs(InvoiceValue - Paid) == 0 ? Paid : null;
        public double Total => InvoiceValue - Paid;
    }
}
